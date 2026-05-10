using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.Infrastructure.Persistence.Repositories;
using Midas.API.Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Midas.API.Business.Implementations
{
    public class ReceitaBusiness : IReceitaBusiness
    {
        private readonly IReceitaRepository _receitaRepository;
        private readonly ILogger<ReceitaBusiness> _logger;

        public ReceitaBusiness(IReceitaRepository receitaRepository, ILogger<ReceitaBusiness> logger)
        {
            _receitaRepository = receitaRepository;
            _logger = logger;
        }

        public async Task<List<Receita>> GetAllAsync()
        {
            _logger.LogInformation("Business: Buscando todas as receitas");
            return (await _receitaRepository.GetAllAsync()).ToList();
        }

        public async Task<Receita?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Business: Buscando receita por ID: {ReceitaId}", id);
            return await _receitaRepository.GetByIdAsync(id);
        }

        public async Task<Receita> CreateAsync(CreateReceitaDTO createReceitaDTO)
        {
            _logger.LogInformation("Business: Criando receita com título: {Titulo}", createReceitaDTO.Titulo);

            if (string.IsNullOrWhiteSpace(createReceitaDTO.Titulo))
                throw new ArgumentException("Título é obrigatório");

            if (createReceitaDTO.UsuarioId <= 0)
                throw new ArgumentException("UsuarioId é obrigatório");

            if (createReceitaDTO.Valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero");

            var receita = new Receita
            {
                UsuarioId = createReceitaDTO.UsuarioId,
                Titulo = createReceitaDTO.Titulo,
                Data = createReceitaDTO.Data,
                Valor = createReceitaDTO.Valor,
                Fixo = NormalizeFixoValue(createReceitaDTO.Fixo)
            };

            var result = await _receitaRepository.CreateAsync(receita);
            _logger.LogInformation("Business: Receita criada com sucesso. ID: {ReceitaId}", result.Id);
            return result;
        }

        public async Task UpdateAsync(Receita receita)
        {
            _logger.LogInformation("Business: Atualizando receita: {ReceitaId}", receita.Id);

            var existingReceita = await _receitaRepository.GetByIdAsync(receita.Id);
            if (existingReceita == null)
                throw new KeyNotFoundException($"Receita com ID {receita.Id} não encontrada");

            receita.Fixo = NormalizeFixoValue(receita.Fixo);
            await _receitaRepository.UpdateAsync(receita);
            _logger.LogInformation("Business: Receita atualizada com sucesso. ID: {ReceitaId}", receita.Id);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Business: Deletando receita: {ReceitaId}", id);

            var receita = await _receitaRepository.GetByIdAsync(id);
            if (receita == null)
                throw new KeyNotFoundException($"Receita com ID {id} não encontrada");

            await _receitaRepository.DeleteAsync(id);
            _logger.LogInformation("Business: Receita deletada com sucesso. ID: {ReceitaId}", id);
        }

        public async Task<PagedResult<Receita>> SearchAsync(ReceitaSearchParameters parameters)
        {
            _logger.LogInformation("Business: Buscando receitas com filtros");
            return await _receitaRepository.SearchAsync(parameters);
        }

        private char NormalizeFixoValue(char fixo)
        {
            return char.ToUpper(fixo) == 'T' ? 'T' : 'F';
        }
    }
}
