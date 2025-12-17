using YasamPsikologProject.WebUi.Models.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace YasamPsikologProject.WebUi.Services
{
    public class BaseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BaseApiService> _logger;

        public BaseApiService(HttpClient httpClient, ILogger<BaseApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        protected async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<T>(content);
                    return new ApiResponse<T>
                    {
                        Success = true,
                        Data = data
                    };
                }

                _logger.LogWarning($"API GET failed: {endpoint} - Status: {response.StatusCode}");
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"API isteği başarısız: {response.StatusCode}",
                    Errors = new List<string> { content }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API GET exception: {endpoint}");
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Bağlantı hatası",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        protected async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonConvert.DeserializeObject<TResponse>(responseContent);
                    return new ApiResponse<TResponse>
                    {
                        Success = true,
                        Data = responseData
                    };
                }

                _logger.LogWarning($"API POST failed: {endpoint} - Status: {response.StatusCode}");
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = $"İşlem başarısız: {response.StatusCode}",
                    Errors = new List<string> { responseContent }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API POST exception: {endpoint}");
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = "Bağlantı hatası",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        protected async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonConvert.DeserializeObject<TResponse>(responseContent);
                    return new ApiResponse<TResponse>
                    {
                        Success = true,
                        Data = responseData
                    };
                }

                _logger.LogWarning($"API PUT failed: {endpoint} - Status: {response.StatusCode}");
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = $"Güncelleme başarısız: {response.StatusCode}",
                    Errors = new List<string> { responseContent }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API PUT exception: {endpoint}");
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = "Bağlantı hatası",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        protected async Task<ApiResponse> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse { Success = true, Message = "Silme işlemi başarılı" };
                }

                _logger.LogWarning($"API DELETE failed: {endpoint} - Status: {response.StatusCode}");
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Silme başarısız: {response.StatusCode}",
                    Errors = new List<string> { content }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API DELETE exception: {endpoint}");
                return new ApiResponse
                {
                    Success = false,
                    Message = "Bağlantı hatası",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
