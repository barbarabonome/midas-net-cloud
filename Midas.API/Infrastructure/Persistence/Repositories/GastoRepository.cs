using Microsoft.EntityFrameworkCore;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.Infrastructure.Persistence.Repositories
{
    public class GastoRepository : IGastoRepository
    {
        private readonly MidasContext _context;

        public GastoRepository(MidasContext context)
        {
            _context = context;
        }

        public async Task<Gasto> AddAsync(Gasto gasto)
        {
            await _context.Gastos.AddAsync(gasto);
                    await _context.SaveChangesAsync();
            return gasto;
        }

        public async Task<Gasto?> GetByIdAsync(int id)
        {
            return await _context.Gastos
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Gasto>> GetAllAsync()
        {
            return await _context.Gastos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(Gasto gasto)
        {
            _context.Gastos.Update(gasto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var gasto = await _context.Gastos.FindAsync(id);
            if (gasto != null)
            {
                _context.Gastos.Remove(gasto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<Gasto>> SearchAsync(GastoSearchParameters parameters)
        {
            var query = _context.Gastos.AsQueryable();

        if (!string.IsNullOrEmpty(parameters.Titulo))
        {
            query = query.Where(g => g.Titulo.Contains(parameters.Titulo));
        }

        if (parameters.UsuarioId.HasValue)
        {
            query = query.Where(g => g.UsuarioId == parameters.UsuarioId.Value);
        }

         if (parameters.CategoriaId.HasValue)
        {
            query = query.Where(g => g.CategoriaId == parameters.CategoriaId.Value);
        }

        if (parameters.DataInicio.HasValue)
        {
            query = query.Where(g => g.Data >= parameters.DataInicio.Value);
        }

     if (parameters.DataFim.HasValue)
{
    query = query.Where(g => g.Data <= parameters.DataFim.Value);
    }

            if (parameters.ValorMinimo.HasValue)
  {
         query = query.Where(g => g.Valor >= parameters.ValorMinimo.Value);
      }

     if (parameters.ValorMaximo.HasValue)
         {
   query = query.Where(g => g.Valor <= parameters.ValorMaximo.Value);
  }

    if (parameters.Fixo.HasValue)
         {
        query = query.Where(g => g.Fixo == parameters.Fixo.Value);
 }

     var totalRecords = await query.CountAsync();

 query = parameters.OrderBy.ToLower() switch
      {
   "titulo" => parameters.Direction.ToLower() == "desc"
   ? query.OrderByDescending(g => g.Titulo)
  : query.OrderBy(g => g.Titulo),
     "data" => parameters.Direction.ToLower() == "desc"
        ? query.OrderByDescending(g => g.Data)
       : query.OrderBy(g => g.Data),
     "valor" => parameters.Direction.ToLower() == "desc"
       ? query.OrderByDescending(g => g.Valor)
     : query.OrderBy(g => g.Valor),
            "usuarioid" => parameters.Direction.ToLower() == "desc"
  ? query.OrderByDescending(g => g.UsuarioId)
     : query.OrderBy(g => g.UsuarioId),
     "categoriaid" => parameters.Direction.ToLower() == "desc"
    ? query.OrderByDescending(g => g.CategoriaId)
       : query.OrderBy(g => g.CategoriaId),
       _ => parameters.Direction.ToLower() == "desc"
      ? query.OrderByDescending(g => g.Id)
    : query.OrderBy(g => g.Id)
          };

     var data = await query
     .Skip(parameters.Skip)
 .Take(parameters.Size)
           .AsNoTracking()
   .ToListAsync();

      return new PagedResult<Gasto>(data, totalRecords, parameters.Page, parameters.Size);
      }
    }
}
