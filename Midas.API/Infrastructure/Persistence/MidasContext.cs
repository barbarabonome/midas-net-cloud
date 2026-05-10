using Microsoft.EntityFrameworkCore;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence
{
    public class MidasContext : DbContext
    {
        public MidasContext(DbContextOptions<MidasContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cofrinho> Cofrinhos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Receita> Receitas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("T_USUARIO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID_USUARIO");
                entity.Property(e => e.Nome).HasColumnName("NM_USUARIO").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasColumnName("EMAIL").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Senha).HasColumnName("SENHA").HasMaxLength(30).IsRequired();
                entity.Property(e => e.DataCriacao).HasColumnName("DT_USUARIO").IsRequired();
            });

            // Configuração de Cofrinho
            modelBuilder.Entity<Cofrinho>(entity =>
            {
                entity.ToTable("T_COFRINHO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID_COFRINHO");
                entity.Property(e => e.UsuarioId).HasColumnName("ID_USUARIO").IsRequired();
                entity.Property(e => e.Titulo).HasColumnName("TT_COFRINHO").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Meta).HasColumnName("META").HasColumnType("NUMBER(10,2)").IsRequired();
                entity.Property(e => e.Atingido).HasColumnName("ATINGIDO").HasColumnType("NUMBER(10,2)").IsRequired();
                entity.Property(e => e.Aplicado).HasColumnName("APLICADO").HasColumnType("CHAR(1)").IsRequired();
            });

            // Configuração de Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("T_CATEGORIA");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID_CATEGORIA");
                entity.Property(e => e.Nome).HasColumnName("NM_CATEGORIA").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Descricao).HasColumnName("DS_CATEGORIA").HasMaxLength(255).IsRequired();
            });

            // Configuração de Gasto
            modelBuilder.Entity<Gasto>(entity =>
            {
                entity.ToTable("T_GASTO");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID_GASTO");
                entity.Property(e => e.UsuarioId).HasColumnName("ID_USUARIO").IsRequired();
                entity.Property(e => e.CategoriaId).HasColumnName("ID_CATEGORIA").IsRequired();
                entity.Property(e => e.Titulo).HasColumnName("TT_GASTO").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Data).HasColumnName("DT_GASTO").IsRequired();
                entity.Property(e => e.Valor).HasColumnName("VL_GASTO").HasColumnType("NUMBER(10,2)").IsRequired();
                entity.Property(e => e.Fixo).HasColumnName("FX_GASTO").HasColumnType("CHAR(1)").IsRequired();
            });

            // Configuração de Receita
            modelBuilder.Entity<Receita>(entity =>
            {
                entity.ToTable("T_RECEITA");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ID_RECEITA");
                entity.Property(e => e.UsuarioId).HasColumnName("ID_USUARIO").IsRequired();
                entity.Property(e => e.Titulo).HasColumnName("TT_RECEITA").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Data).HasColumnName("DT_RECEITA").IsRequired();
                entity.Property(e => e.Valor).HasColumnName("VL_RECEITA").HasColumnType("NUMBER(10,2)").IsRequired();
                entity.Property(e => e.Fixo).HasColumnName("FX_RECEITA").HasColumnType("CHAR(1)").IsRequired();
            });
        }
    }
}