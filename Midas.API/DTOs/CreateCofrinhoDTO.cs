namespace Midas.API.DTOs
{
    public class CreateCofrinhoDTO
    {
        public int UsuarioId { get; set; }
        public string Titulo { get; set; }
        public decimal Meta { get; set; }
        public decimal Atingido { get; set; } = 0;
        public char Aplicado { get; set; } = 'F';
    }
}