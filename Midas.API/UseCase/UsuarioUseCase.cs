using Midas.Infrastructure.Persistence.Entities;
using Midas.Infrastructure.Persistence.Repositories;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Midas.UseCase;

public class UsuarioUseCase : IUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuarioUseCase> _logger;

    public UsuarioUseCase(IUsuarioRepository usuarioRepository, ILogger<UsuarioUseCase> logger)
    {
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    public async Task<Usuario> RegisterAsync(string nome, string email, string senha)
    {
        _logger.LogInformation("Iniciando registro de novo usuário: {Email}", email);

        if (await _usuarioRepository.EmailExistsAsync(email))
        {
            _logger.LogWarning("Tentativa de registro com email já existente: {Email}", email);
            throw new InvalidOperationException("Email já está em uso");
        }

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Senha = HashSenha(senha)
        };

        try
        {
            var result = await _usuarioRepository.AddAsync(usuario);
            _logger.LogInformation("Usuário registrado com sucesso. ID: {UsuarioId}, Email: {Email}", result.Id, email);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário: {Email}", email);
            throw;
        }
    }

    public async Task<Usuario> LoginAsync(string email, string senha)
    {
        _logger.LogInformation("Tentativa de login para email: {Email}", email);

        var senhaHash = HashSenha(senha);

        try
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);

            if (usuario == null || usuario.Senha != senhaHash)
            {
                _logger.LogWarning("Falha na autenticação para email: {Email}. Email não encontrado ou senha inválida", email);
                throw new InvalidOperationException("Email ou senha inválidos");
            }

            _logger.LogInformation("Login bem-sucedido para usuário: {UsuarioId}, Email: {Email}", usuario.Id, email);
            return usuario;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login para email: {Email}", email);
            throw;
        }
    }

    public Task<bool> LogoutAsync()
    {
        _logger.LogInformation("Logout executado");
        return Task.FromResult(true);
    }

    public async Task<Usuario> GetByIdAsync(int id)
    {
        _logger.LogInformation("Buscando usuário por ID: {UsuarioId}", id);

        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado com ID: {UsuarioId}", id);
                return null;
            }

            _logger.LogInformation("Usuário encontrado: {UsuarioId}", id);
            return usuario;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por ID: {UsuarioId}", id);
            throw;
        }
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os usuários");

        try
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            _logger.LogInformation("Total de usuários encontrados: {UsuarioCount}", usuarios.Count);
            return usuarios;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os usuários");
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Iniciando exclusão de usuário: {UsuarioId}", id);

        try
        {
            await _usuarioRepository.DeleteAsync(id);
            _logger.LogInformation("Usuário deletado com sucesso: {UsuarioId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar usuário: {UsuarioId}", id);
            throw;
        }
    }

    private string HashSenha(string senha)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        return Convert.ToBase64String(bytes);
    }
}
