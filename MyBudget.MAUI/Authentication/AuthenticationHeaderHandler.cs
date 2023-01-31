using MyBudget.Shared.Constants.Storage;
using System.Net;
using System.Net.Http.Headers;

namespace MyBudget.MAUI.Authentication
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {

        public AuthenticationHeaderHandler()
        {

        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            if (request.Headers.Authorization?.Scheme != "Bearer")
            {
                string savedToken = await SecureStorage.GetAsync(StorageConstants.Local.AuthToken);

                if (!string.IsNullOrWhiteSpace(savedToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
