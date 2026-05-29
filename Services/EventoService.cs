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

    public async Task<Evento?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Eventos.FirstOrDefaultAsync(x => x.Id == id);

    }

    public async Task<Guid> CriarAsync(CriarEventoRequest request)
    {
        var evento = new Evento
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Descricao = request.Nome,
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
        evento.Descricao = request.Nome;
        evento.DataInicio = request.DataInicio;
        evento.DataFim = request.DataFim;
        evento.Local = request.Local;
        evento.Status = StatusEvento.Planejamento;
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == id);
        if(evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");
        _context.Eventos.Remove(evento);
        await _context.SaveChangesAsync();
    }

    public async Task AdicionarParticipante(Guid idEvento, CriarParticipanteEventoRequest request)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == idEvento);

        if(evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");

        var participante = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == request.BenfeitorId);

        if(participante is null)
            throw new RegraNegocioException("Participante não cadastrado.");

        var participanteEvento = new ParticipacaoEvento
        {
            EventoId = idEvento,
            BenfeitorId = request.BenfeitorId,
            Funcao = request.Funcao,
            Observacao = request.Observacao
        };      

        _context.ParticipacoesEvento.Add(participanteEvento);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverParticipante(Guid idEvento, Guid idBenfeitor)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == idEvento);

        if(evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");

        var participante = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == idBenfeitor);

        if(participante is null)
            throw new RegraNegocioException("Participante não cadastrado.");

        var participanteEvento = await _context.ParticipacoesEvento.FirstOrDefaultAsync(x => x.BenfeitorId == idBenfeitor && x.EventoId == idEvento);     
        
        if(participanteEvento is null)
            throw new RegraNegocioException("Evento não cadastrado.");

        _context.ParticipacoesEvento.Remove(participanteEvento);
        await _context.SaveChangesAsync();
    }
}