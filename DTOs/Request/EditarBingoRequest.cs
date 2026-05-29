namespace BeneficentEvent.DTOs.Request;

public record EditarBingoRequest(
    string Nome,
    decimal ValorCartela,
    DateTime DataSorteio,
    List<PremioBingoRequest> PremiosBingo
);

public record PremioBingoRequest(
    Guid Id,
    string Descricao,
    decimal ValorEstimado
);
