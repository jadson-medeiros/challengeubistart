using ChallengeUbistart.Business.Models;
using ChallengeUbistart.Business.Filter;
using System.Collections.Generic;
using System;

namespace ChallengeUbistart.Business.Intefaces
{
    public interface IItemRepository : IRepository<Item>
    {
        IEnumerable<Item> GetAllByFilterClientId(PaginationFilter validFilter, Guid clientId);
        IEnumerable<Item> GetAll(PaginationFilter validFilter);
        IEnumerable<Item> GetAllDelayed(PaginationFilter validFilter);
    }
}