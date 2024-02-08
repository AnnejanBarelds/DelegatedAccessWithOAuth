using Azure.Storage.Blobs;

namespace BackendService
{
    public interface IAzureClientFactory: IAsyncDisposable, IDisposable
    {
        BlobServiceClient GetBlobClient(Uri serviceUri);
    }
}