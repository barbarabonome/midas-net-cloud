namespace Midas.Infrastructure.Persistence.Entities
{
    public class GastoFixo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Categoria { get; set; }
        public int DataVencimento { get; set; }
    }
}