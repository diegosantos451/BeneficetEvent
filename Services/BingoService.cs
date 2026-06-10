using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;
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

    public async Task<List<BingoResponse>> ListarAsync()
    {
        return await _context.Bingos.Select(x => new BingoResponse(
            x.Id,
            x.Nome,
            x.ValorCartela,
            x.DataSorteio
        )).ToListAsync();
    }

    public async Task<BingoDetalheResponse> BuscarPorIdDetalhesAsync(Guid id)
    {
        var bingo = await _context.Bingos.Include(x => x.Premios).FirstOrDefaultAsync(x => x.Id == id);

        if(bingo is null)
            throw new RegraNegocioException("Bingo não cadastrado.");

        return new BingoDetalheResponse (
            bingo.Id,
            bingo.Nome,
            bingo.ValorCartela,
            bingo.DataSorteio,

            bingo.Premios.Select(x => new ItensBingosResponse(
                x.Id,
                x.Descricao,
                x.ValorEstimado
            )).ToList()
        );
    }

    public async Task<Guid> CriarAsync(CriarBingoRequest request)
    {
        if(!await _context.Eventos.AnyAsync(x => x.Id == request.EventoId))
            throw new RegraNegocioException("Evento não cadastrado.");
        if(request.premiosBingo.Count == 0)
            throw new RegraNegocioException("Ao menos um prêmio deve ser cadastrado.");
        if(request.Nome.Length < 3)
            throw new RegraNegocioException("Insira um nome válido.");
        if(request.ValorCartela <= 0)
            throw new RegraNegocioException("Digite um valor maior que R$0,00");
        if(request.DataSorteio < DateTime.Today)
            throw new RegraNegocioException("A data de sorteio não pode ser menor que a data de hoje.");
        
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
            if(premioRequest.Descricao.Length < 3)
                throw new RegraNegocioException("Insira um nome válido.");
            if(premioRequest.ValorEstimado < 0)
                throw new RegraNegocioException("Digite um valor maior ou igual que R$0,00");

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
        if(await _context.Eventos.AnyAsync(x => x.Id == id))
            throw new RegraNegocioException("Evento não cadastrado.");
        if(request.PremiosBingo.Count == 0)
            throw new RegraNegocioException("Ao menos um prêmio deve ser cadastrado.");
        if(request.Nome.Length < 3)
            throw new RegraNegocioException("Insira um nome válido.");
        if(request.ValorCartela <= 0)
            throw new RegraNegocioException("Digite um valor maior que R$0,00");
        if(request.DataSorteio < DateTime.Now)
            throw new RegraNegocioException("A data de sorteio não pode ser menor que a data de hoje.");
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