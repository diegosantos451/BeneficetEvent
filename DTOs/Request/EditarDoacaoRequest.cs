namespace BeneficentEvent.DTOs.Request;
using BeneficentEvent.Enums;


public record EditarDoacaoRequest(
    TipoDoacao Tipo,
    decimal ValorMonetario,
    string? Observacao,
    List<ItemDoacaoRequest> Itens
);

public record EditarItemDoacaoRequest(
    string Nome,
    decimal Quantidade,
    string Unidade,
    decimal ValorEstimado
);