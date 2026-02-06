using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class UnavailableTimeRepository : Repository<UnavailableTime>, IUnavailableTimeRepository
    {
        private readonly AppDbContext _context;

        public UnavailableTimeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UnavailableTime>> GetByPsychologistAsync(int psychologistId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.UnavailableTimes
                .Where(u => u.PsychologistId == psychologistId && !u.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(u => u.EndDateTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(u => u.StartDateTime <= endDate.Value);

            return await query
                .OrderBy(u => u.StartDateTime)
                .ToListAsync();
        }

        public async Task<bool> HasUnavailableTimeAsync(int psychologistId, DateTime startDate, DateTime endDate)
        {
            return await _context.UnavailableTimes
                .AnyAsync(u => u.PsychologistId == psychologistId
                            && !u.IsDeleted
                            && u.StartDateTime < endDate
                            && u.EndDateTime > startDate);
        }
    }
}
