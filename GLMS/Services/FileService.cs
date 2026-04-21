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

        // This checks that the uploaded file is a pdf
        public bool IsPdf(IFormFile file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.FileName))
                return false;

            var extension = Path.GetExtension(file.FileName);
            return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string?> SavePdfAsync(IFormFile file)
        {
            if (!IsPdf(file))
                return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + ".pdf";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
    }
}