using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.API.Business.Interfaces;

namespace Midas.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaBusiness _categoriaBusiness;

        public CategoriaController(ICategoriaBusiness categoriaBusiness)
        {
            _categoriaBusiness = categoriaBusiness;
        }

        [HttpGet]
        public async Task<ActionResult<List<Categoria>>> GetAll()
        {
            try
            {
                var categorias = await _categoriaBusiness.GetAllAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetById(int id)
        {
            try
            {
                var categoria = await _categoriaBusiness.GetByIdAsync(id);
                if (categoria == null)
                    return NotFound();

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> Create([FromBody] CreateCategoriaDTO createCategoriaDTO)
        {
            try
            {
                if (createCategoriaDTO == null)
                    return BadRequest("Dados da categoria são obrigatórios");

                var result = await _categoriaBusiness.CreateAsync(createCategoriaDTO);
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
        public async Task<IActionResult> Update(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (categoria == null)
                    return BadRequest("Dados da categoria são obrigatórios");

                if (id != categoria.Id)
                    return BadRequest();

                await _categoriaBusiness.UpdateAsync(categoria);
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
                await _categoriaBusiness.DeleteAsync(id);
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
        public async Task<ActionResult<PagedResult<Categoria>>> Search([FromQuery] CategoriaSearchParameters parameters)
        {
            try
            {
                var result = await _categoriaBusiness.SearchAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}