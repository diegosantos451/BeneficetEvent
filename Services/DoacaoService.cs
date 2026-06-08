using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Enums;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Models;

namespace BeneficentEvent.Services;

public class DoacaoService
{
    private readonly AppDbContext _context;

    public DoacaoService(AppDbContext context)
    {
        _context = context;
    }

    //retorna todas as doações e seus respectivos itens
    public async Task<List<Doacao>> ListarAsync()
    {
        return await _context.Doacoes.Include(x => x.Itens).Include(X => X.Evento).Include(X => X.Benfeitor).ToListAsync();
    }

    //retorna uma doacao e seus itens (se houver) por ID
    public async Task<Doacao?> ObterPorIdAsync(Guid id)
    {
        return await _context.Doacoes.Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);
    }

    //editar uma doação
    public async Task EditarAsync(Guid id, EditarDoacaoRequest request)
    {
        var doacao = await _context.Doacoes.Include(x => x.Evento).Include(x => x.Itens).FirstOrDefaultAsync(x => x.Id == id);

        if (doacao is null)
            throw new RegraNegocioException("Doação não encontrada!");


        if (doacao.Evento.Status != StatusEvento.EmAndamento)
            throw new RegraNegocioException("Não é possível editar uma Doação de um Evento encerrado!");


        if (request.Tipo == TipoDoacao.Dinheiro)
        {
            if (request.ValorMonetario <= 0)
            {
                throw new RegraNegocioException("O valor monetário deve ser maior que R$0,00");
            }
        }

        var idsRecebidos = request.Itens.Select(x => x.Id).ToList();
        var idsRemover = doacao.Itens.Where(x => !idsRecebidos.Contains(x.Id)).ToList();
        doacao.Tipo = request.Tipo;
        doacao.Observacao = request.Observacao;
        doacao.ValorMonetario = request.ValorMonetario;

        if (doacao.Tipo == TipoDoacao.Dinheiro)
        {
            var movimento = await _context.MovimentosFinanceiros.FirstOrDefaultAsync(x => x.Id == doacao.Id);
            if (movimento is null)
                throw new RegraNegocioException("erro ao alterar movimento financeiro.");
            movimento.Valor = doacao.ValorMonetario;
        }

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
        //1ª regra de negócio: se a doação conter dinheiro, observar se este é maior que zero ou nulo
        if (request.Tipo == TipoDoacao.Dinheiro)
        {
            if (request.ValorMonetario <= 0)
            {
                throw new RegraNegocioException("O valor informado deve ser maior que R$0,00");
            }
        }

        //2ª regra de negócio: se a doação for material, observar pelo menos um item doado
        if (request.Tipo == TipoDoacao.Material && request.Itens.Count == 0)
        {
            throw new RegraNegocioException("Informe pelo menos um item de doação");
        }

        var benfeitorExiste = await _context.Benfeitores.AnyAsync(x => x.Id == request.BenfeitorId);
        if (!benfeitorExiste)
        {
            throw new RegraNegocioException("O benfeitor informado não existe");
        }

        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);

        if (evento is null)
        {
            throw new RegraNegocioException("O evento informado não existe");
        }

        if (evento.Status != StatusEvento.EmAndamento)
        {
            throw new RegraNegocioException("Não foi possível cadastrar a doação: Evento finalizado!");
        }

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
                    Origem = "Doação",
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
