using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.Infrastructure.Persistence.Repositories
{
    public interface ICofrinhoRepository
    {
        Task<Cofrinho> AddAsync(Cofrinho cofrinho);
        Task<Cofrinho?> GetByIdAsync(int id);
        Task<List<Cofrinho>> GetAllAsync();
        Task UpdateAsync(Cofrinho cofrinho);
        Task DeleteAsync(int id);
        Task<bool> AtualizarProgresso(int id, decimal valorAtingido);
        Task<PagedResult<Cofrinho>> SearchAsync(CofrinhoSearchParameters parameters);
    }
}
