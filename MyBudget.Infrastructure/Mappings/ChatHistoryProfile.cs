using AutoMapper;
using MyBudget.Infrastructure.Models.Identity;
using MyBudget.Application.Interfaces.Chat;
using MyBudget.Application.Models.Chat;

namespace MyBudget.Infrastructure.Mappings
{
    public class ChatHistoryProfile : Profile
    {
        public ChatHistoryProfile()
        {
            _ = CreateMap<ChatHistory<IChatUser>, ChatHistory<ApplicationUser>>().ReverseMap();
        }
    }
}
