namespace BeneficentEvent.DTOs.Request;

public record CriarBingoRequest(
    Guid EventoId,
    string Nome,
    decimal ValorCartela,
    DateTime DataSorteio

);