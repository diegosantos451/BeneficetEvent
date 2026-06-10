using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Response;

public record VendaDetalheResponse(
    Guid Id,
    string NomeEvento,
    DateTime Data,
    decimal ValorTotal,
    FormaPagamento Pagamento,
    List<ItensVendaResponse> Itens
);

public record ItensVendaResponse(
    Guid Id,
    Guid ProdutoId,
    string NomeProduto,
    decimal Quantidade,
    decimal PrecoUnitario
);