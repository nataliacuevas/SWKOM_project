namespace sws.BLL
{
    public interface IDocumentSearchService
    {
        Task<List<DocumentSearchResult>> SearchDocumentsAsync(string query);
    }
}
