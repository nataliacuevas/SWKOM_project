using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
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

        public UploadDocument? Pop(long id)
        {
            var document = Get(id);
            if (document != null) 
            {
                _context.UploadedDocuments.Remove(document);
                _context.SaveChanges();
            }
            return document;
        }

        public UploadDocument? Get(long id)
        {
            return _context.UploadedDocuments.FirstOrDefault(doc => doc.Id == id);
        }

        public List<UploadDocument> GetAll()
        {
            return _context.UploadedDocuments.ToList();
        }
        public UploadDocument? Put(UploadDocument document)
        {
            var findDoc = Get(document.Id);
            if (findDoc == null)
            {
                return null;
            }
            _context.Entry(document).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
                return findDoc;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (Get(document.Id) == null)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
