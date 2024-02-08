using Microsoft.Identity.Web;

namespace BackendService.Data
{
    public class GraphApiHandler : DelegatingHandler
    {
        private const string Scope = "https://graph.microsoft.com/.default";

        // Only there for demo purposes; remove when using this for your own purposes
        internal static event EventHandler<string>? TokenAcquired;

        private readonly ITokenAcquisition _tokenAcquisition;

        public GraphApiHandler(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { Scope });
            TokenAcquired?.Invoke(this, token);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("ConsistencyLevel", "eventual"); // Only needed for the $count query
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
