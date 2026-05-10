using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GastoController : ControllerBase
{
    private readonly IGastoRepository _gastoRepository;

    public GastoController(IGastoRepository gastoRepository)
    {
        _gastoRepository = gastoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Gasto>>> GetAll()
    {
        return await _gastoRepository.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Gasto>> GetById(int id)
    {
        var gasto = await _gastoRepository.GetByIdAsync(id);
        if (gasto == null)
            return NotFound();

        return gasto;
    }

    [HttpPost]
    public async Task<ActionResult<Gasto>> Create([FromBody] Gasto gasto)
    {
        var result = await _gastoRepository.AddAsync(gasto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Gasto gasto)
    {
        if (id != gasto.Id)
            return BadRequest();

        await _gastoRepository.UpdateAsync(gasto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _gastoRepository.DeleteAsync(id);
        return NoContent();
    }
}
