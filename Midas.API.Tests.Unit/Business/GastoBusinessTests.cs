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
    /// Testes unitários para a classe GastoBusiness
    /// Padrão AAA: Arrange, Act, Assert
    /// </summary>
    public class GastoBusinessTests
    {
        private readonly Mock<IGastoRepository> _mockGastoRepository;
        private readonly Mock<ILogger<GastoBusiness>> _mockLogger;
        private readonly GastoBusiness _gastoBusiness;

        public GastoBusinessTests()
        {
            _mockGastoRepository = new Mock<IGastoRepository>();
            _mockLogger = new Mock<ILogger<GastoBusiness>>();
            _gastoBusiness = new GastoBusiness(_mockGastoRepository.Object, _mockLogger.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeGastos_QuandoExistemGastosNoBancoDeDados()
        {
            // Arrange
            var gastosEsperados = GastoFixture.CriarListaGastosValidos(3);
            _mockGastoRepository.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(gastosEsperados);

            // Act
            var resultado = await _gastoBusiness.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.Should().BeEquivalentTo(gastosEsperados);
            _mockGastoRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoExistemGastosNoBancoDeDados()
        {
            // Arrange
            var gastosEsperados = new List<Gasto>();
            _mockGastoRepository.Setup(r => r.GetAllAsync())
    .ReturnsAsync(gastosEsperados);

            // Act
            var resultado = await _gastoBusiness.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockGastoRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRealizarLogDeInformacao_AoExecutarOperacao()
        {
            // Arrange
            var gastos = GastoFixture.CriarListaGastosValidos(1);
            _mockGastoRepository.Setup(r => r.GetAllAsync())
              .ReturnsAsync(gastos);

            // Act
            await _gastoBusiness.GetAllAsync();

            // Assert
            _mockLogger.Verify(
                     l => l.Log(
        LogLevel.Information,
              It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Business: Buscando todos os gastos")),
                  It.IsAny<Exception>(),
         It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
              Times.Once);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_DeveRetornarGasto_QuandoGastoExisteNoBancoDeDados()
        {
            // Arrange
            int gastoId = 1;
            var gastoEsperado = GastoFixture.CriarGastoValido(gastoId);
            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
                   .ReturnsAsync(gastoEsperado);

            // Act
            var resultado = await _gastoBusiness.GetByIdAsync(gastoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(gastoEsperado);
            resultado!.Id.Should().Be(gastoId);
            _mockGastoRepository.Verify(r => r.GetByIdAsync(gastoId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoGastoNaoExiste()
        {
            // Arrange
            int gastoId = 999;
            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
                   .ReturnsAsync((Gasto?)null);

            // Act
            var resultado = await _gastoBusiness.GetByIdAsync(gastoId);

            // Assert
            resultado.Should().BeNull();
            _mockGastoRepository.Verify(r => r.GetByIdAsync(gastoId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRealizarLogComIdDoBuscado_AoExecutarOperacao()
        {
            // Arrange
            int gastoId = 1;
            var gasto = GastoFixture.CriarGastoValido(gastoId);
            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
            .ReturnsAsync(gasto);

            // Act
            await _gastoBusiness.GetByIdAsync(gastoId);

            // Assert
            _mockLogger.Verify(
       l => l.Log(
  LogLevel.Information,
     It.IsAny<EventId>(),
         It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"ID: {gastoId}")),
        It.IsAny<Exception>(),
      It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_DeveRetornarGastoCriado_QuandoDadosValidos()
        {
            // Arrange
            var dto = GastoFixture.CriarCreateGastoDTOValido();
            var gastoEsperado = new Gasto
            {
                Id = 1,
                UsuarioId = dto.UsuarioId,
                CategoriaId = dto.CategoriaId,
                Titulo = dto.Titulo,
                Data = dto.Data,
                Valor = dto.Valor,
                Fixo = 'F'
            };

            _mockGastoRepository.Setup(r => r.AddAsync(It.IsAny<Gasto>()))
                     .ReturnsAsync(gastoEsperado);

            // Act
            var resultado = await _gastoBusiness.CreateAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Titulo.Should().Be(dto.Titulo);
            resultado.Valor.Should().Be(dto.Valor);
            _mockGastoRepository.Verify(r => r.AddAsync(It.IsAny<Gasto>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoTituloEstaVazio()
        {
            // Arrange
            var dto = GastoFixture.CriarCreateGastoDTOComTituloVazio();

            // Act
            var exception = await Record.ExceptionAsync(() => _gastoBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Título é obrigatório");
            _mockGastoRepository.Verify(r => r.AddAsync(It.IsAny<Gasto>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoUsuarioIdEhInvalido()
        {
            // Arrange
            var dto = GastoFixture.CriarCreateGastoDTOComUsuarioIdInvalido();

            // Act
            var exception = await Record.ExceptionAsync(() => _gastoBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("UsuarioId é obrigatório");
            _mockGastoRepository.Verify(r => r.AddAsync(It.IsAny<Gasto>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoCategoriaIdEhInvalido()
        {
            // Arrange
            var dto = GastoFixture.CriarCreateGastoDTOComCategoriaIdInvalido();

            // Act
            var exception = await Record.ExceptionAsync(() => _gastoBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("CategoriaId é obrigatório");
            _mockGastoRepository.Verify(r => r.AddAsync(It.IsAny<Gasto>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoValorEhZero()
        {
            // Arrange
            var dto = GastoFixture.CriarCreateGastoDTOComValorInvalido();

            // Act
            var exception = await Record.ExceptionAsync(() => _gastoBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Valor deve ser maior que zero");
            _mockGastoRepository.Verify(r => r.AddAsync(It.IsAny<Gasto>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoValorEhNegativo()
        {
            // Arrange
            var dto = GastoFixture.CriarCreateGastoDTOComValorNegativo();

            // Act
            var exception = await Record.ExceptionAsync(() => _gastoBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Valor deve ser maior que zero");
            _mockGastoRepository.Verify(r => r.AddAsync(It.IsAny<Gasto>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveNormalizarValorFixo_QuandoDadosValidos()
        {
            // Arrange
            var dto = GastoFixture.CriarCreateGastoDTOValido();
            dto.Fixo = 't'; // minúsculo para teste
            var gastoEsperado = new Gasto
            {
                Id = 1,
                UsuarioId = dto.UsuarioId,
                CategoriaId = dto.CategoriaId,
                Titulo = dto.Titulo,
                Data = dto.Data,
                Valor = dto.Valor,
                Fixo = 'T' // deve ser normalizado para maiúsculo
            };

            _mockGastoRepository.Setup(r => r.AddAsync(It.IsAny<Gasto>()))
       .ReturnsAsync(gastoEsperado);

            // Act
            var resultado = await _gastoBusiness.CreateAsync(dto);

            // Assert
            resultado.Fixo.Should().Be('T');
        }

        [Fact]
        public async Task CreateAsync_DeveRealizarLogDeCriacao_AoExecutarOperacaoComSucesso()
        {
            // Arrange
            var dto = GastoFixture.CriarCreateGastoDTOValido();
            var gastoEsperado = new Gasto
            {
                Id = 1,
                UsuarioId = dto.UsuarioId,
                CategoriaId = dto.CategoriaId,
                Titulo = dto.Titulo,
                Data = dto.Data,
                Valor = dto.Valor,
                Fixo = 'F'
            };
            _mockGastoRepository.Setup(r => r.AddAsync(It.IsAny<Gasto>()))
            .ReturnsAsync(gastoEsperado);

            // Act
            await _gastoBusiness.CreateAsync(dto);

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
        public async Task UpdateAsync_DeveAtualizarGasto_QuandoGastoExiste()
        {
            // Arrange
            int gastoId = 1;
            var gastoExistente = GastoFixture.CriarGastoValido(gastoId);
            var gastoParaAtualizar = new Gasto
            {
                Id = gastoId,
                UsuarioId = 1,
                CategoriaId = 1,
                Titulo = "Novo Título",
                Data = DateTime.Now,
                Valor = 250.00m,
                Fixo = 'F'
            };

            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
            .ReturnsAsync(gastoExistente);
            _mockGastoRepository.Setup(r => r.UpdateAsync(gastoParaAtualizar))
                  .Returns(Task.CompletedTask);

            // Act
            await _gastoBusiness.UpdateAsync(gastoParaAtualizar);

            // Assert
            _mockGastoRepository.Verify(r => r.GetByIdAsync(gastoId), Times.Once);
            _mockGastoRepository.Verify(r => r.UpdateAsync(gastoParaAtualizar), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarKeyNotFoundException_QuandoGastoNaoExiste()
        {
            // Arrange
            int gastoId = 999;
            var gasto = new Gasto
            {
                Id = gastoId,
                UsuarioId = 1,
                CategoriaId = 1,
                Titulo = "Título",
                Data = DateTime.Now,
                Valor = 100.00m,
                Fixo = 'F'
            };
            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
           .ReturnsAsync((Gasto?)null);

            // Act
            var exception = await Record.ExceptionAsync(() => _gastoBusiness.UpdateAsync(gasto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<KeyNotFoundException>();
            exception!.Message.Should().Contain($"ID {gastoId}");
            _mockGastoRepository.Verify(r => r.UpdateAsync(It.IsAny<Gasto>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveRealizarLogDeAtualizacao_AoExecutarComSucesso()
        {
            // Arrange
            int gastoId = 1;
            var gastoExistente = GastoFixture.CriarGastoValido(gastoId);
            var gastoParaAtualizar = new Gasto
            {
                Id = gastoId,
                UsuarioId = 1,
                CategoriaId = 1,
                Titulo = "Título Atualizado",
                Data = DateTime.Now,
                Valor = 150.00m,
                Fixo = 'F'
            };

            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
             .ReturnsAsync(gastoExistente);
            _mockGastoRepository.Setup(r => r.UpdateAsync(It.IsAny<Gasto>()))
        .Returns(Task.CompletedTask);

            // Act
            await _gastoBusiness.UpdateAsync(gastoParaAtualizar);

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
        public async Task DeleteAsync_DeveDeletarGasto_QuandoGastoExiste()
        {
            // Arrange
            int gastoId = 1;
            var gasto = GastoFixture.CriarGastoValido(gastoId);
            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
              .ReturnsAsync(gasto);
            _mockGastoRepository.Setup(r => r.DeleteAsync(gastoId))
                  .Returns(Task.CompletedTask);

            // Act
            await _gastoBusiness.DeleteAsync(gastoId);

            // Assert
            _mockGastoRepository.Verify(r => r.GetByIdAsync(gastoId), Times.Once);
            _mockGastoRepository.Verify(r => r.DeleteAsync(gastoId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveLancarKeyNotFoundException_QuandoGastoNaoExiste()
        {
            // Arrange
            int gastoId = 999;
            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
             .ReturnsAsync((Gasto?)null);

            // Act
            var exception = await Record.ExceptionAsync(() => _gastoBusiness.DeleteAsync(gastoId));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<KeyNotFoundException>();
            exception!.Message.Should().Contain($"ID {gastoId}");
            _mockGastoRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeveRealizarLogDeDeleracao_AoExecutarComSucesso()
        {
            // Arrange
            int gastoId = 1;
            var gasto = GastoFixture.CriarGastoValido(gastoId);
            _mockGastoRepository.Setup(r => r.GetByIdAsync(gastoId))
            .ReturnsAsync(gasto);
            _mockGastoRepository.Setup(r => r.DeleteAsync(gastoId))
         .Returns(Task.CompletedTask);

            // Act
            await _gastoBusiness.DeleteAsync(gastoId);

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
            var parameters = GastoFixture.CriarSearchParametersValidos();
            var gastosMock = GastoFixture.CriarListaGastosValidos(2);
            var pagedResult = new PagedResult<Gasto>
            {
                Data = gastosMock,
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockGastoRepository.Setup(r => r.SearchAsync(parameters))
        .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _gastoBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(2);
            resultado.TotalRecords.Should().Be(2);
            resultado.CurrentPage.Should().Be(1);
            _mockGastoRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarResultadoVazio_QuandoNenhumResultadoEncontrado()
        {
            // Arrange
            var parameters = GastoFixture.CriarSearchParametersValidos("GastoInexistente");
            var pagedResult = new PagedResult<Gasto>
            {
                Data = new List<Gasto>(),
                TotalRecords = 0,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 0,
                HasNext = false,
                HasPrevious = false
            };

            _mockGastoRepository.Setup(r => r.SearchAsync(parameters))
  .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _gastoBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().BeEmpty();
            resultado.TotalRecords.Should().Be(0);
            _mockGastoRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarGastosPorFaixa_QuandoValoresMinMaxDefinidos()
        {
            // Arrange
            var parameters = GastoFixture.CriarSearchParametersValidos(valorMinimo: 100m, valorMaximo: 300m);
            var gastosMock = GastoFixture.CriarListaGastosValidos(2);
            var pagedResult = new PagedResult<Gasto>
            {
                Data = gastosMock,
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockGastoRepository.Setup(r => r.SearchAsync(parameters))
            .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _gastoBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(2);
            _mockGastoRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        #endregion
    }
}
