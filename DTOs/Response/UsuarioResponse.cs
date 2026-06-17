using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Response;

public record UsuarioResponse(
    Guid Id,
    string Nome,
    string Email,
    PerfilUsuario Perfil
);