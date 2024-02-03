using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Web.Resource;

namespace ApplicationAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class BlobController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly OnBehalfOfTokenCredential _onBehalfOfTokenCredential;
        private string? _lastAccessToken;

        public BlobController(IAzureClientFactory azureClientFactory, TokenCredential tokenCredential)
        {
            _blobServiceClient = azureClientFactory.GetBlobClient(new Uri(""));
            _onBehalfOfTokenCredential = (OnBehalfOfTokenCredential)tokenCredential;
            _onBehalfOfTokenCredential.TokenAcquired += OnTokenAcquired;
        }

        private void OnTokenAcquired(object? sender, string e)
        {
            _lastAccessToken = e;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("container1");
            var blobClient = containerClient.GetBlobClient("test.txt");
            await using var stream = await blobClient.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var result = new BlobResult
            {
                Content = reader.ReadToEnd(),
                Token = _lastAccessToken
            };
            return Ok(result);
        }
    }
}
