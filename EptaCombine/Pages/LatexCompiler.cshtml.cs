using LatexCompiler.Contracts;
using LatexCompiler.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EptaCombine.Pages;

[DisableRequestSizeLimit]
[ValidateAntiForgeryToken]
public class LatexCompilerModel : PageModel
{
    private readonly ILogger<FileConverterModel> _logger;
    
    private readonly ILatexCompilingService _latexCompilerService;

    public LatexCompilerModel(ILogger<FileConverterModel> logger, ILatexCompilingService latexCompilerService)
    {
        _logger = logger;
        _latexCompilerService = latexCompilerService;
    }
    
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostUploadAsync([FromForm] IFormFile zipFile, CancellationToken token)
    {
        await using var stream = zipFile.OpenReadStream();
        await _latexCompilerService.UploadAsync(stream, HttpContext.Session, token);
        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostGetMainTexAsync(CancellationToken token)
    {
        var content = await _latexCompilerService.GetMainTexContentAsync(HttpContext.Session, token);
        return new JsonResult(new { content });
    }

    public async Task<IActionResult> OnPostSaveMainTexAsync([FromBody] LatexRequest request, CancellationToken token)
    {
        await _latexCompilerService.UpdateMainTexAsync(HttpContext.Session, request.Content, token);
        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostCompileAsync(CancellationToken token)
    {
        var pdfStream = await _latexCompilerService.CompileAsync(HttpContext.Session, token);
        return File(pdfStream, "application/pdf");
    }

    public async Task<IActionResult> OnPostCleanupAsync(CancellationToken token)
    {
        await _latexCompilerService.CleanupAsync(HttpContext.Session, token);
        return new JsonResult(new { success = true });
    }
}