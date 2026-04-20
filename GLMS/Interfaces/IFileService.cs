namespace GLMS.Interfaces
{
    public interface IFileService
    {
        bool IsPdf(IFormFile file);
        Task<string?> SavePdfAsync(IFormFile file);
    }
}