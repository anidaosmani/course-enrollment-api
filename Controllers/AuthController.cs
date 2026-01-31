using CourseEnrollment.DTOs;
using CourseEnrollment.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseEnrollment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    // Manual setup: call once in Swagger
    [HttpPost("create-role/{roleName}")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
            return Ok("Role already exists.");

        await _roleManager.CreateAsync(new IdentityRole(roleName));
        return Ok($"Role '{roleName}' created.");
    }

    // Manual setup: assign Admin to your email
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromQuery] string email, [FromQuery] string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return NotFound("User not found.");

        if (!await _roleManager.RoleExistsAsync(role))
            return BadRequest("Role does not exist. Create it first.");

        if (!await _userManager.IsInRoleAsync(user, role))
            await _userManager.AddToRoleAsync(user, role);

        return Ok($"Role '{role}' assigned to {email}");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        // If Student role exists, add it automatically
        if (await _roleManager.RoleExistsAsync("Student"))
            await _userManager.AddToRoleAsync(user, "Student");

        return Ok("Registered successfully.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized("Invalid credentials.");

        var ok = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!ok) return Unauthorized("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";

        var (token, expiresAt) = _tokenService.CreateToken(user, role);
        return Ok(new AuthResponseDto(token, expiresAt, user.Email ?? "", role));
    }
}
