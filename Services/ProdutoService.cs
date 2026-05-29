using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace BeneficentEvent.Services;

public class ProdutoService
{
    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Produto>> ListarAsync()
    {
        return await _context.Produtos.ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Guid> CriarAsync(CriarProdutoRequest request)
    {
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            EventoId = request.EventoId,
            Nome = request.Nome,
            Categoria = request.Categoria,
            PrecoVenda = request.PrecoVenda,
            QuantidadeEstoque = request.QuantidadeEstoque
        };
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return produto.Id;
    }

    public async Task EditarAsync(Guid id, CriarProdutoRequest request)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
        if (produto is null)
            throw new RegraNegocioException("Produto não cadastrado.");

        produto.Categoria = request.Categoria;
        produto.EventoId = request.EventoId;
        produto.Nome = request.Nome;
        produto.PrecoVenda = request.PrecoVenda;
        produto.QuantidadeEstoque = request.QuantidadeEstoque;
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
        if(produto is null)
            throw new RegraNegocioException("Produto não cadastrado.");
        
        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
    }
}