using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HelloJkwCore.Tetration;
namespace HelloJkwCore.Controllers;

public class TetrationResultRequest
{
    public required string TaskId { get; set; }
    public required string Base64Image { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HelloController(
    TetrationGlobalService tetrationGlobalService
) : ControllerBase
{
    [HttpGet("HiHi")]
    public IActionResult HiHi()
    {
        return Ok("Hello World!");
    }

    [HttpGet("tetration/task")]
    public IActionResult TetrationTask()
    {
        var task = tetrationGlobalService.GetAnyTask();
        if (task != null)
        {
            return Ok(task);
        }
        else
        {
            return Ok(default(TetrationTask));
        }
    }

    [HttpPost("tetration/result")]
    public IActionResult TetrationResult([FromBody] TetrationResultRequest request)
    {
        tetrationGlobalService.CompleteTask(request.TaskId, request.Base64Image);
        return Ok();
    }
} 