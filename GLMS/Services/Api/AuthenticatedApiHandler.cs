using System.Net.Http.Headers;

namespace GLMS.Services.Api
{
    public class AuthenticatedApiHandler : DelegatingHandler
    {
        private readonly IApiTokenService _apiTokenService;

        public AuthenticatedApiHandler(IApiTokenService apiTokenService)
        {
            _apiTokenService = apiTokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _apiTokenService.GetTokenAsync();

            // Adds the JWT to MVC requests sent to the protected API.
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}