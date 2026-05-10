namespace Midas.Infrastructure.Persistence.Entities
{
    public class Cofrinho
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public decimal ValorMeta { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime Limite { get; set; }
    }
}
