using EptaCombine.Entities;
using EptaCombine.Entities.Requests;
using EptaCombine.Entities.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace EptaCombine.Pages;

public class AuthModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthModel> _logger;

    public AuthModel(
        SignInManager<User> signInManager, 
        UserManager<User> userManager,
        ILogger<AuthModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostLoginAsync([FromBody] LoginRequest request)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(
            request.Username,
            request.Password,
            request.RememberMe,
            false);
        
        if (!result.Succeeded)
            return Unauthorized();
        
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostRegisterAsync([FromBody] RegisterRequest request)
    {
        _logger.LogInformation(request.Username);
        _logger.LogInformation(request.Email);
        _logger.LogInformation(request.Password);
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return BadRequest("Email already exists");

        if (await _userManager.FindByNameAsync(request.Username) != null)
            return BadRequest("Login already exists");
        
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email
        };
        
        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            _logger.LogError(result.Errors.FirstOrDefault()?.Description);
            return Unauthorized();
        }

        await _userManager.AddToRoleAsync(user, UserRoles.User);
        
        await _signInManager.SignInAsync(user, true);
        
        return RedirectToPage("/Index");
    } 

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Auth");
    }
}