using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Refit;

namespace Web.Data
{
    public class BackendServiceHandler: DelegatingHandler
    {
        private readonly ITokenAcquisition _tokenAcquisition;

        public BackendServiceHandler(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "" });
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
