using CleanArchitecture.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers;

/// <summary>
/// Demo auth controller. In a real app, this lives in a separate Identity service
/// and uses ASP.NET Identity or an external IdP. Kept simple here to keep the
/// template self-contained.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokens;

    public AuthController(ITokenService tokens) => _tokens = tokens;

    public record LoginRequest(string Email, string Password);

    [HttpPost("login")]
    public ActionResult<object> Login(LoginRequest request)
    {
        // Demo: accept any email/password, role hardcoded to Admin.
        // Replace with real user store in production.
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email and password are required." });

        var token = _tokens.GenerateToken(
            userId: request.Email,
            email: request.Email,
            roles: new[] { "Admin" });

        return Ok(new { access_token = token, token_type = "Bearer" });
    }
}
