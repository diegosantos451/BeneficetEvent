using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Enums;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Models;
using BeneficentEvent.DTOs.Response;

namespace BeneficentEvent.Services;

public class DoacaoService
{
    private readonly AppDbContext _context;

    public DoacaoService(AppDbContext context)
    {
        _context = context;
    }

    //retorna todas as doações e seus respectivos itens
    public async Task<List<DoacaoResponse>> ListarAsync()
    {
        return await _context.Doacoes.Select(x => new DoacaoResponse(
            x.Id,
            x.Evento.Nome,
            x.Benfeitor.Nome,
            x.Tipo,
            x.ValorMonetario,
            x.DataDoacao,
            x.Observacao
        )).ToListAsync();
    }

    //retorna uma doacao e seus itens (se houver) por ID
    public async Task<DoacaoDetalheResponse> ObterPorIdDetalheAsync(Guid id)
    {
        var doacao = await _context.Doacoes.Include(x => x.Evento).Include(x => x.Itens).Include(x => x.Benfeitor).FirstOrDefaultAsync(x => x.Id == id);

        if (doacao is null)
            throw new RegraNegocioException("Doação não cadastrada.");

        return new DoacaoDetalheResponse(
            doacao.Id,
            doacao.Evento.Nome,
            doacao.Benfeitor.Nome,
            doacao.Tipo,
            doacao.ValorMonetario,
            doacao.DataDoacao,
            doacao.Observacao,
            doacao.Itens.Select(x => new ItemDoacaoResponse(
                x.Id,
                x.Nome,
                x.Quantidade,
                x.Unidade,
                x.ValorEstimado
            )).ToList()
        );
    }

    //editar uma doação
    public async Task EditarAsync(Guid id, EditarDoacaoRequest request)
    {
        var doacao = await _context.Doacoes.Include(x => x.Evento).Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
        var benfeitor = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == request.BenfeitorId);

        if (benfeitor is null)
            throw new RegraNegocioException("Informe um benfeitor cadastrado.");

        if (doacao is null)
            throw new RegraNegocioException("Doação não encontrada!");

        if (doacao.Evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Não é possível editar uma Doação de um Evento encerrado!");

        if (request.Tipo == TipoDoacao.Material)
            if (request.Itens.Count == 0)
                throw new RegraNegocioException("Informe ao menos um item a ser doado.");

        if (request.Data < DateTime.Today)
            throw new RegraNegocioException("A data de doação deve ser maior que a data atual.");

        if (request.Tipo == TipoDoacao.Dinheiro)
        {
            if (request.ValorMonetario <= 0)
                throw new RegraNegocioException("O valor monetário deve ser maior que R$0,00");
            else
            {
                var movimento = await _context.MovimentosFinanceiros.FirstOrDefaultAsync(x => x.Id == doacao.Id);

                if (movimento is null)
                    throw new RegraNegocioException("erro ao alterar movimento financeiro.");

                movimento.Valor = doacao.ValorMonetario;
            }
        }

        var idsRecebidos = request.Itens.Select(x => x.Id).ToList();
        var idsRemover = doacao.Itens.Where(x => !idsRecebidos.Contains(x.Id)).ToList();

        doacao.BenfeitorId = request.BenfeitorId;
        doacao.Tipo = request.Tipo;
        doacao.Observacao = request.Observacao;
        doacao.ValorMonetario = request.ValorMonetario;
        doacao.DataDoacao = request.Data;

        foreach (var item in request.Itens)
        {
            var itemExistente = doacao.Itens.FirstOrDefault(x => x.Id == item.Id);

            if (itemExistente is null)
            {
                doacao.Itens.Add(new ItemDoacao
                {
                    Id = Guid.NewGuid(),
                    Nome = item.Nome,
                    Quantidade = item.Quantidade,
                    Unidade = item.Unidade,
                    ValorEstimado = item.ValorEstimado
                });
            }
            else
            {
                itemExistente.Nome = item.Nome;
                itemExistente.Quantidade = item.Quantidade;
                itemExistente.Unidade = item.Unidade;
                itemExistente.ValorEstimado = item.ValorEstimado;
            }
        }

        _context.RemoveRange(idsRemover);
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> CriarAsync(CriarDoacaoRequest request)
    {
        var benfeitor = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == request.BenfeitorId);
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);

        if(evento is null || evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Evento não cadastrado ou encerrado.");

        if (benfeitor is null)
            throw new RegraNegocioException("Informe um benfeitor cadastrado.");

        if (request.Tipo == TipoDoacao.Material)
            if (request.Itens.Count == 0)
                throw new RegraNegocioException("Informe ao menos um item a ser doado.");

        if (request.Data < DateTime.Today)
            throw new RegraNegocioException("A data de doação deve ser maior que a data atual.");

        //persistência:
        var doacao = new Doacao
        {
            Id = Guid.NewGuid(),
            EventoId = request.EventoId,
            BenfeitorId = request.BenfeitorId,
            Tipo = request.Tipo,
            ValorMonetario = request.ValorMonetario,
            Observacao = request.Observacao,
            DataDoacao = DateTime.UtcNow
        };

        foreach (var item in request.Itens)
        {
            doacao.Itens.Add(new ItemDoacao
            {
                Id = Guid.NewGuid(),
                Nome = item.Nome,
                Quantidade = item.Quantidade,
                Unidade = item.Unidade,
                ValorEstimado = item.ValorEstimado
            });
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (request.Tipo == TipoDoacao.Dinheiro)
            {
                var movimento = new MovimentoFinanceiro
                {
                    Id = doacao.Id,
                    EventoId = request.EventoId,
                    Tipo = TipoMovimento.Receita,
                    Valor = request.ValorMonetario,
                    Origem = OrigemMovimento.Doacao,
                    DataMovimento = DateTime.UtcNow
                };
                _context.MovimentosFinanceiros.Add(movimento);
            }
            _context.Doacoes.Add(doacao);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return doacao.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new RegraNegocioException("Erro ao persistir os dados: contate o administrador do sistema!");
        }

    }

    //apaga o registro de uma doação
    public async Task ExcluirAsync(Guid id)
    {
        var doacao = await _context.Doacoes.FirstOrDefaultAsync(x => x.Id == id);
        if (doacao is null)
        {
            throw new RegraNegocioException("Doação inexistente!");
        }

        _context.Doacoes.Remove(doacao);
        await _context.SaveChangesAsync();
    }
}
