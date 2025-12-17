using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface ISystemSettingRepository : IRepository<SystemSetting>
    {
        Task<SystemSetting?> GetByKeyAsync(string key);
        Task<IEnumerable<SystemSetting>> GetByCategoryAsync(string category);
        Task<string?> GetValueAsync(string key);
    }
}
