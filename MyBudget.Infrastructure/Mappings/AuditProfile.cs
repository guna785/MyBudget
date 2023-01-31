using AutoMapper;
using MyBudget.Infrastructure.Models.Audit;
using MyBudget.Application.Responses.Audit;

namespace MyBudget.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            _ = CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}
