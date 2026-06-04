using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeneficentEvent.Services;

public class BingoService
{
    private readonly AppDbContext _context;

    public BingoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bingo>> ListarAsync()
    {
        return await _context.Bingos.Include(x => x.Premios).ToListAsync();
    }

    public async Task<Bingo?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Bingos.Include(x => x.Premios).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Guid> CriarAsync(CriarBingoRequest request)
    {
        var bingo = new Bingo
        {
          Id = Guid.NewGuid(),
          EventoId = request.EventoId,
          Nome = request.Nome,
          ValorCartela = request.ValorCartela,
          DataSorteio = request.DataSorteio  
        };
        
        foreach(var premioRequest in request.premiosBingo)
        {
            bingo.Premios.Add(new PremioBingo
            {
               Id = Guid.NewGuid(),
               BingoId = bingo.Id,
               Descricao = premioRequest.Descricao,
               ValorEstimado = premioRequest.ValorEstimado 
            });
        }

        _context.Bingos.Add(bingo);
        await _context.SaveChangesAsync();

        return bingo.Id;
    }

    public async Task EditarAsync(Guid id, EditarBingoRequest request)
    {
        var bingo = await _context.Bingos.Include(x => x.Premios).FirstOrDefaultAsync(x => x.Id == id);
        
        if(bingo is null)
            throw new RegraNegocioException("Bingo não cadastrado.");
        
        bingo.Nome = request.Nome;
        bingo.DataSorteio = request.DataSorteio;
        bingo.ValorCartela = request.ValorCartela;
        
        foreach(var requestPremio in request.PremiosBingo)
        {
            var premio = bingo.Premios.FirstOrDefault(x => x.Id == requestPremio.Id);
            if(premio is null)
            {
                bingo.Premios.Add(new PremioBingo
                {
                   Id = Guid.NewGuid(),
                   BingoId = bingo.Id,
                   Descricao = requestPremio.Descricao,
                   ValorEstimado = requestPremio.ValorEstimado 
                });
                continue;
            }
            else
            {
                premio.Descricao = requestPremio.Descricao;
                premio.ValorEstimado = requestPremio.ValorEstimado;    
            }
            
        }

        //selecionando os prêmios que serão removidos do banco
        var idsRecebidos = request.PremiosBingo.Select(x => x.Id).ToHashSet();
        var premiosRemover = bingo.Premios.Where(x => !idsRecebidos.Contains(x.Id)).ToList();

        //removendo os prêmios
        _context.PremiosBingo.RemoveRange(premiosRemover);
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var bingo = await _context.Bingos.Include(x => x.Premios).FirstOrDefaultAsync(x => x.Id == id);
        if(bingo is null)
            throw new RegraNegocioException("Bingo não cadastrado.");
        _context.RemoveRange(bingo.Premios);
        _context.Bingos.Remove(bingo);
        await _context.SaveChangesAsync();
    }
}