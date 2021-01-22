using ChallengeUbistart.Business.Models;
using System;
using System.Threading.Tasks;

namespace ChallengeUbistart.Business.Intefaces
{
    public interface IItemService
    {
        Task<bool> Delete(Guid id);
        void Dispose();
        Task<bool> Insert(Item item);
        Task<bool> Update(Item item);
    }
}