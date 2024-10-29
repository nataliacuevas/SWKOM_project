using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using sws.DAL;
using sws.DAL.Entities;
using log4net;
using System.Collections.Generic;

namespace sws.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly UploadDocumentContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentRepository));

        public DocumentRepository(UploadDocumentContext context)
        {
            _context = context;
        }

        public UploadDocument Add(UploadDocument document)
        {
            _context.UploadedDocuments.Add(document);
            _context.SaveChanges();
            log.Info("Document added to the database successfully.");
            return document;
        }

        public UploadDocument? Pop(long id)
        {
            log.Info($"Attempting to delete document with ID: {id}");
            var document = Get(id);
            if (document != null)
            {
                _context.UploadedDocuments.Remove(document);
                _context.SaveChanges();
                log.Info($"Document with ID: {id} deleted from database.");
            }
            else
            {
                log.Warn($"Document with ID: {id} not found for deletion.");
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
            log.Info("Retrieving all documents from database.");
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
                log.Info($"Document with ID: {document.Id} updated successfully in the database.");
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
