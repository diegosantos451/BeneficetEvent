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

public class UsuarioService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public UsuarioService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<List<UsuarioResponse>> Listar()
    {
        return await _context.Usuarios.Select(x => new UsuarioResponse(
            x.Id,
            x.Nome,
            x.Email,
            x.Perfil
        )).ToListAsync();
    }

    public async Task<UsuarioResponse> ObterPorIdAsync(Guid Id)
    {
        var Login = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == Id);

        if (Login is null)
            throw new RegraNegocioException("Usuário não cadastrado.");

        return new UsuarioResponse(
            Login.Id,
            Login.Nome,
            Login.Email,
            Login.Perfil
        );
    }
    public async Task<Guid> CriarAsync(UsuarioRequest request)
    {
        if (request.Nome.Length < 3)
            throw new RegraNegocioException("Digite um login válido.");
        if (request.SenhaHash.Length < 6)
            throw new RegraNegocioException("A senha deve conter ao menos 6 caracteres.");
        if (request.Email.Length < 3 || !request.Email.Contains("@"))
            throw new RegraNegocioException("Digite um email válido com pelo menos 3 caracteres.");

        var existeUsuario = await _context.Usuarios.AnyAsync(x => x.Email == request.Email);

        if (existeUsuario)
            throw new RegraNegocioException("Já existe um usuário com esse email.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Email = request.Email,
            //Criptografia da senha de usuário...
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.SenhaHash),
            Perfil = request.Perfil
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario.Id;
    }

    public async Task EditarAsync(Guid id, UsuarioRequest request)
    {
        var Login = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

        if (Login is null)
            throw new RegraNegocioException("Usuário não encontrado.");

        if (request.Nome.Length < 3)
            throw new RegraNegocioException("Digite um login válido.");
        if (request.SenhaHash.Length < 6)
            throw new RegraNegocioException("A senha deve conter ao menos 6 caracteres.");
        if (request.Email.Length < 3 || !request.Email.Contains("@"))
            throw new RegraNegocioException("Digite um email válido com pelo menos 3 caracteres.");

        Login.Nome = request.Nome;
        Login.Email = request.Email;
        Login.SenhaHash = Login.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.SenhaHash);
        Login.Perfil = request.Perfil;
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid Id)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == Id);

        if (usuario is null)
            throw new RegraNegocioException("Usuário não cadastrado.");

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
    }
}
