using BackendService.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace BackendService.Data
{
    // Only there for demo purposes; remove when using this for your own purposes
    public class WeatherApiHandler: DelegatingHandler
    {
        internal static event EventHandler<string>? TokenAcquired;

        private readonly WeatherApiOptions _weatherApiOptions;
        private readonly ITokenAcquisition _tokenAcquisition;

        public WeatherApiHandler(ITokenAcquisition tokenAcquisition, IOptions<WeatherApiOptions> weatherApiOptions)
        {
            _tokenAcquisition = tokenAcquisition;
            _weatherApiOptions = weatherApiOptions.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { $"api://{_weatherApiOptions.ClientId}/.default" });
            TokenAcquired?.Invoke(this, token);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
