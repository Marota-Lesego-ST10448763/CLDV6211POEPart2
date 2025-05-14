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
        private readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=eventeasebookingls;AccountKey=kSbgJqMth8na4I51SWon2mYiozUbDyPZNzcvAa7m4hCBJysOvj/+TbSH0pSXdO02I2ndjdzhj22C+ASthTV1Sg==;EndpointSuffix=core.windows.net";

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
