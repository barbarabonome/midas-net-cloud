using Microsoft.AspNetCore.Mvc;

namespace Midas.API.Controllers;

[ApiController]
[Route("api/pipeline")]
public class PipelineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Pipeline funcionando via Azure DevOps!");
    }
}