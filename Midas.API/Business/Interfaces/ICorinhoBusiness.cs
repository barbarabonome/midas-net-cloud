using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.API.Business.Interfaces
{
    public interface ICorinhoBusiness
    {
        Task<List<Cofrinho>> GetAllAsync();
        Task<Cofrinho?> GetByIdAsync(int id);
        Task<Cofrinho> CreateAsync(CreateCofrinhoDTO createCofrinhoDTO);
        Task UpdateAsync(Cofrinho cofrinho);
        Task DeleteAsync(int id);
        Task<PagedResult<Cofrinho>> SearchAsync(CofrinhoSearchParameters parameters);
        Task<bool> AtualizarProgressoAsync(int id, decimal valorAtingido);
    }
}
