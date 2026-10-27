using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HouseRentalManagementSystem.Services
{
    public interface IFileStorageService
    {
        Task<(bool Success, string? FilePath, string? ErrorMessage)> SaveImageAsync(IFormFile file, string folder = "properties");
        bool DeleteFile(string relativePath);
    }
}

