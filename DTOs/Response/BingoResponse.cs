namespace BeneficentEvent.DTOs.Response;

public record BingoResponse(
    Guid Id,
    string Nome,
    decimal ValorCartela,
    DateTime DataSorteio
);