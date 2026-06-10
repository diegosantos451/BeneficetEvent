namespace BeneficentEvent.DTOs.Response;

public record LeilaoResponse(
    Guid Id,
    string Nome,
    string NomeEvento,
    DateTime Data

);
