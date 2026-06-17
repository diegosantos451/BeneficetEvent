namespace BeneficentEvent.DTOs.Request;

public record LoginRequest(
    string Email,
    string Senha
);