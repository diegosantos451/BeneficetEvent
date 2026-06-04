namespace BeneficentEvent.DTOs.Request;


public record EditarLeilaoRequest
(   
    string Nome,
    DateTime Data,
    List<EditarItemLeilaoRequest> Itens
);

public record EditarItemLeilaoRequest
(
    Guid Id,
    string Nome,
    string Lote,
    string? Descricao,
    decimal LanceInicial,
    decimal LanceAtual,
    List<EditarLanceRequest> Lances
);

public record EditarLanceRequest
(
    Guid Id,
    Guid BenfeitorID,
    decimal Valor,
    DateTime Data
);