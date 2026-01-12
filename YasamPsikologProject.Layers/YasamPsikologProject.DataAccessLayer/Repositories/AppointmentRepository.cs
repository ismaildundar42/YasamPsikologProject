using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public override async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> HasConflictAsync(int psychologistId, DateTime startDate, DateTime endDate, int? excludeAppointmentId = null)
        {
            // Mevcut randevuları çek
            var appointments = await _context.Appointments
                .Where(a => a.PsychologistId == psychologistId
                         && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();

            if (excludeAppointmentId.HasValue)
            {
                appointments = appointments.Where(a => a.Id != excludeAppointmentId.Value).ToList();
            }

            // Her randevu için DİNAMİK olarak bitiş saatini hesapla (buffer dahil)
            foreach (var appointment in appointments)
            {
                var actualEndDate = appointment.AppointmentDate.AddMinutes((int)appointment.Duration + appointment.BreakDuration);
                
                // Çakışma kontrolü: Yeni randevu mevcut randevuyla çakışıyor mu?
                if (startDate < actualEndDate && endDate > appointment.AppointmentDate)
                {
                    return true; // Çakışma var!
                }
            }

            return false; // Çakışma yok
        }

        public async Task<IEnumerable<Appointment>> GetByPsychologistAsync(int psychologistId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Appointments
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .Where(a => a.PsychologistId == psychologistId);

            if (startDate.HasValue)
                query = query.Where(a => a.AppointmentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.AppointmentDate <= endDate.Value);

            return await query
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByClientAsync(int clientId)
        {
            return await _context.Appointments
                .Include(a => a.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(a => a.ClientId == clientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int psychologistId, int days = 7)
        {
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            return await _context.Appointments
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .Where(a => a.PsychologistId == psychologistId
                         && a.AppointmentDate >= startDate
                         && a.AppointmentDate <= endDate
                         && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .Include(a => a.Psychologist)
                    .ThenInclude(p => p.User)
                .Where(a => a.Status == AppointmentStatus.Pending)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }
    }
}
