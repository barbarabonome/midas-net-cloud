using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.API.Tests.Unit.Fixtures
{
    /// <summary>
    /// Fixture com dados padrão para testes de Categoria
    /// </summary>
    public class CategoriaFixture
    {
        public static Categoria CriarCategoriaValida(int id = 1)
        {
            return new Categoria
            {
                Id = id,
                Nome = "Alimentação",
                Descricao = "Despesas com alimentação em geral"
            };
        }

        public static CreateCategoriaDTO CriarCreateCategoriaDTOValido()
        {
            return new CreateCategoriaDTO
            {
                Nome = "Transportes",
                Descricao = "Despesas com transporte e locomoção"
            };
        }

        public static CreateCategoriaDTO CriarCreateCategoriaDTOComNomeVazio()
        {
            return new CreateCategoriaDTO
            {
                Nome = string.Empty,
                Descricao = "Descrição válida"
            };
        }

        public static CreateCategoriaDTO CriarCreateCategoriaDTOComNomeNulo()
        {
            return new CreateCategoriaDTO
            {
                Nome = null!,
                Descricao = "Descrição válida"
            };
        }

        public static CreateCategoriaDTO CriarCreateCategoriaDTOComNomeEspacos()
        {
            return new CreateCategoriaDTO
            {
                Nome = "   ",
                Descricao = "Descrição válida"
            };
        }

        public static CreateCategoriaDTO CriarCreateCategoriaDTOComDescricaoVazia()
        {
            return new CreateCategoriaDTO
            {
                Nome = "Saúde",
                Descricao = string.Empty
            };
        }

        public static CreateCategoriaDTO CriarCreateCategoriaDTOComDescricaoNula()
        {
            return new CreateCategoriaDTO
            {
                Nome = "Educação",
                Descricao = null!
            };
        }

        public static List<Categoria> CriarListaCategoriasValidas(int quantidade = 3)
        {
            var categorias = new List<Categoria>();
            var nomes = new[] { "Alimentação", "Transportes", "Saúde", "Educação", "Diversão", "Utilidades" };
            var descricoes = new[]
                 {
          "Despesas com alimentação",
        "Despesas com transporte",
                "Despesas com saúde",
              "Despesas com educação",
            "Despesas com diversão",
   "Despesas com utilidades"
            };

            for (int i = 1; i <= quantidade; i++)
            {
                categorias.Add(new Categoria
                {
                    Id = i,
                    Nome = nomes[(i - 1) % nomes.Length],
                    Descricao = descricoes[(i - 1) % descricoes.Length]
                });
            }

            return categorias;
        }

        public static CategoriaSearchParameters CriarSearchParametersValidos(string? nome = null, string? descricao = null)
        {
            return new CategoriaSearchParameters
            {
                Nome = nome,
                Descricao = descricao,
                Page = 1,
                Size = 10
            };
        }

        public static Categoria CriarCategoriaSemDescricao(int id = 1)
        {
            return new Categoria
            {
                Id = id,
                Nome = "Lazer",
                Descricao = ""
            };
        }

        public static CreateCategoriaDTO CriarCreateCategoriaDTOSemDescricao()
        {
            return new CreateCategoriaDTO
            {
                Nome = "Moradia",
                Descricao = ""
            };
        }
    }
}
