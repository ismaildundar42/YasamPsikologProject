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

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            // Soft delete filtresi ile birlikte User ve AssignedPsychologist bilgilerini getir
            // Sadece EN AZ 1 RANDEVUSU OLAN danışanları göster
            return await _context.Clients
                .Where(c => !c.IsDeleted && _context.Appointments.Any(a => a.ClientId == c.Id))
                .Include(c => c.User)
                .Include(c => c.AssignedPsychologist)
                    .ThenInclude(p => p.User)
                .ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients
                .Where(c => !c.IsDeleted && c.Id == id)
                .Include(c => c.User)
                .Include(c => c.AssignedPsychologist)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync();
        }

        public async Task<Client?> GetByUserIdAsync(int userId)
        {
            return await _context.Clients
                .Where(c => !c.IsDeleted)
                .Include(c => c.User)
                .Include(c => c.AssignedPsychologist)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<IEnumerable<Client>> GetByPsychologistAsync(int psychologistId)
        {
            // Sadece EN AZ 1 RANDEVUSU OLAN danışanları göster
            return await _context.Clients
                .Where(c => !c.IsDeleted 
                    && c.AssignedPsychologistId == psychologistId
                    && _context.Appointments.Any(a => a.ClientId == c.Id))
                .Include(c => c.User)
                .ToListAsync();
        }
    }
}
