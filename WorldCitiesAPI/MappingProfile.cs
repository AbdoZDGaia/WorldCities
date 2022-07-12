using AutoMapper;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserForRegistrationDto, ApplicationUser>()
               .ForMember(u => u.UserName,
               opt => opt.MapFrom(x => x.Email));
        }
    }
}
