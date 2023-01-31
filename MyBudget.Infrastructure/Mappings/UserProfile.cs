using AutoMapper;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Responses.Identity;

namespace MyBudget.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            _ = CreateMap<UserResponse, ApplicationUser>().ReverseMap();
            _ = CreateMap<ChatUserResponse, ApplicationUser>().ReverseMap()
                .ForMember(dest => dest.EmailAddress, source => source.MapFrom(source => source.Email)); //Specific Mapping
        }
    }
}
