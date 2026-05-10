using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence.Mappings;

public class GastoFixoMapping : IEntityTypeConfiguration<GastoFixo>
{
    public void Configure(EntityTypeBuilder<GastoFixo> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Valor)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(g => g.Categoria)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.DataVencimento)
            .IsRequired();
    }
}