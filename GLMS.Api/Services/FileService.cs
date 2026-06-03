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

        // Checks that the uploaded file has a PDF extension.
        public bool IsPdf(IFormFile file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.FileName))
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName);

            return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        // Saves an uploaded PDF using a generated filename.
        public async Task<string?> SavePdfAsync(IFormFile file)
        {
            if (!IsPdf(file))
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

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

        // Safely locates a stored PDF for download.
        public string? GetPdfPath(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            var safeFileName = Path.GetFileName(fileName);

            if (!string.Equals(safeFileName, fileName, StringComparison.Ordinal))
            {
                return null;
            }

            if (!safeFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsFolder, safeFileName);

            return File.Exists(filePath) ? filePath : null;
        }

        // Deletes a stored PDF agreement when its contract is deleted.
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
    }
}