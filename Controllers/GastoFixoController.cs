using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Controllers;

[ApiController]
[Route("api/gastos-fixos")]
public class GastoFixoController : ControllerBase
{
    private readonly IGastoFixoRepository _gastoFixoRepository;

    public GastoFixoController(IGastoFixoRepository gastoFixoRepository)
    {
        _gastoFixoRepository = gastoFixoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<GastoFixo>>> GetAll()
    {
        return await _gastoFixoRepository.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GastoFixo>> GetById(int id)
    {
        var gastoFixo = await _gastoFixoRepository.GetByIdAsync(id);
        if (gastoFixo == null)
            return NotFound();

        return gastoFixo;
    }

    [HttpPost]
    public async Task<ActionResult<GastoFixo>> Create([FromBody] GastoFixo gastoFixo)
    {
        var result = await _gastoFixoRepository.AddAsync(gastoFixo);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] GastoFixo gastoFixo)
    {
        if (id != gastoFixo.Id)
            return BadRequest();

        await _gastoFixoRepository.UpdateAsync(gastoFixo);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _gastoFixoRepository.DeleteAsync(id);
        return NoContent();
    }
}