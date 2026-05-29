using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Request;

public record CriarVendaRequest(
    Guid Id,
    Guid EventoId,
    DateTime DataVenda,
    decimal ValorTotal,
    FormaPagamento formaPagamento,
    List<ItemVendaRequest> ItemVendas
);

public record ItemVendaRequest(
    Guid ProdutoId,
    Guid VendaId,
    decimal Quantidade,
    decimal PrecoUnitario
);