using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.BussinessLayer.Abstract
{
    public interface IPsychologistService
    {
        Task<Psychologist?> GetByIdAsync(int id);
        Task<Psychologist?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Psychologist>> GetAllActiveAsync();
        Task<Psychologist> CreateAsync(Psychologist psychologist);
        Task<Psychologist> UpdateAsync(Psychologist psychologist);
        Task DeleteAsync(int id);
    }
}
