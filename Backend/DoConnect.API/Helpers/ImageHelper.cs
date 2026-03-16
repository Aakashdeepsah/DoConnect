// Helpers/ImageHelper.cs
namespace DoConnect.API.Helpers
{
    public class ImageHelper
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public ImageHelper(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var maxSize = _config.GetValue<long>("FileUpload:MaxFileSizeBytes", 5242880);
            if (imageFile.Length > maxSize)
                throw new InvalidOperationException($"File too large. Max {maxSize / 1024 / 1024}MB.");

            var allowed = _config.GetSection("FileUpload:AllowedExtensions").Get<string[]>()
                          ?? new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                throw new InvalidOperationException($"File type '{ext}' not allowed.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return fileName;
        }

        public void DeleteImage(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        public static string? BuildImageUrl(string? fileName, string baseUrl)
        {
            if (string.IsNullOrEmpty(fileName)) return null;
            if (!string.IsNullOrEmpty(baseUrl))
                return $"{baseUrl.TrimEnd('/')}/uploads/{fileName}";
            return $"/uploads/{fileName}";
        }
    }
}
