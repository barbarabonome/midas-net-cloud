using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Microsoft.Extensions.Logging;
using Midas.API.Business.Interfaces;

namespace Midas.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioBusiness _usuarioBusiness;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioBusiness usuarioBusiness, ILogger<UsuarioController> logger)
        {
            _usuarioBusiness = usuarioBusiness;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> GetAll()
        {
            _logger.LogInformation("Controller: Buscando todos os usuários");

            try
            {
                var usuarios = await _usuarioBusiness.GetAllAsync();
                _logger.LogInformation("Controller: Total de usuários retornados: {UsuarioCount}", usuarios.Count);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Erro ao listar todos os usuários");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetById(int id)
        {
            _logger.LogInformation("Controller: Buscando usuário por ID: {UsuarioId}", id);

            try
            {
                var usuario = await _usuarioBusiness.GetByIdAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Controller: Usuário não encontrado com ID: {UsuarioId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Controller: Usuário encontrado com sucesso: {UsuarioId}", id);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Erro ao buscar usuário por ID: {UsuarioId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> Create([FromBody] CreateUsuarioDTO createUsuarioDTO)
        {
            try
            {
                _logger.LogInformation("Controller: Tentando criar usuário com email: {Email}", createUsuarioDTO?.Email);

                if (createUsuarioDTO == null)
                {
                    _logger.LogWarning("Controller: Tentativa de criar usuário com dados nulos");
                    return BadRequest("Dados do usuário são obrigatórios");
                }

                var result = await _usuarioBusiness.CreateAsync(createUsuarioDTO);
                _logger.LogInformation("Controller: Usuário criado com sucesso. ID: {UsuarioId}, Email: {Email}", result.Id, result.Email);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Controller: Erro de validação ao criar usuário");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Controller: Email já existe");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Erro ao criar usuário com email: {Email}", createUsuarioDTO?.Email);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Usuario usuario)
        {
            _logger.LogInformation("Controller: Tentando atualizar usuário: {UsuarioId}", id);

            try
            {
                if (usuario == null)
                {
                    _logger.LogWarning("Controller: Tentativa de atualizar usuário com dados nulos. ID: {UsuarioId}", id);
                    return BadRequest("Dados do usuário são obrigatórios");
                }

                if (id != usuario.Id)
                {
                    _logger.LogWarning("Controller: ID do parâmetro não confere com ID do usuário. ParâmetroId: {ParametroId}, UsuarioId: {UsuarioId}", id, usuario.Id);
                    return BadRequest("ID do parâmetro não confere com o ID do usuário");
                }

                await _usuarioBusiness.UpdateAsync(usuario);
                _logger.LogInformation("Controller: Usuário atualizado com sucesso. ID: {UsuarioId}", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Controller: Usuário não encontrado para atualização");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Erro ao atualizar usuário: {UsuarioId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Controller: Tentando deletar usuário: {UsuarioId}", id);

            try
            {
                await _usuarioBusiness.DeleteAsync(id);
                _logger.LogInformation("Controller: Usuário deletado com sucesso. ID: {UsuarioId}", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Controller: Usuário não encontrado para deleção");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Erro ao deletar usuário: {UsuarioId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<Usuario>>> Search([FromQuery] UsuarioSearchParameters parameters)
        {
            _logger.LogInformation("Controller: Buscando usuários com filtros. Nome: {Nome}, Email: {Email}", parameters.Nome, parameters.Email);

            try
            {
                var result = await _usuarioBusiness.SearchAsync(parameters);
                _logger.LogInformation("Controller: Busca executada com sucesso. Total de registros: {TotalRecords}, Página: {CurrentPage}", result.TotalRecords, result.CurrentPage);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Erro ao buscar usuários com parâmetros: {@Parameters}", parameters);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
