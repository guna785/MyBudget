using MyBudget.Application.Responses.Audit;
using MyBudget.Shared.Wrapper;
using MyBudget.UI.Infrastructure.Extensions;
using MyBudget.UI.Infrastructure.Routes;

namespace MyBudget.MAUI.Managers.Audit
{
    public class AuditManager : IAuditManager
    {
        private readonly HttpClient _httpClient;

        public AuditManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(AuditEndpoints.GetCurrentUserTrails);
            IResult<IEnumerable<AuditResponse>> data = await response.ToResult<IEnumerable<AuditResponse>>();
            return data;
        }

        public async Task<IResult<string>> DownloadFileAsync(string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? AuditEndpoints.DownloadFile
                : AuditEndpoints.DownloadFileFiltered(searchString, searchInOldValues, searchInNewValues));
            return await response.ToResult<string>();
        }
    }
}
