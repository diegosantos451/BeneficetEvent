namespace BeneficentEvent.DTOs.Request;
using BeneficentEvent.Enums;


public record EditarDoacaoRequest(
    TipoDoacao Tipo,
    decimal ValorMonetario,
    string? Observacao,
    List<EditarItemDoacaoRequest> Itens
);

public record EditarItemDoacaoRequest(
    Guid Id,
    string Nome,
    decimal Quantidade,
    string Unidade,
    decimal ValorEstimado
);