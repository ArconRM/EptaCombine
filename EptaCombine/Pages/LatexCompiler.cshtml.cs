using Common.DTO;
using EptaCombine.Entities;
using EptaCombine.HttpService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EptaCombine.Pages;

[DisableRequestSizeLimit]
[ValidateAntiForgeryToken]
[Authorize]
public class LatexCompilerModel : PageModel
{
    private readonly ILogger<FileConverterModel> _logger;
    private readonly ILatexCompilingHttpService _latexCompilerService;
    private readonly UserManager<User> _userManager;

    public LatexCompilerModel(
        ILogger<FileConverterModel> logger,
        ILatexCompilingHttpService latexCompilerService,
        UserManager<User> userManager)
    {
        _logger = logger;
        _latexCompilerService = latexCompilerService;
        _userManager = userManager;
    }

    private async Task<long?> GetCurrentUserIdAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return user?.Id;
    }

    public void OnGet()
    {
        Console.WriteLine(HttpContext.Session.GetString("LatexProjectId"));
    }

    public async Task<IActionResult> OnGetUserProjectsAsync(CancellationToken token)
    {
        var userId = await GetCurrentUserIdAsync();
        if (userId is null)
            return Unauthorized();

        var content = await _latexCompilerService.GetUserProjectsAsync(userId.Value, token);
        return new JsonResult(new { content });
    }

    public IActionResult OnPostSelectActiveProject([FromBody] ProjectRequest projectRequest)
    {
        _latexCompilerService.SelectActiveProject(projectRequest.ProjectUuid, HttpContext.Session);
        return new JsonResult(new { success = true });
    }
    
    public async Task<IActionResult> OnPostCreateProjectFromTemplateAsync(CancellationToken token)
    {
        var userId = await GetCurrentUserIdAsync();
        if (userId is null)
            return Unauthorized();

        await _latexCompilerService.CreateProjectFromTemplateAsync(userId.Value, HttpContext.Session, token);
        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostCreateProjectFromZipAsync([FromForm] IFormFile zipFile, CancellationToken token)
    {
        var userId = await GetCurrentUserIdAsync();
        if (userId is null)
            return Unauthorized();

        await using var stream = zipFile.OpenReadStream();
        await _latexCompilerService.CreateProjectFromZipAsync(userId.Value, stream, HttpContext.Session, token);
        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnGetMainTexAsync(CancellationToken token)
    {
        var content = await _latexCompilerService.GetMainTexContentAsync(HttpContext.Session, token);
        return new JsonResult(new { content });
    }

    public async Task<IActionResult> OnGetMainBibAsync(CancellationToken token)
    {
        var content = await _latexCompilerService.GetMainBibContentAsync(HttpContext.Session, token);
        return new JsonResult(new { content });
    }

    public async Task<IActionResult> OnPostSaveProjectAsync(
        [FromBody] LatexContentUpdateRequest contentUpdateRequest,
        CancellationToken token)
    {
        await _latexCompilerService.UpdateProjectAsync(
            HttpContext.Session,
            contentUpdateRequest.TexContent,
            contentUpdateRequest.BibContent,
            token);
        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostCompileAsync(CancellationToken token)
    {
        var pdfStream = await _latexCompilerService.CompileAsync(HttpContext.Session, token);
        return File(pdfStream, "application/pdf");
    }

    public async Task<IActionResult> OnPostDeleteProjectAsync([FromBody] ProjectRequest request, CancellationToken token)
    {
        await _latexCompilerService.DeleteProjectAsync(request.ProjectUuid, token);
        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostCleanupAsync(CancellationToken token)
    {
        await _latexCompilerService.CleanupAsync(HttpContext.Session, token);
        return new JsonResult(new { success = true });
    }
}