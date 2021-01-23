using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Models;
using ChallengeUbistart.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ChallengeUbistart.Data.Repository
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(MyDbContext context) : base(context)
        { }

        public async Task<Client> GetByUserId(string userId)
        {
            return await Db.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.UserId.Equals(userId));
        }

        public async Task<Client> GetByUserId(Guid id)
        {
            return await Db.Clients
                .FirstOrDefaultAsync(o => o.Id.Equals(id) || o.UserId.Equals(id.ToString()));
        }
    }
}
