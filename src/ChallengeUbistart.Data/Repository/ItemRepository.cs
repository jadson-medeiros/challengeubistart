using ChallengeUbistart.Business.Filter;
using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Models;
using ChallengeUbistart.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChallengeUbistart.Data.Repository
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(MyDbContext context) : base(context)
        {
        }

        public IEnumerable<Item> GetAllByFilterClientId(PaginationFilter validFilter, Guid clientId)
        {
            return Db.Items
                .AsNoTracking()
                .Include(i => i.ItemStatus)
                .Include(p => p.Client)
                .Where(o => o.ClientId.Equals(clientId))
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .AsQueryable();
        }

        public IEnumerable<Item> GetAll(PaginationFilter validFilter)
        {
            return Db.Items
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .AsQueryable();
        }

        public IEnumerable<Item> GetAllDelayed(PaginationFilter validFilter)
        {
            return Db.Items
                .Where(o => o.ItemStatus.Equals(ItemStatus.Delayed))
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .AsQueryable();
        }
    }
}
