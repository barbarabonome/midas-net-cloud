using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence.Repositories;

public interface IGastoFixoRepository
{
    Task<GastoFixo> AddAsync(GastoFixo gastoFixo);
    Task DeleteAsync(int id);
    Task<GastoFixo> GetByIdAsync(int id);
    Task<List<GastoFixo>> GetAllAsync();
    Task UpdateAsync(GastoFixo gastoFixo);
}