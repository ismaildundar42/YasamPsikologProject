using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class PsychologistArchiveRepository : Repository<PsychologistArchive>, IPsychologistArchiveRepository
    {
        private readonly AppDbContext _context;

        public PsychologistArchiveRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PsychologistArchive>> GetAllOrderedByDateAsync()
        {
            return await _context.PsychologistArchive
                .OrderByDescending(pa => pa.ArchivedAt)
                .ToListAsync();
        }
    }
}
