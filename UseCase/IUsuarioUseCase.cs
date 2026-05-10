using System;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.UseCase
{
    public class Class1
    {
        public Class1()
        {
        }
    }

    public interface IUsuarioUseCase
    {
        Task<Usuario> RegisterAsync(string nome, string email, string senha);
        Task<Usuario> LoginAsync(string email, string senha);
        Task<bool> LogoutAsync();
        Task<Usuario> GetByIdAsync(int id);
        Task<List<Usuario>> GetAllAsync();
        Task DeleteAsync(int id);
    }
}
