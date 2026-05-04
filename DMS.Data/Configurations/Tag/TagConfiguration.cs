using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMS.Data.Configurations.Tag
{
    public class TagConfiguration : IEntityTypeConfiguration<Models.Entities.Tag.Tag>
    {
        public void Configure(EntityTypeBuilder<Models.Entities.Tag.Tag> builder)
        {
            builder.ToTable("Tags");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.NormalizedName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(t => t.NormalizedName)
                .IsUnique();
        }
    }
}