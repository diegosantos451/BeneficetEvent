using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;
using BeneficentEvent.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeneficentEvent.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuarioController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UsuarioResponse>>> Listar()
    {
        return Ok(await _usuarioService.Listar());
    }  

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioResponse>> ObterPorId(Guid id)
    {
        return Ok(await _usuarioService.ObterPorIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Criar(UsuarioRequest request)
    {
        var id = await _usuarioService.CriarAsync(request);
        return Ok(id);
    }

    [HttpPut("{id}")]
    public async Task Editar(Guid id, UsuarioRequest request)
    {
        await _usuarioService.EditarAsync(id, request);
    }
    [HttpDelete("{Id}")]
    public async Task Excluir(Guid Id)
    {
        await _usuarioService.ExcluirAsync(Id);
    }

}