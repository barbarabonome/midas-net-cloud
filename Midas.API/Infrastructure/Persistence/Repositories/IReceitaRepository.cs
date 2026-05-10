using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.Infrastructure.Persistence.Repositories
{
    public interface IReceitaRepository
    {
        Task<Receita> CreateAsync(Receita receita);
        Task<Receita?> GetByIdAsync(int id);
        Task<IEnumerable<Receita>> GetAllAsync();
        Task<Receita> UpdateAsync(Receita receita);
        Task DeleteAsync(int id);
        Task<PagedResult<Receita>> SearchAsync(ReceitaSearchParameters parameters);
    }
}
