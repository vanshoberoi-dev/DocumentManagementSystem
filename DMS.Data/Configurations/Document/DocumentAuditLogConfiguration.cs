using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DMS.Models.Entities.Document;
using DMS.Models.Enums;

namespace DMS.Data.Configurations.Document
{
    public class DocumentAuditLogConfiguration : IEntityTypeConfiguration<DocumentAuditLog>
    {
        public void Configure(EntityTypeBuilder<DocumentAuditLog> builder)
        {
            builder.ToTable("DocumentAuditLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.Details)
                .HasMaxLength(500);

            builder.Property(a => a.PerformedAt)
                .IsRequired();

            builder.HasOne(a => a.Document)
                .WithMany(d => d.AuditLogs)
                .HasForeignKey(a => a.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}