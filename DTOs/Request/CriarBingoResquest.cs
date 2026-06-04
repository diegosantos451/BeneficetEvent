namespace BeneficentEvent.DTOs.Request;

public record CriarBingoRequest(
    Guid EventoId,
    string Nome,
    decimal ValorCartela,
    DateTime DataSorteio,
    List<ItensBingoRequest> premiosBingo

);

public record ItensBingoRequest(
    string Descricao,
    decimal ValorEstimado
);