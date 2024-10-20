using sws.DAL;
using sws.DAL.Entities;

namespace sws.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly UploadDocumentContext _context;

        public DocumentRepository(UploadDocumentContext context)
        {
            _context = context;
        }

        public UploadDocument Add(UploadDocument document)
        {
            _context.UploadedDocuments.Add(document);
            _context.SaveChanges();
            return document;
        }

    }
}
