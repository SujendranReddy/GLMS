using GLMS.Interfaces;

namespace GLMS.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public bool IsPdf(IFormFile file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.FileName))
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName);

            return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string?> SavePdfAsync(IFormFile file)
        {
            if (!IsPdf(file))
            {
                return null;
            }

            var uploadsFolder = GetUploadsFolder();

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid() + ".pdf";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        public string? GetPdfPath(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            // Prevents path traversal by only allowing stored file names.
            var safeFileName = Path.GetFileName(fileName);

            if (!string.Equals(safeFileName, fileName, StringComparison.Ordinal))
            {
                return null;
            }

            if (!safeFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var uploadsFolder = GetUploadsFolder();
            var filePath = Path.Combine(uploadsFolder, safeFileName);

            return File.Exists(filePath) ? filePath : null;
        }

        public bool DeletePdf(string? fileName)
        {
            var filePath = GetPdfPath(fileName);

            if (filePath == null)
            {
                return false;
            }

            File.Delete(filePath);

            return true;
        }

        private string GetUploadsFolder()
        {
            var webRootPath = _environment.WebRootPath;

            // Docker may not always provide WebRootPath.
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            return Path.Combine(webRootPath, "uploads");
        }
    }
}