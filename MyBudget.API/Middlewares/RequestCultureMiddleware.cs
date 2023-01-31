using System.Globalization;

namespace MyBudget.API.Middlewares
{
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Microsoft.Extensions.Primitives.StringValues cultureQuery = context.Request.Query["culture"];
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                CultureInfo culture = new(cultureQuery);

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            else if (context.Request.Headers.ContainsKey("Accept-Language"))
            {
                Microsoft.Extensions.Primitives.StringValues cultureHeader = context.Request.Headers["Accept-Language"];
                if (cultureHeader.Any())
                {
                    CultureInfo culture = new(cultureHeader.First().Split(',').First().Trim());

                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
            }

            await _next(context);
        }
    }
}
