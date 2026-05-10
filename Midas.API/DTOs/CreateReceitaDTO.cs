namespace Midas.API.DTOs
{
    public class CreateReceitaDTO
    {
        public int UsuarioId { get; set; }
        public string Titulo { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public char Fixo { get; set; } = 'F';
    }
}