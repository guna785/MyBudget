using AutoMapper;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Responses.Identity;

namespace MyBudget.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            _ = CreateMap<RoleResponse, ApplicationRole>().ReverseMap();
        }
    }
}
