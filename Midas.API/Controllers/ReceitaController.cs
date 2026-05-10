using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.API.Business.Interfaces;

namespace Midas.API.Controllers
{
    [ApiController]
    [Route("api/receitas")]
    public class ReceitaController : ControllerBase
    {
        private readonly IReceitaBusiness _receitaBusiness;
        private readonly ILogger<ReceitaController> _logger;

        public ReceitaController(IReceitaBusiness receitaBusiness, ILogger<ReceitaController> logger)
        {
            _receitaBusiness = receitaBusiness;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Receita>>> GetAll()
        {
            try
            {
                var receitas = await _receitaBusiness.GetAllAsync();
                return Ok(receitas.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Receita>> GetById(int id)
        {
            try
            {
                var receita = await _receitaBusiness.GetByIdAsync(id);
                if (receita == null)
                    return NotFound();

                return Ok(receita);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Receita>> Create([FromBody] CreateReceitaDTO createReceitaDTO)
        {
            try
            {
                if (createReceitaDTO == null)
                    return BadRequest("Dados da receita são obrigatórios");

                var result = await _receitaBusiness.CreateAsync(createReceitaDTO);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Receita receita)
        {
            try
            {
                if (receita == null)
                    return BadRequest("Dados da receita são obrigatórios");

                if (id != receita.Id)
                    return BadRequest("ID do parâmetro não confere com o ID da receita");

                await _receitaBusiness.UpdateAsync(receita);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _receitaBusiness.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<Receita>>> Search([FromQuery] ReceitaSearchParameters parameters)
        {
            try
            {
                var result = await _receitaBusiness.SearchAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Erro ao buscar receitas com parâmetros: {@Parameters}", parameters);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
