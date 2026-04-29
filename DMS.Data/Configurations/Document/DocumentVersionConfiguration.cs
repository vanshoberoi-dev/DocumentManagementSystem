using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DMS.Models.Entities.Document;

namespace DMS.Data.Configurations.Document
{
    public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
    {
        public void Configure(EntityTypeBuilder<DocumentVersion> builder)
        {
            builder.ToTable("DocumentVersions");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.FileName)
                .IsRequired()
                .HasMaxLength(260);

            builder.Property(v => v.StoredFileName)
                .IsRequired()
                .HasMaxLength(260);

            builder.Property(v => v.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.VersionNumber)
                .IsRequired();

            builder.Property(v => v.FileSize)
                .IsRequired();

            builder.HasOne(v => v.Document)
                .WithMany(d => d.Versions)
                .HasForeignKey(v => v.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.UploadedBy)
                .WithMany()
                .HasForeignKey(v => v.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}