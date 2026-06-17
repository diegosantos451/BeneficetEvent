using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Response;

public record LoginResponse(
    string Token,
    string Nome,
    string Email,
    PerfilUsuario Perfil
);  