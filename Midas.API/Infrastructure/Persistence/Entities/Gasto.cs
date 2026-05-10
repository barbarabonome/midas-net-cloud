namespace Midas.Infrastructure.Persistence.Entities
{
    public class Gasto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int CategoriaId { get; set; }
        public string Titulo { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public char Fixo { get; set; } = 'F';
    }
}
