using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Response;

public record VendaResponse(
    Guid Id,
    string NomeEvento,
    DateTime Data,
    decimal ValorTotal,
    FormaPagamento Pagamento
);