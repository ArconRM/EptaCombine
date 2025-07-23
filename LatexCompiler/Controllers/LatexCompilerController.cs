using AutoMapper;
using Common.DTO;
using LatexCompiler.Entities;
using LatexCompiler.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LatexCompiler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LatexCompilerController : ControllerBase
{
    private readonly ILatexCompilingService _latexService;
    private readonly ILogger<LatexCompilerController> _logger;
    private readonly IMapper _mapper;
    public LatexCompilerController(
        ILatexCompilingService latexService, 
        ILogger<LatexCompilerController> logger,
        IMapper mapper)
    {
        _latexService = latexService;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost(nameof(Upload))]
    public async Task<IActionResult> Upload(IFormFile zipFile, CancellationToken token)
    {
        try
        {
            if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
                return BadRequest("No file uploaded.");

            using var zipStream = zipFile.OpenReadStream();

            LatexProject project = await _latexService.UploadAsync(zipStream, token);
            LatexProjectDTO projectDTO = _mapper.Map<LatexProjectDTO>(project);
            return Ok(projectDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed.");
            return StatusCode(500, "Internal server error during upload.");
        }
    }

    [HttpGet(nameof(GetMainTexContent))]
    public async Task<IActionResult> GetMainTexContent(Guid projectUuid, CancellationToken token)
    {
        try
        {
            string content = await _latexService.GetMainTexContentAsync(projectUuid, token);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve .bib content.");
            return StatusCode(500, "Internal server error while reading .bib.");
        }
    }
    

    [HttpGet(nameof(GetMainBibContent))]
    public async Task<IActionResult> GetMainBibContent(Guid projectUuid, CancellationToken token)
    {
        try
        {
            string content = await _latexService.GetMainBibContentAsync(projectUuid, token);
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve .bib content.");
            return StatusCode(500, "Internal server error while reading .bib.");
        }
    }

    [HttpPost(nameof(UpdateProject))]
    public async Task<IActionResult> UpdateProject(
        [FromBody] LatexContentUpdateRequest contentUpdateRequest, 
        CancellationToken token)
    {
        try
        {
            await _latexService.UpdateProject(
                contentUpdateRequest.ProjectUuid,
                contentUpdateRequest.TexContent,
                contentUpdateRequest.BibContent,
                token);
            return Ok("project updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to update project.");
            return StatusCode(500,
                "Internal server error while updating project.");
        }
    }

    [HttpGet(nameof(Compile))]
    public async Task<IActionResult> Compile(Guid projectUuid, CancellationToken token)
    {
        try
        {
            Stream pdfStream = await _latexService.CompileAsync(projectUuid, token);
            return File(pdfStream, "application/pdf", "output.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Compilation failed.");
            return StatusCode(500, "Internal server error during PDF compilation.");
        }
    }

    [HttpDelete(nameof(Cleanup))]
    public async Task<IActionResult> Cleanup(Guid projectUuid, CancellationToken token)
    {
        try
        {
            await _latexService.CleanupAsync(projectUuid, token);
            return Ok("Project cleaned up.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cleanup failed.");
            return StatusCode(500, "Internal server error during cleanup.");
        }
    }
}