using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    public interface IPsychologistService
    {
        Task<ApiResponse<List<PsychologistDto>>> GetAllAsync();
        Task<ApiResponse<PsychologistDto>> GetByIdAsync(int id);
        Task<ApiResponse<PsychologistDto>> CreateAsync(PsychologistDto psychologist);
        Task<ApiResponse<PsychologistDto>> UpdateAsync(int id, PsychologistDto psychologist);
        Task<ApiResponse> DeleteAsync(int id);
    }

    public class PsychologistService : BaseApiService, IPsychologistService
    {
        public PsychologistService(HttpClient httpClient, ILogger<PsychologistService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<ApiResponse<List<PsychologistDto>>> GetAllAsync()
        {
            return await GetAsync<List<PsychologistDto>>("api/psychologists");
        }

        public async Task<ApiResponse<PsychologistDto>> GetByIdAsync(int id)
        {
            return await GetAsync<PsychologistDto>($"api/psychologists/{id}");
        }

        public async Task<ApiResponse<PsychologistDto>> CreateAsync(PsychologistDto psychologist)
        {
            // DTO'yu API'nin beklediği formata dönüştür
            var createDto = new
            {
                FirstName = psychologist.User?.FirstName,
                LastName = psychologist.User?.LastName,
                Email = psychologist.User?.Email,
                PhoneNumber = psychologist.User?.PhoneNumber,
                LicenseNumber = psychologist.LicenseNumber,
                Specialization = psychologist.Specialization,
                CalendarColor = psychologist.CalendarColor,
                ConsultationFee = psychologist.ConsultationFee,
                IsActive = psychologist.IsActive
            };
            
            return await PostAsync<object, PsychologistDto>("api/psychologists", createDto);
        }

        public async Task<ApiResponse<PsychologistDto>> UpdateAsync(int id, PsychologistDto psychologist)
        {
            return await PutAsync<PsychologistDto, PsychologistDto>($"api/psychologists/{id}", psychologist);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            return await DeleteAsync($"api/psychologists/{id}");
        }
    }
}
