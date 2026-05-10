using Microsoft.EntityFrameworkCore;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.Infrastructure.Persistence.Repositories;

public class CofrinhoRepository : ICofrinhoRepository
{
    private readonly MidasContext _context;

    public CofrinhoRepository(MidasContext context)
    {
        _context = context;
    }

    public async Task<Cofrinho> AddAsync(Cofrinho cofrinho)
    {
        cofrinho.Aplicado = NormalizeAplicadoValue(cofrinho.Aplicado);
        await _context.Cofrinhos.AddAsync(cofrinho);
        await _context.SaveChangesAsync();
        return cofrinho;
    }

    public async Task<Cofrinho?> GetByIdAsync(int id)
    {
        return await _context.Cofrinhos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Cofrinho>> GetAllAsync()
    {
        return await _context.Cofrinhos
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateAsync(Cofrinho cofrinho)
    {
        cofrinho.Aplicado = NormalizeAplicadoValue(cofrinho.Aplicado);
        _context.Cofrinhos.Update(cofrinho);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cofrinho = await _context.Cofrinhos.FindAsync(id);
        if (cofrinho != null)
        {
            _context.Cofrinhos.Remove(cofrinho);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> AtualizarProgresso(int id, decimal valorAtingido)
    {
        var cofrinho = await _context.Cofrinhos.FindAsync(id);
        if (cofrinho == null) return false;

        cofrinho.Atingido = valorAtingido;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<Cofrinho>> SearchAsync(CofrinhoSearchParameters parameters)
    {
        var query = _context.Cofrinhos.AsQueryable();

        if (!string.IsNullOrEmpty(parameters.Titulo))
        {
            query = query.Where(c => c.Titulo.Contains(parameters.Titulo));
        }

        if (parameters.UsuarioId.HasValue)
        {
            query = query.Where(c => c.UsuarioId == parameters.UsuarioId.Value);
        }

        if (parameters.MetaMinima.HasValue)
        {
            query = query.Where(c => c.Meta >= parameters.MetaMinima.Value);
        }

        if (parameters.MetaMaxima.HasValue)
        {
            query = query.Where(c => c.Meta <= parameters.MetaMaxima.Value);
        }

        if (parameters.Aplicado.HasValue)
        {
            query = query.Where(c => c.Aplicado == parameters.Aplicado.Value);
        }

        var totalRecords = await query.CountAsync();

        query = parameters.OrderBy.ToLower() switch
        {
            "titulo" => parameters.Direction.ToLower() == "desc"
                ? query.OrderByDescending(c => c.Titulo)
                : query.OrderBy(c => c.Titulo),
            "meta" => parameters.Direction.ToLower() == "desc"
                ? query.OrderByDescending(c => c.Meta)
                : query.OrderBy(c => c.Meta),
            "atingido" => parameters.Direction.ToLower() == "desc"
                ? query.OrderByDescending(c => c.Atingido)
                : query.OrderBy(c => c.Atingido),
            "usuarioid" => parameters.Direction.ToLower() == "desc"
                ? query.OrderByDescending(c => c.UsuarioId)
                : query.OrderBy(c => c.UsuarioId),
            _ => parameters.Direction.ToLower() == "desc"
                ? query.OrderByDescending(c => c.Id)
                : query.OrderBy(c => c.Id)
        };

        var data = await query
            .Skip(parameters.Skip)
            .Take(parameters.Size)
            .AsNoTracking()
            .ToListAsync();

        return new PagedResult<Cofrinho>(data, totalRecords, parameters.Page, parameters.Size);
    }

    private char NormalizeAplicadoValue(char aplicado)
    {
        return char.ToUpper(aplicado) == 'T' ? 'T' : 'F';
    }
}
