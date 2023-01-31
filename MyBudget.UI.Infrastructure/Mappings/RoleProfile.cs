using AutoMapper;
using MyBudget.Application.Requests.Identity;
using MyBudget.Application.Responses.Identity;

namespace MyBudget.UI.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            _ = CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            _ = CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}
