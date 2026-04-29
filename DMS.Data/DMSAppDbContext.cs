using DMS.Data.Configurations.Document;
using DMS.Data.Configurations.Tag;
using DMS.Data.Configurations.User;
using DMS.Models.Entities.Document;
using DMS.Models.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMS.Data
{
    public class DMSAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DMSAppDbContext(DbContextOptions<DMSAppDbContext> options) : base(options)
        {
        }

        public DbSet<DocumentEntity> Documents { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<DocumentTag> DocumentTags { get; set; }
        public DbSet<DocumentAuditLog> DocumentAuditLogs { get; set; }
        public DbSet<Models.Entities.Tag.Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentVersionConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentTagConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentAuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        }
    }
}