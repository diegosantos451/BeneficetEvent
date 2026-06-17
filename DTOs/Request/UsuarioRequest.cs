using BeneficentEvent.Enums;

namespace BeneficentEvent.DTOs.Request;

public record UsuarioRequest(
    Guid Id,
    string Nome,
    string Email,
    string SenhaHash,
    PerfilUsuario Perfil
);