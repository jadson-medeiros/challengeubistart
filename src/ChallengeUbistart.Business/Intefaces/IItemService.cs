using ChallengeUbistart.Business.Filter;
using ChallengeUbistart.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChallengeUbistart.Business.Intefaces
{
    public interface IItemService
    {
        Task<bool> Delete(Guid id);
        void Dispose();
        Task<bool> Insert(Item item);
        Task<bool> Update(Item item);

        Task<IEnumerable<Item>> GetAll(PaginationFilter validFilter);
        Task<IEnumerable<Item>> GetAllByFilterClientId(PaginationFilter validFilter, Guid clientId);
        Task<bool> ConcludeItem(Item item);
    }
}