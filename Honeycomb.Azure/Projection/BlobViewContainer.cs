namespace Honeycomb.Azure.Projection
{
    using Microsoft.WindowsAzure.Storage.Blob;

    public class BlobViewContainer
    {
        private readonly CloudBlobContainer container;

        public BlobViewContainer(CloudBlobContainer container)
        {
            this.container = container;
        }

        public CloudBlockBlob GetBlockBlobReference(string relativePath)
        {
            return container.GetBlockBlobReference(relativePath);
        }
    }
}