using BeneficentEvent.Data;
using BeneficentEvent.Exceptions;
using BeneficentEvent.Models;
using BeneficentEvent.DTOs.Request;
using BeneficentEvent.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace BeneficentEvent.Services;

public class BenfeitorService
{
    private readonly AppDbContext _context;

    public BenfeitorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BenfeitorResponse>> ListarAsync()
    {
        return await _context.Benfeitores.Select(x => new BenfeitorResponse(
            x.Id,
            x.Nome,
            x.CPF,
            x.Telefone,
            x.Email,
            x.Endereco
        )).ToListAsync();
    }
    public async Task<BenfeitorDetalheResponse> ObterPorIdDetalhesAsync(Guid benfeitorId)
    {
        var benfeitor = await _context.Benfeitores.Include(x => x.Eventos).ThenInclude(x => x.Evento).Include(x => x.Doacoes).ThenInclude(x => x.Itens).
            Include(x => x.Lances).ThenInclude(x => x.ItemLeilao).FirstOrDefaultAsync(x => x.Id == benfeitorId);

        if (benfeitor is null)
            throw new RegraNegocioException("Benfeitor não cadastrado.");

        return new BenfeitorDetalheResponse(
            benfeitor.Id,
            benfeitor.Nome,
            benfeitor.CPF,
            benfeitor.Telefone,
            benfeitor.Email,
            benfeitor.Endereco,

            benfeitor.Eventos.Select(x => new BenfeitorEventosResponse(
                x.EventoId,
                x.Evento.Nome,
                x.Funcao,
                x.Observacao
            )).ToList(),

            benfeitor.Doacoes.Select(x => new BenfeitorDoacoesResponse(
                x.Id,
                x.DataDoacao,
                x.ValorMonetario,

                x.Itens.Select(x => new BenfeitorItensDoacaoResponse(
                    x.Nome,
                    x.Quantidade,
                    x.Unidade,
                    x.ValorEstimado)).ToList()
            )).ToList(),

            benfeitor.Lances.Select(x => new BenfeitorLancesResponse(
                x.ItemLeilao.Nome,
                x.DataHora,
                x.Valor
            )).ToList()
        );
    }

    public async Task EditarAsync(Guid id, CriarBenfeitorRequest request)
    {
        var benfeitor = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == id);

        if (benfeitor is null)
            throw new RegraNegocioException("Benfeitor não cadastrado.");
        if (request.Cpf.Length <= 0 || request.Cpf.Length > 11)
            throw new RegraNegocioException("CPF inválido.");
        if (request.Nome.Length < 3)
            throw new RegraNegocioException("Nome inválido");

        benfeitor.Nome = request.Nome;
        benfeitor.CPF = request.Cpf;
        benfeitor.Email = request.Email;
        benfeitor.Endereco = request.Endereco;
        benfeitor.Telefone = request.Telefone;
        await _context.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Guid id)
    {
        var benfeitor = await _context.Benfeitores.FirstOrDefaultAsync(x => x.Id == id);
        if (benfeitor is null)
            throw new RegraNegocioException("Benfeitor não cadastrado.");
        _context.Benfeitores.Remove(benfeitor);
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> CriarAsync(CriarBenfeitorRequest request)
    {
        if (request.Cpf.Length <= 0 || request.Cpf.Length > 11)
            throw new RegraNegocioException("CPF inválido.");
        if (request.Nome.Length < 3)
            throw new RegraNegocioException("Nome inválido");

        var benfeitor = new Benfeitor
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            CPF = request.Cpf,
            Telefone = request.Telefone,
            Email = request.Email,
            Endereco = request.Endereco
        };

        _context.Benfeitores.Add(benfeitor);
        await _context.SaveChangesAsync();
        return benfeitor.Id;
    }
}