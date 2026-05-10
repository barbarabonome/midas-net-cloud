using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Moq;

namespace Midas.API.Tests.Unit.Fixtures
{
    /// <summary>
    /// Fixture com dados padrão para testes de Usuario
    /// </summary>
    public class UsuarioFixture
    {
        public static Usuario CriarUsuarioValido(int id = 1)
        {
            return new Usuario
            {
                Id = id,
                Nome = "João Silva",
                Email = "joao@example.com",
                Senha = "senha123",
                DataCriacao = DateTime.Now
            };
        }

        public static Usuario CriarUsuarioComNomeVazio(int id = 1)
        {
            return new Usuario
            {
                Id = id,
                Nome = string.Empty,
                Email = "usuario@example.com",
                Senha = "senha123",
                DataCriacao = DateTime.Now
            };
        }

        public static CreateUsuarioDTO CriarCreateUsuarioDTOValido()
        {
            return new CreateUsuarioDTO
            {
                Nome = "Maria Silva",
                Email = "maria@example.com",
                Senha = "senha456"
            };
        }

        public static CreateUsuarioDTO CriarCreateUsuarioDTOComNomeVazio()
        {
            return new CreateUsuarioDTO
            {
                Nome = string.Empty,
                Email = "teste@example.com",
                Senha = "senha123"
            };
        }

        public static CreateUsuarioDTO CriarCreateUsuarioDTOComEmailVazio()
        {
            return new CreateUsuarioDTO
            {
                Nome = "João",
                Email = string.Empty,
                Senha = "senha123"
            };
        }

        public static CreateUsuarioDTO CriarCreateUsuarioDTOComSenhaVazia()
        {
            return new CreateUsuarioDTO
            {
                Nome = "João",
                Email = "joao@example.com",
                Senha = string.Empty
            };
        }

        public static List<Usuario> CriarListaUsuariosValidos(int quantidade = 3)
        {
            var usuarios = new List<Usuario>();
            for (int i = 1; i <= quantidade; i++)
            {
                usuarios.Add(new Usuario
                {
                    Id = i,
                    Nome = $"Usuario {i}",
                    Email = $"usuario{i}@example.com",
                    Senha = "senha123",
                    DataCriacao = DateTime.Now
                });
            }
            return usuarios;
        }

        public static UsuarioSearchParameters CriarSearchParametersValidos(string? nome = null, string? email = null)
        {
            return new UsuarioSearchParameters
            {
                Nome = nome,
                Email = email,
                Page = 1,
                Size = 10
            };
        }
    }
}
