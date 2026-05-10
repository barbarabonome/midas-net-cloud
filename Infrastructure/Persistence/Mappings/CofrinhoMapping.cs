using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence.Mappings;

public class CofrinhoMapping : IEntityTypeConfiguration<Cofrinho>
{
    public void Configure(EntityTypeBuilder<Cofrinho> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Titulo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ValorMeta)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.ValorAtual)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.Limite)
            .IsRequired();
    }
}
