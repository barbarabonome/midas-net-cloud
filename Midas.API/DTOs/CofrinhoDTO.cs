namespace Midas.API.DTOs
{
    public class CofrinhoDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Titulo { get; set; }
        public decimal Meta { get; set; }
        public decimal Atingido { get; set; }
        public char Aplicado { get; set; }
    }
}
