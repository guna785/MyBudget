using AutoMapper;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Requests.Identity;
using MyBudget.Application.Responses.Identity;

namespace MyBudget.Infrastructure.Mappings
{
    public class RoleClaimProfile : Profile
    {
        public RoleClaimProfile()
        {
            _ = CreateMap<RoleClaimResponse, ApplicationRoleClaim>()
                .ForMember(nameof(ApplicationRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(ApplicationRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();

            _ = CreateMap<RoleClaimRequest, ApplicationRoleClaim>()
                .ForMember(nameof(ApplicationRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(ApplicationRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();
        }
    }
}
