using Common.DTO;
using Common.Entities.Enums;
using EptaCombine.HttpService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EptaCombine.Pages;

[DisableRequestSizeLimit]
[ValidateAntiForgeryToken]
[Authorize]
public class CodeRunnerModel : PageModel
{
    private readonly ILogger<CodeRunnerModel> _logger;
    
    private readonly ICodeRunnerHttpsService  _codeRunnerHttpsService;
 
    public List<SelectListItem> LanguageOptions { get; set; }
    
    public CodeRunnerModel(ILogger<CodeRunnerModel> logger, ICodeRunnerHttpsService codeRunnerHttpsService)
    {
        _logger = logger;
        _codeRunnerHttpsService = codeRunnerHttpsService;
    }
    
    public void OnGet()
    {
        LanguageOptions = Enum.GetValues(typeof(ProgramLanguage))
            .Cast<ProgramLanguage>()
            .Select(lang => new SelectListItem
            {
                Text = lang.ToString(),
                Value = lang.ToString(),
                Selected = lang == ProgramLanguage.CSharp
            })
            .ToList();
    }

    public async Task<IActionResult> OnPostRunCodeAsync(
        [FromBody] CodeExecutionRequest request,
        CancellationToken token)
    {
        var result = await _codeRunnerHttpsService.RunCodeAsync(request.Code, request.ProgramLanguage, token);
        return new JsonResult(new
        {
            isSuccess = result.IsSuccess,
            Output = result.Output
        });
    }
}