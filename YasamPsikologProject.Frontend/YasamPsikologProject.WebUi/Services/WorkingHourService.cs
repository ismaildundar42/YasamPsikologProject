using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    public interface IApiWorkingHourService
    {
        Task<ApiResponse<List<WorkingHourDto>>> GetAllAsync();
        Task<ApiResponse<List<WorkingHourDto>>> GetAllByPsychologistAsync(int psychologistId);
        Task<ApiResponse<WorkingHourDto>> GetByIdAsync(int id);
        Task<ApiResponse<WorkingHourDto>> CreateAsync(WorkingHourDto workingHour);
        Task<ApiResponse<WorkingHourDto>> UpdateAsync(int id, WorkingHourDto workingHour);
        Task<ApiResponse> DeleteAsync(int id);
    }

    public class WorkingHourService : BaseApiService, IApiWorkingHourService
    {
        private readonly ILogger<WorkingHourService> _serviceLogger;

        public WorkingHourService(HttpClient httpClient, ILogger<WorkingHourService> logger)
            : base(httpClient, logger)
        {
            _serviceLogger = logger;
        }

        public async Task<ApiResponse<List<WorkingHourDto>>> GetAllAsync()
        {
            return await GetAsync<List<WorkingHourDto>>("api/workinghours");
        }

        public async Task<ApiResponse<List<WorkingHourDto>>> GetAllByPsychologistAsync(int psychologistId)
        {
            return await GetAsync<List<WorkingHourDto>>($"api/workinghours/psychologist/{psychologistId}");
        }

        public async Task<ApiResponse<WorkingHourDto>> GetByIdAsync(int id)
        {
            return await GetAsync<WorkingHourDto>($"api/workinghours/{id}");
        }

        public async Task<ApiResponse<WorkingHourDto>> CreateAsync(WorkingHourDto workingHour)
        {
            return await PostAsync<WorkingHourDto, WorkingHourDto>("api/workinghours", workingHour);
        }

        public async Task<ApiResponse<WorkingHourDto>> UpdateAsync(int id, WorkingHourDto workingHour)
        {
            return await PutAsync<WorkingHourDto, WorkingHourDto>($"api/workinghours/{id}", workingHour);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            return await DeleteAsync($"api/workinghours/{id}");
        }
    }
}
