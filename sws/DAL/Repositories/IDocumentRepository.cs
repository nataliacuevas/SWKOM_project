using sws.DAL.Entities;
using System;

namespace sws.DAL.Repositories
{
    public interface IDocumentRepository
    {
        UploadDocument Add(UploadDocument document);
        // Pop deletes and returns the document if found, else null
        UploadDocument? Pop(long  id);

        UploadDocument? Get(long id);

        Task<UploadDocument?> GetAsync(long id);

        List<UploadDocument> GetAll();
        // Returns the old value if existant
        UploadDocument? Put(UploadDocument document);
    }
}
