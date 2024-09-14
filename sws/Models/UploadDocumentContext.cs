using Microsoft.EntityFrameworkCore;

namespace sws.Models
{
    public class UploadDocumentContext : DbContext
    {
        public UploadDocumentContext(DbContextOptions<UploadDocumentContext> options) : base(options)
        {
        }
        public DbSet<UploadDocument> UploadedDocuments { get; set; } = null;
    }
}
