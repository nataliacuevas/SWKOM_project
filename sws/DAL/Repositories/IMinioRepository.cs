using sws.DAL.Entities;

namespace sws.DAL.Repositories
{
    public interface IMinioRepository
    {
        Task Add(UploadDocument document);
      
    }
}
