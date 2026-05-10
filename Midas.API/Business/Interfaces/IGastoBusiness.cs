using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.API.Business.Interfaces
{
    public interface IGastoBusiness
    {
        Task<List<Gasto>> GetAllAsync();
        Task<Gasto?> GetByIdAsync(int id);
        Task<Gasto> CreateAsync(CreateGastoDTO createGastoDTO);
        Task UpdateAsync(Gasto gasto);
        Task DeleteAsync(int id);
        Task<PagedResult<Gasto>> SearchAsync(GastoSearchParameters parameters);
    }
}
