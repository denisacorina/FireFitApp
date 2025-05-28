using Microsoft.AspNetCore.Components.Forms;

namespace FireFitBlazor.Domain.Interfaces
{
    public interface IPhotoUploadService
    {
        public Task<string> UploadFileAsync(string userId, IBrowserFile file);
        public void DeletePhoto(string filePath);
    }
}
