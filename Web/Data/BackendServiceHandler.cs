using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Web.Configuration;

namespace Web.Data
{
    public class BackendServiceHandler: DelegatingHandler
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly BackendServiceOptions _backendServiceOptions;

        public BackendServiceHandler(ITokenAcquisition tokenAcquisition, IOptions<BackendServiceOptions> backendServiceOptions)
        {
            _tokenAcquisition = tokenAcquisition;
            _backendServiceOptions = backendServiceOptions.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { $"api://{_backendServiceOptions.ClientId}/.default" });
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
