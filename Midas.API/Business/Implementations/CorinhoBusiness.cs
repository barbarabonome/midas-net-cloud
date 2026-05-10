using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.Infrastructure.Persistence.Repositories;
using Midas.API.Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Midas.API.Business.Implementations
{
    public class CorinhoBusiness : ICorinhoBusiness
    {
        private readonly ICofrinhoRepository _cofrinhoRepository;
        private readonly ILogger<CorinhoBusiness> _logger;

        public CorinhoBusiness(ICofrinhoRepository cofrinhoRepository, ILogger<CorinhoBusiness> logger)
        {
            _cofrinhoRepository = cofrinhoRepository;
            _logger = logger;
        }

        public async Task<List<Cofrinho>> GetAllAsync()
        {
            _logger.LogInformation("Business: Buscando todos os cofrinhos");
            return await _cofrinhoRepository.GetAllAsync();
        }

        public async Task<Cofrinho?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Business: Buscando cofrinho por ID: {CofrinhoId}", id);
            return await _cofrinhoRepository.GetByIdAsync(id);
        }

        public async Task<Cofrinho> CreateAsync(CreateCofrinhoDTO createCofrinhoDTO)
        {
            _logger.LogInformation("Business: Criando cofrinho com título: {Titulo}", createCofrinhoDTO.Titulo);

            if (string.IsNullOrWhiteSpace(createCofrinhoDTO.Titulo))
                throw new ArgumentException("Título é obrigatório");

            if (createCofrinhoDTO.UsuarioId <= 0)
                throw new ArgumentException("UsuarioId é obrigatório");

            if (createCofrinhoDTO.Meta <= 0)
                throw new ArgumentException("Meta deve ser maior que zero");

            var cofrinho = new Cofrinho
            {
                UsuarioId = createCofrinhoDTO.UsuarioId,
                Titulo = createCofrinhoDTO.Titulo,
                Meta = createCofrinhoDTO.Meta,
                Atingido = createCofrinhoDTO.Atingido,
                Aplicado = NormalizeAplicadoValue(createCofrinhoDTO.Aplicado)
            };

            var result = await _cofrinhoRepository.AddAsync(cofrinho);
            _logger.LogInformation("Business: Cofrinho criado com sucesso. ID: {CofrinhoId}", result.Id);
            return result;
        }

        public async Task UpdateAsync(Cofrinho cofrinho)
        {
            _logger.LogInformation("Business: Atualizando cofrinho: {CofrinhoId}", cofrinho.Id);

            var existingCofrinho = await _cofrinhoRepository.GetByIdAsync(cofrinho.Id);
            if (existingCofrinho == null)
                throw new KeyNotFoundException($"Cofrinho com ID {cofrinho.Id} não encontrado");

            cofrinho.Aplicado = NormalizeAplicadoValue(cofrinho.Aplicado);
            await _cofrinhoRepository.UpdateAsync(cofrinho);
            _logger.LogInformation("Business: Cofrinho atualizado com sucesso. ID: {CofrinhoId}", cofrinho.Id);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Business: Deletando cofrinho: {CofrinhoId}", id);

            var cofrinho = await _cofrinhoRepository.GetByIdAsync(id);
            if (cofrinho == null)
                throw new KeyNotFoundException($"Cofrinho com ID {id} não encontrado");

            await _cofrinhoRepository.DeleteAsync(id);
            _logger.LogInformation("Business: Cofrinho deletado com sucesso. ID: {CofrinhoId}", id);
        }

        public async Task<PagedResult<Cofrinho>> SearchAsync(CofrinhoSearchParameters parameters)
        {
            _logger.LogInformation("Business: Buscando cofrinhos com filtros");
            return await _cofrinhoRepository.SearchAsync(parameters);
        }

        public async Task<bool> AtualizarProgressoAsync(int id, decimal valorAtingido)
        {
            _logger.LogInformation("Business: Atualizando progresso do cofrinho: {CofrinhoId}", id);
            return await _cofrinhoRepository.AtualizarProgresso(id, valorAtingido);
        }

        private char NormalizeAplicadoValue(char aplicado)
        {
            return char.ToUpper(aplicado) == 'T' ? 'T' : 'F';
        }
    }
}
