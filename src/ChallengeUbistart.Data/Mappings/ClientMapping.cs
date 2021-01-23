using ChallengeUbistart.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChallengeUbistart.Data.Mappings
{
    public class ClientMapping : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(p => p.Id);

            // 1 : N => Client : Items
            builder.HasMany(f => f.Items)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId);

            builder.ToTable("Clients");
        }
    }
}