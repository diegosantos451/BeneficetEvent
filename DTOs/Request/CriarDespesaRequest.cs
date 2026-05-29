namespace BeneficentEvent.DTOs.Request;

public record CriarDespesaRequest(
    Guid Id,
    Guid EventoId,
    string Descricao,
    string Categoria,
    string? Fornecedor,
    decimal Valor,
    DateTime Data
);