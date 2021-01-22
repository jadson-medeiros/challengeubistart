using System;
using System.Linq;
using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChallengeUbistart.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotify _notify;
        public readonly IUser AppUser;

        protected Guid UserId { get; set; }
        protected bool UserAutenticated { get; set; }

        protected MainController(INotify notify, IUser appUser)
        {
            _notify = notify;
            AppUser = appUser;

            if (appUser.IsAuthenticated())
            {
                UserId = appUser.GetUserId();
                UserAutenticated = true;
            }
        }

        protected bool OperationValidate()
        {
            return !_notify.HasNotification();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperationValidate())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notify.GetNotifications().Select(n => n.Message)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) InformErroModelInvalid(modelState);
            return CustomResponse();
        }

        protected void InformErroModelInvalid(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                InformError(errorMsg);
            }
        }

        protected void InformError(string message)
        {
            _notify.Handle(new Notification(message));
        }
    }
}
