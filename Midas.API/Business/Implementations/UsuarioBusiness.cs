using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.Infrastructure.Persistence.Repositories;
using Midas.API.Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Midas.API.Business.Implementations
{
    public class UsuarioBusiness : IUsuarioBusiness
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioBusiness> _logger;

        public UsuarioBusiness(IUsuarioRepository usuarioRepository, ILogger<UsuarioBusiness> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            _logger.LogInformation("Business: Buscando todos os usuários");
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Business: Buscando usuário por ID: {UsuarioId}", id);
            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task<Usuario> CreateAsync(CreateUsuarioDTO createUsuarioDTO)
        {
            _logger.LogInformation("Business: Criando usuário com email: {Email}", createUsuarioDTO.Email);

            if (string.IsNullOrWhiteSpace(createUsuarioDTO.Nome))
                throw new ArgumentException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(createUsuarioDTO.Email))
                throw new ArgumentException("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(createUsuarioDTO.Senha))
                throw new ArgumentException("Senha é obrigatória");

            if (await _usuarioRepository.EmailExistsAsync(createUsuarioDTO.Email))
                throw new InvalidOperationException("Email já está em uso");

            var usuario = new Usuario
            {
                Nome = createUsuarioDTO.Nome,
                Email = createUsuarioDTO.Email,
                Senha = createUsuarioDTO.Senha
            };

            var result = await _usuarioRepository.AddAsync(usuario);
            _logger.LogInformation("Business: Usuário criado com sucesso. ID: {UsuarioId}", result.Id);
            return result;
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _logger.LogInformation("Business: Atualizando usuário: {UsuarioId}", usuario.Id);

            var existingUser = await _usuarioRepository.GetByIdAsync(usuario.Id);
            if (existingUser == null)
                throw new KeyNotFoundException($"Usuário com ID {usuario.Id} não encontrado");

            await _usuarioRepository.UpdateAsync(usuario);
            _logger.LogInformation("Business: Usuário atualizado com sucesso. ID: {UsuarioId}", usuario.Id);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Business: Deletando usuário: {UsuarioId}", id);

            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");

            await _usuarioRepository.DeleteAsync(id);
            _logger.LogInformation("Business: Usuário deletado com sucesso. ID: {UsuarioId}", id);
        }

        public async Task<PagedResult<Usuario>> SearchAsync(UsuarioSearchParameters parameters)
        {
            _logger.LogInformation("Business: Buscando usuários com filtros. Nome: {Nome}, Email: {Email}", parameters.Nome, parameters.Email);
            return await _usuarioRepository.SearchAsync(parameters);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            _logger.LogInformation("Business: Verificando se email existe: {Email}", email);
            return await _usuarioRepository.EmailExistsAsync(email);
        }
    }
}
