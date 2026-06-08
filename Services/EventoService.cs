using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
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

    public async Task<List<Evento>> ListarAsync()
    {
        return await _context.Eventos.ToListAsync();
    }

    public async Task<Evento?> ObterPorIdAsync(Guid id)
    {
        return await _context.Eventos.FirstOrDefaultAsync(x => x.Id == id);

    }

    public async Task<Guid> CriarAsync(CriarEventoRequest request)
    {
        var evento = new Evento
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Descricao = request.Descricao,
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            Local = request.Local,
            Status = StatusEvento.Planejamento
        };
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
        return evento.Id;
    }

    public async Task EditarAsync(Guid id, CriarEventoRequest request)
    {
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

}