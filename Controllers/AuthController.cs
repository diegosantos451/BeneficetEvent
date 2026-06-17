using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Services;
using Microsoft.AspNetCore.Mvc;


namespace BeneficentEvent.Controllers;

[Route("api/[controller]")]  
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authSerivce)
    {
        _authService = authSerivce;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

}