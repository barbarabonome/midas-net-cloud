using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence.Mappings;

public class GastoMapping : IEntityTypeConfiguration<Gasto>
{
    public void Configure(EntityTypeBuilder<Gasto> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Titulo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Valor)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(g => g.Data)
            .IsRequired();

        builder.Property(g => g.Categoria)
            .IsRequired()
            .HasMaxLength(50);
    }
}
