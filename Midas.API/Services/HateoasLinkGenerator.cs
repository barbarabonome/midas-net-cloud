using Microsoft.AspNetCore.Mvc;

namespace Midas.API.Services
{
    public class HateoasLinkGenerator
    {
        public List<DTOs.HateoasLink> GenerateCategoriaLinks(int? categoriaId = null, string baseUrl = "/api")
        {
            var links = new List<DTOs.HateoasLink>();

            if (categoriaId.HasValue)
            {
                // Links para uma categoria específica
                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/categoriasHateoas/{categoriaId.Value}",
                    "self",
                    "GET"));

                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/categoriasHateoas/{categoriaId.Value}",
                    "update",
                    "PUT"));

                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/categoriasHateoas/{categoriaId.Value}",
                    "delete",
                    "DELETE"));
            }

            // Links gerais para categorias
            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/categoriasHateoas",
                "all-categorias",
                "GET"));

            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/categoriasHateoas",
                "create",
                "POST"));

            return links;
        }

        public List<DTOs.HateoasLink> GenerateReceitaLinks(int? receitaId = null, string baseUrl = "/api")
        {
            var links = new List<DTOs.HateoasLink>();

            if (receitaId.HasValue)
            {
                // Links para uma receita específica
                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/receitasHateoas/{receitaId.Value}",
                    "self",
                    "GET"));

                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/receitasHateoas/{receitaId.Value}",
                    "update",
                    "PUT"));

                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/receitasHateoas/{receitaId.Value}",
                    "delete",
                    "DELETE"));
            }

            // Links gerais para receitas
            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/receitasHateoas",
                "all-receitas",
                "GET"));

            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/receitasHateoas",
                "create",
                "POST"));

            return links;
        }

        public List<DTOs.HateoasLink> GenerateGastoLinks(int? gastoId = null, string baseUrl = "/api")
        {
            var links = new List<DTOs.HateoasLink>();

            if (gastoId.HasValue)
            {
                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/gastosHateoas/{gastoId.Value}",
                    "self",
                    "GET"));

                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/gastosHateoas/{gastoId.Value}",
                    "update",
                    "PUT"));
            }

            // Links gerais
            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/gastosHateoas",
                "all-gastos",
                "GET"));

            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/gastosHateoas",
                "create",
                "POST"));

            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/gastosHateoas",
                "delete",
                "DELETE"));

            return links;
        }

        public List<DTOs.HateoasLink> GenerateCofrinhoLinks(int? cofrinhoId = null, string baseUrl = "/api")
        {
            var links = new List<DTOs.HateoasLink>();

            if (cofrinhoId.HasValue)
            {
                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/cofrinhoHateoas/{cofrinhoId.Value}",
                    "self",
                    "GET"));

                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/cofrinhoHateoas/{cofrinhoId.Value}",
                    "delete",
                    "DELETE"));

                links.Add(new DTOs.HateoasLink(
                    $"{baseUrl}/cofrinhoHateoas/{cofrinhoId.Value}",
                    "update-progress",
                    "PUT"));
            }

            // Links gerais
            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/cofrinhoHateoas",
                "all-cofrinhos",
                "GET"));

            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/cofrinhoHateoas",
                "create",
                "POST"));

            links.Add(new DTOs.HateoasLink(
                $"{baseUrl}/cofrinhoHateoas",
                "update",
                "PUT"));

            return links;
        }
    }
}