namespace TaskManager.Application.Common.Configuration
{
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";

        public string BasePath { get; set; } = "uploads";
        public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024; // 5MB default
        public string[] AllowedImageExtensions { get; set; } = [".jpg", ".jpeg", ".png"];
        public string DefaultProfileImagePath { get; set; } = "Assets/Images/default-profile.png";
    }
}
