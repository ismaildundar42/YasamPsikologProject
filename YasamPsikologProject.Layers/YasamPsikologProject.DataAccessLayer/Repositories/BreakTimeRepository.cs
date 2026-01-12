using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class BreakTimeRepository : Repository<BreakTime>, IBreakTimeRepository
    {
        private readonly AppDbContext _context;

        public BreakTimeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BreakTime>> GetByPsychologistAsync(int psychologistId)
        {
            return await _context.BreakTimes
                .Include(b => b.WorkingHour)
                .Where(b => b.WorkingHour.PsychologistId == psychologistId)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }
    }
}
