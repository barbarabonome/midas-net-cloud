using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.API.Business.Interfaces
{
    public interface IReceitaBusiness
    {
        Task<List<Receita>> GetAllAsync();
        Task<Receita?> GetByIdAsync(int id);
        Task<Receita> CreateAsync(CreateReceitaDTO createReceitaDTO);
        Task UpdateAsync(Receita receita);
        Task DeleteAsync(int id);
        Task<PagedResult<Receita>> SearchAsync(ReceitaSearchParameters parameters);
    }
}
