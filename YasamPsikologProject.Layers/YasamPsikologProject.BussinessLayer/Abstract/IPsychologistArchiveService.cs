using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.BussinessLayer.Abstract
{
    public interface IPsychologistArchiveService
    {
        Task<IEnumerable<PsychologistArchive>> GetAllArchivedAsync();
    }
}
