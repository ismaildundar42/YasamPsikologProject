using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    public interface IApiSystemSettingService
    {
        Task<ApiResponse<List<SystemSettingDto>>> GetAllAsync();
        Task<ApiResponse<SystemSettingDto>> GetByKeyAsync(string key);
        Task<ApiResponse> UpdateAsync(SystemSettingDto setting);
    }

    public class SystemSettingService : BaseApiService, IApiSystemSettingService
    {
        public SystemSettingService(HttpClient httpClient, ILogger<SystemSettingService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<ApiResponse<List<SystemSettingDto>>> GetAllAsync()
        {
            return await GetAsync<List<SystemSettingDto>>("api/SystemSettings");
        }

        public async Task<ApiResponse<SystemSettingDto>> GetByKeyAsync(string key)
        {
            return await GetAsync<SystemSettingDto>($"api/SystemSettings/key/{key}");
        }

        public async Task<ApiResponse> UpdateAsync(SystemSettingDto setting)
        {
            var response = await PutAsync<SystemSettingDto, SystemSettingDto>($"api/SystemSettings/{setting.Id}", setting);
            return new ApiResponse
            {
                Success = response.Success,
                Message = response.Message
            };
        }
    }
}
