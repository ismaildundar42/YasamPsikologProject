using Microsoft.EntityFrameworkCore;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Repositories
{
    public class SystemSettingRepository : Repository<SystemSetting>, ISystemSettingRepository
    {
        private readonly AppDbContext _context;

        public SystemSettingRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SystemSetting?> GetByKeyAsync(string key)
        {
            return await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == key);
        }

        public async Task<IEnumerable<SystemSetting>> GetByCategoryAsync(string category)
        {
            return await _context.SystemSettings
                .Where(s => s.Category == category)
                .ToListAsync();
        }

        public async Task<string?> GetValueAsync(string key)
        {
            var setting = await GetByKeyAsync(key);
            return setting?.Value;
        }
    }
}
