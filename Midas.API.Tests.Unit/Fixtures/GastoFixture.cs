using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.API.Tests.Unit.Fixtures
{
    /// <summary>
    /// Fixture com dados padrão para testes de Gasto
    /// </summary>
    public class GastoFixture
    {
        public static Gasto CriarGastoValido(int id = 1, int usuarioId = 1, int categoriaId = 1)
        {
            return new Gasto
            {
                Id = id,
                UsuarioId = usuarioId,
                CategoriaId = categoriaId,
                Titulo = "Supermercado",
                Data = DateTime.Now,
                Valor = 150.50m,
                Fixo = 'F'
            };
        }

        public static CreateGastoDTO CriarCreateGastoDTOValido()
        {
            return new CreateGastoDTO
            {
                UsuarioId = 1,
                CategoriaId = 1,
                Titulo = "Compras no Supermercado",
                Data = DateTime.Now,
                Valor = 200.75m,
                Fixo = 'F'
            };
        }

        public static CreateGastoDTO CriarCreateGastoDTOComTituloVazio()
        {
            return new CreateGastoDTO
            {
                UsuarioId = 1,
                CategoriaId = 1,
                Titulo = string.Empty,
                Data = DateTime.Now,
                Valor = 100.00m,
                Fixo = 'F'
            };
        }

        public static CreateGastoDTO CriarCreateGastoDTOComUsuarioIdInvalido()
        {
            return new CreateGastoDTO
            {
                UsuarioId = 0,
                CategoriaId = 1,
                Titulo = "Compras",
                Data = DateTime.Now,
                Valor = 100.00m,
                Fixo = 'F'
            };
        }

        public static CreateGastoDTO CriarCreateGastoDTOComCategoriaIdInvalido()
        {
            return new CreateGastoDTO
            {
                UsuarioId = 1,
                CategoriaId = 0,
                Titulo = "Compras",
                Data = DateTime.Now,
                Valor = 100.00m,
                Fixo = 'F'
            };
        }

        public static CreateGastoDTO CriarCreateGastoDTOComValorInvalido()
        {
            return new CreateGastoDTO
            {
                UsuarioId = 1,
                CategoriaId = 1,
                Titulo = "Compras",
                Data = DateTime.Now,
                Valor = 0m,
                Fixo = 'F'
            };
        }

        public static CreateGastoDTO CriarCreateGastoDTOComValorNegativo()
        {
            return new CreateGastoDTO
            {
                UsuarioId = 1,
                CategoriaId = 1,
                Titulo = "Compras",
                Data = DateTime.Now,
                Valor = -50.00m,
                Fixo = 'F'
            };
        }

        public static List<Gasto> CriarListaGastosValidos(int quantidade = 3)
        {
            var gastos = new List<Gasto>();
            for (int i = 1; i <= quantidade; i++)
            {
                gastos.Add(new Gasto
                {
                    Id = i,
                    UsuarioId = 1,
                    CategoriaId = 1,
                    Titulo = $"Gasto {i}",
                    Data = DateTime.Now.AddDays(-i),
                    Valor = 100.00m * i,
                    Fixo = i % 2 == 0 ? 'T' : 'F'
                });
            }
            return gastos;
        }

        public static GastoSearchParameters CriarSearchParametersValidos(string? titulo = null, decimal? valorMinimo = null, decimal? valorMaximo = null)
        {
            return new GastoSearchParameters
            {
                Titulo = titulo,
                ValorMinimo = valorMinimo,
                ValorMaximo = valorMaximo,
                Page = 1,
                Size = 10
            };
        }

        public static Gasto CriarGastoFixo(int id = 1, int usuarioId = 1, int categoriaId = 1)
        {
            return new Gasto
            {
                Id = id,
                UsuarioId = usuarioId,
                CategoriaId = categoriaId,
                Titulo = "Aluguel",
                Data = DateTime.Now,
                Valor = 1500.00m,
                Fixo = 'T'
            };
        }
    }
}
