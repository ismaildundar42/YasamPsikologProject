using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    public interface IApiAuthService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(string email, string password);
    }

    public class AuthService : BaseApiService, IApiAuthService
    {
        private readonly ILogger<AuthService> _serviceLogger;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
            : base(httpClient, logger)
        {
            _serviceLogger = logger;
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(string email, string password)
        {
            var loginRequest = new { Email = email, Password = password };
            return await PostAsync<object, LoginResponseDto>("api/auth/login", loginRequest);
        }
    }
}
