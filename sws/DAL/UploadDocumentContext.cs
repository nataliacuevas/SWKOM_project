using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using sws.DAL.Entities;

namespace sws.DAL
{
    public class UploadDocumentContext : DbContext, IUploadDocumentContext
    {
        public UploadDocumentContext(DbContextOptions<UploadDocumentContext> options) : base(options)
        {
        }
        public virtual DbSet<UploadDocument> UploadedDocuments { get; set; } = null;
        // Required for the IUploadDocument interface
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override EntityEntry Entry(object entity)
        {
            return base.Entry(entity);
        }
    }
        
}
