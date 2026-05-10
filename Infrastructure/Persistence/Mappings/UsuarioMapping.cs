using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Senha)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.DataCriacao)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
