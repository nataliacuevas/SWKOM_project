using sws.SL.DTOs;

namespace sws.BLL
{
    public interface IDocumentSearchService
    {
        Task<List<DocumentSearchDTO>> SearchDocumentsAsync(string query);
    }
}
