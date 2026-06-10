using BeneficentEvent.Data;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;
using BeneficentEvent.Enums;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace BeneficentEvent.Services;

public class DespesaService
{
    private readonly AppDbContext _context;

    public DespesaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DespesaResponse>> ListarAsync()
    {
        return await _context.Despesas.Select(x => new DespesaResponse(
            x.Id,
            x.Evento.Nome,
            x.Descricao,
            x.Categoria,
            x.Fornecedor,
            x.Valor,
            x.Data
        )).ToListAsync(); //Não precisou usar >>> Include(x => x.Evento) // no console mostra a execução de um inner join com eventos, onde evento as e // nome evento = e.nome
    }

    public async Task<DespesaResponse> ObterPorIdAsync(Guid id)
    {
        var despesa = await _context.Despesas.Include(x => x.Evento).FirstOrDefaultAsync(x => x.Id == id);

        if (despesa is null)
            throw new RegraNegocioException("Despesa não cadastrada.");

        return new DespesaResponse(
            despesa.Id,
            despesa.Evento.Nome,
            despesa.Descricao,
            despesa.Categoria,
            despesa.Fornecedor,
            despesa.Valor,
            despesa.Data    
        );
    }
    public async Task<Guid> CriarAsync(CriarDespesaRequest request)
    {
        if (!await _context.Eventos.AnyAsync(x => x.Id == request.EventoId))
            throw new RegraNegocioException("Evento não cadastrado.");

        if (request.Data < DateTime.Today)
            throw new RegraNegocioException("O campo data não pode ser menor que a data atual.");

        if (request.Categoria.Length == 0 || request.Descricao.Length == 0)
            throw new RegraNegocioException("Preencha corretamento os campos: Categoria e Descrição.");

        var despesa = new Despesa
        {
            Id = Guid.NewGuid(),
            EventoId = request.EventoId,
            Categoria = request.Categoria,
            Data = request.Data,
            Descricao = request.Descricao,
            Fornecedor = request.Fornecedor,
            Valor = request.Valor
        };

        var movimento = new MovimentoFinanceiro
        {
            Id = despesa.Id,
            EventoId = request.EventoId,
            Tipo = TipoMovimento.Despesa,
            Valor = despesa.Valor,
            Origem = despesa.Descricao,
            DataMovimento = despesa.Data
        };

        _context.MovimentosFinanceiros.Add(movimento);
        _context.Despesas.Add(despesa);
        await _context.SaveChangesAsync();
        return despesa.Id;
    }

    public async Task EditarAsync(Guid id, CriarDespesaRequest request)
    {
        var despesa = await _context.Despesas.FirstOrDefaultAsync(x => x.Id == id);

        if (despesa is null)
            throw new RegraNegocioException("Despesa não cadastrada.");

        var movimento = await _context.MovimentosFinanceiros.FirstOrDefaultAsync(x => x.Id == despesa.Id);
        if (movimento is null)
            throw new RegraNegocioException("Erro ao editar movimento financeiro.");

        if (request.Data < DateTime.Today)
            throw new RegraNegocioException("O campo data não pode ser menor que a data atual.");

        if (request.Categoria.Length == 0 || request.Descricao.Length == 0)
            throw new RegraNegocioException("Preencha corretamento os campos: Categoria e Descrição.");

        despesa.Categoria = request.Categoria;
        despesa.Data = request.Data;
        despesa.Descricao = request.Descricao;
        despesa.Fornecedor = request.Fornecedor;
        despesa.Valor = request.Valor;
        //editando movimento financeiro de acordo com o valor alterado na despesa 
        movimento.Valor = despesa.Valor;
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var despesa = await _context.Despesas.FirstOrDefaultAsync(x => x.Id == id);
        var movimento = await _context.MovimentosFinanceiros.FirstOrDefaultAsync(x => x.Id == id);

        if (despesa is null || movimento is null)
            throw new RegraNegocioException("Erro ao excluir registro.");

        _context.MovimentosFinanceiros.Remove(movimento);
        _context.Despesas.Remove(despesa);
        await _context.SaveChangesAsync();
    }
}