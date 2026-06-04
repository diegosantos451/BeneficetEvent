using System.Diagnostics.Eventing.Reader;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Models;

namespace BeneficentEvent.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Benfeitor> Benfeitores => Set<Benfeitor>();
    public DbSet<ParticipacaoEvento> ParticipacoesEvento => Set<ParticipacaoEvento>();
    public DbSet<Doacao> Doacoes => Set<Doacao>();
    public DbSet<ItemDoacao> ItensDoacao => Set<ItemDoacao>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<Despesa> Despesas => Set<Despesa>();
    public DbSet<Bingo> Bingos => Set<Bingo>();
    public DbSet<PremioBingo> PremiosBingo => Set<PremioBingo>();
    public DbSet<Leilao> Leiloes => Set<Leilao>();
    public DbSet<ItemLeilao> ItensLeilao => Set<ItemLeilao>();
    public DbSet<Lance> Lances => Set<Lance>();
    public DbSet<MovimentoFinanceiro> MovimentosFinanceiros => Set<MovimentoFinanceiro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParticipacaoEvento>()
            .HasKey(x => new { x.EventoId, x.BenfeitorId });

        base.OnModelCreating(modelBuilder);
    }
}