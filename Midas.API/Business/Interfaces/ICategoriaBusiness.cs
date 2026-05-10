using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.API.Business.Interfaces
{
    public interface ICategoriaBusiness
    {
        Task<List<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<Categoria> CreateAsync(CreateCategoriaDTO createCategoriaDTO);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);
        Task<PagedResult<Categoria>> SearchAsync(CategoriaSearchParameters parameters);
    }
}
