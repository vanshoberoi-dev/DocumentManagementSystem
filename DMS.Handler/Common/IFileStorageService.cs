using Microsoft.AspNetCore.Http;

namespace DMS.Handlers.Common
{
    public interface IFileStorageService
    {
        Task<(string storedFileName, string filePath)> SaveFileAsync(IFormFile file);
        Task<byte[]> ReadFileAsync(string filePath);
        Task DeleteFileAsync(string filePath);
    }
}