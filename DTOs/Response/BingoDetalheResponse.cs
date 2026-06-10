namespace BeneficentEvent.DTOs.Response;

public record BingoDetalheResponse(
    Guid Id,
    string Nome,
    decimal ValorCartela,
    DateTime DataSorteio,
    List<ItensBingosResponse> Itens
);

public record ItensBingosResponse(
    Guid ItemId,
    string Descricao,
    decimal ValorEstimado
);