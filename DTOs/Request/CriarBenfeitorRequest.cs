namespace BeneficentEvent.DTOs.Request;

public record CriarBenfeitorRequest(
    string Nome,
    string Cpf,
    string Telefone,
    string Email,
    string Endereco
);

