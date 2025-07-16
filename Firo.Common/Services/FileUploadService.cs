using Microsoft.AspNetCore.Http;

namespace Firo.Common.Services
{
    public class FileUploadService
    {
        private readonly string _webRootPath;

        public FileUploadService(string webRootPath)
        {
            _webRootPath = webRootPath ?? throw new ArgumentNullException(nameof(webRootPath));
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is invalid.");

            // Create the folder path in wwwroot
            string folderPath = Path.Combine(_webRootPath, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            string cleanFileName = file.FileName
                .Trim()
                .Replace(" ", "_")
                .Replace("\t", "_")
                .Split(Path.GetInvalidFileNameChars())
                .Aggregate((x, y) => x + y);

            string fileName = $"{Guid.NewGuid()}_{cleanFileName}";
            string filePath = Path.Combine(folderPath, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderName, fileName); // Return the relative path
        }

        public async Task<string> UploadImageFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is invalid.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", "webp" };
            var fileExtension = Path.GetExtension(file.FileName)?.ToLower();

            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return null;
            }

            // Create the folder path in wwwroot
            string folderPath = Path.Combine(_webRootPath, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            string cleanFileName = file.FileName
                .Trim()
                .Replace(" ", "_")
                .Replace("\t", "_")
                .Split(Path.GetInvalidFileNameChars())
                .Aggregate((x, y) => x + y);

            string fileName = $"{Guid.NewGuid()}_{cleanFileName}";
            string filePath = Path.Combine(folderPath, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderName, fileName); // Return the relative path
        }
    }
}
