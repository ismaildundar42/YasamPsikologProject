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

        public override async Task<WorkingHour?> GetByIdAsync(int id)
        {
            return await _context.WorkingHours
                .Include(w => w.BreakTimes)
                .Include(w => w.Psychologist)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);
        }

        public override async Task<IEnumerable<WorkingHour>> GetAllAsync()
        {
            return await _context.WorkingHours
                .Include(w => w.BreakTimes)
                .Where(w => !w.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkingHour>> GetByPsychologistAsync(int psychologistId)
        {
            return await _context.WorkingHours
                .Where(w => !w.IsDeleted && w.PsychologistId == psychologistId)
                .Include(w => w.Psychologist)
                    .ThenInclude(p => p.User)
                .Include(w => w.BreakTimes)
                .OrderBy(w => w.DayOfWeek)
                .ToListAsync();
        }

        public async Task<WorkingHour?> GetByPsychologistAndDayAsync(int psychologistId, WeekDay day)
        {
            return await _context.WorkingHours
                .Where(w => !w.IsDeleted)
                .Include(w => w.Psychologist)
                .Include(w => w.BreakTimes)
                .FirstOrDefaultAsync(w => w.PsychologistId == psychologistId && w.DayOfWeek == day);
        }
    }
}
