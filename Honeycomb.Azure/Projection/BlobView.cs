namespace Honeycomb.Azure.Projection
{
    using System.IO;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class BlobView
    {
        private readonly CloudBlockBlob blob;

        public BlobView(BlobViewContainer container, string relativePath)
        {
            blob = container.GetBlockBlobReference(relativePath);
        }

        public JObject GetContent()
        {
            if (!blob.Exists())
                return new JObject();

            using (var s = new PreservedMemoryStream())
            using (var sr = new StreamReader(s))
            using (var jr = new JsonTextReader(sr))
            {
                blob.DownloadToStream(s);
                return (JObject) JToken.ReadFrom(jr);
            }
        }

        public void UpdateContent(JObject obj)
        {
            var accessCondition = string.IsNullOrEmpty(blob.Properties.ETag)
                ? AccessCondition.GenerateEmptyCondition()
                : AccessCondition.GenerateIfMatchCondition(blob.Properties.ETag);

            using (var s = new PreservedMemoryStream())
            using (var sw = new StreamWriter(s))
            using (var jw = new JsonTextWriter(sw))
            {
                obj.WriteTo(jw);
                blob.UploadFromStream(s, accessCondition);
            }
        }
    }
}