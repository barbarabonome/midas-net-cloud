using Midas.Infrastructure.Persistence.Entities;
using Midas.API.DTOs;
using Midas.Infrastructure.Persistence.Repositories;
using Midas.API.Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Midas.API.Business.Implementations
{
    public class CategoriaBusiness : ICategoriaBusiness
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILogger<CategoriaBusiness> _logger;

        public CategoriaBusiness(ICategoriaRepository categoriaRepository, ILogger<CategoriaBusiness> logger)
        {
            _categoriaRepository = categoriaRepository;
            _logger = logger;
        }

        public async Task<List<Categoria>> GetAllAsync()
        {
            _logger.LogInformation("Business: Buscando todas as categorias");
            return await _categoriaRepository.GetAllAsync();
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Business: Buscando categoria por ID: {CategoriaId}", id);
            return await _categoriaRepository.GetByIdAsync(id);
        }

        public async Task<Categoria> CreateAsync(CreateCategoriaDTO createCategoriaDTO)
        {
            _logger.LogInformation("Business: Criando categoria com nome: {Nome}", createCategoriaDTO.Nome);

            if (string.IsNullOrWhiteSpace(createCategoriaDTO.Nome))
                throw new ArgumentException("Nome é obrigatório");

            var categoria = new Categoria
            {
                Nome = createCategoriaDTO.Nome,
                Descricao = createCategoriaDTO.Descricao
            };

            var result = await _categoriaRepository.AddAsync(categoria);
            _logger.LogInformation("Business: Categoria criada com sucesso. ID: {CategoriaId}", result.Id);
            return result;
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            _logger.LogInformation("Business: Atualizando categoria: {CategoriaId}", categoria.Id);

            var existingCategoria = await _categoriaRepository.GetByIdAsync(categoria.Id);
            if (existingCategoria == null)
                throw new KeyNotFoundException($"Categoria com ID {categoria.Id} não encontrada");

            await _categoriaRepository.UpdateAsync(categoria);
            _logger.LogInformation("Business: Categoria atualizada com sucesso. ID: {CategoriaId}", categoria.Id);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Business: Deletando categoria: {CategoriaId}", id);

            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada");

            await _categoriaRepository.DeleteAsync(id);
            _logger.LogInformation("Business: Categoria deletada com sucesso. ID: {CategoriaId}", id);
        }

        public async Task<PagedResult<Categoria>> SearchAsync(CategoriaSearchParameters parameters)
        {
            _logger.LogInformation("Business: Buscando categorias com filtros");
            return await _categoriaRepository.SearchAsync(parameters);
        }
    }
}
