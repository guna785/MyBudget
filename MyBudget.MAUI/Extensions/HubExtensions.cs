using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MyBudget.Shared.Constants.Application;

namespace MyBudget.MAUI.Extensions
{
    public static class HubExtensions
    {
        public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager)
        {

            /* Unmerged change from project 'MyBudget.MAUI (net6.0-maccatalyst)'
            Before:
                        if (hubConnection == null)
                        {
                            hubConnection = new HubConnectionBuilder()
            After:
                        hubConnection ??= new HubConnectionBuilder()
            */

            /* Unmerged change from project 'MyBudget.MAUI (net6.0-ios)'
            Before:
                        if (hubConnection == null)
                        {
                            hubConnection = new HubConnectionBuilder()
            After:
                        hubConnection ??= new HubConnectionBuilder()
            */

            /* Unmerged change from project 'MyBudget.MAUI (net6.0-windows10.0.19041.0)'
            Before:
                        if (hubConnection == null)
                        {
                            hubConnection = new HubConnectionBuilder()
            After:
                        hubConnection ??= new HubConnectionBuilder()
            */
            hubConnection ??= new HubConnectionBuilder()
                                  .WithUrl($"https://localhost:7172{ApplicationConstants.SignalR.HubUrl}", options =>
                                  {
                                      options.AccessTokenProvider = async () => await SecureStorage.GetAsync("authToken");
                                  })
                                  .WithAutomaticReconnect()
                                  .Build();
            return hubConnection;
        }
        //public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager)
        //{
        //    if (hubConnection == null)
        //    {
        //        hubConnection = new HubConnectionBuilder()
        //                          .WithUrl(navigationManager.ToAbsoluteUri(ApplicationConstants.SignalR.HubUrl))
        //                          .Build();
        //    }
        //    return hubConnection;
        //}
    }
}
