namespace Midas.API.DTOs
{
    public class SearchParameters
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string OrderBy { get; set; } = "Id";
        public string Direction { get; set; } = "asc";

 public int Skip => (Page - 1) * Size;
    }

    public class UsuarioSearchParameters : SearchParameters
    {
 public string? Nome { get; set; }
    public string? Email { get; set; }
        public DateTime? DataCriacaoInicio { get; set; }
  public DateTime? DataCriacaoFim { get; set; }
    }

    public class GastoSearchParameters : SearchParameters
    {
    public string? Titulo { get; set; }
        public int? UsuarioId { get; set; }
 public int? CategoriaId { get; set; }
  public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
  public decimal? ValorMinimo { get; set; }
        public decimal? ValorMaximo { get; set; }
        public char? Fixo { get; set; }
    }

    public class ReceitaSearchParameters : SearchParameters
    {
        public string? Titulo { get; set; }
  public int? UsuarioId { get; set; }
      public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public decimal? ValorMinimo { get; set; }
        public decimal? ValorMaximo { get; set; }
      public char? Fixo { get; set; }
    }

    public class CofrinhoSearchParameters : SearchParameters
    {
        public string? Titulo { get; set; }
      public int? UsuarioId { get; set; }
   public decimal? MetaMinima { get; set; }
        public decimal? MetaMaxima { get; set; }
        public char? Aplicado { get; set; }
    }

public class CategoriaSearchParameters : SearchParameters
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
    }
}