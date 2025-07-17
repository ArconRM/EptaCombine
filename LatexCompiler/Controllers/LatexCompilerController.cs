using LatexCompiler.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LatexCompiler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LatexCompilerController: ControllerBase
{
    private readonly ILatexCompilingService _service;
    
    private readonly ILogger<LatexCompilerController> _logger;

    public LatexCompilerController(
        ILatexCompilingService service, 
        ILogger<LatexCompilerController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Compile(IFormFile zipFile, CancellationToken token)
    {
        try
        {
            await using var zipStream = zipFile.OpenReadStream();
            Stream outputStream = await _service.CompileAsync(zipStream, token);
            outputStream.Seek(0, SeekOrigin.Begin);

            return File(
                outputStream, 
                "application/pdf", 
                "result.pdf");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}