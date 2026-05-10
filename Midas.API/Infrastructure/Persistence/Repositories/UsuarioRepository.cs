using System;
using Midas.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Midas.API.DTOs;
using Microsoft.Extensions.Logging;

namespace Midas.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly MidasContext _context;
    private readonly ILogger<UsuarioRepository> _logger;

    public UsuarioRepository(MidasContext context, ILogger<UsuarioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        _logger.LogInformation("Adicionando novo usuário. Email: {Email}", usuario.Email);

        try
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuário adicionado com sucesso. ID: {UsuarioId}", usuario.Id);
            return usuario;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar usuário. Email: {Email}", usuario.Email);
            throw;
        }
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Buscando usuário por ID: {UsuarioId}", id);

        try
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado. ID: {UsuarioId}", id);
            }
            else
            {
                _logger.LogInformation("Usuário encontrado. ID: {UsuarioId}", id);
            }

            return usuario;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por ID: {UsuarioId}", id);
            throw;
        }
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        _logger.LogInformation("Buscando usuário por email: {Email}", email);

        try
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado para email: {Email}", email);
            }
            else
            {
                _logger.LogInformation("Usuário encontrado para email: {Email}", email);
            }

            return usuario;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
            throw;
        }
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos os usuários");

        try
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Total de usuários encontrados: {UsuarioCount}", usuarios.Count);
            return usuarios;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os usuários");
            throw;
        }
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _logger.LogInformation("Atualizando usuário. ID: {UsuarioId}, Email: {Email}", usuario.Id, usuario.Email);

        try
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Usuário atualizado com sucesso. ID: {UsuarioId}", usuario.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário. ID: {UsuarioId}", usuario.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deletando usuário. ID: {UsuarioId}", id);

        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Usuário deletado com sucesso. ID: {UsuarioId}", id);
            }
            else
            {
                _logger.LogWarning("Tentativa de deletar usuário não encontrado. ID: {UsuarioId}", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar usuário. ID: {UsuarioId}", id);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        _logger.LogInformation("Verificando se email existe: {Email}", email);

        try
        {
            var exists = await _context.Usuarios.AnyAsync(u => u.Email == email);

            if (exists)
            {
                _logger.LogWarning("Email já existe na base de dados: {Email}", email);
            }
            else
            {
                _logger.LogInformation("Email disponível: {Email}", email);
            }

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se email existe: {Email}", email);
            throw;
        }
    }

    public async Task<PagedResult<Usuario>> SearchAsync(UsuarioSearchParameters parameters)
    {
        _logger.LogInformation("Buscando usuários com filtros. Nome: {Nome}, Email: {Email}, Página: {Page}", parameters.Nome, parameters.Email, parameters.Page);

        try
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(parameters.Nome))
            {
                query = query.Where(u => u.Nome.Contains(parameters.Nome));
            }

            if (!string.IsNullOrEmpty(parameters.Email))
            {
                query = query.Where(u => u.Email.Contains(parameters.Email));
            }

            if (parameters.DataCriacaoInicio.HasValue)
            {
                query = query.Where(u => u.DataCriacao >= parameters.DataCriacaoInicio.Value);
            }

            if (parameters.DataCriacaoFim.HasValue)
            {
                query = query.Where(u => u.DataCriacao <= parameters.DataCriacaoFim.Value);
            }

            var totalRecords = await query.CountAsync();

            query = parameters.OrderBy.ToLower() switch
            {
                "nome" => parameters.Direction.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.Nome)
                    : query.OrderBy(u => u.Nome),
                "email" => parameters.Direction.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "datacriacao" => parameters.Direction.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.DataCriacao)
                    : query.OrderBy(u => u.DataCriacao),
                _ => parameters.Direction.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.Id)
                    : query.OrderBy(u => u.Id)
            };

            var data = await query
                .Skip(parameters.Skip)
                .Take(parameters.Size)
                .AsNoTracking()
                .ToListAsync();

            var result = new PagedResult<Usuario>(data, totalRecords, parameters.Page, parameters.Size);
            _logger.LogInformation("Busca concluída com sucesso. Total de registros: {TotalRecords}, Página: {CurrentPage}, Registros retornados: {Count}", totalRecords, parameters.Page, data.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários com filtros. Nome: {Nome}, Email: {Email}", parameters.Nome, parameters.Email);
            throw;
        }
    }
}
