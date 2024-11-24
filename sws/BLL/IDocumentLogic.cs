using sws.SL.DTOs;

namespace sws.BLL
{
    public interface IDocumentLogic
    {
        Task Add(UploadDocumentDTO document);
        // Returns the deleted document, null if not found
        DownloadDocumentDTO? PopById(long id);

        DownloadDocumentDTO? GetById(long id);

        Task<DownloadDocumentDTO?> GetByIdAsync(long id);

        List<DownloadDocumentDTO> GetAll();

        // If replaced, returns the older value
        DownloadDocumentDTO? Put(UploadDocumentDTO document);
    }
}
