using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    /// <summary>
    /// API üzerinden Danışan CRUD işlemlerini yöneten HTTP Client Service
    /// </summary>
    public interface IApiClientService
    {
        Task<ApiResponse<List<ClientDto>>> GetAllAsync();
        Task<ApiResponse<ClientDto>> GetByIdAsync(int id);
        Task<ApiResponse<ClientDto>> CreateAsync(ClientDto client);
        Task<ApiResponse<ClientDto>> UpdateAsync(int id, ClientDto client);
        Task<ApiResponse> DeleteAsync(int id);
    }

    public class ClientService : BaseApiService, IApiClientService
    {
        public ClientService(HttpClient httpClient, ILogger<ClientService> logger)
            : base(httpClient, logger)
        {
        }

        public async Task<ApiResponse<List<ClientDto>>> GetAllAsync()
        {
            return await GetAsync<List<ClientDto>>("api/clients");
        }

        public async Task<ApiResponse<ClientDto>> GetByIdAsync(int id)
        {
            return await GetAsync<ClientDto>($"api/clients/{id}");
        }

        public async Task<ApiResponse<List<ClientDto>>> GetByPsychologistAsync(int psychologistId)
        {
            return await GetAsync<List<ClientDto>>($"api/clients/psychologist/{psychologistId}");
        }

        public async Task<ApiResponse<ClientDto>> CreateAsync(ClientDto client)
        {
            // DTO'yu API'nin beklediği formata dönüştür
            var createDto = new
            {
                FirstName = client.User?.FirstName,
                LastName = client.User?.LastName,
                Email = client.User?.Email,
                PhoneNumber = client.User?.PhoneNumber,
                AssignedPsychologistId = client.AssignedPsychologistId,
                PreferredNotificationMethod = client.PreferredNotificationMethod ?? "Email",
                KvkkConsent = client.KvkkConsent
            };
            
            return await PostAsync<object, ClientDto>("api/clients", createDto);
        }

        public async Task<ApiResponse<ClientDto>> UpdateAsync(int id, ClientDto client)
        {
            // DTO'yu API'nin beklediği formata dönüştür (UpdateClientDto)
            var updateDto = new
            {
                FirstName = client.User?.FirstName,
                LastName = client.User?.LastName,
                Email = client.User?.Email,
                PhoneNumber = client.User?.PhoneNumber,
                AssignedPsychologistId = client.AssignedPsychologistId,
                PreferredNotificationMethod = client.PreferredNotificationMethod ?? "Email",
                KvkkConsent = client.KvkkConsent
            };
            
            return await PutAsync<object, ClientDto>($"api/clients/{id}", updateDto);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            return await DeleteAsync($"api/clients/{id}");
        }
    }
}
