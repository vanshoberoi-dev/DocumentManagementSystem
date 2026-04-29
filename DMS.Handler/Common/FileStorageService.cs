using Microsoft.AspNetCore.Http;

namespace DMS.Handlers.Common
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _uploadsPath;

        public FileStorageService(string uploadsPath)
        {
            _uploadsPath = uploadsPath;

            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
        }

        public async Task<(string storedFileName, string filePath)> SaveFileAsync(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            string storedFileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(_uploadsPath, storedFileName);
            string relativePath = Path.Combine("uploads", storedFileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (storedFileName, relativePath);
        }

        public async Task<byte[]> ReadFileAsync(string filePath)
        {
            string fullPath = Path.Combine(
                Directory.GetParent(_uploadsPath)!.FullName,
                filePath
            );
            return await File.ReadAllBytesAsync(fullPath);
        }

        public async Task DeleteFileAsync(string filePath)
        {
            string fullPath = Path.Combine(
                Directory.GetParent(_uploadsPath)!.FullName,
                filePath
            );

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }
    }
}