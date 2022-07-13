using AutoMapper;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Source --> Destination

            CreateMap<UserForRegistrationDto, ApplicationUser>()
               .ForMember(u => u.UserName,
               opt => opt.MapFrom(x => x.Email));

            CreateMap<City, CityDTO>().ReverseMap();
            CreateMap<Country, CountryDTO>().ReverseMap();
        }
    }
}
