namespace BeneficentEvent.DTOs.Request;
using BeneficentEvent.Enums;


public record EditarDoacaoRequest(
    Guid BenfeitorId,
    TipoDoacao Tipo,
    decimal ValorMonetario,
    string? Observacao,
    DateTime Data,
    List<EditarItemDoacaoRequest> Itens
);

public record EditarItemDoacaoRequest(
    Guid Id,
    string Nome,
    decimal Quantidade,
    string Unidade,
    decimal ValorEstimado
);