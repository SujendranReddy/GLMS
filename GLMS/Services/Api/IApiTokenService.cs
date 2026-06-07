namespace GLMS.Services.Api
{
    public interface IApiTokenService
    {
        Task<string?> GetTokenAsync();
    }
}