using System;
using Midas.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Midas.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DbContext _context;

    public UsuarioRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        _context.Set<Usuario>().Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task DeleteAsync(int id)
    {
        var usuario = await _context.Set<Usuario>().FindAsync(id);
        
        if (usuario is not null)
            _context.Set<Usuario>().Remove(usuario);
        
        await _context.SaveChangesAsync();
    }

    public async Task<Usuario> GetByIdAsync(int id)
    {
        return await _context.Set<Usuario>().FindAsync(id);
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        return await _context.Set<Usuario>().ToListAsync();
    }

    public async Task<Usuario> GetByEmailAsync(string email)
    {
        return await _context.Set<Usuario>()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario> AuthenticateAsync(string email, string senha)
    {
        return await _context.Set<Usuario>()
            .FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Set<Usuario>()
            .AnyAsync(u => u.Email == email);
    }
}
