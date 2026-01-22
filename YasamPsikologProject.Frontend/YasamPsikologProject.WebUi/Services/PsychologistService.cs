using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    /// <summary>
    /// API üzerinden Psikolog CRUD işlemlerini yöneten HTTP Client Service
    /// </summary>
    public interface IApiPsychologistService
    {
        Task<ApiResponse<List<PsychologistDto>>> GetAllAsync();
        Task<ApiResponse<PsychologistDto>> GetByIdAsync(int id);
        Task<ApiResponse<PsychologistDto>> CreateAsync(PsychologistDto psychologist);
        Task<ApiResponse<PsychologistDto>> UpdateAsync(int id, PsychologistDto psychologist);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse<List<PsychologistArchiveDto>>> GetArchivedAsync();
    }

    public class PsychologistService : BaseApiService, IApiPsychologistService
    {
        private readonly ILogger<PsychologistService> _serviceLogger;

        public PsychologistService(HttpClient httpClient, ILogger<PsychologistService> logger)
            : base(httpClient, logger)
        {
            _serviceLogger = logger;
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
            try
            {
                // DTO'yu API'nin beklediği formata dönüştür
                var createDto = new
                {
                    FirstName = psychologist.User?.FirstName ?? throw new ArgumentNullException(nameof(psychologist.User.FirstName), "Ad alanı boş olamaz"),
                    LastName = psychologist.User?.LastName ?? throw new ArgumentNullException(nameof(psychologist.User.LastName), "Soyad alanı boş olamaz"),
                    Email = psychologist.User?.Email ?? throw new ArgumentNullException(nameof(psychologist.User.Email), "Email alanı boş olamaz"),
                    PhoneNumber = psychologist.User?.PhoneNumber ?? throw new ArgumentNullException(nameof(psychologist.User.PhoneNumber), "Telefon alanı boş olamaz"),
                    Password = psychologist.User?.Password ?? throw new ArgumentNullException(nameof(psychologist.User.Password), "Şifre alanı boş olamaz"),
                    CalendarColor = psychologist.CalendarColor ?? "#4CAF50",
                    IsOnlineConsultationAvailable = psychologist.IsOnlineConsultationAvailable,
                    IsInPersonConsultationAvailable = psychologist.IsInPersonConsultationAvailable,
                    IsActive = psychologist.IsActive
                };
                
                _serviceLogger.LogInformation("API'ye psikolog gönderiliyor: {Email}", createDto.Email);
                var response = await PostAsync<object, PsychologistDto>("api/psychologists", createDto);
                _serviceLogger.LogInformation("API yanıtı: Success={Success}, Message={Message}", response.Success, response.Message);
                
                return response;
            }
            catch (ArgumentNullException ex)
            {
                _serviceLogger.LogError(ex, "Psikolog oluşturma hatası: {Message}", ex.Message);
                return new ApiResponse<PsychologistDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<PsychologistDto>> UpdateAsync(int id, PsychologistDto psychologist)
        {
            // DTO'yu API'nin beklediği formata dönüştür (UpdatePsychologistDto)
            var updateDto = new
            {
                FirstName = psychologist.User?.FirstName ?? throw new ArgumentNullException(nameof(psychologist.User.FirstName)),
                LastName = psychologist.User?.LastName ?? throw new ArgumentNullException(nameof(psychologist.User.LastName)),
                Email = psychologist.User?.Email ?? throw new ArgumentNullException(nameof(psychologist.User.Email)),
                PhoneNumber = psychologist.User?.PhoneNumber ?? throw new ArgumentNullException(nameof(psychologist.User.PhoneNumber)),
                Password = psychologist.User?.Password, // Opsiyonel - boş ise güncellenmez
                CalendarColor = psychologist.CalendarColor ?? "#4CAF50",
                IsOnlineConsultationAvailable = psychologist.IsOnlineConsultationAvailable,
                IsInPersonConsultationAvailable = psychologist.IsInPersonConsultationAvailable,
                IsActive = psychologist.IsActive
            };
            
            return await PutAsync<object, PsychologistDto>($"api/psychologists/{id}", updateDto);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            return await DeleteAsync($"api/psychologists/{id}");
        }

        public async Task<ApiResponse<List<PsychologistArchiveDto>>> GetArchivedAsync()
        {
            return await GetAsync<List<PsychologistArchiveDto>>("api/psychologists/archived");
        }
    }
}
