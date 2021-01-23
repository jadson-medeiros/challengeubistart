using AutoMapper;
using ChallengeUbistart.Api.Controllers;
using ChallengeUbistart.Api.Extensions;
using ChallengeUbistart.Api.ViewModels;
using ChallengeUbistart.Business.Filter;
using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChallengeUbistart.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/items")]
    public class ItemController : MainController
    {
        #region Fields
        private readonly IItemRepository _itemRepository;
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IClientRepository _clientRepository;
        #endregion

        #region Ctor
        public ItemController(INotify notify,
            IItemRepository itemRepository,
            IItemService itemService,
            IMapper mapper,
            IUser user,
            UserManager<IdentityUser> userManager,
            IClientRepository clientRepository) : base(notify, user)
        {
            _itemRepository = itemRepository;
            _itemService = itemService;
            _mapper = mapper;
            _userManager = userManager;
            _clientRepository = clientRepository;
        }
        #endregion

        #region Utilities
        private async Task<ItemViewModel> GetItem(Guid id)
        {
            return _mapper.Map<ItemViewModel>(await _itemRepository.GetById(id));
        }

        private async Task<IEnumerable<ItemViewModel>> GetItemViewModel(IEnumerable<ItemViewModel> itemViewModels)
        {
            foreach(var item in itemViewModels)
            {
                var client = await _clientRepository.GetByUserId(item.ClientId);

                if (client == null) continue;

                item.ClientEmail = client.Email;
            }

            return itemViewModels;
        }
        #endregion

        #region Methods

        #region Client Methods

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ItemViewModel>> GetById(Guid id)
        {
            var itemViewModel = await GetItem(id);

            if (itemViewModel == null) return NotFound();

            return itemViewModel;
        }

        [HttpPost]
        [ClaimsAuthorize("Item", "Create")]
        public async Task<ActionResult<ItemViewModel>> Insert(ItemViewModel itemViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _itemService.Insert(_mapper.Map<Item>(itemViewModel));

            return CustomResponse(itemViewModel);
        }

        [HttpPut("{id:guid}")]
        [ClaimsAuthorize("Item", "Update")]
        public async Task<IActionResult> Update(Guid id, ItemViewModel itemViewModel)
        {
            if (id != itemViewModel.Id)
            {
                InformError("The 'ids' entered are differents!");
                return CustomResponse();
            }

            var itemUpdate = await GetItem(id);

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            itemUpdate.Description = itemViewModel.Description;
            itemUpdate.DueDate = itemViewModel.DueDate;

            await _itemService.Update(_mapper.Map<Item>(itemUpdate));

            return CustomResponse(itemViewModel);
        }

        [HttpPut]
        [Route("Conclude/{id:guid}")]
        [ClaimsAuthorize("Item", "Update")]
        public async Task<IActionResult> Conclude(Guid id)
        {
            var item = await GetItem(id);

            if (item == null) return NotFound();

            var itens = await _itemService.ConcludeItem(_mapper.Map<Item>(item));

            return Ok(itens);
        }

        [HttpDelete("{id:guid}")]
        [ClaimsAuthorize("Item", "Delete")]
        public async Task<ActionResult<ItemViewModel>> Delete(Guid id)
        {
            var item = await GetItem(id);

            if (item == null) return NotFound();

            await _itemService.Delete(id);

            return CustomResponse(item);
        }

        [HttpGet]
        [Route("GetAllByClientId")]
        [ClaimsAuthorize("Item", "List")]
        public async Task<IEnumerable<ItemViewModel>> GetAllByClientId([FromQuery] PaginationFilter filter, Guid clientId)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var items = await _itemService.GetAllByFilterClientId(validFilter, clientId);
            return _mapper.Map<IEnumerable<ItemViewModel>>(items);
        }

        #endregion

        #region Administrator Methods

        [HttpGet]
        [Route("GetAll")]
        [ClaimsAuthorize("Administrator", "List")]
        public async Task<IEnumerable<ItemViewModel>> GetAll([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var items = await _itemService.GetAll(validFilter);

            return await GetItemViewModel(_mapper.Map<IEnumerable<ItemViewModel>>(items));
        }

        [HttpGet]
        [Route("GetAllDalayed")]
        [ClaimsAuthorize("Administrator", "List")]
        public async Task<IEnumerable<ItemViewModel>> GetAllDalayed([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var items = _itemRepository.GetAllDelayed(validFilter);

            return await GetItemViewModel(_mapper.Map<IEnumerable<ItemViewModel>>(items));
        }
    
        #endregion

        #endregion
    }
}
