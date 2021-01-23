using ChallengeUbistart.Business.Filter;
using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Models;
using ChallengeUbistart.Business.Models.Validations;
using System;
using System.Collections.Generic;
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

        private async Task<IEnumerable<Item>> UpdateItemsDelayed(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                if (item.DueDate != DateTime.MinValue && item.DueDate < DateTime.Today)
                {
                    item.ItemStatus = ItemStatus.Delayed;
                    await Update(item);
                }
            }

            return items;
        }

        public async Task<bool> Insert(Item item)
        {
            if (!ExecuteValidation(new ItemValidation(), item)) return false;
            item.CreatedAt = DateTime.Now;
            await _itemRepository.Insert(item);
            return true;
        }

        public async Task<bool> ConcludeItem(Item item)
        {
            item.ItemStatus = ItemStatus.Finished;
            item.FinishedAt = DateTime.Now;

            await _itemRepository.Update(item);
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
            item.ItemStatus = ItemStatus.Updated;
            item.UpdatedAt = DateTime.Now;
            await _itemRepository.Update(item);
            return true;
        }

        public async Task<IEnumerable<Item>> GetAll(PaginationFilter validFilter)
        {
            return await UpdateItemsDelayed(_itemRepository.GetAll(validFilter));
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

        public async Task<IEnumerable<Item>> GetAllByFilterClientId(PaginationFilter validFilter, Guid clientId)
        {
            return await UpdateItemsDelayed(_itemRepository.GetAllByFilterClientId(validFilter, clientId));
        }
    }
}
