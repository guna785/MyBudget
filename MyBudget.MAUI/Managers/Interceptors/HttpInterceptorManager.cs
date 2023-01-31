using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MyBudget.MAUI.Managers.Identity.Authentication;
using System.Net.Http.Headers;
using Toolbelt.Blazor;

namespace MyBudget.MAUI.Managers.Interceptors
{
    public class HttpInterceptorManager : IHttpInterceptorManager
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly NavigationManager _navigationManager;
        private readonly ISnackbar _snackBar;
        private readonly IStringLocalizer<HttpInterceptorManager> _localizer;

        public HttpInterceptorManager(
            HttpClientInterceptor interceptor,
            IAuthenticationManager authenticationManager,
            NavigationManager navigationManager,
            ISnackbar snackBar,
            IStringLocalizer<HttpInterceptorManager> localizer)
        {
            _interceptor = interceptor;
            _authenticationManager = authenticationManager;
            _navigationManager = navigationManager;
            _snackBar = snackBar;
            _localizer = localizer;

        }

        public void RegisterEvent()
        {
            _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
        }

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            string absPath = e.Request.RequestUri.AbsolutePath;
            if (!absPath.Contains("token") && !absPath.Contains("accounts"))
            {
                try
                {
                    string token = await _authenticationManager.TryRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _ = _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
                        e.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _ = _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
                    _ = await _authenticationManager.Logout();
                    _navigationManager.NavigateTo("/");
                }
            }
        }

        public void DisposeEvent()
        {
            _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
        }
    }
}
