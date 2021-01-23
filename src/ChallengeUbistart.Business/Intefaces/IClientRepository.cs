using ChallengeUbistart.Business.Models;
using System;
using System.Threading.Tasks;

namespace ChallengeUbistart.Business.Intefaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client> GetByUserId(Guid id);
        Task<Client> GetByUserId(string userId);
    }
}