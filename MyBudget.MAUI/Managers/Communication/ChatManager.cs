using MyBudget.Application.Interfaces.Chat;
using MyBudget.Application.Models.Chat;
using MyBudget.Application.Responses.Identity;
using MyBudget.Shared.Wrapper;
using MyBudget.UI.Infrastructure.Extensions;
using MyBudget.UI.Infrastructure.Routes;
using System.Net.Http.Json;

namespace MyBudget.MAUI.Managers.Communication
{
    public class ChatManager : IChatManager
    {
        private readonly HttpClient _httpClient;

        public ChatManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(ChatEndpoint.GetChatHistory(cId));
            IResult<IEnumerable<ChatHistoryResponse>> data = await response.ToResult<IEnumerable<ChatHistoryResponse>>();
            return data;
        }

        public async Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(ChatEndpoint.GetAvailableUsers);
            IResult<IEnumerable<ChatUserResponse>> data = await response.ToResult<IEnumerable<ChatUserResponse>>();
            return data;
        }

        public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(ChatEndpoint.SaveMessage, chatHistory);
            IResult data = await response.ToResult();
            return data;
        }
    }
}
