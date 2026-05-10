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
    /// Testes unitários para a classe ReceitaBusiness
    /// Padrão AAA: Arrange, Act, Assert
    /// </summary>
    public class ReceitaBusinessTests
    {
        private readonly Mock<IReceitaRepository> _mockReceitaRepository;
        private readonly Mock<ILogger<ReceitaBusiness>> _mockLogger;
        private readonly ReceitaBusiness _receitaBusiness;

        public ReceitaBusinessTests()
        {
            _mockReceitaRepository = new Mock<IReceitaRepository>();
            _mockLogger = new Mock<ILogger<ReceitaBusiness>>();
            _receitaBusiness = new ReceitaBusiness(_mockReceitaRepository.Object, _mockLogger.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeReceitas_QuandoExistemReceitasNoBancoDeDados()
        {
            // Arrange
            var receitasEsperadas = ReceitaFixture.CriarListaReceitasValidas(3);
            _mockReceitaRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(receitasEsperadas);

            // Act
            var resultado = await _receitaBusiness.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.Should().BeEquivalentTo(receitasEsperadas);
            _mockReceitaRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoExistemReceitasNoBancoDeDados()
        {
            // Arrange
            var receitasEsperadas = new List<Receita>();
            _mockReceitaRepository.Setup(r => r.GetAllAsync())
              .ReturnsAsync(receitasEsperadas);

            // Act
            var resultado = await _receitaBusiness.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockReceitaRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRealizarLogDeInformacao_AoExecutarOperacao()
        {
            // Arrange
            var receitas = ReceitaFixture.CriarListaReceitasValidas(1);
            _mockReceitaRepository.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(receitas);

            // Act
            await _receitaBusiness.GetAllAsync();

            // Assert
            _mockLogger.Verify(
           l => l.Log(
           LogLevel.Information,
     It.IsAny<EventId>(),
      It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Business: Buscando todas as receitas")),
                  It.IsAny<Exception>(),
    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
           Times.Once);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_DeveRetornarReceita_QuandoReceitaExisteNoBancoDeDados()
        {
            // Arrange
            int receitaId = 1;
            var receitaEsperada = ReceitaFixture.CriarReceitaValida(receitaId);
            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
               .ReturnsAsync(receitaEsperada);

            // Act
            var resultado = await _receitaBusiness.GetByIdAsync(receitaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(receitaEsperada);
            resultado!.Id.Should().Be(receitaId);
            _mockReceitaRepository.Verify(r => r.GetByIdAsync(receitaId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoReceitaNaoExiste()
        {
            // Arrange
            int receitaId = 999;
            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
                    .ReturnsAsync((Receita?)null);

            // Act
            var resultado = await _receitaBusiness.GetByIdAsync(receitaId);

            // Assert
            resultado.Should().BeNull();
            _mockReceitaRepository.Verify(r => r.GetByIdAsync(receitaId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRealizarLogComIdDaBuscado_AoExecutarOperacao()
        {
            // Arrange
            int receitaId = 1;
            var receita = ReceitaFixture.CriarReceitaValida(receitaId);
            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
              .ReturnsAsync(receita);

            // Act
            await _receitaBusiness.GetByIdAsync(receitaId);

            // Assert
            _mockLogger.Verify(
                 l => l.Log(
                LogLevel.Information,
          It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"ID: {receitaId}")),
                  It.IsAny<Exception>(),
                  It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
              Times.Once);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_DeveRetornarReceitaCriada_QuandoDadosValidos()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOValido();
            var receitaEsperada = new Receita
            {
                Id = 1,
                UsuarioId = dto.UsuarioId,
                Titulo = dto.Titulo,
                Data = dto.Data,
                Valor = dto.Valor,
                Fixo = 'F'
            };

            _mockReceitaRepository.Setup(r => r.CreateAsync(It.IsAny<Receita>()))
               .ReturnsAsync(receitaEsperada);

            // Act
            var resultado = await _receitaBusiness.CreateAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Titulo.Should().Be(dto.Titulo);
            resultado.Valor.Should().Be(dto.Valor);
            _mockReceitaRepository.Verify(r => r.CreateAsync(It.IsAny<Receita>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoTituloEstaVazio()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOComTituloVazio();

            // Act
            var exception = await Record.ExceptionAsync(() => _receitaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Título é obrigatório");
            _mockReceitaRepository.Verify(r => r.CreateAsync(It.IsAny<Receita>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoTituloEhNulo()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOComTituloNulo();

            // Act
            var exception = await Record.ExceptionAsync(() => _receitaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Título é obrigatório");
            _mockReceitaRepository.Verify(r => r.CreateAsync(It.IsAny<Receita>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoTituloEhApenasEspacos()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOComTituloEspacos();

            // Act
            var exception = await Record.ExceptionAsync(() => _receitaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Título é obrigatório");
            _mockReceitaRepository.Verify(r => r.CreateAsync(It.IsAny<Receita>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoUsuarioIdEhInvalido()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOComUsuarioIdInvalido();

            // Act
            var exception = await Record.ExceptionAsync(() => _receitaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("UsuarioId é obrigatório");
            _mockReceitaRepository.Verify(r => r.CreateAsync(It.IsAny<Receita>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoValorEhZero()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOComValorInvalido();

            // Act
            var exception = await Record.ExceptionAsync(() => _receitaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Valor deve ser maior que zero");
            _mockReceitaRepository.Verify(r => r.CreateAsync(It.IsAny<Receita>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoValorEhNegativo()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOComValorNegativo();

            // Act
            var exception = await Record.ExceptionAsync(() => _receitaBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Valor deve ser maior que zero");
            _mockReceitaRepository.Verify(r => r.CreateAsync(It.IsAny<Receita>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveNormalizarValorFixo_QuandoDadosValidos()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOValido();
            dto.Fixo = 't'; // minúsculo para teste
            var receitaEsperada = new Receita
            {
                Id = 1,
                UsuarioId = dto.UsuarioId,
                Titulo = dto.Titulo,
                Data = dto.Data,
                Valor = dto.Valor,
                Fixo = 'T' // deve ser normalizado para maiúsculo
            };

            _mockReceitaRepository.Setup(r => r.CreateAsync(It.IsAny<Receita>()))
          .ReturnsAsync(receitaEsperada);

            // Act
            var resultado = await _receitaBusiness.CreateAsync(dto);

            // Assert
            resultado.Fixo.Should().Be('T');
        }

        [Fact]
        public async Task CreateAsync_DeveRealizarLogDeCriacao_AoExecutarOperacaoComSucesso()
        {
            // Arrange
            var dto = ReceitaFixture.CriarCreateReceitaDTOValido();
            var receitaEsperada = new Receita
            {
                Id = 1,
                UsuarioId = dto.UsuarioId,
                Titulo = dto.Titulo,
                Data = dto.Data,
                Valor = dto.Valor,
                Fixo = 'F'
            };

            _mockReceitaRepository.Setup(r => r.CreateAsync(It.IsAny<Receita>()))
             .ReturnsAsync(receitaEsperada);

            // Act
            await _receitaBusiness.CreateAsync(dto);

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
        public async Task UpdateAsync_DeveAtualizarReceita_QuandoReceitaExiste()
        {
            // Arrange
            int receitaId = 1;
            var receitaExistente = ReceitaFixture.CriarReceitaValida(receitaId);
            var receitaParaAtualizar = new Receita
            {
                Id = receitaId,
                UsuarioId = 1,
                Titulo = "Novo Título",
                Data = DateTime.Now,
                Valor = 4000.00m,
                Fixo = 'F'
            };

            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
                       .ReturnsAsync(receitaExistente);
            _mockReceitaRepository.Setup(r => r.UpdateAsync(receitaParaAtualizar))
            .ReturnsAsync(receitaParaAtualizar);

            // Act
            await _receitaBusiness.UpdateAsync(receitaParaAtualizar);

            // Assert
            _mockReceitaRepository.Verify(r => r.GetByIdAsync(receitaId), Times.Once);
            _mockReceitaRepository.Verify(r => r.UpdateAsync(receitaParaAtualizar), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarKeyNotFoundException_QuandoReceitaNaoExiste()
        {
            // Arrange
            int receitaId = 999;
            var receita = new Receita
            {
                Id = receitaId,
                UsuarioId = 1,
                Titulo = "Título",
                Data = DateTime.Now,
                Valor = 500.00m,
                Fixo = 'F'
            };

            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
                 .ReturnsAsync((Receita?)null);

            // Act
            var exception = await Record.ExceptionAsync(() => _receitaBusiness.UpdateAsync(receita));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<KeyNotFoundException>();
            exception!.Message.Should().Contain($"ID {receitaId}");
            _mockReceitaRepository.Verify(r => r.UpdateAsync(It.IsAny<Receita>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveRealizarLogDeAtualizacao_AoExecutarComSucesso()
        {
            // Arrange
            int receitaId = 1;
            var receitaExistente = ReceitaFixture.CriarReceitaValida(receitaId);
            var receitaParaAtualizar = new Receita
            {
                Id = receitaId,
                UsuarioId = 1,
                Titulo = "Título Atualizado",
                Data = DateTime.Now,
                Valor = 3500.00m,
                Fixo = 'F'
            };

            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
     .ReturnsAsync(receitaExistente);
            _mockReceitaRepository.Setup(r => r.UpdateAsync(It.IsAny<Receita>()))
      .ReturnsAsync(receitaParaAtualizar);

            // Act
            await _receitaBusiness.UpdateAsync(receitaParaAtualizar);

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
        public async Task DeleteAsync_DeveDeletarReceita_QuandoReceitaExiste()
        {
            // Arrange
            int receitaId = 1;
            var receita = ReceitaFixture.CriarReceitaValida(receitaId);
            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
                 .ReturnsAsync(receita);
            _mockReceitaRepository.Setup(r => r.DeleteAsync(receitaId))
       .Returns(Task.CompletedTask);

            // Act
            await _receitaBusiness.DeleteAsync(receitaId);

            // Assert
            _mockReceitaRepository.Verify(r => r.GetByIdAsync(receitaId), Times.Once);
            _mockReceitaRepository.Verify(r => r.DeleteAsync(receitaId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveLancarKeyNotFoundException_QuandoReceitaNaoExiste()
        {
            // Arrange
            int receitaId = 999;
            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
           .ReturnsAsync((Receita?)null);

            // Act
            var exception = await Record.ExceptionAsync(() => _receitaBusiness.DeleteAsync(receitaId));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<KeyNotFoundException>();
            exception!.Message.Should().Contain($"ID {receitaId}");
            _mockReceitaRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeveRealizarLogDeDeleracao_AoExecutarComSucesso()
        {
            // Arrange
            int receitaId = 1;
            var receita = ReceitaFixture.CriarReceitaValida(receitaId);
            _mockReceitaRepository.Setup(r => r.GetByIdAsync(receitaId))
     .ReturnsAsync(receita);
            _mockReceitaRepository.Setup(r => r.DeleteAsync(receitaId))
         .Returns(Task.CompletedTask);

            // Act
            await _receitaBusiness.DeleteAsync(receitaId);

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
            var parameters = ReceitaFixture.CriarSearchParametersValidos();
            var receitasMock = ReceitaFixture.CriarListaReceitasValidas(2);
            var pagedResult = new PagedResult<Receita>
            {
                Data = receitasMock,
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockReceitaRepository.Setup(r => r.SearchAsync(parameters))
               .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _receitaBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(2);
            resultado.TotalRecords.Should().Be(2);
            resultado.CurrentPage.Should().Be(1);
            _mockReceitaRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarResultadoVazio_QuandoNenhumResultadoEncontrado()
        {
            // Arrange
            var parameters = ReceitaFixture.CriarSearchParametersValidos("ReceitaInexistente");
            var pagedResult = new PagedResult<Receita>
            {
                Data = new List<Receita>(),
                TotalRecords = 0,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 0,
                HasNext = false,
                HasPrevious = false
            };

            _mockReceitaRepository.Setup(r => r.SearchAsync(parameters))
     .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _receitaBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().BeEmpty();
            resultado.TotalRecords.Should().Be(0);
            _mockReceitaRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarReceitasPorFaixa_QuandoValoresMinMaxDefinidos()
        {
            // Arrange
            var parameters = ReceitaFixture.CriarSearchParametrosPorFaixa(1000m, 5000m);
            var receitasMock = ReceitaFixture.CriarListaReceitasValidas(2);
            var pagedResult = new PagedResult<Receita>
            {
                Data = receitasMock,
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockReceitaRepository.Setup(r => r.SearchAsync(parameters))
                  .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _receitaBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(2);
            _mockReceitaRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarReceitasPorPeriodo_QuandoDataInicioFimDefinidas()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(-30);
            var dataFim = DateTime.Now;
            var parameters = ReceitaFixture.CriarSearchParametrosPorPeriodo(dataInicio, dataFim);
            var receitasMock = ReceitaFixture.CriarListaReceitasValidas(2);
            var pagedResult = new PagedResult<Receita>
            {
                Data = receitasMock,
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockReceitaRepository.Setup(r => r.SearchAsync(parameters))
           .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _receitaBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(2);
            _mockReceitaRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarReceitasPorTipo_QuandoFixoDefinido()
        {
            // Arrange
            var parameters = ReceitaFixture.CriarSearchParametrosPorTipo('T');
            var receitasMock = new List<Receita> { ReceitaFixture.CriarReceitaFixa(1) };
            var pagedResult = new PagedResult<Receita>
            {
                Data = receitasMock,
                TotalRecords = 1,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockReceitaRepository.Setup(r => r.SearchAsync(parameters))
           .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _receitaBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(1);
            resultado.Data.First().Fixo.Should().Be('T');
            _mockReceitaRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRealizarLogDeBusca_AoExecutarOperacao()
        {
            // Arrange
            var parameters = ReceitaFixture.CriarSearchParametersValidos();
            var receitasMock = ReceitaFixture.CriarListaReceitasValidas(1);
            var pagedResult = new PagedResult<Receita>
            {
                Data = receitasMock,
                TotalRecords = 1,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockReceitaRepository.Setup(r => r.SearchAsync(parameters))
             .ReturnsAsync(pagedResult);

            // Act
            await _receitaBusiness.SearchAsync(parameters);

            // Assert
            _mockLogger.Verify(
            l => l.Log(
        LogLevel.Information,
           It.IsAny<EventId>(),
    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Business: Buscando receitas com filtros")),
                It.IsAny<Exception>(),
   It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
   Times.Once);
        }

        #endregion
    }
}
