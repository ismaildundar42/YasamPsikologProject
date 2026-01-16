using YasamPsikologProject.WebUi.Models.DTOs;

namespace YasamPsikologProject.WebUi.Services
{
    /// <summary>
    /// API üzerinden SuperAdmin işlemlerini yöneten HTTP Client Service
    /// </summary>
    public interface IApiSuperAdminService
    {
        Task<ApiResponse<List<SuperAdminDto>>> GetAllAsync();
        Task<ApiResponse<SuperAdminDto>> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(SuperAdminDto superAdmin);
        Task<ApiResponse> UpdateAsync(SuperAdminDto superAdmin);
        Task<ApiResponse> DeleteAsync(int id);
    }

    public class SuperAdminService : BaseApiService, IApiSuperAdminService
    {
        private readonly ILogger<SuperAdminService> _serviceLogger;

        public SuperAdminService(HttpClient httpClient, ILogger<SuperAdminService> logger)
            : base(httpClient, logger)
        {
            _serviceLogger = logger;
        }

        public async Task<ApiResponse<List<SuperAdminDto>>> GetAllAsync()
        {
            try
            {
                _serviceLogger.LogInformation("Tüm süper adminleri getirme isteği yapılıyor...");
                
                // Önce tüm kullanıcıları çek
                var usersResponse = await GetAsync<List<UserDto>>("api/users");
                
                if (!usersResponse.Success || usersResponse.Data == null)
                {
                    return new ApiResponse<List<SuperAdminDto>>
                    {
                        Success = false,
                        Message = "Kullanıcılar getirilemedi.",
                        Data = new List<SuperAdminDto>()
                    };
                }

                // Sadece SuperAdmin rolündeki kullanıcıları filtrele (Role = 0)
                var superAdmins = usersResponse.Data
                    .Where(u => u.Role != null && (u.Role == "0" || u.Role.ToLower() == "superadmin"))
                    .Select(u => new SuperAdminDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Gender = u.Gender,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt
                    })
                    .ToList();

                _serviceLogger.LogInformation("Toplam {Count} süper admin bulundu", superAdmins.Count);

                return new ApiResponse<List<SuperAdminDto>>
                {
                    Success = true,
                    Data = superAdmins
                };
            }
            catch (Exception ex)
            {
                _serviceLogger.LogError(ex, "Süper adminler getirilirken hata: {Message}", ex.Message);
                return new ApiResponse<List<SuperAdminDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new List<SuperAdminDto>()
                };
            }
        }

        public async Task<ApiResponse<SuperAdminDto>> GetByIdAsync(int id)
        {
            try
            {
                _serviceLogger.LogInformation("Süper admin getirme isteği: ID={Id}", id);
                
                var response = await GetAsync<UserDto>($"api/users/{id}");
                
                if (!response.Success || response.Data == null)
                {
                    return new ApiResponse<SuperAdminDto>
                    {
                        Success = false,
                        Message = "Süper admin bulunamadı."
                    };
                }

                var user = response.Data;
                
                // Role kontrolü
                if (user.Role == null || (user.Role != "0" && user.Role.ToLower() != "superadmin"))
                {
                    return new ApiResponse<SuperAdminDto>
                    {
                        Success = false,
                        Message = "Kullanıcı süper admin değil."
                    };
                }

                var superAdmin = new SuperAdminDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                return new ApiResponse<SuperAdminDto>
                {
                    Success = true,
                    Data = superAdmin
                };
            }
            catch (Exception ex)
            {
                _serviceLogger.LogError(ex, "Süper admin getirilirken hata: {Message}", ex.Message);
                return new ApiResponse<SuperAdminDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> CreateAsync(SuperAdminDto superAdmin)
        {
            try
            {
                _serviceLogger.LogInformation("Yeni süper admin oluşturma isteği: {Email}", superAdmin.Email);

                // User nesnesini oluştur
                var user = new
                {
                    FirstName = superAdmin.FirstName,
                    LastName = superAdmin.LastName,
                    Email = superAdmin.Email,
                    PhoneNumber = superAdmin.PhoneNumber,
                    PasswordHash = superAdmin.Password, // API'de hashlenecek
                    Role = 0, // SuperAdmin
                    Gender = superAdmin.Gender,
                    IsActive = true
                };

                var response = await PostAsync<object, UserDto>("api/users", user);

                if (response.Success)
                {
                    _serviceLogger.LogInformation("Süper admin başarıyla oluşturuldu: {Email}", superAdmin.Email);
                }
                else
                {
                    _serviceLogger.LogWarning("Süper admin oluşturulamadı: {Message}", response.Message);
                }

                return new ApiResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _serviceLogger.LogError(ex, "Süper admin oluşturma hatası: {Message}", ex.Message);
                return new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> UpdateAsync(SuperAdminDto superAdmin)
        {
            try
            {
                _serviceLogger.LogInformation("Süper admin güncelleme isteği: ID={Id}", superAdmin.Id);

                // Şifre değişikliği varsa ekle
                object user;
                if (!string.IsNullOrWhiteSpace(superAdmin.Password))
                {
                    user = new
                    {
                        FirstName = superAdmin.FirstName,
                        LastName = superAdmin.LastName,
                        Email = superAdmin.Email,
                        PhoneNumber = superAdmin.PhoneNumber,
                        PasswordHash = superAdmin.Password, // Yeni şifre
                        Gender = superAdmin.Gender,
                        IsActive = superAdmin.IsActive
                    };
                    _serviceLogger.LogInformation("Güncelleme verisi (şifreli): {@User}", user);
                }
                else
                {
                    user = new
                    {
                        FirstName = superAdmin.FirstName,
                        LastName = superAdmin.LastName,
                        Email = superAdmin.Email,
                        PhoneNumber = superAdmin.PhoneNumber,
                        Gender = superAdmin.Gender,
                        IsActive = superAdmin.IsActive
                    };
                    _serviceLogger.LogInformation("Güncelleme verisi (şifresiz): {@User}", user);
                }

                var response = await PatchAsync<object, UserDto>($"api/users/{superAdmin.Id}", user);
                
                if (!response.Success)
                {
                    _serviceLogger.LogError("API PATCH başarısız: {Message}, Errors: {Errors}", 
                        response.Message, 
                        response.Errors != null ? string.Join(", ", response.Errors) : "null");
                }

                return new ApiResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _serviceLogger.LogError(ex, "Süper admin güncelleme hatası: {Message}", ex.Message);
                return new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                _serviceLogger.LogInformation("Süper admin silme isteği: ID={Id}", id);
                
                var response = await base.DeleteAsync($"api/users/{id}");

                return new ApiResponse
                {
                    Success = response.Success,
                    Message = response.Message
                };
            }
            catch (Exception ex)
            {
                _serviceLogger.LogError(ex, "Süper admin silme hatası: {Message}", ex.Message);
                return new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
