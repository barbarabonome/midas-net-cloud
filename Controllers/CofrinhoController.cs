using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Controllers;

[ApiController]
[Route("api/metas")]
public class CofrinhoController : ControllerBase
{
    private readonly ICofrinhoRepository _cofrinhoRepository;

    public CofrinhoController(ICofrinhoRepository cofrinhoRepository)
    {
        _cofrinhoRepository = cofrinhoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Cofrinho>>> GetAll()
    {
        return await _cofrinhoRepository.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cofrinho>> GetById(int id)
    {
        var meta = await _cofrinhoRepository.GetByIdAsync(id);
        if (meta == null)
            return NotFound();

        return meta;
    }

    [HttpPost]
    public async Task<ActionResult<Cofrinho>> Create([FromBody] Cofrinho meta)
    {
        var result = await _cofrinhoRepository.AddAsync(meta);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Cofrinho meta)
    {
        if (id != meta.Id)
            return BadRequest();

        await _cofrinhoRepository.UpdateAsync(meta);
        return NoContent();
    }

    [HttpPut("{id}/progresso")]
    public async Task<IActionResult> AtualizarProgresso(int id, [FromBody] decimal valorAtual)
    {
        var result = await _cofrinhoRepository.AtualizarProgresso(id, valorAtual);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _cofrinhoRepository.DeleteAsync(id);
        return NoContent();
    }
}
