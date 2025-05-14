using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EventEase.Services
{
    public interface IAzureBlobStorage
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
