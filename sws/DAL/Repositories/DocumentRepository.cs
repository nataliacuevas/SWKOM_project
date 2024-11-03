using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using sws.BLL;
using sws.DAL;
using sws.DAL.Entities;
using sws.SL.Controllers;

namespace sws.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IUploadDocumentContext _context;
        private readonly ILogger _logger;

        public DocumentRepository(IUploadDocumentContext context, ILogger<DocumentRepository> logger)
        {
            _context = context;
            _logger = logger;

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

        public async Task<UploadDocument?> GetAsync(long id)
        {
            return await _context.UploadedDocuments.FirstOrDefaultAsync(doc => doc.Id == id);
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
