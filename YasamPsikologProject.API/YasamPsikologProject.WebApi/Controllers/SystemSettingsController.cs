using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;

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
            return Ok(settings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var setting = await _unitOfWork.SystemSettingRepository.GetByIdAsync(id);
            if (setting == null)
                return NotFound(new { message = "Ayar bulunamadı." });

            return Ok(setting);
        }

        [HttpGet("key/{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var setting = await _unitOfWork.SystemSettingRepository.GetByKeyAsync(key);
            if (setting == null)
                return NotFound(new { message = "Ayar bulunamadı." });

            return Ok(setting);
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
        public async Task<IActionResult> Update(int id, [FromBody] SystemSetting setting)
        {
            if (id != setting.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                _unitOfWork.SystemSettingRepository.Update(setting);
                await _unitOfWork.SaveChangesAsync();
                return Ok(setting);
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
