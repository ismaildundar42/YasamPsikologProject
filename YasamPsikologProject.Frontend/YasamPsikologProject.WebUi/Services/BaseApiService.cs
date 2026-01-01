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
                    _logger.LogInformation("API Response for {Endpoint}: {Content}", endpoint, content.Length > 500 ? content.Substring(0, 500) + "..." : content);
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
                    _logger.LogInformation("API POST success: {Endpoint} - Status: {StatusCode}", endpoint, response.StatusCode);
                    
                    var responseData = JsonConvert.DeserializeObject<TResponse>(responseContent);
                    return new ApiResponse<TResponse>
                    {
                        Success = true,
                        Data = responseData,
                        Message = "İşlem başarılı"
                    };
                }

                // API'den gelen hata mesajını parse et
                string errorMessage = $"İşlem başarısız: {response.StatusCode}";
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    if (errorResponse?.message != null)
                    {
                        errorMessage = errorResponse.message.ToString();
                    }
                    else if (errorResponse?.errors != null)
                    {
                        errorMessage = JsonConvert.SerializeObject(errorResponse.errors);
                    }
                }
                catch
                {
                    // JSON parse edilemezse raw content kullan
                    errorMessage = responseContent;
                }

                _logger.LogWarning("API POST failed: {Endpoint} - Status: {StatusCode} - Error: {Error}", endpoint, response.StatusCode, errorMessage);
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = errorMessage,
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

        protected async Task<ApiResponse<TResponse>> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };
                
                var response = await _httpClient.SendAsync(request);
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

                _logger.LogWarning($"API PATCH failed: {endpoint} - Status: {response.StatusCode}");
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = $"Güncelleme başarısız: {response.StatusCode}",
                    Errors = new List<string> { responseContent }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API PATCH exception: {endpoint}");
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
