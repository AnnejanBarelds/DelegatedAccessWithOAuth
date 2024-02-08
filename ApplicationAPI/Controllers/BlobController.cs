using BackendService.Configuration;
using Azure.Storage.Blobs;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web.Resource;

namespace BackendService.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class BlobController : ControllerBase
    {
        private readonly StorageOptions _storageOptions;
        private readonly BlobServiceClient _blobServiceClient;
        private string? _lastAccessToken;

        public BlobController(IAzureClientFactory azureClientFactory, IOptions<StorageOptions> storageOptions)
        {
            _storageOptions = storageOptions.Value;
            _blobServiceClient = azureClientFactory.GetBlobClient(new Uri(_storageOptions.Url));
            OnBehalfOfTokenCredential.TokenAcquired += OnTokenAcquired;
        }

        private void OnTokenAcquired(object? sender, string e)
        {
            _lastAccessToken = e;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_storageOptions.ContainerName);
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
