using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    public interface IApiUnavailableTimeService
    {
        Task<ApiResponse<List<UnavailableTimeDto>>> GetByPsychologistAsync(int psychologistId);
        Task<ApiResponse> CreateAsync(UnavailableTimeDto unavailableTime);
        Task<ApiResponse> DeleteAsync(int id);
    }

    public class UnavailableTimeService : BaseApiService, IApiUnavailableTimeService
    {
        public UnavailableTimeService(HttpClient httpClient, ILogger<UnavailableTimeService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<ApiResponse<List<UnavailableTimeDto>>> GetByPsychologistAsync(int psychologistId)
        {
            return await GetAsync<List<UnavailableTimeDto>>($"api/UnavailableTimes/psychologist/{psychologistId}");
        }

        public async Task<ApiResponse> CreateAsync(UnavailableTimeDto unavailableTime)
        {
            var response = await PostAsync<UnavailableTimeDto, object>("api/UnavailableTimes", unavailableTime);
            return new ApiResponse
            {
                Success = response.Success,
                Message = response.Message
            };
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            return await base.DeleteAsync($"api/UnavailableTimes/{id}");
        }
    }
}
