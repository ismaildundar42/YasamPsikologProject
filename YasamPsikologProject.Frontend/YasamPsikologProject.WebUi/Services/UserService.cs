using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    /// <summary>
    /// API üzerinden User işlemlerini yöneten HTTP Client Service
    /// </summary>
    public interface IApiUserService
    {
        Task<ApiResponse> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }

    public class UserService : BaseApiService, IApiUserService
    {
        private readonly ILogger<UserService> _serviceLogger;

        public UserService(HttpClient httpClient, ILogger<UserService> logger)
            : base(httpClient, logger)
        {
            _serviceLogger = logger;
        }

        public async Task<ApiResponse> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var changePasswordDto = new
                {
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                _serviceLogger.LogInformation("Şifre değiştirme isteği: UserId={UserId}", userId);
                var response = await PostAsync<object, object>($"api/users/{userId}/change-password", changePasswordDto);
                
                return new ApiResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _serviceLogger.LogError(ex, "Şifre değiştirme hatası: {Message}", ex.Message);
                return new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
