using Microsoft.Identity.Web;

namespace ApplicationAPI
{
    public class GraphAuthenticationMessageHandler : DelegatingHandler
    {
        internal static event EventHandler<string>? TokenAcquired;

        private readonly ITokenAcquisition _tokenAcquisition;

        public GraphAuthenticationMessageHandler(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://graph.microsoft.com/.default" });
            TokenAcquired?.Invoke(this, token);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("ConsistencyLevel", "eventual"); // Only needed for the $count query
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
