using Microsoft.AspNetCore.Components.Authorization;
using MyBudget.Shared.Constants.Permission;
using MyBudget.Shared.Constants.Storage;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace MyBudget.MAUI.Authentication
{
    public class DentalAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        public DentalAuthenticationStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task StateChangedAsync()
        {
            Task<AuthenticationState> authState = Task.FromResult(await GetAuthenticationStateAsync());

            NotifyAuthenticationStateChanged(authState);

        }

        public void MarkUserAsLoggedOut()
        {
            ClaimsPrincipal anonymousUser = new(new ClaimsIdentity());
            Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(anonymousUser));

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
        {
            AuthenticationState state = await GetAuthenticationStateAsync();
            ClaimsPrincipal authenticationStateProviderUser = state.User;
            return authenticationStateProviderUser;
        }

        public ClaimsPrincipal AuthenticationStateUser { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string savedToken = await SecureStorage.GetAsync(StorageConstants.Local.AuthToken);
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            AuthenticationState state = new(new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(savedToken), "jwt")));
            AuthenticationStateUser = state.User;
            return state;
        }

        private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
        {
            List<Claim> claims = new();
            string payload = jwt.Split('.')[1];
            byte[] jsonBytes = ParseBase64WithoutPadding(payload);
            Dictionary<string, object> keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                _ = keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

                if (roles != null)
                {
                    if (roles.ToString().Trim().StartsWith("["))
                    {
                        string[] parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                        claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                    }

                    _ = keyValuePairs.Remove(ClaimTypes.Role);
                }

                _ = keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out object permissions);
                if (permissions != null)
                {
                    if (permissions.ToString().Trim().StartsWith("["))
                    {
                        string[] parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString());
                        claims.AddRange(parsedPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));
                    }
                    else
                    {
                        claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()));
                    }
                    _ = keyValuePairs.Remove(ApplicationClaimTypes.Permission);
                }

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string payload)
        {
            payload = payload.Trim().Replace('-', '+').Replace('_', '/');
            string base64 = payload.PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');
            return Convert.FromBase64String(base64);
        }
    }
}
