using AutoMapper;
using ChallengeUbistart.Api.ViewModels;
using ChallengeUbistart.Business.Models;

namespace ChallengeUbistart.Api.Configuration
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<ItemViewModel, Item>()
                .ReverseMap();
            CreateMap<Item, ItemViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ItemStatus.ToString()));
        }
    }
}