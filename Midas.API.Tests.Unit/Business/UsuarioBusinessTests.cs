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
    /// Testes unitários para a classe UsuarioBusiness
    /// Padrão AAA: Arrange, Act, Assert
    /// </summary>
    public class UsuarioBusinessTests
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<ILogger<UsuarioBusiness>> _mockLogger;
        private readonly UsuarioBusiness _usuarioBusiness;

        public UsuarioBusinessTests()
        {
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockLogger = new Mock<ILogger<UsuarioBusiness>>();
            _usuarioBusiness = new UsuarioBusiness(_mockUsuarioRepository.Object, _mockLogger.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaDeUsuarios_QuandoExistemUsuariosNoBancoDeDados()
        {
            // Arrange
            var usuariosEsperados = UsuarioFixture.CriarListaUsuariosValidos(3);
            _mockUsuarioRepository.Setup(r => r.GetAllAsync())
         .ReturnsAsync(usuariosEsperados);

            // Act
            var resultado = await _usuarioBusiness.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.Should().BeEquivalentTo(usuariosEsperados);
            _mockUsuarioRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoExistemUsuariosNoBancoDeDados()
        {
            // Arrange
            var usuariosEsperados = new List<Usuario>();
            _mockUsuarioRepository.Setup(r => r.GetAllAsync())
               .ReturnsAsync(usuariosEsperados);

            // Act
            var resultado = await _usuarioBusiness.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockUsuarioRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRealizarLogDeInformacao_AoExecutarOperacao()
        {
            // Arrange
            var usuarios = UsuarioFixture.CriarListaUsuariosValidos(1);
            _mockUsuarioRepository.Setup(r => r.GetAllAsync())
               .ReturnsAsync(usuarios);

            // Act
            await _usuarioBusiness.GetAllAsync();

            // Assert
            _mockLogger.Verify(
          l => l.Log(
       LogLevel.Information,
              It.IsAny<EventId>(),
      It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Business: Buscando todos os usuários")),
      It.IsAny<Exception>(),
             It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
              Times.Once);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_DeveRetornarUsuario_QuandoUsuarioExisteNoBancoDeDados()
        {
            // Arrange
            int usuarioId = 1;
            var usuarioEsperado = UsuarioFixture.CriarUsuarioValido(usuarioId);
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
                .ReturnsAsync(usuarioEsperado);

            // Act
            var resultado = await _usuarioBusiness.GetByIdAsync(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(usuarioEsperado);
            resultado!.Id.Should().Be(usuarioId);
            _mockUsuarioRepository.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoUsuarioNaoExiste()
        {
            // Arrange
            int usuarioId = 999;
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
              .ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _usuarioBusiness.GetByIdAsync(usuarioId);

            // Assert
            resultado.Should().BeNull();
            _mockUsuarioRepository.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRealizarLogComIdDoBuscado_AoExecutarOperacao()
        {
            // Arrange
            int usuarioId = 1;
            var usuario = UsuarioFixture.CriarUsuarioValido(usuarioId);
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
                 .ReturnsAsync(usuario);

            // Act
            await _usuarioBusiness.GetByIdAsync(usuarioId);

            // Assert
            _mockLogger.Verify(
               l => l.Log(
                  LogLevel.Information,
                It.IsAny<EventId>(),
             It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"ID: {usuarioId}")),
               It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                 Times.Once);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_DeveRetornarUsuarioCriado_QuandoDadosValidos()
        {
            // Arrange
            var dto = UsuarioFixture.CriarCreateUsuarioDTOValido();
            var usuarioEsperado = new Usuario
            {
                Id = 1,
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                DataCriacao = DateTime.Now
            };

            _mockUsuarioRepository.Setup(r => r.AddAsync(It.IsAny<Usuario>()))
         .ReturnsAsync(usuarioEsperado);
            _mockUsuarioRepository.Setup(r => r.EmailExistsAsync(dto.Email))
               .ReturnsAsync(false);

            // Act
            var resultado = await _usuarioBusiness.CreateAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Nome.Should().Be(dto.Nome);
            resultado.Email.Should().Be(dto.Email);
            resultado.Senha.Should().Be(dto.Senha);
            _mockUsuarioRepository.Verify(r => r.EmailExistsAsync(dto.Email), Times.Once);
            _mockUsuarioRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoNomeEstaVazio()
        {
            // Arrange
            var dto = UsuarioFixture.CriarCreateUsuarioDTOComNomeVazio();

            // Act
            var exception = await Record.ExceptionAsync(() => _usuarioBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Nome é obrigatório");
            _mockUsuarioRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoEmailEstaVazio()
        {
            // Arrange
            var dto = UsuarioFixture.CriarCreateUsuarioDTOComEmailVazio();

            // Act
            var exception = await Record.ExceptionAsync(() => _usuarioBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Email é obrigatório");
            _mockUsuarioRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarArgumentException_QuandoSenhaEstaVazia()
        {
            // Arrange
            var dto = UsuarioFixture.CriarCreateUsuarioDTOComSenhaVazia();

            // Act
            var exception = await Record.ExceptionAsync(() => _usuarioBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception!.Message.Should().Contain("Senha é obrigatória");
            _mockUsuarioRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarInvalidOperationException_QuandoEmailJaExiste()
        {
            // Arrange
            var dto = UsuarioFixture.CriarCreateUsuarioDTOValido();
            _mockUsuarioRepository.Setup(r => r.EmailExistsAsync(dto.Email))
           .ReturnsAsync(true);

            // Act
            var exception = await Record.ExceptionAsync(() => _usuarioBusiness.CreateAsync(dto));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<InvalidOperationException>();
            exception!.Message.Should().Contain("Email já está em uso");
            _mockUsuarioRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DeveRealizarLogDeCriacao_AoExecutarOperacaoComSucesso()
        {
            // Arrange
            var dto = UsuarioFixture.CriarCreateUsuarioDTOValido();
            var usuarioEsperado = new Usuario { Id = 1, Nome = dto.Nome, Email = dto.Email, Senha = dto.Senha };
            _mockUsuarioRepository.Setup(r => r.AddAsync(It.IsAny<Usuario>()))
           .ReturnsAsync(usuarioEsperado);
            _mockUsuarioRepository.Setup(r => r.EmailExistsAsync(dto.Email))
    .ReturnsAsync(false);

            // Act
            await _usuarioBusiness.CreateAsync(dto);

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
        public async Task UpdateAsync_DeveAtualizarUsuario_QuandoUsuarioExiste()
        {
            // Arrange
            int usuarioId = 1;
            var usuarioExistente = UsuarioFixture.CriarUsuarioValido(usuarioId);
            var usuarioParaAtualizar = new Usuario
            {
                Id = usuarioId,
                Nome = "Novo Nome",
                Email = "novoemail@example.com",
                Senha = "novaSenha",
                DataCriacao = usuarioExistente.DataCriacao
            };

            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
          .ReturnsAsync(usuarioExistente);
            _mockUsuarioRepository.Setup(r => r.UpdateAsync(usuarioParaAtualizar))
        .Returns(Task.CompletedTask);

            // Act
            await _usuarioBusiness.UpdateAsync(usuarioParaAtualizar);

            // Assert
            _mockUsuarioRepository.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
            _mockUsuarioRepository.Verify(r => r.UpdateAsync(usuarioParaAtualizar), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarKeyNotFoundException_QuandoUsuarioNaoExiste()
        {
            // Arrange
            int usuarioId = 999;
            var usuario = new Usuario { Id = usuarioId, Nome = "Nome", Email = "email@example.com", Senha = "senha" };
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
           .ReturnsAsync((Usuario?)null);

            // Act
            var exception = await Record.ExceptionAsync(() => _usuarioBusiness.UpdateAsync(usuario));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<KeyNotFoundException>();
            exception!.Message.Should().Contain($"ID {usuarioId}");
            _mockUsuarioRepository.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DeveRealizarLogDeAtualizacao_AoExecutarComSucesso()
        {
            // Arrange
            int usuarioId = 1;
            var usuarioExistente = UsuarioFixture.CriarUsuarioValido(usuarioId);
            var usuarioParaAtualizar = new Usuario
            {
                Id = usuarioId,
                Nome = "Nome Atualizado",
                Email = "email@example.com",
                Senha = "senha"
            };

            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
         .ReturnsAsync(usuarioExistente);
            _mockUsuarioRepository.Setup(r => r.UpdateAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);

            // Act
            await _usuarioBusiness.UpdateAsync(usuarioParaAtualizar);

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
        public async Task DeleteAsync_DeveDeletarUsuario_QuandoUsuarioExiste()
        {
            // Arrange
            int usuarioId = 1;
            var usuario = UsuarioFixture.CriarUsuarioValido(usuarioId);
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
               .ReturnsAsync(usuario);
            _mockUsuarioRepository.Setup(r => r.DeleteAsync(usuarioId))
           .Returns(Task.CompletedTask);

            // Act
            await _usuarioBusiness.DeleteAsync(usuarioId);

            // Assert
            _mockUsuarioRepository.Verify(r => r.GetByIdAsync(usuarioId), Times.Once);
            _mockUsuarioRepository.Verify(r => r.DeleteAsync(usuarioId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveLancarKeyNotFoundException_QuandoUsuarioNaoExiste()
        {
            // Arrange
            int usuarioId = 999;
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
              .ReturnsAsync((Usuario?)null);

            // Act
            var exception = await Record.ExceptionAsync(() => _usuarioBusiness.DeleteAsync(usuarioId));

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<KeyNotFoundException>();
            exception!.Message.Should().Contain($"ID {usuarioId}");
            _mockUsuarioRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_DeveRealizarLogDeDeleracao_AoExecutarComSucesso()
        {
            // Arrange
            int usuarioId = 1;
            var usuario = UsuarioFixture.CriarUsuarioValido(usuarioId);
            _mockUsuarioRepository.Setup(r => r.GetByIdAsync(usuarioId))
           .ReturnsAsync(usuario);
            _mockUsuarioRepository.Setup(r => r.DeleteAsync(usuarioId))
            .Returns(Task.CompletedTask);

            // Act
            await _usuarioBusiness.DeleteAsync(usuarioId);

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
            var parameters = UsuarioFixture.CriarSearchParametersValidos();
            var usuariosMock = UsuarioFixture.CriarListaUsuariosValidos(2);
            var pagedResult = new PagedResult<Usuario>
            {
                Data = usuariosMock,
                TotalRecords = 2,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };

            _mockUsuarioRepository.Setup(r => r.SearchAsync(parameters))
        .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _usuarioBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().HaveCount(2);
            resultado.TotalRecords.Should().Be(2);
            resultado.CurrentPage.Should().Be(1);
            _mockUsuarioRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_DeveRetornarResultadoVazio_QuandoNenhumResultadoEncontrado()
        {
            // Arrange
            var parameters = UsuarioFixture.CriarSearchParametersValidos("UsuarioInexistente");
            var pagedResult = new PagedResult<Usuario>
            {
                Data = new List<Usuario>(),
                TotalRecords = 0,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 0,
                HasNext = false,
                HasPrevious = false
            };

            _mockUsuarioRepository.Setup(r => r.SearchAsync(parameters))
                   .ReturnsAsync(pagedResult);

            // Act
            var resultado = await _usuarioBusiness.SearchAsync(parameters);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Data.Should().BeEmpty();
            resultado.TotalRecords.Should().Be(0);
            _mockUsuarioRepository.Verify(r => r.SearchAsync(parameters), Times.Once);
        }

        #endregion

        #region EmailExistsAsync

        [Fact]
        public async Task EmailExistsAsync_DeveRetornarTrue_QuandoEmailExiste()
        {
            // Arrange
            string email = "joao@example.com";
            _mockUsuarioRepository.Setup(r => r.EmailExistsAsync(email))
               .ReturnsAsync(true);

            // Act
            var resultado = await _usuarioBusiness.EmailExistsAsync(email);

            // Assert
            resultado.Should().BeTrue();
            _mockUsuarioRepository.Verify(r => r.EmailExistsAsync(email), Times.Once);
        }

        [Fact]
        public async Task EmailExistsAsync_DeveRetornarFalse_QuandoEmailNaoExiste()
        {
            // Arrange
            string email = "novo@example.com";
            _mockUsuarioRepository.Setup(r => r.EmailExistsAsync(email))
              .ReturnsAsync(false);

            // Act
            var resultado = await _usuarioBusiness.EmailExistsAsync(email);

            // Assert
            resultado.Should().BeFalse();
            _mockUsuarioRepository.Verify(r => r.EmailExistsAsync(email), Times.Once);
        }

        #endregion
    }
}
