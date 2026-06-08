using BeneficentEvent.Enums;
namespace BeneficentEvent.DTOs.Request;

public record CriarEventoRequest(
    string Nome,
    string? Descricao,
    DateTime DataInicio,
    DateTime DataFim,
    string? Local,
    StatusEvento Status
);