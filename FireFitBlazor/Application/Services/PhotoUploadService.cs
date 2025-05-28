using FireFitBlazor.Domain.ContextInterfaces;

using FireFitBlazor.Domain.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace FireFitBlazor.Application.Services
{

    public class PhotoUploadService : IPhotoUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PhotoUploadService> _logger;
        private readonly IUserProgressContext _userProgressContext;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

        public PhotoUploadService(
            IWebHostEnvironment environment,
        ILogger<PhotoUploadService> logger,
            IUserProgressContext userProgressContext)
        {
            _environment = environment;
            _logger = logger;
            _userProgressContext = userProgressContext;
        }

        public async Task<string> UploadFileAsync(string userId, IBrowserFile file)
        {
            if (file.Size > MaxFileSize)
                throw new ArgumentException("File size exceeds maximum limit of 10MB");

            var extension = Path.GetExtension(file.Name).ToLowerInvariant();
            if (!IsAllowedExtension(extension))
                throw new ArgumentException("Invalid file type. Only .jpg, .jpeg, and .png files are allowed.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var relativePath = Path.Combine("uploads", "users", userId, fileName);
            var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var stream = file.OpenReadStream(MaxFileSize))
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return relativePath;
        }

        private bool IsAllowedExtension(string extension)
        {
            return new[] { ".jpg", ".jpeg", ".png" }.Contains(extension);
        }

        public void DeletePhoto(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting photo {FilePath}", filePath);
                throw new ApplicationException("Failed to delete photo", ex);
            }
        }
    }
}
