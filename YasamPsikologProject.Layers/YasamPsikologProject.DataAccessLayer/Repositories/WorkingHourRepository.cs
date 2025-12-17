using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;
using YasamPsikologProject.EntityLayer.Enums;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class WorkingHourRepository : Repository<WorkingHour>, IWorkingHourRepository
    {
        private readonly AppDbContext _context;

        public WorkingHourRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkingHour>> GetByPsychologistAsync(int psychologistId)
        {
            return await _context.WorkingHours
                .Where(w => !w.IsDeleted && w.PsychologistId == psychologistId)
                .Include(w => w.Psychologist)
                    .ThenInclude(p => p.User)
                .OrderBy(w => w.DayOfWeek)
                .ToListAsync();
        }

        public async Task<WorkingHour?> GetByPsychologistAndDayAsync(int psychologistId, WeekDay day)
        {
            return await _context.WorkingHours
                .Where(w => !w.IsDeleted)
                .Include(w => w.Psychologist)
                .FirstOrDefaultAsync(w => w.PsychologistId == psychologistId && w.DayOfWeek == day);
        }
    }
}
