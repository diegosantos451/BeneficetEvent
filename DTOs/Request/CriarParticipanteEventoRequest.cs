namespace BeneficentEvent.DTOs.Request;

public record CriarParticipanteEventoRequest(
    Guid BenfeitorId,
    Guid EventoId,
    string Funcao,
    string Observacao
);