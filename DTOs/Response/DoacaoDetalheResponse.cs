using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Response;

public record DoacaoDetalheResponse(
    Guid Id,
    string NomeEvento,
    string NomeBenfeitor,
    TipoDoacao tipo,
    decimal ValorMonetario,
    DateTime Data,
    string? Observacao,
    List<ItemDoacaoResponse> Itens
);

public record ItemDoacaoResponse(
    Guid Id,
    string Nome,
    decimal Quantidade,
    string Unidade,
    decimal ValorEstimado
);
