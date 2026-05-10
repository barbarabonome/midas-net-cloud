using Midas.Infrastructure.Persistence.Entities;
using Midas.Infrastructure.Persistence.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Midas.UseCase;

public class UsuarioUseCase : IUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioUseCase(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario> RegisterAsync(string nome, string email, string senha)
    {
        if (await _usuarioRepository.EmailExistsAsync(email))
            throw new InvalidOperationException("Email já está em uso");

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Senha = HashSenha(senha),
            DataCriacao = DateTime.UtcNow
        };

        return await _usuarioRepository.AddAsync(usuario);
    }

    public async Task<Usuario> LoginAsync(string email, string senha)
    {
        var senhaHash = HashSenha(senha);
        var usuario = await _usuarioRepository.GetByEmailAsync(email);

        if (usuario == null || usuario.Senha != senhaHash)
            throw new InvalidOperationException("Email ou senha inválidos");

        return usuario;
    }

    public Task<bool> LogoutAsync()
    {
        // Implementação do logout será feita no frontend
        return Task.FromResult(true);
    }

    public async Task<Usuario> GetByIdAsync(int id)
    {
        return await _usuarioRepository.GetByIdAsync(id);
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        return await _usuarioRepository.GetAllAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _usuarioRepository.DeleteAsync(id);
    }

    private string HashSenha(string senha)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }
}
