using Plugin.Fingerprint.Abstractions;

namespace MyBudget.MAUI
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App(IFingerprint fingerprint)
        {
            InitializeComponent();

            MainPage = new MainPage(fingerprint);
        }
    }
}