using Microsoft.EntityFrameworkCore;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.Infrastructure.Persistence.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly MidasContext _context;

        public CategoriaRepository(MidasContext context)
        {
            _context = context;
        }

        public async Task<Categoria> AddAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            return await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Categoria>> GetAllAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<Categoria>> SearchAsync(CategoriaSearchParameters parameters)
        {
            var query = _context.Categorias.AsQueryable();

            if (!string.IsNullOrEmpty(parameters.Nome))
            {
                query = query.Where(c => c.Nome.Contains(parameters.Nome));
            }

            if (!string.IsNullOrEmpty(parameters.Descricao))
            {
                query = query.Where(c => c.Descricao.Contains(parameters.Descricao));
            }

            var totalRecords = await query.CountAsync();

            query = parameters.OrderBy.ToLower() switch
            {
                "nome" => parameters.Direction.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Nome)
                    : query.OrderBy(c => c.Nome),
                "descricao" => parameters.Direction.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Descricao)
                    : query.OrderBy(c => c.Descricao),
                _ => parameters.Direction.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.Id)
                    : query.OrderBy(c => c.Id)
            };

            var data = await query
                .Skip(parameters.Skip)
                .Take(parameters.Size)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Categoria>(data, totalRecords, parameters.Page, parameters.Size);
        }
    }
}