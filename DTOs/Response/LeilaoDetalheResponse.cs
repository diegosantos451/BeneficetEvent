namespace BeneficentEvent.DTOs.Response;

public record LeilaoDetalheResponse(
    Guid Id,
    string Nome,
    string NomeEvento,
    DateTime Data,
    List<ItensLeilaoResponse> Itens
);

public record ItensLeilaoResponse(
    Guid Id,
    string Nome,
    string Lote,
    string? Descricao,
    decimal LanceInicial,
    decimal LanceAtual,
    List<LancesItensLeilaoResponse> Lances
);

public record LancesItensLeilaoResponse(
    Guid Id,
    string NomeBenfeitor,
    decimal Valor,
    DateTime Data
);