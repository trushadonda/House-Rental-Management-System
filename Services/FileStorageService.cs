using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HouseRentalManagementSystem.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileStorageService> _logger;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<(bool Success, string? FilePath, string? ErrorMessage)> SaveImageAsync(IFormFile file, string folder = "properties")
        {
            if (file == null || file.Length == 0)
            {
                return (false, null, "No file uploaded.");
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                return (false, null, "File size exceeds the maximum limit of 5 MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            {
                return (false, null, $"Invalid file type ({extension}). Only JPG, JPEG, PNG, and WEBP formats are allowed.");
            }

            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var relativeUrl = $"/uploads/{folder}/{uniqueFileName}";
                return (true, relativeUrl, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving uploaded file.");
                return (false, null, "Failed to save the uploaded image. Please try again.");
            }
        }

        public bool DeleteFile(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return false;

            // Don't delete default placeholder/stock images
            if (relativePath.StartsWith("/images/")) return false;

            try
            {
                var normalizedPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(_env.WebRootPath, normalizedPath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file at {Path}", relativePath);
            }

            return false;
        }
    }
}

