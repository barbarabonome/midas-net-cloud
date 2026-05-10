using Midas.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Midas.Infrastructure.Persistence.Repositories;

public class CofrinhoRepository : ICofrinhoRepository
{
    private readonly DbContext _context;

    public CofrinhoRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Cofrinho> AddAsync(Cofrinho meta)
    {
        _context.Set<Cofrinho>().Add(meta);
        await _context.SaveChangesAsync();
        return meta;
    }

    public async Task DeleteAsync(int id)
    {
        var meta = await _context.Set<Cofrinho>().FindAsync(id);
        
        if (meta is not null)
            _context.Set<Cofrinho>().Remove(meta);
        
        await _context.SaveChangesAsync();
    }

    public async Task<Cofrinho> GetByIdAsync(int id)
    {
        return await _context.Set<Cofrinho>().FindAsync(id);
    }

    public async Task<List<Cofrinho>> GetAllAsync()
    {
        return await _context.Set<Cofrinho>().ToListAsync();
    }

    public async Task UpdateAsync(Cofrinho meta)
    {
        _context.Entry(meta).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AtualizarProgresso(int id, decimal valorAtual)
    {
        var meta = await _context.Set<Cofrinho>().FindAsync(id);
        if (meta == null) return false;

        meta.ValorAtual = valorAtual;
        await _context.SaveChangesAsync();
        return true;
    }
}
