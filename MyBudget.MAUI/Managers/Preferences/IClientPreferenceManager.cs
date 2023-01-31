using MudBlazor;
using MyBudget.Shared.Managers;

namespace MyBudget.MAUI.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> ToggleDarkModeAsync();
    }
}
