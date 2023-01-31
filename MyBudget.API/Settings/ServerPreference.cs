using MyBudget.Shared.Constants.Localization;
using MyBudget.Shared.Settings;

namespace MyBudget.API.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";

        //TODO - add server preferences
    }
}
