namespace BeneficentEvent.DTOs.Response;
public record ListaParticipanteResponse(
    Guid BenfeitorId,
    string BenfeitorNome,
    string Funcao,
    string? Observacao
);
