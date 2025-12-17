using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    public interface IAppointmentService
    {
        Task<ApiResponse<List<AppointmentDto>>> GetAllAsync();
        Task<ApiResponse<AppointmentDto>> GetByIdAsync(int id);
        Task<ApiResponse<List<AppointmentDto>>> GetByPsychologistAsync(int psychologistId);
        Task<ApiResponse<List<DateTime>>> GetAvailableSlotsAsync(int psychologistId, DateTime date, string duration);
        Task<ApiResponse<AppointmentDto>> CreateAsync(AppointmentDto appointment);
        Task<ApiResponse> CancelAsync(int id, string reason);
    }

    public class AppointmentService : BaseApiService, IAppointmentService
    {
        public AppointmentService(HttpClient httpClient, ILogger<AppointmentService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetAllAsync()
        {
            return await GetAsync<List<AppointmentDto>>("api/appointments");
        }

        public async Task<ApiResponse<AppointmentDto>> GetByIdAsync(int id)
        {
            return await GetAsync<AppointmentDto>($"api/appointments/{id}");
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetByPsychologistAsync(int psychologistId)
        {
            return await GetAsync<List<AppointmentDto>>($"api/appointments/psychologist/{psychologistId}");
        }

        public async Task<ApiResponse<List<DateTime>>> GetAvailableSlotsAsync(int psychologistId, DateTime date, string duration)
        {
            return await GetAsync<List<DateTime>>($"api/appointments/available-slots?psychologistId={psychologistId}&date={date:yyyy-MM-dd}&duration={duration}");
        }

        public async Task<ApiResponse<AppointmentDto>> CreateAsync(AppointmentDto appointment)
        {
            return await PostAsync<AppointmentDto, AppointmentDto>("api/appointments", appointment);
        }

        public async Task<ApiResponse> CancelAsync(int id, string reason)
        {
            var response = await PostAsync<object, object>($"api/appointments/{id}/cancel", new { reason });
            return new ApiResponse
            {
                Success = response.Success,
                Message = response.Message,
                Errors = response.Errors
            };
        }
    }
}
