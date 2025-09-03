using AutoMapper;
using CodeRunner.Service.Interfaces;
using Common.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CodeRunner.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CodeRunnerController : ControllerBase
{
    private readonly ICodeRunnerService _service;
    private readonly ILogger<CodeRunnerController> _logger;
    
    public CodeRunnerController(
        ICodeRunnerService service,
        ILogger<CodeRunnerController> logger
        )
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet(nameof(RunCode))]
    public async Task<IActionResult> RunCode(string code, ProgramLanguage language, CancellationToken token)
    {
        try
        {
            var result = await _service.RunCodeAsync(code, language, token);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error while trying to run code");
        }
    }
}