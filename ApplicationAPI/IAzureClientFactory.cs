using Azure.Storage.Blobs;

namespace ApplicationAPI
{
    public interface IAzureClientFactory: IAsyncDisposable
    {
        BlobServiceClient GetBlobClient(Uri serviceUri);
    }
}