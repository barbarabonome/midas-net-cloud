using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario> AddAsync(Usuario usuario);
    Task DeleteAsync(int id);
    Task<Usuario> GetByIdAsync(int id);
    Task<List<Usuario>> GetAllAsync();
    Task<Usuario> GetByEmailAsync(string email);
    Task<Usuario> AuthenticateAsync(string email, string senha);
    Task<bool> EmailExistsAsync(string email);
}
