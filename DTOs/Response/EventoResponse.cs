using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Response;

public record EventoResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    string? Local,
    StatusEvento Status
);