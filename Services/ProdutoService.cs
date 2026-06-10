using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.EntityFrameworkCore;
using BeneficentEvent.Enums;
using SQLitePCL;
using BeneficentEvent.DTOs.Response;

namespace BeneficentEvent.Services;

public class ProdutoService
{
    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProdutoResponse>> ListarAsync()
    {
        return await _context.Produtos.Select(x => new ProdutoResponse(
            x.Id,
            x.Nome,
            x.Categoria,
            x.PrecoVenda
        )).ToListAsync();
    }

    public async Task<ProdutoDetalheResponse> ObterPorIdDetalheAsync(Guid id)
    {
        var produto = await _context.Produtos.Include(x => x.Evento).FirstOrDefaultAsync(x => x.Id == id);

        if(produto is null)
            throw new RegraNegocioException("Produto não cadastrado.");

        return new ProdutoDetalheResponse(
            produto.Id,
            produto.Nome,
            produto.Evento.Nome,
            produto.Categoria,
            produto.PrecoVenda,
            produto.QuantidadeEstoque
        );
    }

    public async Task<Guid> CriarAsync(CriarProdutoRequest request)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);
        
        if(evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");
        
        if(evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Não é possível cadastrar um produto para um evento encerrado.");

        if(request.Nome.Length < 3)
            throw new RegraNegocioException("O campo nome deve conter ao menos 3 letras.");
        
        if(request.Categoria.Length < 3)
            throw new RegraNegocioException("O campo categoria deve conter ao menos 3 letras.");

        if(request.PrecoVenda <= 0)
            throw new RegraNegocioException("O valor do produto deve ser maior que R$0,00.");

        if(request.QuantidadeEstoque < 0)
            throw new RegraNegocioException("O estoque não pode ser um valor negativo.");

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
        var evento = await _context.Eventos.FirstOrDefaultAsync(x => x.Id == request.EventoId);
        var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);

        if (produto is null)
            throw new RegraNegocioException("Produto não cadastrado.");

        if(evento is null)
            throw new RegraNegocioException("Evento não cadastrado.");
        
        if(evento.Status == StatusEvento.Encerrado)
            throw new RegraNegocioException("Não é possível cadastrar um produto para um evento encerrado.");

        if(request.Nome.Length < 3)
            throw new RegraNegocioException("O campo nome deve conter ao menos 3 letras.");
        
        if(request.Categoria.Length < 3)
            throw new RegraNegocioException("O campo categoria deve conter ao menos 3 letras.");

        if(request.PrecoVenda <= 0)
            throw new RegraNegocioException("O valor do produto deve ser maior que R$0,00.");

        if(request.QuantidadeEstoque < 0)
            throw new RegraNegocioException("O estoque não pode ser um valor negativo.");

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