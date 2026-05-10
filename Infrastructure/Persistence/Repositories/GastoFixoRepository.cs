using Midas.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Midas.Infrastructure.Persistence.Repositories;

public class GastoFixoRepository : IGastoFixoRepository
{
    private readonly DbContext _context;

    public GastoFixoRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<GastoFixo> AddAsync(GastoFixo gastoFixo)
    {
        _context.Set<GastoFixo>().Add(gastoFixo);
        await _context.SaveChangesAsync();
        return gastoFixo;
    }

    public async Task DeleteAsync(int id)
    {
        var gastoFixo = await _context.Set<GastoFixo>().FindAsync(id);
        
        if (gastoFixo is not null)
            _context.Set<GastoFixo>().Remove(gastoFixo);
        
        await _context.SaveChangesAsync();
    }

    public async Task<GastoFixo> GetByIdAsync(int id)
    {
        return await _context.Set<GastoFixo>().FindAsync(id);
    }

    public async Task<List<GastoFixo>> GetAllAsync()
    {
        return await _context.Set<GastoFixo>().ToListAsync();
    }

    public async Task UpdateAsync(GastoFixo gastoFixo)
    {
        _context.Entry(gastoFixo).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}