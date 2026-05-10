using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.Infrastructure.Persistence.Repositories;
using Midas.API.Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Midas.API.Business.Implementations
{
    public class GastoBusiness : IGastoBusiness
    {
        private readonly IGastoRepository _gastoRepository;
        private readonly ILogger<GastoBusiness> _logger;

        public GastoBusiness(IGastoRepository gastoRepository, ILogger<GastoBusiness> logger)
        {
            _gastoRepository = gastoRepository;
            _logger = logger;
        }

        public async Task<List<Gasto>> GetAllAsync()
        {
            _logger.LogInformation("Business: Buscando todos os gastos");
            return await _gastoRepository.GetAllAsync();
        }

        public async Task<Gasto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Business: Buscando gasto por ID: {GastoId}", id);
            return await _gastoRepository.GetByIdAsync(id);
        }

        public async Task<Gasto> CreateAsync(CreateGastoDTO createGastoDTO)
        {
            _logger.LogInformation("Business: Criando gasto com título: {Titulo}", createGastoDTO.Titulo);

            if (string.IsNullOrWhiteSpace(createGastoDTO.Titulo))
                throw new ArgumentException("Título é obrigatório");

            if (createGastoDTO.UsuarioId <= 0)
                throw new ArgumentException("UsuarioId é obrigatório");

            if (createGastoDTO.CategoriaId <= 0)
                throw new ArgumentException("CategoriaId é obrigatório");

            if (createGastoDTO.Valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero");

            var gasto = new Gasto
            {
                UsuarioId = createGastoDTO.UsuarioId,
                CategoriaId = createGastoDTO.CategoriaId,
                Titulo = createGastoDTO.Titulo,
                Data = createGastoDTO.Data,
                Valor = createGastoDTO.Valor,
                Fixo = NormalizeFixoValue(createGastoDTO.Fixo)
            };

            var result = await _gastoRepository.AddAsync(gasto);
            _logger.LogInformation("Business: Gasto criado com sucesso. ID: {GastoId}", result.Id);
            return result;
        }

        public async Task UpdateAsync(Gasto gasto)
        {
            _logger.LogInformation("Business: Atualizando gasto: {GastoId}", gasto.Id);

            var existingGasto = await _gastoRepository.GetByIdAsync(gasto.Id);
            if (existingGasto == null)
                throw new KeyNotFoundException($"Gasto com ID {gasto.Id} não encontrado");

            gasto.Fixo = NormalizeFixoValue(gasto.Fixo);
            await _gastoRepository.UpdateAsync(gasto);
            _logger.LogInformation("Business: Gasto atualizado com sucesso. ID: {GastoId}", gasto.Id);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Business: Deletando gasto: {GastoId}", id);

            var gasto = await _gastoRepository.GetByIdAsync(id);
            if (gasto == null)
                throw new KeyNotFoundException($"Gasto com ID {id} não encontrado");

            await _gastoRepository.DeleteAsync(id);
            _logger.LogInformation("Business: Gasto deletado com sucesso. ID: {GastoId}", id);
        }

        public async Task<PagedResult<Gasto>> SearchAsync(GastoSearchParameters parameters)
        {
            _logger.LogInformation("Business: Buscando gastos com filtros");
            return await _gastoRepository.SearchAsync(parameters);
        }

        private char NormalizeFixoValue(char fixo)
        {
            return char.ToUpper(fixo) == 'T' ? 'T' : 'F';
        }
    }
}
