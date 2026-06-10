using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;
using BeneficentEvent.Enums;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace BeneficentEvent.Services;

public class EventoService
{
    private readonly AppDbContext _context;
    public EventoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventoResponse>> ListarAsync()
    {
        return await _context.Eventos.Select(x => new EventoResponse(
            x.Id,
            x.Nome,
            x.Descricao,
            x.DataInicio,
            x.DataFim,
            x.Local,
            x.Status
        )).ToListAsync();
    }

    public async Task<EventoDetalheResponse> ObterPorIdDetalhesAsync(Guid id)
    {
        var evento = await _context.Eventos.AsSplitQuery().Include(x => x.Participantes).ThenInclude(x => x.Benfeitor).Include(x => x.Doacoes).
            ThenInclude(x => x.Benfeitor).Include(x => x.Doacoes).ThenInclude(x => x.Itens).Include(x => x.Produtos).Include(x => x.Despesas).Include(x => x.Bingos).
                ThenInclude(x => x.Premios).Include(x => x.Leiloes).ThenInclude(x => x.Itens)
                    .ThenInclude(x => x.Lances).ThenInclude(x => x.Benfeitor).Include(x => x.MovimentosFinanceiros).
                        FirstOrDefaultAsync(x => x.Id == id);

        if (evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");

        return new EventoDetalheResponse(
            evento.Id,
            evento.Nome,
            evento.Descricao,
            evento.DataInicio,
            evento.DataFim,
            evento.Local,
            evento.Status,

            evento.Participantes.Select(p => new BenfeitoresEventoResponse(
                p.Benfeitor.Nome,
                p.Funcao,
                p.Observacao
            )).ToList(),

            evento.Doacoes.Select(d => new DoacoesEventoResponse(
                d.Benfeitor.Nome,
                d.Tipo,
                d.ValorMonetario,
                d.DataDoacao,
                d.Observacao,
                d.Itens.Select(i => new ItensDoacoesEventoResponse(
                    i.Nome,
                    i.Quantidade,
                    i.Unidade,
                    i.ValorEstimado
                )).ToList()
            )).ToList(),

            evento.Produtos.Select(p => new ProdutosEventoResponse(
                p.Nome,
                p.Categoria,
                p.PrecoVenda,
                p.QuantidadeEstoque
            )).ToList(),

            evento.Despesas.Select(d => new DespesasEventoResponse(
                d.Descricao,
                d.Categoria,
                d.Fornecedor,
                d.Valor,
                d.Data
            )).ToList(),

            evento.Bingos.Select(b => new BingosEventosResponse(
                b.Nome,
                b.ValorCartela,
                b.DataSorteio,
                b.Premios.Select(i => new PremiosBingosEventosResponse(
                    i.Descricao,
                    i.ValorEstimado
                )).ToList()
            )).ToList(),

            evento.Leiloes.Select(l => new LeiloesEventosResponse(
                l.Nome,
                l.Data,
                l.Itens.Select(x => new ItensLeiloesEventosResponse(
                    x.Nome,
                    x.Lote,
                    x.Descricao,
                    x.LanceInicial,
                    x.LanceAtual,
                    x.Lances.Select(x => new LancesItensLeiloesEventoResponse(
                        x.Benfeitor.Nome,
                        x.Valor,
                        x.DataHora
                    )).ToList()
                )).ToList()
            )).ToList(),

            evento.MovimentosFinanceiros.Select(m => new MovimentosEventoResponse(
                m.Tipo,
                m.Valor,
                m.Origem,
                m.DataMovimento
            )).ToList()
        );
    }

    public async Task<Guid> CriarAsync(CriarEventoRequest request)
    {
        if (request.Nome.Length < 3)
            throw new RegraNegocioException("O campo nome do evento precisa conter ao menos 3 letras.");

        if (request.Descricao.Length < 3)
            throw new RegraNegocioException("O campo descrição do evento precisa conter ao menos 3 letras.");

        if (request.DataInicio < DateTime.Today)
            throw new RegraNegocioException("A data de início do evento precisa ser maior que a data atual.");

        if (request.DataFim < DateTime.Today)
            throw new RegraNegocioException("A data de término do evento precisa ser maior que a data atual.");

        if (request.Local.Length < 3)
            throw new RegraNegocioException("O campo local do evento precisa conter ao menos 3 letras.");

        var evento = new Evento
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Descricao = request.Descricao,
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            Local = request.Local,
            Status = request.Status
        };
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
        return evento.Id;
    }

    public async Task EditarAsync(Guid id, CriarEventoRequest request)
    {
        if (request.Nome.Length < 3)
            throw new RegraNegocioException("O campo nome do evento precisa conter ao menos 3 letras.");

        if (request.Descricao.Length < 3)
            throw new RegraNegocioException("O campo descrição do evento precisa conter ao menos 3 letras.");

        if (request.DataInicio < DateTime.Today)
            throw new RegraNegocioException("A data de início do evento precisa ser maior que a data atual.");

        if (request.DataFim < DateTime.Today)
            throw new RegraNegocioException("A data de término do evento precisa ser maior que a data atual.");

        if (request.Local.Length < 3)
            throw new RegraNegocioException("O campo local do evento precisa conter ao menos 3 letras.");

        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == id);
        if (evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");

        evento.Nome = request.Nome;
        evento.Descricao = request.Descricao;
        evento.DataInicio = request.DataInicio;
        evento.DataFim = request.DataFim;
        evento.Local = request.Local;
        evento.Status = request.Status;
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == id);
        if (evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");
        _context.Eventos.Remove(evento);
        await _context.SaveChangesAsync();
    }

    public async Task AddBenfeitorAsync(Guid eventoId, CriarParticipanteEventoRequest request)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == eventoId);
        var benfeitor = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == request.BenfeitorId);

        if (await _context.ParticipacoesEvento.AnyAsync(x => x.BenfeitorId == request.BenfeitorId && x.EventoId == eventoId))
            throw new RegraNegocioException("Participante já vinculado ao evento.");

        if (!(evento is null) && !(benfeitor is null))
        {
            var participanteEvento = new ParticipacaoEvento
            {
                Funcao = request.Funcao,
                Observacao = request.Observacao,
                BenfeitorId = request.BenfeitorId,
                EventoId = eventoId
            };

            _context.ParticipacoesEvento.Add(participanteEvento);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new RegraNegocioException("Evento ou Benfeitor não cadastrado.");
        }
    }

    public async Task RemoverBenfeitorAsync(Guid eventoId, Guid benfeitorId)
    {
        var participanteEvento = await _context.ParticipacoesEvento.FirstOrDefaultAsync(x => x.BenfeitorId == benfeitorId && x.EventoId == eventoId);

        if (!(participanteEvento is null))
        {
            _context.ParticipacoesEvento.Remove(participanteEvento);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new RegraNegocioException("Erro ao desvincular benfeitor.");
        }
    }

    public async Task<List<ParticipacaoEvento>> ListarParticipantesAsync(Guid eventoId)
    {
        return await _context.ParticipacoesEvento.Where(x => x.EventoId == eventoId).Include(x => x.Benfeitor).ToListAsync();
    }

    public async Task EncerrarEventoAsync(Guid id)
    {
        var evento = await _context.Eventos.Include(x => x.Leiloes).ThenInclude(x => x.Itens).ThenInclude(x => x.Lances).FirstOrDefaultAsync(x => x.Id == id);

        if (evento is null)
            throw new RegraNegocioException("Evento não registrado.");

        if (evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Evento já encerrado.");

        evento.Status = StatusEvento.Encerrado;

        foreach (var leilao in evento.Leiloes)
        {
            foreach (var item in leilao.Itens)
            {
                var lanceVencedor = item.Lances.OrderByDescending(x => x.Valor).FirstOrDefault();

                if (lanceVencedor is null)
                    continue;

                var movimento = new MovimentoFinanceiro
                {
                    Id = lanceVencedor.Id,
                    EventoId = evento.Id,
                    Tipo = TipoMovimento.Receita,
                    Valor = lanceVencedor.Valor,
                    Origem = $"Lance de arremate do leilão: {leilao.Nome}, item: {item.Descricao}.",
                    DataMovimento = DateTime.UtcNow
                };
                _context.MovimentosFinanceiros.Add(movimento);
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task FechamentoDeBingoAsync(Guid eventoId, Guid bingoId, int quantidade)
    {
        var bingo = await _context.Bingos.FirstOrDefaultAsync(x => x.Id == bingoId);
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == eventoId);

        if (evento is null)
            throw new RegraNegocioException("Evento não registrado.");

        if (bingo is null)
            throw new RegraNegocioException("Bingo não registrado.");

        if (bingo.EventoId != evento.Id)
            throw new RegraNegocioException("Este bingo não pertence ao evento selecionado.");

        if (evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Evento já encerrado.");

        if (await _context.MovimentosFinanceiros.AnyAsync(x => x.Id == bingoId))
            throw new RegraNegocioException("Bingo já contabilizado");

        if (quantidade <= 0)
            throw new RegraNegocioException("Nenhuma cartela deste bingo foi vendida.");

        var movimento = new MovimentoFinanceiro
        {
            Id = bingoId,
            EventoId = eventoId,
            Tipo = TipoMovimento.Receita,
            Valor = bingo.ValorCartela * quantidade,
            Origem = $"Venda de cartelas de bingo: {bingo.Nome}",
            DataMovimento = DateTime.UtcNow
        };

        _context.MovimentosFinanceiros.Add(movimento);
        await _context.SaveChangesAsync();
    }
}
