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
            blob.Properties.ContentType = "application/json";
        }

        public dynamic GetContent()
        {
            if (!blob.Exists())
                return new JObject();

            var s = new PreservedMemoryStream();
            blob.DownloadToStream(s);

            using (var sr = new StreamReader(s))
            using (var jr = new JsonTextReader(sr))
            {
                return JToken.ReadFrom(jr);
            }
        }

        public void UpdateContent(dynamic obj)
        {
            var accessCondition = string.IsNullOrEmpty(blob.Properties.ETag)
                ? AccessCondition.GenerateEmptyCondition()
                : AccessCondition.GenerateIfMatchCondition(blob.Properties.ETag);

            var s = new PreservedMemoryStream();

            using (var sw = new StreamWriter(s))
            using (var jw = new JsonTextWriter(sw))
            {
                JObject jo = JObject.FromObject(obj);
                jo.WriteTo(jw);
            }

            blob.UploadFromStream(s, accessCondition);
        }
    }
}