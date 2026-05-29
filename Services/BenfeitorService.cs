using BeneficentEvent.Data;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace BeneficentEvent.DTOs.Request;

public class BenfeitorService
{
    private readonly AppDbContext _context;

    public BenfeitorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Benfeitor>> ListarAsync()
    {
        return await _context.Benfeitores.Include(x => x.Doacoes).Include(x =>x.Eventos).Include(x => x.Lances).ToListAsync();
    }

    public async Task<Benfeitor?> BuscarPorId(Guid id)
    {
        return await _context.Benfeitores.Include(x => x.Doacoes).Include(x =>x.Eventos).Include(x => x.Lances).FirstOrDefaultAsync(x => x.Id == id);
        
    }

    public async Task EditarAsync(Guid id, CriarBenfeitorRequest request)
    {
        var benfeitor = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == id);

        if(benfeitor is null)
            throw new RegraNegocioException("Benfeitor não cadastrado.");
        if(benfeitor.CPF.Length<=0 || benfeitor.CPF.Length > 11)
            throw new RegraNegocioException("CPF inválido.");
        if(benfeitor.Nome.Length < 3)
            throw new RegraNegocioException("Nome inválido");
        
        benfeitor.CPF = request.Cpf;
        benfeitor.Email = request.Email;
        benfeitor.Endereco = request.Endereco;
        benfeitor.Telefone = request.Telefone;
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var benfeitor = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == id);
        if(benfeitor is null)
            throw new RegraNegocioException("Benfeitor não cadastrado.");
        _context.Benfeitores.Remove(benfeitor);
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> CriarAsync(CriarBenfeitorRequest request)
    {
        var benfeitor = new Benfeitor
        {
          Id = Guid.NewGuid(),
          Nome = request.Nome,
          CPF = request.Nome,
          Telefone = request.Telefone,
          Email = request.Email,
          Endereco = request.Endereco  
        };

        _context.Benfeitores.Add(benfeitor);
        await _context.SaveChangesAsync();
        return benfeitor.Id;
    }
}