using Microsoft.EntityFrameworkCore;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.Infrastructure.Persistence.Repositories
{
    public class ReceitaRepository : IReceitaRepository
    {
        private readonly MidasContext _context;

        public ReceitaRepository(MidasContext context)
    {
          _context = context;
     }

        public async Task<Receita> CreateAsync(Receita receita)
     {
   await _context.Receitas.AddAsync(receita);
  await _context.SaveChangesAsync();
        return receita;
        }

        public async Task<Receita?> GetByIdAsync(int id)
        {
            return await _context.Receitas
   .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
      }

     public async Task<IEnumerable<Receita>> GetAllAsync()
        {
            return await _context.Receitas
                .AsNoTracking()
              .ToListAsync();
     }

        public async Task<Receita> UpdateAsync(Receita receita)
   {
            _context.Receitas.Update(receita);
  await _context.SaveChangesAsync();
          return receita;
  }

   public async Task DeleteAsync(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);
   if (receita != null)
     {
  _context.Receitas.Remove(receita);
         await _context.SaveChangesAsync();
 }
        }

        public async Task<PagedResult<Receita>> SearchAsync(ReceitaSearchParameters parameters)
        {
      var query = _context.Receitas.AsQueryable();

         if (!string.IsNullOrEmpty(parameters.Titulo))
            {
     query = query.Where(r => r.Titulo.Contains(parameters.Titulo));
  }

            if (parameters.UsuarioId.HasValue)
   {
                query = query.Where(r => r.UsuarioId == parameters.UsuarioId.Value);
     }

   if (parameters.DataInicio.HasValue)
    {
        query = query.Where(r => r.Data >= parameters.DataInicio.Value);
     }

            if (parameters.DataFim.HasValue)
            {
    query = query.Where(r => r.Data <= parameters.DataFim.Value);
         }

          if (parameters.ValorMinimo.HasValue)
        {
          query = query.Where(r => r.Valor >= parameters.ValorMinimo.Value);
      }

            if (parameters.ValorMaximo.HasValue)
   {
     query = query.Where(r => r.Valor <= parameters.ValorMaximo.Value);
        }

            if (parameters.Fixo.HasValue)
            {
    query = query.Where(r => r.Fixo == parameters.Fixo.Value);
     }

            var totalRecords = await query.CountAsync();

  query = parameters.OrderBy.ToLower() switch
       {
    "titulo" => parameters.Direction.ToLower() == "desc"
   ? query.OrderByDescending(r => r.Titulo)
         : query.OrderBy(r => r.Titulo),
     "data" => parameters.Direction.ToLower() == "desc"
            ? query.OrderByDescending(r => r.Data)
             : query.OrderBy(r => r.Data),
       "valor" => parameters.Direction.ToLower() == "desc"
          ? query.OrderByDescending(r => r.Valor)
        : query.OrderBy(r => r.Valor),
                "usuarioid" => parameters.Direction.ToLower() == "desc"
           ? query.OrderByDescending(r => r.UsuarioId)
          : query.OrderBy(r => r.UsuarioId),
        _ => parameters.Direction.ToLower() == "desc"
    ? query.OrderByDescending(r => r.Id)
          : query.OrderBy(r => r.Id)
    };

    var data = await query
        .Skip(parameters.Skip)
   .Take(parameters.Size)
            .AsNoTracking()
     .ToListAsync();

      return new PagedResult<Receita>(data, totalRecords, parameters.Page, parameters.Size);
  }
    }
}