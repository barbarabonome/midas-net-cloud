using Microsoft.AspNetCore.Mvc;
using Midas.Infrastructure.Persistence.Entities;
using Midas.Infrastructure.Persistence.Repositories;
using Midas.API.DTOs;
using Midas.API.Services;

namespace Midas.Controllers
{
    [ApiController]
    [Route("api")]
 public class HateoasController : ControllerBase
    {
      private readonly ICategoriaRepository _categoriaRepository;
        private readonly IReceitaRepository _receitaRepository;
    private readonly IGastoRepository _gastoRepository;
        private readonly ICofrinhoRepository _cofrinhoRepository;
        private readonly HateoasLinkGenerator _linkGenerator;
        private readonly ILogger<HateoasController> _logger;

        public HateoasController(
         ICategoriaRepository categoriaRepository,
          IReceitaRepository receitaRepository,
     IGastoRepository gastoRepository,
         ICofrinhoRepository cofrinhoRepository,
   HateoasLinkGenerator linkGenerator,
    ILogger<HateoasController> logger)
  {
     _categoriaRepository = categoriaRepository;
    _receitaRepository = receitaRepository;
            _gastoRepository = gastoRepository;
 _cofrinhoRepository = cofrinhoRepository;
          _linkGenerator = linkGenerator;
 _logger = logger;
    }

        /// <summary>
  /// GET api/categoriasHateoas - Lista todas as categorias com links HATEOAS
        /// </summary>
        [HttpGet("categoriasHateoas")]
public async Task<ActionResult<HateoasPagedResponse<Categoria>>> GetCategorias([FromQuery] CategoriaSearchParameters parameters = null)
        {
          try
     {
      parameters ??= new CategoriaSearchParameters();
          
       var result = await _categoriaRepository.SearchAsync(parameters);
    
        var response = new HateoasPagedResponse<Categoria>(
         result.Data,
    result.TotalRecords,
      result.CurrentPage,
   result.PageSize);

      // Adicionar links HATEOAS gerais
   response.Links = _linkGenerator.GenerateCategoriaLinks();

    // Links de paginação
  if (response.HasNext)
            {
      response.Links.Add(new HateoasLink(
         $"/api/categoriasHateoas?page={response.CurrentPage + 1}&size={response.PageSize}",
       "next",
     "GET"));
     }
        
 if (response.HasPrevious)
     {
               response.Links.Add(new HateoasLink(
         $"/api/categoriasHateoas?page={response.CurrentPage - 1}&size={response.PageSize}",
      "previous", 
   "GET"));
       }

     return Ok(response);
            }
   catch (Exception ex)
      {
              _logger.LogError(ex, "Erro ao buscar categorias com HATEOAS");
      return StatusCode(500, "Erro interno do servidor");
  }
        }

 /// <summary>
   /// POST api/receitasHateoas - Cria uma nova receita com links HATEOAS
    /// </summary>
    [HttpPost("receitasHateoas")]
  public async Task<ActionResult<HateoasResponse<Receita>>> CreateReceita([FromBody] CreateReceitaDTO createReceitaDTO)
   {
        try
    {
       if (createReceitaDTO == null)
           return BadRequest("Dados da receita são obrigatórios");

    if (string.IsNullOrEmpty(createReceitaDTO.Titulo))
         return BadRequest("Título é obrigatório");

            if (createReceitaDTO.UsuarioId <= 0)
    return BadRequest("UsuarioId é obrigatório");

        if (createReceitaDTO.Valor <= 0)
     return BadRequest("Valor deve ser maior que zero");

      var receita = new Receita
     {
      UsuarioId = createReceitaDTO.UsuarioId,
 Titulo = createReceitaDTO.Titulo,
 Data = createReceitaDTO.Data,
            Valor = createReceitaDTO.Valor,
     Fixo = NormalizeFixoValue(createReceitaDTO.Fixo)
            };

            var result = await _receitaRepository.CreateAsync(receita);

  var response = new HateoasResponse<Receita>(result);
  response.Links = _linkGenerator.GenerateReceitaLinks(result.Id);

     return CreatedAtAction(
  nameof(GetReceitaById), 
            new { id = result.Id }, 
      response);
        }
  catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar receita com HATEOAS");
  return StatusCode(500, "Erro interno do servidor");
    }
        }

     /// <summary>
 /// DELETE api/gastosHateoas/{id} - Remove um gasto com confirmação HATEOAS
        /// </summary>
        [HttpDelete("gastosHateoas/{id}")]
        public async Task<ActionResult<HateoasResponse<string>>> DeleteGasto(int id)
   {
      try
    {
       var gasto = await _gastoRepository.GetByIdAsync(id);
    if (gasto == null)
    return NotFound(new HateoasResponse<string>("Gasto não encontrado")
      {
    Links = _linkGenerator.GenerateGastoLinks()
      });

       await _gastoRepository.DeleteAsync(id);

    var response = new HateoasResponse<string>($"Gasto '{gasto.Titulo}' removido com sucesso");
    response.Links = _linkGenerator.GenerateGastoLinks();

    return Ok(response);
     }
       catch (Exception ex)
  {
       _logger.LogError(ex, "Erro ao deletar gasto com HATEOAS. ID: {Id}", id);
      return StatusCode(500, new HateoasResponse<string>("Erro interno do servidor")
     {
           Links = _linkGenerator.GenerateGastoLinks()
    });
            }
  }

        /// <summary>
        /// PUT api/cofrinhoHateoas/{id} - Atualiza um cofrinho com links HATEOAS
        /// </summary>
        [HttpPut("cofrinhoHateoas/{id}")]
       public async Task<ActionResult<HateoasResponse<Cofrinho>>> UpdateCofrinho(int id, [FromBody] Cofrinho cofrinho)
        {
    try
            {
if (cofrinho == null)
       return BadRequest(new HateoasResponse<string>("Dados do cofrinho são obrigatórios")
        {
      Links = _linkGenerator.GenerateCofrinhoLinks(id)
     });

     if (id != cofrinho.Id)
      return BadRequest(new HateoasResponse<string>("ID do parâmetro não confere com o ID do cofrinho")
     {
      Links = _linkGenerator.GenerateCofrinhoLinks(id)
       });

       if (!ModelState.IsValid)
                return BadRequest(new HateoasResponse<object>(ModelState)
 {
           Links = _linkGenerator.GenerateCofrinhoLinks(id)
     });

     var existingCofrinho = await _cofrinhoRepository.GetByIdAsync(id);
         if (existingCofrinho == null)
       return NotFound(new HateoasResponse<string>("Cofrinho não encontrado")
      {
        Links = _linkGenerator.GenerateCofrinhoLinks()
     });

     cofrinho.Aplicado = NormalizeAplicadoValue(cofrinho.Aplicado);

    await _cofrinhoRepository.UpdateAsync(cofrinho);

       var updatedCofrinho = await _cofrinhoRepository.GetByIdAsync(id);
     var response = new HateoasResponse<Cofrinho>(updatedCofrinho);
            response.Links = _linkGenerator.GenerateCofrinhoLinks(id);

    return Ok(response);
   }
         catch (Exception ex)
      {
     _logger.LogError(ex, "Erro ao atualizar cofrinho com HATEOAS. ID: {Id}", id);
    return StatusCode(500, new HateoasResponse<string>("Erro interno do servidor")
          {
  Links = _linkGenerator.GenerateCofrinhoLinks(id)
     });
      }
        }

 // Métodos auxiliares para buscar por ID (necessários para os links HATEOAS)
 [HttpGet("receitasHateoas/{id}")]
      public async Task<ActionResult<HateoasResponse<Receita>>> GetReceitaById(int id)
    {
 try
 {
           var receita = await _receitaRepository.GetByIdAsync(id);
    if (receita == null)
     return NotFound();

        var response = new HateoasResponse<Receita>(receita);
response.Links = _linkGenerator.GenerateReceitaLinks(id);

                return Ok(response);
          }
     catch (Exception ex)
    {
 _logger.LogError(ex, "Erro ao buscar receita por ID com HATEOAS. ID: {Id}", id);
       return StatusCode(500, "Erro interno do servidor");
 }
        }

   [HttpGet("gastosHateoas/{id}")]
      public async Task<ActionResult<HateoasResponse<Gasto>>> GetGastoById(int id)
        {
   try
   {
     var gasto = await _gastoRepository.GetByIdAsync(id);
           if (gasto == null)
        return NotFound();

    var response = new HateoasResponse<Gasto>(gasto);
     response.Links = _linkGenerator.GenerateGastoLinks(id);

        return Ok(response);
   }
          catch (Exception ex)
            {
   _logger.LogError(ex, "Erro ao buscar gasto por ID com HATEOAS. ID: {Id}", id);
         return StatusCode(500, "Erro interno do servidor");
            }
      }

     [HttpGet("cofrinhoHateoas/{id}")]
     public async Task<ActionResult<HateoasResponse<Cofrinho>>> GetCofrinhoById(int id)
   {
       try
     {
   var cofrinho = await _cofrinhoRepository.GetByIdAsync(id);
if (cofrinho == null)
       return NotFound();

    var response = new HateoasResponse<Cofrinho>(cofrinho);
     response.Links = _linkGenerator.GenerateCofrinhoLinks(id);

       return Ok(response);
          }
         catch (Exception ex)
            {
         _logger.LogError(ex, "Erro ao buscar cofrinho por ID com HATEOAS. ID: {Id}", id);
  return StatusCode(500, "Erro interno do servidor");
         }
        }

     // Métodos auxiliares de normalização
        private char NormalizeFixoValue(char fixo)
    {
            return char.ToUpper(fixo) == 'T' ? 'T' : 'F';
        }

        private char NormalizeAplicadoValue(char aplicado)
        {
       return char.ToUpper(aplicado) == 'T' ? 'T' : 'F';
        }
    }
}