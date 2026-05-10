using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence.Mappings;

public class ReceitaMapping : IEntityTypeConfiguration<Receita>
{
    public void Configure(EntityTypeBuilder<Receita> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Titulo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Valor)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.Data)
            .IsRequired();

        builder.Property(r => r.Categoria)
            .IsRequired()
            .HasMaxLength(50);
    }
}
