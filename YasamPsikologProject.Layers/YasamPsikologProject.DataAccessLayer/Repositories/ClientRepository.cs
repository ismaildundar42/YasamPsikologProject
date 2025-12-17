using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Client?> GetByUserIdAsync(int userId)
        {
            return await _context.Clients
                .Include(c => c.User)
                .Include(c => c.AssignedPsychologist)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<IEnumerable<Client>> GetByPsychologistAsync(int psychologistId)
        {
            return await _context.Clients
                .Include(c => c.User)
                .Where(c => c.AssignedPsychologistId == psychologistId)
                .ToListAsync();
        }
    }
}
