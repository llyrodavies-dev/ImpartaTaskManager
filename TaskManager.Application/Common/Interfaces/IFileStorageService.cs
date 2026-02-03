namespace TaskManager.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default);
        Task<(Stream FileStream, string ContentType)> GetFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
        bool FileExists(string filePath);
        Task<(Stream FileStream, string ContentType)> GetDefaultProfileImageAsync(CancellationToken cancellationToken = default);
    }
}
