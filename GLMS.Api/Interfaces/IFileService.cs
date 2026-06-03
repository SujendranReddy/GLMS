namespace GLMS.Interfaces
{
    public interface IFileService
    {
        bool IsPdf(IFormFile file);

        Task<string?> SavePdfAsync(IFormFile file);

        string? GetPdfPath(string? fileName);

        bool DeletePdf(string? fileName);
    }
}