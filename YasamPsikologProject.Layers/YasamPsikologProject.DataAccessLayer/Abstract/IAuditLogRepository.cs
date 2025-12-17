using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByUserAsync(int userId, int take = 100);
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, int entityId);
    }
}
