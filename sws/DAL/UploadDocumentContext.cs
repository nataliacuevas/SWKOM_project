using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using sws.DAL.Entities;

namespace sws.DAL
{
    public class UploadDocumentContext : DbContext, IUploadDocumentContext
    {
        public UploadDocumentContext(DbContextOptions<UploadDocumentContext> options) : base(options) { }
        public DbSet<UploadDocument> UploadedDocuments { get; set; } = null;

    }
        
}
