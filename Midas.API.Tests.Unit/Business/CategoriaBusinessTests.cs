using Xunit;
using Moq;
using FluentAssertions;
using Midas.Infrastructure.Persistence.Entities;
using Midas.Infrastructure.Persistence.Repositories;
using Midas.API.Business.Implementations;
using Midas.API.DTOs;
using Microsoft.Extensions.Logging;
using Midas.API.Tests.Unit.Fixtures;

namespace Midas.API.Tests.Unit.Business
{
    /// <summary>
    /// Testes unitários para a classe CategoriaBusiness
    /// Padrão AAA: Arrange, Act, Assert
    /// </summary>
    public class CategoriaBusinessTests
    {
        private readonly Mock<ICategoriaRepository> _mockCategoriaRepository;
        private readonly Mock<ILogger<CategoriaBusiness>> _mockLogger;
        private readonly CategoriaBusiness _categoriaBusiness;

        public CategoriaBusinessTests()
        {
            _mockCategoriaRepository = new Mock<ICategoriaRepository>();
            _mockLogger = new Mock<ILogger<CategoriaBusiness>>();
            _categoriaBusiness = new CategoriaBusiness(_mockCategoriaRepository.Object, _mockLogger.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeCategorias_QuandoExistemCategoriasNoBancoDeDados()
        {
            // Arrange
            var categoriasEsperadas = CategoriaFixture.CriarListaCategoriasValidas(3);
            _mockCategoriaRepository.Setup(r => r.GetAllAsync())
              .ReturnsAsync(categoriasEsperadas);

            // Act
            var resultado = await _categoriaBusiness.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.Should().BeEquivalentTo(categoriasEsperadas);
            _mockCategoriaRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoExistemCategoriasNoBancoDeDados()
        {
            // Arrange
            var categoriasEsperadas = new List<Categoria>();
            _mockCategoriaRepository.Setup(r => r.GetAllAsync())
           .ReturnsAsync(categoriasEsperadas);

            // Act
            var resultado = await _categoriaBusiness.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockCategoriaRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRealizarLogDeInformacao_AoExecutarOperacao()
        {
            // Arrange
            var categorias = CategoriaFixture.CriarListaCategoriasValidas(1);
            _mockCategoriaRepository.Setup(r => r.GetAllAsync())
        .ReturnsAsync(categorias);

            // Act
            await _categoriaBusiness.GetAllAsync();

            // Assert
            _mockLogger.Verify(
 l => l.Log(
      LogLevel.Information,
         It.IsAny<EventId>(),
     It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Business: Buscando todas as categorias")),
               It.IsAny<Exception>(),
     It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    Times.Once);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_DeveRetornarCategoria_QuandoCategoriaExisteNoBancoDeDados()
        {
            // Arrange
            int categoriaId = 1;
            var categoriaEsperada = CategoriaFixture.CriarCategoriaValida(categoriaId);
            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
                       .ReturnsAsync(categoriaEsperada);

            // Act
            var resultado = await _categoriaBusiness.GetByIdAsync(categoriaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(categoriaEsperada);
            resultado!.Id.Should().Be(categoriaId);
            _mockCategoriaRepository.Verify(r => r.GetByIdAsync(categoriaId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoCategoriaJaNaoExiste()
        {
            // Arrange
            int categoriaId = 999;
            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
         .ReturnsAsync((Categoria?)null);

            // Act
            var resultado = await _categoriaBusiness.GetByIdAsync(categoriaId);

            // Assert
            resultado.Should().BeNull();
            _mockCategoriaRepository.Verify(r => r.GetByIdAsync(categoriaId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRealizarLogComIdDaBuscado_AoExecutarOperacao()
        {
            // Arrange
            int categoriaId = 1;
            var categoria = CategoriaFixture.CriarCategoriaValida(categoriaId);
            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
           .ReturnsAsync(categoria);

            // Act
            await _categoriaBusiness.GetByIdAsync(categoriaId);

            // Assert
            _mockLogger.Verify(
        l => l.Log(
            LogLevel.Information,
          It.IsAny<EventId>(),
     It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"ID: {categoriaId}")),
               It.IsAny<Exception>(),
     It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_DeveRetornarCategoriaCriada_QuandoDadosValidos()
        {
            // Arrange
            var dto = CategoriaFixture.CriarCreateCategoriaDTOValido();
            var categoriaEsperada = new Categoria
            {
                Id = 1,
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };

            _mockCategoriaRepository.Setup(r => r.AddAsync(It.IsAny<Categoria>()))
                 .ReturnsAsync(categoriaEsperada);

            // Act
            var resultado = await _categoriaBusiness.CreateAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Nome.Should().Be(dto.Nome);
            resultado.Descricao.Should().Be(dto.Descricao);
            _mockCategoriaRepository.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoNomeEstaVazio()
        {
            // Arrange
            var dto = CategoriaFixture.CriarCreateCategoriaDTOComNomeVazio();

            // Act
            var exception = await Record.ExceptionAsync(() => _categoriaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Nome é obrigatório");
            _mockCategoriaRepository.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoNomeEhNulo()
        {
            // Arrange
            var dto = CategoriaFixture.CriarCreateCategoriaDTOComNomeNulo();

            // Act
            var exception = await Record.ExceptionAsync(() => _categoriaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Nome é obrigatório");
            _mockCategoriaRepository.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoNomeEhApenasEspacos()
        {
            // Arrange
            var dto = CategoriaFixture.CriarCreateCategoriaDTOComNomeEspacos();

            // Act
            var exception = await Record.ExceptionAsync(() => _categoriaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Nome é obrigatório");
            _mockCategoriaRepository.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveCriarComDescricaoVazia_QuandoApenasNomeValido()
        {
            // Arrange
            var dto = CategoriaFixture.CriarCreateCategoriaDTOComDescricaoVazia();
            var categoriaEsperada = new Categoria
            {
                Id = 1,
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };

            _mockCategoriaRepository.Setup(r => r.AddAsync(It.IsAny<Categoria>()))
    .ReturnsAsync(categoriaEsperada);

            // Act
            var resultado = await _categoriaBusiness.CreateAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be(dto.Nome);
            resultado.Descricao.Should().Be(string.Empty);
            _mockCategoriaRepository.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveCriarComDescricaoNula_QuandoApenasNomeValido()
        {
            // Arrange
            var dto = CategoriaFixture.CriarCreateCategoriaDTOComDescricaoNula();
            var categoriaEsperada = new Categoria
            {
                Id = 1,
                Nome = dto.Nome,
                Descricao = null!
            };

            _mockCategoriaRepository.Setup(r => r.AddAsync(It.IsAny<Categoria>()))
                 .ReturnsAsync(categoriaEsperada);

            // Act
            var resultado = await _categoriaBusiness.CreateAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be(dto.Nome);
            _mockCategoriaRepository.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveRealizarLogDeCriacao_AoExecutarOperacaoComSucesso()
        {
            // Arrange
            var dto = CategoriaFixture.CriarCreateCategoriaDTOValido();
            var categoriaEsperada = new Categoria
            {
                Id = 1,
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };

            _mockCategoriaRepository.Setup(r => r.AddAsync(It.IsAny<Categoria>()))
    .ReturnsAsync(categoriaEsperada);

            // Act
            await _categoriaBusiness.CreateAsync(dto);

            // Assert
            _mockLogger.Verify(
      l => l.Log(
         LogLevel.Information,
          It.IsAny<EventId>(),
    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("criado com sucesso")),
     It.IsAny<Exception>(),
         It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
      Times.Once);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_DeveAtualizarCategoria_QuandoCategoriaExiste()
        {
            // Arrange
            int categoriaId = 1;
            var categoriaExistente = CategoriaFixture.CriarCategoriaValida(categoriaId);
            var categoriaParaAtualizar = new Categoria
            {
                Id = categoriaId,
                Nome = "Novo Nome",
                Descricao = "Nova descrição"
            };

            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
                 .ReturnsAsync(categoriaExistente);
            _mockCategoriaRepository.Setup(r => r.UpdateAsync(categoriaParaAtualizar))
          .Returns(Task.CompletedTask);

            // Act
            await _categoriaBusiness.UpdateAsync(categoriaParaAtualizar);

            // Assert
            _mockCategoriaRepository.Verify(r => r.GetByIdAsync(categoriaId), Times.Once);
            _mockCategoriaRepository.Verify(r => r.UpdateAsync(categoriaParaAtualizar), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarKeyNotFoundException_QuandoCategoriaJaNaoExiste()
        {
            // Arrange
            int categoriaId = 999;
            var categoria = new Categoria
            {
                Id = categoriaId,
                Nome = "Título",
                Descricao = "Descrição"
            };

            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
          .ReturnsAsync((Categoria?)null);

            // Act
            var exception = await Record.ExceptionAsync(() => _categoriaBusiness.UpdateAsync(categoria));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<KeyNotFoundException>();
            exception!.Message.Should().Contain($"ID {categoriaId}");
            _mockCategoriaRepository.Verify(r => r.UpdateAsync(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveRealizarLogDeAtualizacao_AoExecutarComSucesso()
        {
            // Arrange
            int categoriaId = 1;
            var categoriaExistente = CategoriaFixture.CriarCategoriaValida(categoriaId);
            var categoriaParaAtualizar = new Categoria
            {
                Id = categoriaId,
                Nome = "Nome Atualizado",
                Descricao = "Descrição atualizada"
            };

            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
           .ReturnsAsync(categoriaExistente);
            _mockCategoriaRepository.Setup(r => r.UpdateAsync(It.IsAny<Categoria>()))
             .Returns(Task.CompletedTask);

            // Act
            await _categoriaBusiness.UpdateAsync(categoriaParaAtualizar);

            // Assert
            _mockLogger.Verify(
             l => l.Log(
       LogLevel.Information,
        It.IsAny<EventId>(),
  It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("atualizado com sucesso")),
        It.IsAny<Exception>(),
       It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    Times.Once);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_DeveDeleterCategoria_QuandoCategoriaExiste()
        {
            // Arrange
            int categoriaId = 1;
            var categoria = CategoriaFixture.CriarCategoriaValida(categoriaId);
            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
        .ReturnsAsync(categoria);
            _mockCategoriaRepository.Setup(r => r.DeleteAsync(categoriaId))
            .Returns(Task.CompletedTask);

            // Act
            await _categoriaBusiness.DeleteAsync(categoriaId);

            // Assert
            _mockCategoriaRepository.Verify(r => r.GetByIdAsync(categoriaId), Times.Once);
            _mockCategoriaRepository.Verify(r => r.DeleteAsync(categoriaId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveLancarKeyNotFoundException_QuandoCategoriaJaNaoExiste()
        {
            // Arrange
            int categoriaId = 999;
            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
                       .ReturnsAsync((Categoria?)null);

            // Act
            var exception = await Record.ExceptionAsync(() => _categoriaBusiness.DeleteAsync(categoriaId));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<KeyNotFoundException>();
            exception!.Message.Should().Contain($"ID {categoriaId}");
            _mockCategoriaRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeveRealizarLogDeDeleracao_AoExecutarComSucesso()
        {
            // Arrange
            int categoriaId = 1;
            var categoria = CategoriaFixture.CriarCategoriaValida(categoriaId);
            _mockCategoriaRepository.Setup(r => r.GetByIdAsync(categoriaId))
            .ReturnsAsync(categoria);
            _mockCategoriaRepository.Setup(r => r.DeleteAsync(categoriaId))
           .Returns(Task.CompletedTask);

            // Act
            await _categoriaBusiness.DeleteAsync(categoriaId);

            // Assert
            _mockLogger.Verify(
          l => l.Log(
              LogLevel.Information,
         It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("deletado com sucesso")),
                       It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
          Times.Once);
        }

        #endregion

        #region SearchAsync

        [Fact]
        public async Task SearchAsync_DeveRetornarResultadosPaginados_QuandoParametrosValidos()
        {
            // Arrange
            var parameters = CategoriaFixture.CriarSearchParametersValidos();
            var categoriasMock = CategoriaFixture.CriarListaCategoriasValidas(2);
            var pagedResult = new PagedResult<Categoria>
            {
                Data = categoriasMock,
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockCategoriaRepository.Setup(r => r.SearchAsync(parameters))
         .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _categoriaBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(2);
            resultado.TotalRecords.Should().Be(2);
            resultado.CurrentPage.Should().Be(1);
            _mockCategoriaRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarResultadoVazio_QuandoNenhumResultadoEncontrado()
        {
            // Arrange
            var parameters = CategoriaFixture.CriarSearchParametersValidos("CategoriaInexistente");
            var pagedResult = new PagedResult<Categoria>
            {
                Data = new List<Categoria>(),
                TotalRecords = 0,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 0,
                HasNext = false,
                HasPrevious = false
            };

            _mockCategoriaRepository.Setup(r => r.SearchAsync(parameters))
            .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _categoriaBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().BeEmpty();
            resultado.TotalRecords.Should().Be(0);
            _mockCategoriaRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarCategoriasPorNome_QuandoNomeDefinido()
        {
            // Arrange
            var parameters = CategoriaFixture.CriarSearchParametersValidos(nome: "Alimentação");
            var categoriasMock = new List<Categoria> { CategoriaFixture.CriarCategoriaValida(1) };
            var pagedResult = new PagedResult<Categoria>
            {
                Data = categoriasMock,
                TotalRecords = 1,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockCategoriaRepository.Setup(r => r.SearchAsync(parameters))
                            .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _categoriaBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(1);
            resultado.Data.First().Nome.Should().Be("Alimentação");
            _mockCategoriaRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRealizarLogDeBusca_AoExecutarOperacao()
        {
            // Arrange
            var parameters = CategoriaFixture.CriarSearchParametersValidos();
            var categoriasMock = CategoriaFixture.CriarListaCategoriasValidas(1);
            var pagedResult = new PagedResult<Categoria>
            {
                Data = categoriasMock,
                TotalRecords = 1,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockCategoriaRepository.Setup(r => r.SearchAsync(parameters))
                          .ReturnsAsync(pagedResult);

            // Act
            await _categoriaBusiness.SearchAsync(parameters);

            // Assert
            _mockLogger.Verify(
         l => l.Log(
                 LogLevel.Information,
                 It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Business: Buscando categorias com filtros")),
            It.IsAny<Exception>(),
         It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
             Times.Once);
        }

        #endregion
    }
}
