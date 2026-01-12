using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class PsychologistRepository : Repository<Psychologist>, IPsychologistRepository
    {
        private readonly AppDbContext _context;

        public PsychologistRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Override GetByIdAsync to include User
        public new async Task<Psychologist?> GetByIdAsync(int id)
        {
            return await _context.Psychologists
                .Include(p => p.User)
                .Include(p => p.WorkingHours)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Psychologist?> GetByUserIdAsync(int userId)
        {
            return await _context.Psychologists
                .Include(p => p.User)
                .Include(p => p.WorkingHours)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<Psychologist>> GetActiveWithWorkingHoursAsync()
        {
            return await _context.Psychologists
                .Include(p => p.User)
                .Include(p => p.WorkingHours)
                .Where(p => p.IsActive)
                .ToListAsync();
        }
    }
}
