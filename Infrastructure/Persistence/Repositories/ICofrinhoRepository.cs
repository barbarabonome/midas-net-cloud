using Midas.Infrastructure.Persistence.Entities;

namespace Midas.Infrastructure.Persistence.Repositories;

public interface ICofrinhoRepository
{
    Task<Cofrinho> AddAsync(Cofrinho meta);
    Task DeleteAsync(int id);
    Task<Cofrinho> GetByIdAsync(int id);
    Task<List<Cofrinho>> GetAllAsync();
    Task UpdateAsync(Cofrinho meta);
    Task<bool> AtualizarProgresso(int id, decimal valorAtual);
}
