namespace Test.Honeycomb.Azure
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using NUnit.Framework;
    using global::Honeycomb.Azure.Projection;

    [TestFixture]
    public class TestBlobView
    {
        [Test]
        public void it_should_create_a_json_blob_for_the_event()
        {
            var container = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient().GetContainerReference("views");
            container.CreateIfNotExists();
            var blobViewContainer = new BlobViewContainer(container);
            var subject = new BlobView(blobViewContainer, "a/b/c");

            subject.UpdateContent(new {Monkey = "Cheese"});
        }
    }
}