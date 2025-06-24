using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EventEase.Services
{
    public class AzureBlobStorage : IAzureBlobStorage
    {
        // 🔒 Old hardcoded connection string (restored by request)  HERE LESEGO
        private readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=eventeasebookingsa;AccountKey=0j3wHs9fflbfSO6gM7XOCV+0VTsy6nl5B5SExy6lM0D6iMBr+4j9Jxxl6mkLTqWQhLPD5NNIB367+AStoCde+w==;EndpointSuffix=core.windows.net";

        // 🔒 Static container name
        private readonly string _containerName = "eventeasecontainer";

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var blobClient = new BlobContainerClient(_connectionString, _containerName);
            await blobClient.CreateIfNotExistsAsync();
            await blobClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blob = blobClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, overwrite: true);
            }

            return blob.Uri.ToString();
        }
    }
}
