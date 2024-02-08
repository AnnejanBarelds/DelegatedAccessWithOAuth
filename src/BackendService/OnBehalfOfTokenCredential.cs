using Azure.Core;
using Microsoft.Identity.Web;

namespace BackendService
{
    public class OnBehalfOfTokenCredential : TokenCredential
    {
        // Only there for demo purposes; remove when using this for your own purposes
        internal static event EventHandler<string>? TokenAcquired;

        private readonly ITokenAcquisition _tokenAcquisition;

        public OnBehalfOfTokenCredential(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return GetTokenInternalAsync(requestContext).GetAwaiter().GetResult();
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return GetTokenInternalAsync(requestContext);
        }

        private async ValueTask<AccessToken> GetTokenInternalAsync(TokenRequestContext requestContext)
        {
            var result = await _tokenAcquisition.GetAuthenticationResultForUserAsync(requestContext.Scopes);
            TokenAcquired?.Invoke(this, result.AccessToken);
            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }
    }
}
