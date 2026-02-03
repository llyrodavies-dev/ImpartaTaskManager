using Microsoft.Extensions.Options;
using TaskManager.Application.Common.Configuration;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _baseStoragePath;
        private readonly string _defaultProfileImagePath;
        private static readonly Dictionary<string, string> _mimeTypes = new()
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" }
        };

        public FileStorageService(IOptions<FileStorageOptions> options)
        {
            var basePath = options.Value.BasePath;
            _baseStoragePath = Path.IsPathRooted(basePath)
                ? basePath
                : Path.Combine(AppContext.BaseDirectory, basePath);

            if (!Directory.Exists(_baseStoragePath))
            {
                Directory.CreateDirectory(_baseStoragePath);
            }

            // Set up default profile image path from configuration
            var defaultImagePath = options.Value.DefaultProfileImagePath;
            _defaultProfileImagePath = Path.IsPathRooted(defaultImagePath)
                ? defaultImagePath
                : Path.Combine(AppContext.BaseDirectory, defaultImagePath);
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
        {
            var folderPath = Path.Combine(_baseStoragePath, folder);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

            return Path.Combine(folder, uniqueFileName);
        }

        public async Task<(Stream FileStream, string ContentType)> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(_baseStoragePath, filePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            var extension = Path.GetExtension(fullPath).ToLowerInvariant();
            var contentType = _mimeTypes.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";

            return (fileStream, contentType);
        }

        public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(_baseStoragePath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public bool FileExists(string filePath)
        {
            var fullPath = Path.Combine(_baseStoragePath, filePath);
            return File.Exists(fullPath);
        }

        public async Task<(Stream FileStream, string ContentType)> GetDefaultProfileImageAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_defaultProfileImagePath))
            {
                throw new FileNotFoundException("Default profile image not found.", _defaultProfileImagePath);
            }

            var fileStream = new FileStream(_defaultProfileImagePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            var extension = Path.GetExtension(_defaultProfileImagePath).ToLowerInvariant();
            var contentType = _mimeTypes.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";

            return (fileStream, contentType);
        }
    }
}