using DMS.Models.Entities.Document;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Data.Configurations.Document
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Models.Entities.Document.DocumentEntity>
    {
        public void Configure(EntityTypeBuilder<Models.Entities.Document.DocumentEntity> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Description)
                .HasMaxLength(1000);

            builder.Property(d => d.AccessCount)
                .HasDefaultValue(0);

            builder.Property(d => d.IsDeleted)
                .HasDefaultValue(false);

            builder.HasOne(d => d.UploadedBy)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.CurrentVersion)
                .WithOne()
                .HasForeignKey<Models.Entities.Document.DocumentEntity>(d => d.CurrentVersionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}