using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Models;
using ChallengeUbistart.Data.Context;

namespace ChallengeUbistart.Data.Repository
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(MyDbContext context) : base(context)
        {
        }
    }
}
