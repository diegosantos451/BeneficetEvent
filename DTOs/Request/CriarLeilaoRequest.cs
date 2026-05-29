namespace BeneficentEvent.DTOs.Request;
public record CriarLeilaoRequest(
    Guid EventoId,
    string Nome,
    DateTime Data,
    List<ItemLeilaoRequest> Itens
);

public record ItemLeilaoRequest(
    Guid Id,
    string Nome,
    string Lote,
    string? Descricao,
    decimal LanceInicial,
    List<CriarLanceRequest> Lances
);

public record CriarLanceRequest
(
    Guid Id,
    Guid BenfeitorID,
    decimal Valor,
    DateTime Data
);