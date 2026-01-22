using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.EntityLayer.Concrete;

namespace YasamPsikologProject.BussinessLayer.Concrete
{
    public class PsychologistArchiveManager : IPsychologistArchiveService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PsychologistArchiveManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PsychologistArchive>> GetAllArchivedAsync()
        {
            return await _unitOfWork.PsychologistArchiveRepository.GetAllOrderedByDateAsync();
        }
    }
}
