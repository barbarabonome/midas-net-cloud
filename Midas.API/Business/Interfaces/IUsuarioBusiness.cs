using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;

namespace Midas.API.Business.Interfaces
{
    public interface IUsuarioBusiness
    {
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> CreateAsync(CreateUsuarioDTO createUsuarioDTO);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<PagedResult<Usuario>> SearchAsync(UsuarioSearchParameters parameters);
        Task<bool> EmailExistsAsync(string email);
    }
}
