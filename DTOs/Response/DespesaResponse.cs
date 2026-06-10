namespace BeneficentEvent.DTOs.Response;

public record DespesaResponse(
    Guid Id,
    string EventoNome,
    string Descricao,
    string Categoria,
    string? Fornecedor,
    decimal Valor,
    DateTime Data
);