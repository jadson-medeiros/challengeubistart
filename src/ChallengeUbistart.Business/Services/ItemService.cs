using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Models;
using ChallengeUbistart.Business.Models.Validations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeUbistart.Business.Services
{
    public class ItemService : BaseService, IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository,
            INotify notify) : base(notify)
        {
            _itemRepository = itemRepository;
        }

        public async Task<bool> Insert(Item item)
        {
            await _itemRepository.Insert(item);
            return true;
        }

        public async Task<bool> Update(Item item)
        {
            if (!ExecuteValidation(new ItemValidation(), item)) return false;

            if (_itemRepository.Search(f => f.Id != item.Id).Result.Any())
            {
                Inform("Item duplicated.");
                return false;
            }

            await _itemRepository.Update(item);
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            await _itemRepository.Delete(id);
            return true;
        }

        public void Dispose()
        {
            _itemRepository?.Dispose();
        }
    }
}
