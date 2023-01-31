using MyBudget.Application.Interfaces.Chat;
using MyBudget.Application.Models.Chat;
using MyBudget.Application.Responses.Identity;
using MyBudget.Shared.Wrapper;

namespace MyBudget.MAUI.Managers.Communication
{
    public interface IChatManager : IManager
    {
        Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId);
    }
}
