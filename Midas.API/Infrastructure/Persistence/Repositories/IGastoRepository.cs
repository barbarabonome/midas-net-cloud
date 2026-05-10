using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.Infrastructure.Persistence.Repositories
{
    public interface IGastoRepository
    {
        Task<Gasto> AddAsync(Gasto gasto);
        Task<Gasto?> GetByIdAsync(int id);
        Task<List<Gasto>> GetAllAsync();
        Task UpdateAsync(Gasto gasto);
        Task DeleteAsync(int id);
        Task<PagedResult<Gasto>> SearchAsync(GastoSearchParameters parameters);
    }
}
