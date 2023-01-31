using Microsoft.AspNetCore.Components.WebView.Maui;
using MyBudget.Shared.Constants.Storage;
using Plugin.Fingerprint.Abstractions;

namespace MyBudget.MAUI
{
    public partial class MainPage : ContentPage
    {

        public MainPage(IFingerprint fingerprint)
        {
            InitializeComponent();

        }


    }
}