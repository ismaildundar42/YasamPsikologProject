using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnavailableTimesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnavailableTimesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var unavailableTime = await _unitOfWork.UnavailableTimeRepository.GetByIdAsync(id);
            if (unavailableTime == null)
                return NotFound(new { message = "Müsait olmayan zaman bulunamadı." });

            return Ok(unavailableTime);
        }

        [HttpGet("psychologist/{psychologistId}")]
        public async Task<IActionResult> GetByPsychologist(int psychologistId)
        {
            var unavailableTimes = await _unitOfWork.UnavailableTimeRepository.GetByPsychologistAsync(psychologistId);
            return Ok(unavailableTimes);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UnavailableTime unavailableTime)
        {
            try
            {
                if (unavailableTime.StartDateTime >= unavailableTime.EndDateTime)
                    return BadRequest(new { message = "Bitiş tarihi başlangıç tarihinden önce olamaz." });

                await _unitOfWork.UnavailableTimeRepository.AddAsync(unavailableTime);
                await _unitOfWork.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = unavailableTime.Id }, unavailableTime);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UnavailableTime unavailableTime)
        {
            if (id != unavailableTime.Id)
                return BadRequest(new { message = "ID uyuşmazlığı." });

            try
            {
                if (unavailableTime.StartDateTime >= unavailableTime.EndDateTime)
                    return BadRequest(new { message = "Bitiş tarihi başlangıç tarihinden önce olamaz." });

                _unitOfWork.UnavailableTimeRepository.Update(unavailableTime);
                await _unitOfWork.SaveChangesAsync();
                return Ok(unavailableTime);
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
                var unavailableTime = await _unitOfWork.UnavailableTimeRepository.GetByIdAsync(id);
                if (unavailableTime == null)
                    return NotFound(new { message = "Müsait olmayan zaman bulunamadı." });

                _unitOfWork.UnavailableTimeRepository.Delete(unavailableTime);
                await _unitOfWork.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("check-unavailable")]
        public async Task<IActionResult> CheckUnavailable([FromQuery] int psychologistId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var isUnavailable = await _unitOfWork.UnavailableTimeRepository.HasUnavailableTimeAsync(psychologistId, startDate, endDate);
            return Ok(new { isUnavailable });
        }
    }
}
