using BeneficentEvent.Enums;

namespace BeneficentEvent.Models;

public class Usuario
{
    public Guid Id { get; set; }
    public string  Nome { get; set; } = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
}