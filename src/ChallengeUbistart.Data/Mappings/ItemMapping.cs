using ChallengeUbistart.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChallengeUbistart.Data.Mappings
{
    public class ItemMapping : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(c => c.DueDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(c => c.FinishedAt)
                .HasColumnType("datetime");

            builder.Property(c => c.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime");

            builder.ToTable("Item");
        }
    }
}