using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.Infrastructure.Persistence.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria> AddAsync(Categoria categoria);
        Task<Categoria?> GetByIdAsync(int id);
        Task<List<Categoria>> GetAllAsync();
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);
        Task<PagedResult<Categoria>> SearchAsync(CategoriaSearchParameters parameters);
    }
}