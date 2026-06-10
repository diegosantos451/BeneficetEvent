using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Response;

public record DoacaoResponse(
    Guid Id,
    string NomeEvento,
    string NomeBenfeitor,
    TipoDoacao tipo,
    decimal ValorMonetario,
    DateTime Data,
    string? Observacao
);
