using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DMS.Models.Entities.Document;

namespace DMS.Data.Configurations.Document
{
    public class DocumentTagConfiguration : IEntityTypeConfiguration<DocumentTag>
    {
        public void Configure(EntityTypeBuilder<DocumentTag> builder)
        {
            builder.ToTable("DocumentTags");

            builder.HasKey(dt => new { dt.DocumentId, dt.TagId });

            builder.HasOne(dt => dt.Document)
                .WithMany(d => d.DocumentTags)
                .HasForeignKey(dt => dt.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dt => dt.Tag)
                .WithMany(t => t.DocumentTags)
                .HasForeignKey(dt => dt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}