namespace BeneficentEvent.DTOs.Response;

public record BenfeitorResponse(
    Guid Id,
    string Nome,
    string Cpf,
    string? Telefone,
    string? Email,
    string? Endereco
);
