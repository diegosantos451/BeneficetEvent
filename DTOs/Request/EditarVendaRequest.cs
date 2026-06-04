using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Request;

public record EditarVendaRequest(
    Guid Id,
    Guid EventoId,
    DateTime DataVenda,
    decimal ValorTotal,
    FormaPagamento formaPagamento,
    List<EItemVendaRequest> ItensVenda
);

public record EItemVendaRequest(
    Guid Id,
    Guid ProdutoId,
    Guid VendaId,
    decimal Quantidade,
    decimal PrecoUnitario
);