using Azure.Core;
using Azure.Storage.Blobs;
using System.Collections.Concurrent;

namespace BackendService
{
    public class AzureClientFactory : IAzureClientFactory
    {
        private readonly TokenCredential _tokenCredential;
        private ConcurrentDictionary<Uri, BlobServiceClient> _blobServiceClients = new();

        public AzureClientFactory(TokenCredential tokenCredential)
        {
            _tokenCredential = tokenCredential;
        }

        public BlobServiceClient GetBlobClient(Uri serviceUri)
        {
            return _blobServiceClients.GetOrAdd(serviceUri, uri =>
            {
                return new BlobServiceClient(uri, _tokenCredential);
            });
        }

        public ValueTask DisposeAsync()
        {
            // No-op, but implement when using disposable clients, such as Service Bus Clients
            return ValueTask.CompletedTask;
        }

        public void Dispose()
        {
            // No-op, but implement when using disposable clients, such as Service Bus Clients
        }
    }
}
