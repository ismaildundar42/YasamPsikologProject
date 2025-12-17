using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.DataAccessLayer.Abstract;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditLogsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int take = 100)
        {
            var logs = await _unitOfWork.AuditLogRepository.GetAllAsync();
            return Ok(logs.Take(take));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _unitOfWork.AuditLogRepository.GetByIdAsync(id);
            if (log == null)
                return NotFound(new { message = "Log kaydı bulunamadı." });

            return Ok(log);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, [FromQuery] int take = 100)
        {
            var logs = await _unitOfWork.AuditLogRepository.GetByUserAsync(userId, take);
            return Ok(logs);
        }

        [HttpGet("entity/{entityName}")]
        public async Task<IActionResult> GetByEntity(string entityName, [FromQuery] int take = 100)
        {
            var logs = await _unitOfWork.AuditLogRepository.GetByEntityAsync(entityName, take);
            return Ok(logs);
        }
    }
}
