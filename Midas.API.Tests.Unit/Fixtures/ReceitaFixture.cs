using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.API.Tests.Unit.Fixtures
{
    /// <summary>
    /// Fixture com dados padrão para testes de Receita
    /// </summary>
    public class ReceitaFixture
    {
        public static Receita CriarReceitaValida(int id = 1, int usuarioId = 1)
        {
            return new Receita
            {
                Id = id,
                UsuarioId = usuarioId,
                Titulo = "Salário",
                Data = DateTime.Now,
                Valor = 3000.00m,
                Fixo = 'T'
            };
        }

        public static CreateReceitaDTO CriarCreateReceitaDTOValido()
        {
            return new CreateReceitaDTO
            {
                UsuarioId = 1,
                Titulo = "Freelance",
                Data = DateTime.Now,
                Valor = 500.00m,
                Fixo = 'F'
            };
        }

        public static CreateReceitaDTO CriarCreateReceitaDTOComTituloVazio()
        {
            return new CreateReceitaDTO
            {
                UsuarioId = 1,
                Titulo = string.Empty,
                Data = DateTime.Now,
                Valor = 500.00m,
                Fixo = 'F'
            };
        }

        public static CreateReceitaDTO CriarCreateReceitaDTOComTituloNulo()
        {
            return new CreateReceitaDTO
            {
                UsuarioId = 1,
                Titulo = null!,
                Data = DateTime.Now,
                Valor = 500.00m,
                Fixo = 'F'
            };
        }

        public static CreateReceitaDTO CriarCreateReceitaDTOComTituloEspacos()
        {
            return new CreateReceitaDTO
            {
                UsuarioId = 1,
                Titulo = "   ",
                Data = DateTime.Now,
                Valor = 500.00m,
                Fixo = 'F'
            };
        }

        public static CreateReceitaDTO CriarCreateReceitaDTOComUsuarioIdInvalido()
        {
            return new CreateReceitaDTO
            {
                UsuarioId = 0,
                Titulo = "Receita",
                Data = DateTime.Now,
                Valor = 500.00m,
                Fixo = 'F'
            };
        }

        public static CreateReceitaDTO CriarCreateReceitaDTOComValorInvalido()
        {
            return new CreateReceitaDTO
            {
                UsuarioId = 1,
                Titulo = "Receita",
                Data = DateTime.Now,
                Valor = 0m,
                Fixo = 'F'
            };
        }

        public static CreateReceitaDTO CriarCreateReceitaDTOComValorNegativo()
        {
            return new CreateReceitaDTO
            {
                UsuarioId = 1,
                Titulo = "Receita",
                Data = DateTime.Now,
                Valor = -100.00m,
                Fixo = 'F'
            };
        }

        public static List<Receita> CriarListaReceitasValidas(int quantidade = 3)
        {
            var receitas = new List<Receita>();
            for (int i = 1; i <= quantidade; i++)
            {
                receitas.Add(new Receita
                {
                    Id = i,
                    UsuarioId = 1,
                    Titulo = $"Receita {i}",
                    Data = DateTime.Now.AddDays(-i),
                    Valor = 1000.00m * i,
                    Fixo = i % 2 == 0 ? 'T' : 'F'
                });
            }
            return receitas;
        }

        public static ReceitaSearchParameters CriarSearchParametersValidos(
      string? titulo = null,
 int? usuarioId = null,
      DateTime? dataInicio = null,
            DateTime? dataFim = null,
            decimal? valorMinimo = null,
            decimal? valorMaximo = null,
   char? fixo = null)
        {
            return new ReceitaSearchParameters
            {
                Titulo = titulo,
                UsuarioId = usuarioId,
                DataInicio = dataInicio,
                DataFim = dataFim,
                ValorMinimo = valorMinimo,
                ValorMaximo = valorMaximo,
                Fixo = fixo,
                Page = 1,
                Size = 10
            };
        }

        public static Receita CriarReceitaFixa(int id = 1, int usuarioId = 1)
        {
            return new Receita
            {
                Id = id,
                UsuarioId = usuarioId,
                Titulo = "Salário Mensal",
                Data = DateTime.Now,
                Valor = 5000.00m,
                Fixo = 'T'
            };
        }

        public static Receita CriarReceitaVariavel(int id = 1, int usuarioId = 1)
        {
            return new Receita
            {
                Id = id,
                UsuarioId = usuarioId,
                Titulo = "Bônus",
                Data = DateTime.Now,
                Valor = 2000.00m,
                Fixo = 'F'
            };
        }

        public static ReceitaSearchParameters CriarSearchParametrosPorFaixa(decimal valorMinimo, decimal valorMaximo)
        {
            return new ReceitaSearchParameters
            {
                ValorMinimo = valorMinimo,
                ValorMaximo = valorMaximo,
                Page = 1,
                Size = 10
            };
        }

        public static ReceitaSearchParameters CriarSearchParametrosPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            return new ReceitaSearchParameters
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                Page = 1,
                Size = 10
            };
        }

        public static ReceitaSearchParameters CriarSearchParametrosPorTipo(char fixo)
        {
            return new ReceitaSearchParameters
            {
                Fixo = fixo,
                Page = 1,
                Size = 10
            };
        }
    }
}
