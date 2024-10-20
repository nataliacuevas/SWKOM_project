using sws.SL.DTOs;

namespace sws.BLL
{
    public interface IDocumentLogic
    {
        void Add(UploadDocumentDTO document);
        // Returns the deleted document, null if not found
        UploadDocumentDTO? PopById(long id);

        UploadDocumentDTO? GetById(long id);

        Task<UploadDocumentDTO?> GetByIdAsync(long id);

        List<UploadDocumentDTO> GetAll();

        // If replaced, returns the older value
        UploadDocumentDTO? Put(UploadDocumentDTO document);
    }
}
