using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.WebApi.DTOs;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemSettingsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _unitOfWork.SystemSettingRepository.GetAllAsync();
            var dtos = settings.Select(s => new SystemSettingDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                Description = s.Description,
                Category = s.Category
            }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var setting = await _unitOfWork.SystemSettingRepository.GetByIdAsync(id);
            if (setting == null)
                return NotFound(new { message = "Ayar bulunamadı." });

            var dto = new SystemSettingDto
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                Category = setting.Category
            };
            return Ok(dto);
        }

        [HttpGet("key/{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var setting = await _unitOfWork.SystemSettingRepository.GetByKeyAsync(key);
            if (setting == null)
                return NotFound(new { message = "Ayar bulunamadı." });

            var dto = new SystemSettingDto
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                Category = setting.Category
            };
            return Ok(dto);
        }

        [HttpGet("key/{key}/value")]
        public async Task<IActionResult> GetValue(string key)
        {
            var value = await _unitOfWork.SystemSettingRepository.GetValueAsync(key);
            if (value == null)
                return NotFound(new { message = "Ayar bulunamadı." });

            return Ok(new { key, value });
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var settings = await _unitOfWork.SystemSettingRepository.GetByCategoryAsync(category);
            return Ok(settings);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SystemSetting setting)
        {
            try
            {
                var existing = await _unitOfWork.SystemSettingRepository.GetByKeyAsync(setting.Key);
                if (existing != null)
                    return BadRequest(new { message = "Bu anahtar zaten mevcut." });

                await _unitOfWork.SystemSettingRepository.AddAsync(setting);
                await _unitOfWork.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = setting.Id }, setting);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SystemSettingDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                var setting = await _unitOfWork.SystemSettingRepository.GetByIdAsync(id);
                if (setting == null)
                    return NotFound(new { message = "Ayar bulunamadı." });

                setting.Value = dto.Value;
                setting.Description = dto.Description;
                setting.Category = dto.Category;
                setting.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SystemSettingRepository.Update(setting);
                await _unitOfWork.SaveChangesAsync();
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var setting = await _unitOfWork.SystemSettingRepository.GetByIdAsync(id);
                if (setting == null)
                    return NotFound(new { message = "Ayar bulunamadı." });

                _unitOfWork.SystemSettingRepository.Delete(setting);
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
