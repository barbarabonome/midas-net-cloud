using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.API.Business.Interfaces;

namespace Midas.Controllers
{
    [ApiController]
    [Route("api/gastos")]
    public class GastoController : ControllerBase
    {
        private readonly IGastoBusiness _gastoBusiness;

        public GastoController(IGastoBusiness gastoBusiness)
        {
            _gastoBusiness = gastoBusiness;
        }

        [HttpGet]
        public async Task<ActionResult<List<Gasto>>> GetAll()
        {
            try
            {
                var gastos = await _gastoBusiness.GetAllAsync();
                return Ok(gastos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Gasto>> GetById(int id)
        {
            try
            {
                var gasto = await _gastoBusiness.GetByIdAsync(id);
                if (gasto == null)
                    return NotFound();

                return Ok(gasto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Gasto>> Create([FromBody] CreateGastoDTO createGastoDTO)
        {
            try
            {
                if (createGastoDTO == null)
                    return BadRequest("Dados do gasto são obrigatórios");

                var result = await _gastoBusiness.CreateAsync(createGastoDTO);
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
        public async Task<IActionResult> Update(int id, [FromBody] Gasto gasto)
        {
            try
            {
                if (gasto == null)
                    return BadRequest();

                if (id != gasto.Id)
                    return BadRequest();

                await _gastoBusiness.UpdateAsync(gasto);
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
                await _gastoBusiness.DeleteAsync(id);
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
        public async Task<ActionResult<PagedResult<Gasto>>> Search([FromQuery] GastoSearchParameters parameters)
        {
            try
            {
                var result = await _gastoBusiness.SearchAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
