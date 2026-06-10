using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;
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
    public async Task<List<LeilaoResponse>> ListarAsync()
    {
        return await _context.Leiloes.Select(x => new LeilaoResponse(
            x.Id,
            x.Nome,
            x.Evento.Nome,
            x.Data
        )).ToListAsync();
    }

    //retorna 1 registro correspondente ao ID recebido
    public async Task<LeilaoDetalheResponse> ObterPorIdDetalhesAsync(Guid id)
    {
        var leilao = await _context.Leiloes.Include(x => x.Itens).ThenInclude(x => x.Lances).
            ThenInclude(x => x.Benfeitor).Include(x => x.Evento).FirstOrDefaultAsync(x => x.Id == id);

        if (leilao is null)
            throw new RegraNegocioException("Leilão não cadastrado.");

        return new LeilaoDetalheResponse(
            leilao.Id,
            leilao.Nome,
            leilao.Evento.Nome,
            leilao.Data,

            leilao.Itens.Select(i => new ItensLeilaoResponse(
                i.Id,
                i.Nome,
                i.Lote,
                i.Descricao,
                i.LanceInicial,
                i.LanceAtual,
                i.Lances.Select(l => new LancesItensLeilaoResponse(
                    l.Id,
                    l.Benfeitor.Nome,
                    l.Valor,
                    l.DataHora
                )).ToList()
            )).ToList()
        );
    }

    //editar um leilao e seus itens
    public async Task Editar(Guid id, EditarLeilaoRequest request)
    {
        var leilao = await _context.Leiloes.Include(x => x.Itens).ThenInclude(x => x.Lances).FirstOrDefaultAsync(x => x.Id == id);
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);

        if (leilao is null)
            throw new RegraNegocioException("Leião inexistente.");

        if (evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");

        if (evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Não é possível editar um leilão já finalizado.");

        if (request.Data < DateTime.Today)
            throw new RegraNegocioException("O campo data deve ser maior que a data atual.");

        if (request.Nome.Length < 3)
            throw new RegraNegocioException("O campo nome deve conter ao menos 3 letras.");

        if (request.Itens.Count == 0)
            throw new RegraNegocioException("É necessário ao menos 1 item cadastrado para leilão.");

        leilao.Nome = request.Nome;
        leilao.Data = request.Data;

        foreach (var itemRequest in request.Itens)
        {
            var item = leilao.Itens.FirstOrDefault(x => x.Id == itemRequest.Id);

            if (item is null)
            {
                leilao.Itens.Add(new ItemLeilao
                {
                    Id = Guid.NewGuid(),
                    LeilaoId = leilao.Id,
                    Nome = itemRequest.Nome,
                    Lote = itemRequest.Lote,
                    Descricao = itemRequest.Descricao,
                    LanceInicial = itemRequest.LanceInicial,
                    LanceAtual = itemRequest.LanceAtual
                });
            }
            else
            {
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
                            DataHora = lanceRequest.Data
                        });
                        continue;
                    }
                    lance.BenfeitorId = lanceRequest.BenfeitorID;
                    lance.Valor = lanceRequest.Valor;
                    lance.DataHora = lanceRequest.Data;
                }
                var idsRecebidos = itemRequest.Lances.Select(x => x.Id).ToHashSet();
                var lancesRemover = item.Lances.Where(x => !idsRecebidos.Contains(x.Id)).ToList();
                _context.Lances.RemoveRange(lancesRemover);
                item.LanceAtual = item.Lances.Any() ? item.Lances.Max(x => x.Valor) : item.LanceInicial;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> CriarAsync(CriarLeilaoRequest request)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);

        if (evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");

        if (evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Não é possível editar um leilão já finalizado.");

        if (request.Data < DateTime.Today)
            throw new RegraNegocioException("O campo data deve ser maior que a data atual.");

        if (request.Nome.Length < 3)
            throw new RegraNegocioException("O campo nome deve conter ao menos 3 letras.");

        if (request.Itens.Count == 0)
            throw new RegraNegocioException("É necessário ao menos 1 item cadastrado para leilão.");

        var leilao = new Leilao
        {
            Id = Guid.NewGuid(),
            EventoId = request.EventoId,
            Nome = request.Nome,
            Data = request.Data,

        };

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