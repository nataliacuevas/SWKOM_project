using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using sws.BLL;
using sws.DAL;
using sws.DAL.Entities;

using log4net;

using sws.SL.Controllers;
using System.Collections.Generic;

namespace sws.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IUploadDocumentContext _context;

        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentRepository));



        public DocumentRepository(IUploadDocumentContext context)
        {
            _context = context;
           

        }

        public UploadDocument Add(UploadDocument document)
        {
            log.Info("Adding document to the database");
            _context.UploadedDocuments.Add(document);
            _context.SaveChanges();
            log.Info($"Document '{document.Name}' added to the database.");
            return document;
        }

        public UploadDocument? Pop(long id)
        {
            log.Info($"Popping document with ID {id}.");
            var document = Get(id);
            if (document != null) 
            {
                _context.UploadedDocuments.Remove(document);
                _context.SaveChanges();
                log.Info($"Document with ID {id} removed from the database.");
            }
            else
            {
                log.Warn($"Document with ID {id} not found.");
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

        public async Task<List<UploadDocument>> GetDocumentsByIdsAsync(List<long> ids)
        {
            log.Info("Fetching metadata for documents with specified IDs " + string.Join(", ", ids));
            return await _context.UploadedDocuments
                .Where(doc => ids.Contains(doc.Id))
                .ToListAsync();
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
