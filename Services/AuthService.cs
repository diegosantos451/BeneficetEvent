using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;
using BeneficentEvent.Models;
using BeneficentEvent.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BeneficentEvent.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (usuario is null)
            throw new RegraNegocioException("Usuário ou senha inválidos.");

        // valida senha
        if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            throw new RegraNegocioException("Usuário ou senha inválidos.");

        var token = GerarToken(usuario);

        return new LoginResponse(
            token,
            usuario.Nome,
            usuario.Email,
            usuario.Perfil
        );
    }

    private string GerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(6), signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
