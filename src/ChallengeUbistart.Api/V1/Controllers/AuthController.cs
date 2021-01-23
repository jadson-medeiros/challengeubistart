using ChallengeUbistart.Api.Controllers;
using ChallengeUbistart.Api.Extensions;
using ChallengeUbistart.Api.ViewModels;
using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeUbistart.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AuthController : MainController
    {
        #region Fields
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IClientRepository _clientRepository;
        private readonly AppSettings _appSettings;
        #endregion

        #region Ctor
        public AuthController(INotify notify, 
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            IUser user, 
            ILogger<AuthController> logger,
            IOptions<AppSettings> appSettings, 
            RoleManager<IdentityRole> roleManager,
            IClientRepository clientRepository) : base(notify, user)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            _clientRepository = clientRepository;
            _appSettings = appSettings.Value;
        }
        #endregion

        #region Utilities
        private async Task<LoginResponseViewModel> GenerateJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var client = await _clientRepository.GetByUserId(user.Id);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.ValidIn,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);

            var response = new LoginResponseViewModel
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpirationHours).TotalSeconds,
                UserToken = new UserTokenViewModel
                {
                    Id = client != null ? client.Id.ToString() : user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value })
                }
            };

            return response;
        }

        private async Task CreateRoles(string role)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                if (role.Equals("Administrator"))
                {
                    var identityRole = new IdentityRole
                    {
                        Name = role
                    };

                    await _roleManager.CreateAsync(identityRole);
                }
                else if (role.Equals("Client"))
                {
                    var identityRole = new IdentityRole
                    {
                        Name = role
                    };

                    await _roleManager.CreateAsync(identityRole);
                }
            }
        }
     
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        #endregion

        #region Methods
        [HttpPost("new-account")]
        public async Task<ActionResult> Register(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                await CreateRoles("Client");
                await _userManager.GetSecurityStampAsync(user);
                await _userManager.AddToRoleAsync(user, "Client");
                await _userManager.AddClaimAsync(user, new Claim("Item", "Create, Update, Delete, List"));

                var client = new Client
                {
                    UserId = user.Id,
                    Email = user.Email
                };

                await _clientRepository.Insert(client);

                return CustomResponse(await GenerateJwt(user.Email));
            }
            foreach (var error in result.Errors)
            {
                InformError(error.Description);
            }

            return CustomResponse(registerUser);
        }

        [HttpPost("new-admin-account")]
        public async Task<ActionResult> RegisterAdministrator(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                await CreateRoles("Administrator");
                await _userManager.GetSecurityStampAsync(user);
                await _userManager.AddToRoleAsync(user, "Administrator");
                await _userManager.AddClaimAsync(user, new Claim("Administrator", "Create, Update, Delete, List"));

                return CustomResponse(await GenerateJwt(user.Email));
            }
            foreach (var error in result.Errors)
            {
                InformError(error.Description);
            }

            return CustomResponse(registerUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User " + loginUser.Email + " successfully logged in");
                return CustomResponse(await GenerateJwt(loginUser.Email));
            }
            if (result.IsLockedOut)
            {
                InformError("User temporarily blocked by invalid attempts");
                return CustomResponse(loginUser);
            }

            InformError("Incorrect User or Password");
            return CustomResponse(loginUser);
        }

        #endregion
    }
}
