using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Enums;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;

namespace BeneficentEvent.Services;

public class LeilaoService
{
    private readonly AppDbContext _context;

    public LeilaoService(AppDbContext context)
    {
        _context = context;
    }

    //listar todos os leilões
    public async Task<List<Leilao>> ListarAsync()
    {
        return await _context.Leiloes.Include(x => x.Itens).ThenInclude(x => x.Lances).ToListAsync();
    }

    //retorna 1 registro correspondente ao ID recebido
    public async Task<Leilao?> ObterPorId(Guid id)
    {
        return await _context.Leiloes.Include(x => x.Itens).ThenInclude(x => x.Lances).FirstOrDefaultAsync(x => x.Id == id);
    }

    //editar um leilao e seus itens
    public async Task Editar(Guid id, EditarLeilaoRequest request)
    {
        var leilao = await _context.Leiloes.Include(x => x.Itens).ThenInclude(x => x.Lances).FirstOrDefaultAsync(x => x.Id == id);

        if (leilao is null)
            throw new RegraNegocioException("Leião inexistente.");

        if (leilao.Evento.Status != StatusEvento.EmAndamento)
            throw new RegraNegocioException("Não é possível editar um leilão já finalizado.");

        leilao.Nome = request.Nome;
        leilao.Data = request.Data;

        foreach (var itemRequest in request.Itens)
        {
            var item = leilao.Itens.FirstOrDefault(x => x.Id == itemRequest.Id);

            if (item is null)
                continue;

            item.Nome = itemRequest.Nome;
            item.Lote = itemRequest.Lote;
            item.Descricao = itemRequest.Descricao;
            item.LanceInicial = itemRequest.LanceInicial;

            foreach (var lanceRequest in itemRequest.Lances)
            {
                var lance = item.Lances.FirstOrDefault(x => x.Id == lanceRequest.Id);

                if (lance is null)
                {
                    item.Lances.Add(new Lance
                    {
                        Id = Guid.NewGuid(),
                        BenfeitorId = lanceRequest.BenfeitorID,
                        Valor = lanceRequest.Valor,
                        DataHora = DateTime.UtcNow
                    });
                    continue;
                }
                lance.BenfeitorId = lanceRequest.BenfeitorID;
                lance.Valor = lanceRequest.Valor;
            }
            // Remove lances excluídos
            var idsRecebidos = itemRequest.Lances.Select(x => x.Id).ToHashSet();
            var lancesRemover = item.Lances.Where(x => !idsRecebidos.Contains(x.Id)).ToList();
            _context.Lances.RemoveRange(lancesRemover);
            // Recalcula LanceAtual
            item.LanceAtual = item.Lances.Any() ? item.Lances.Max(x => x.Valor) : item.LanceInicial;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> CriarAsync(CriarLeilaoRequest request)
    {
        var leilao = new Leilao
        {
            Id = Guid.NewGuid(),
            EventoId = request.EventoId,
            Nome = request.Nome,
            Data = request.Data,

        };

        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);

        // Regra 1: Evento existe?
        if (evento is null)
            throw new RegraNegocioException("Evento não encontrado.");

        // Regra 2: Deve existir ao menos um item
        if (request.Itens.Count == 0)
            throw new RegraNegocioException("O leilão deve possuir ao menos um item.");

        foreach (var itemRequest in request.Itens)
        {
            var item = new ItemLeilao
            {
                Id = Guid.NewGuid(),
                LeilaoId = leilao.Id,
                Nome = itemRequest.Nome,
                Lote = itemRequest.Lote,
                Descricao = itemRequest.Descricao,
                LanceInicial = itemRequest.LanceInicial,
                LanceAtual = 0,
            };

            foreach (var lanceRequest in itemRequest.Lances)
            {
                item.Lances.Add(new Lance
                {
                    Id = Guid.NewGuid(),
                    BenfeitorId = lanceRequest.BenfeitorID,
                    Valor = lanceRequest.Valor,
                    DataHora = lanceRequest.Data
                });
            }
            if (item.Lances.Count > 0)
                item.LanceAtual = item.Lances.Max(x => x.Valor);

            leilao.Itens.Add(item);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Leiloes.Add(leilao);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return leilao.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new RegraNegocioException("Falha ao salvar registros.");
        }
    }

    public async Task ExcluirAsync(Guid id)
    {
        var leilao = await _context.Leiloes.Include(x => x.Itens).ThenInclude(x => x.Lances).FirstOrDefaultAsync(x => x.Id == id);
        if (leilao is null)
            throw new RegraNegocioException("Leilão não encontrado.");

        var lances = leilao.Itens.SelectMany(x => x.Lances).ToList();
        _context.Lances.RemoveRange(lances);
        _context.ItensLeilao.RemoveRange(leilao.Itens);
        _context.Leiloes.Remove(leilao);
        await _context.SaveChangesAsync();

    }
}