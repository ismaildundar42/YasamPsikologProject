using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class AppointmentNotificationRepository : Repository<AppointmentNotification>, IAppointmentNotificationRepository
    {
        private readonly AppDbContext _context;

        public AppointmentNotificationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppointmentNotification>> GetByAppointmentAsync(int appointmentId)
        {
            return await _context.AppointmentNotifications
                .Where(n => n.AppointmentId == appointmentId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentNotification>> GetPendingNotificationsAsync()
        {
            return await _context.AppointmentNotifications
                .Include(n => n.Appointment)
                .Where(n => !n.IsSent && !n.HasError)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
