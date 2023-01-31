using Microsoft.Extensions.Localization;
using MyBudget.API.Settings;
using MyBudget.Application.Interfaces.Services.Storage;
using MyBudget.Shared.Constants.Storage;
using MyBudget.Shared.Settings;
using MyBudget.Shared.Wrapper;
using IResult = MyBudget.Shared.Wrapper.IResult;

namespace MyBudget.API.Managers.Preferences
{
    public class ServerPreferenceManager : IServerPreferenceManager
    {
        private readonly IServerStorageService _serverStorageService;
        private readonly IStringLocalizer<ServerPreferenceManager> _localizer;

        public ServerPreferenceManager(
            IServerStorageService serverStorageService,
            IStringLocalizer<ServerPreferenceManager> localizer)
        {
            _serverStorageService = serverStorageService;
            _localizer = localizer;
        }

        public async Task<IResult> ChangeLanguageAsync(string languageCode)
        {
            ServerPreference? preference = await GetPreference() as ServerPreference;
            if (preference != null)
            {
                preference.LanguageCode = languageCode;
                await SetPreference(preference);
                return new Result
                {
                    Succeeded = true,
                    Messages = new List<string> { _localizer["Server Language has been changed"] }
                };
            }

            return new Result
            {
                Succeeded = false,
                Messages = new List<string> { _localizer["Failed to get server preferences"] }
            };
        }

        public async Task<IPreference> GetPreference()
        {
            return await _serverStorageService.GetItemAsync<ServerPreference>(StorageConstants.Server.Preference) ?? new ServerPreference();
        }

        public async Task SetPreference(IPreference preference)
        {
            await _serverStorageService.SetItemAsync(StorageConstants.Server.Preference, preference as ServerPreference);
        }
    }
}
