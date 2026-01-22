using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.DataAccessLayer.Abstract
{
    public interface IPsychologistArchiveRepository : IRepository<PsychologistArchive>
    {
        Task<IEnumerable<PsychologistArchive>> GetAllOrderedByDateAsync();
    }
}
