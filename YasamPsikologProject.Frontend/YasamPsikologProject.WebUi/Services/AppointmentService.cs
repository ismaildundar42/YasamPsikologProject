using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    /// <summary>
    /// API üzerinden Randevu CRUD işlemlerini yöneten HTTP Client Service
    /// </summary>
    public interface IApiAppointmentService
    {
        Task<ApiResponse<List<AppointmentDto>>> GetAllAsync();
        Task<ApiResponse<AppointmentDto>> GetByIdAsync(int id);
        Task<ApiResponse<List<AppointmentDto>>> GetByPsychologistAsync(int psychologistId);
        Task<ApiResponse<List<DateTime>>> GetAvailableSlotsAsync(int psychologistId, DateTime date, int duration, string? clientEmail = null, string? clientPhone = null);
        Task<ApiResponse<AppointmentDto>> CreateAsync(AppointmentDto appointment);
        Task<ApiResponse<AppointmentDto>> UpdateAsync(int id, AppointmentDto appointment);
        Task<ApiResponse> UpdateStatusAsync(int id, string status, string? reason = null);
        Task<ApiResponse> CancelAsync(int id, string reason);
    }

    public class AppointmentService : BaseApiService, IApiAppointmentService
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

        public async Task<ApiResponse<List<DateTime>>> GetAvailableSlotsAsync(int psychologistId, DateTime date, int duration, string? clientEmail = null, string? clientPhone = null)
        {
            var url = $"api/appointments/available-slots?psychologistId={psychologistId}&date={date:yyyy-MM-dd}&duration={duration}";
            
            if (!string.IsNullOrWhiteSpace(clientEmail))
            {
                url += $"&clientEmail={Uri.EscapeDataString(clientEmail)}";
            }
            else if (!string.IsNullOrWhiteSpace(clientPhone))
            {
                url += $"&clientPhone={Uri.EscapeDataString(clientPhone)}";
            }
            
            return await GetAsync<List<DateTime>>(url);
        }

        public async Task<ApiResponse<AppointmentDto>> CreateAsync(AppointmentDto appointment)
        {
            return await PostAsync<AppointmentDto, AppointmentDto>("api/appointments", appointment);
        }

        public async Task<ApiResponse<AppointmentDto>> UpdateAsync(int id, AppointmentDto appointment)
        {
            return await PutAsync<AppointmentDto, AppointmentDto>($"api/appointments/{id}", appointment);
        }

        public async Task<ApiResponse> UpdateStatusAsync(int id, string status, string? reason = null)
        {
            // API endpoint'e doğru formatta gönder - DTO property isimleri büyük harfle başlamalı
            var requestData = new 
            { 
                Status = status,
                Reason = reason 
            };
            
            var response = await PatchAsync<object, object>($"api/appointments/{id}/status", requestData);
            return new ApiResponse
            {
                Success = response.Success,
                Message = response.Message,
                Errors = response.Errors
            };
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
