using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using sws.DAL.Entities;

namespace sws.DAL
{
    public interface IUploadDocumentContext 
    {
        public DbSet<UploadDocument> UploadedDocuments { get; set; }
        
        
        // Required for Mocking
        int SaveChanges();

        public EntityEntry Entry(object entity);
    }
}
