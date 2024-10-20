using Microsoft.EntityFrameworkCore;
using sws.DAL.Entities;

namespace sws.DAL
{
    public class UploadDocumentContext : DbContext
    {
        public UploadDocumentContext(DbContextOptions<UploadDocumentContext> options) : base(options)
        {
        }
        public DbSet<UploadDocument> UploadedDocuments { get; set; } = null;
    }
}
