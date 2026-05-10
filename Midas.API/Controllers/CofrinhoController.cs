using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.API.Business.Interfaces;

namespace Midas.Controllers
{
    [ApiController]
    [Route("api/cofrinhos")]
    public class CofrinhoController : ControllerBase
    {
        private readonly ICorinhoBusiness _corinhoBusiness;

        public CofrinhoController(ICorinhoBusiness corinhoBusiness)
        {
            _corinhoBusiness = corinhoBusiness;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cofrinho>>> GetAll()
        {
            try
            {
                var cofrinhos = await _corinhoBusiness.GetAllAsync();
                return Ok(cofrinhos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cofrinho>> GetById(int id)
        {
            try
            {
                var cofrinho = await _corinhoBusiness.GetByIdAsync(id);
                if (cofrinho == null)
                    return NotFound();

                return Ok(cofrinho);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Cofrinho>> Create([FromBody] CreateCofrinhoDTO createCofrinhoDTO)
        {
            try
            {
                if (createCofrinhoDTO == null)
                    return BadRequest("Dados do cofrinho são obrigatórios");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _corinhoBusiness.CreateAsync(createCofrinhoDTO);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar cofrinho: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Cofrinho cofrinho)
        {
            try
            {
                if (id != cofrinho.Id)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _corinhoBusiness.UpdateAsync(cofrinho);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar cofrinho: {ex.Message}");
            }
        }

        [HttpPut("{id}/progresso")]
        public async Task<IActionResult> AtualizarProgresso(int id, [FromBody] decimal valorAtingido)
        {
            try
            {
                var result = await _corinhoBusiness.AtualizarProgressoAsync(id, valorAtingido);
                if (!result)
                    return NotFound();

                return NoContent();
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
                await _corinhoBusiness.DeleteAsync(id);
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
        public async Task<ActionResult<PagedResult<Cofrinho>>> Search([FromQuery] CofrinhoSearchParameters parameters)
        {
            try
            {
                var result = await _corinhoBusiness.SearchAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
