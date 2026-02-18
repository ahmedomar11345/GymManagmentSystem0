using GymManagmentBLL.Service.Interfaces.AttachmentService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GymManagmentBLL.Service.Classes.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly ILogger<AttachmentService> _logger;
        
        // Configuration
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/pjpeg" };
        private readonly long _maxFileSize = 3 * 1024 * 1024; // 3 MB

        public AttachmentService(IWebHostEnvironment webHost, ILogger<AttachmentService> logger)
        {
            _webHost = webHost;
            _logger = logger;
        }

        public string? Upload(string folderName, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return null;

                // 1. Size Validation
                if (file.Length > _maxFileSize)
                {
                    _logger.LogWarning("File upload rejected: Size {Size} exceeds limit", file.Length);
                    return null;
                }

                // 2. Extension Validation
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    _logger.LogWarning("File upload rejected: Extension {Ext} not allowed", extension);
                    return null;
                }

                // 3. MIME Type Validation
                if (!_allowedMimeTypes.Contains(file.ContentType.ToLower()))
                {
                    _logger.LogWarning("File upload rejected: MIME type {Mime} not allowed", file.ContentType);
                    return null;
                }

                // 4. Content Validation (Magic Bytes)
                if (!ValidateFileSignature(file, extension))
                {
                    _logger.LogWarning("File upload rejected: Magic bytes validation failed for {File}", file.FileName);
                    return null;
                }

                var folderPath = Path.Combine(_webHost.WebRootPath, "images", folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                _logger.LogInformation("File uploaded successfully: {FileName} to {Folder}", fileName, folderName);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file to folder {Folder}", folderName);
                return null;
            }
        }

        public bool Delete(string fileName, string folderName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return false;

                var fullPath = Path.Combine(_webHost.WebRootPath, "images", folderName, fileName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted: {Path}", fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file {File} from {Folder}", fileName, folderName);
                return false;
            }
        }

        private bool ValidateFileSignature(IFormFile file, string extension)
        {
            var signatures = new Dictionary<string, List<byte[]>>
            {
                { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 } } },
                { ".jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 } } },
                { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } }
            };

            if (!signatures.ContainsKey(extension)) return false;

            using var reader = new BinaryReader(file.OpenReadStream());
            var headerBytes = reader.ReadBytes(8);
            
            return signatures[extension].Any(signature => 
                headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }
}
