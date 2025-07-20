using LatexCompiler.Contracts;
using LatexCompiler.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LatexCompiler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LatexCompilerController : ControllerBase
{
    private readonly ILatexCompilingService _latexService;
    private readonly ILogger<LatexCompilerController> _logger;

    public LatexCompilerController(ILatexCompilingService latexService, ILogger<LatexCompilerController> logger)
    {
        _latexService = latexService;
        _logger = logger;
    }

    [HttpPost(nameof(Upload))]
    public async Task<IActionResult> Upload(IFormFile zipFile, CancellationToken token)
    {
        try
        {
            if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
                return BadRequest("No file uploaded.");

            using var zipStream = zipFile.OpenReadStream();

            var project = await _latexService.UploadAsync(zipStream, HttpContext.Session, token);
            return Ok(new { project.Uuid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed.");
            return StatusCode(500, "Internal server error during upload.");
        }
    }

    [HttpGet(nameof(GetMainTexContent))]
    public async Task<IActionResult> GetMainTexContent(CancellationToken token)
    {
        try
        {
            var content = await _latexService.GetMainTexContentAsync(HttpContext.Session, token);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve main.tex content.");
            return StatusCode(500, "Internal server error while reading main.tex.");
        }
    }

    [HttpPost(nameof(UpdateMainTex))]
    public async Task<IActionResult> UpdateMainTex([FromBody] LatexRequest request, CancellationToken token)
    {
        try
        {
            await _latexService.UpdateMainTexAsync(HttpContext.Session, request.Content, token);
            return Ok("main.tex updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update main.tex.");
            return StatusCode(500, "Internal server error while updating main.tex.");
        }
    }

    [HttpGet(nameof(Compile))]
    public async Task<IActionResult> Compile(CancellationToken token)
    {
        try
        {
            var pdfStream = await _latexService.CompileAsync(HttpContext.Session, token);
            return File(pdfStream, "application/pdf", "output.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Compilation failed.");
            return StatusCode(500, "Internal server error during PDF compilation.");
        }
    }

    [HttpDelete(nameof(Cleanup))]
    public async Task<IActionResult> Cleanup(CancellationToken token)
    {
        try
        {
            await _latexService.CleanupAsync(HttpContext.Session, token);
            return Ok("Project cleaned up.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cleanup failed.");
            return StatusCode(500, "Internal server error during cleanup.");
        }
    }
}