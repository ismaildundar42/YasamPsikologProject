using Microsoft.AspNetCore.Mvc;
using YasamPsikologProject.DataAccessLayer.Abstract;

namespace YasamPsikologProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var notification = await _unitOfWork.AppointmentNotificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound(new { message = "Bildirim bulunamadı." });

            return Ok(notification);
        }

        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var notifications = await _unitOfWork.AppointmentNotificationRepository.GetByAppointmentAsync(appointmentId);
            return Ok(notifications);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending([FromQuery] int take = 100)
        {
            var notifications = await _unitOfWork.AppointmentNotificationRepository.GetPendingNotificationsAsync();
            return Ok(notifications.Take(take));
            return Ok(notifications);
        }

    }
}
