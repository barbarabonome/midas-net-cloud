# 💰 Midas — Aplicativo de Controle Financeiro

O Midas é um app mobile que ajuda usuários a gerenciar suas finanças pessoais, controlar gastos recorrentes, definir metas e prever o saldo futuro com base no histórico de transações.

## 🚀 Funcionalidades
- Registro de ganhos e gastos
- Controle de assinaturas e contas recorrentes
- Metas financeiras com progresso visual
- Previsão de saldo no fim do mês (baseado em histórico e gastos médios)
- Leitura de extrato bancário
- Cadastro e autenticação dos usuários
- **Monitoramento da saúde da aplicação via Health Check**
- **HATEOAS (Hypermedia As The Engine Of Application State)**
- **Logging estruturado com Serilog**
- **Correlação de requisições para rastreamento**

## Requisitos funcionais
- O sistema deve permitir o cadastro de usuários com autenticação segura
- O usuário deve poder registrar transações financeiras (ganhos e gastos)
- O sistema deve permitir a categorização das transações
- O usuário deve poder cadastrar despesas recorrentes
- O sistema deve calcular e exibir o saldo previsto para o fim do mês
- O usuário deve poder criar metas financeiras com valor alvo e progresso
- O sistema deve gerar relatórios mensais com resumo de gastos
- O sistema deve realizar a leitura dos arquivos de extrato bancário enviados
- **O sistema deve fornecer endpoints de Health Check para monitoramento**
- **O sistema deve implementar HATEOAS para navegação entre recursos**

## Requisitos não funcionais
- A aplicação deve ser desenvolvida com arquitetura limpa (Clean Architecture)
- A API deve ser construída em ASP.NET Core com Entity Framework
- O sistema deve utilizar banco de dados
- O app mobile deve ser desenvolvido com React Native
- O sistema deve garantir segurança no armazenamento de dados sensíveis
- O sistema deve ser intuitivo para o usuário final
- O código deve seguir boas práticas de versionamento e testes automatizados
- **O sistema deve implementar logging estruturado para monitoramento**
- **O sistema deve suportar correlação de requisições para rastreamento distribuído**

## 🧱 Tecnologia
- .NET 9 com ASP.NET Core (API)
- Entity Framework Core 7.0
- Oracle Database
- Serilog (Logging estruturado)
- Swagger/OpenAPI (Documentação)
- xUnit, Moq, FluentAssertions (Testes)
- Clean Architecture

## 📁 Estrutura do Projeto
- **Controllers**: Camada de apresentação (API endpoints)
- **DTOs**: Objetos de transferência de dados
- **UseCase/Business**: Lógica de negócio e regras de domínio
- **Infrastructure**: Acesso a dados e persistência
- **Utils**: Utilitários e configurações
- **HealthChecks**: Verificação de saúde da aplicação
- **Filters**: Filtros e validações customizadas
- **Logging**: Configurações de logging estruturado

## 📊 Endpoints da API

### Recursos Principais
- **📊 Categoria**: `/api/categoria`
- **👤 Usuario**: `/api/usuario`
- **🪙 Cofrinho**: `/api/cofrinho`
- **💸 Gasto**: `/api/gasto`
- **💰 Receita**: `/api/receita`

### Health Check Endpoints
- **`GET /health`** - Verificação de saúde padrão do ASP.NET Core
- **`GET /api/health/api`** - Verifica a saúde geral da API
- **`GET /api/health/database`** - Verifica a conectividade com o banco de dados Oracle
- **`GET /api/health/complete`** - Verifica a saúde completa do sistema (API + Database)
  - Retorna: `Status`, `Timestamp`, `Environment`, `Uptime`, `ApiHealth`, `DatabaseHealth`
- **`GET /api/health/ready`** - Verifica a disponibilidade/readiness da aplicação (útil para Kubernetes)

### Exemplo de Resposta - Health Check Completo
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:45.123Z",
  "environment": "Production",
  "uptime": "5d 12h 30m 45s",
  "apiHealth": {
    "name": "API",
    "status": "Healthy",
    "description": "API está funcionando normalmente",
    "timestamp": "2024-01-15T10:30:45.123Z"
  },
  "databaseHealth": {
    "name": "Oracle Database",
    "status": "Healthy",
    "description": "Conexão com banco de dados estabelecida",
    "timestamp": "2024-01-15T10:30:45.123Z"
  }
}
```

## 🔍 Monitoramento da Aplicação

### Health Check
O sistema implementa verificações de saúde em múltiplos níveis:

1. **API Health**: Verifica o status geral da aplicação
2. **Database Health**: Verifica a conectividade com o Oracle
3. **Complete Health**: Fornece visão integrada de todos os componentes

### Logging Estruturado
- Todas as requisições são registradas com:
  - `Correlation ID` único para rastreamento
  - `Path` da requisição
  - `Method` HTTP utilizado
  - `Status Code` da resposta
  - Ambiente de execução
  - Timestamp de execução

### Monitoramento em Produção
Para monitorar a aplicação em produção:
```bash
# Verificar saúde da API
curl -X GET "http://localhost:5220/health"

# Verificar saúde completa
curl -X GET "http://localhost:5220/api/health/complete"

# Verificar readiness (para Kubernetes)
curl -X GET "http://localhost:5220/api/health/ready"
```

## 🧪 Testes

### Executar Todos os Testes
```bash
dotnet test
```

### Executar Testes Unitários
```bash
dotnet test Midas.API.Tests.Unit/Midas.API.Tests.Unit.csproj
```

### Executar Testes de Integração
```bash
dotnet test Midas.API.Tests.Integration/Midas.API.Tests.Integration.csproj
```

### Executar Testes com Cobertura
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Testes Implementados
- **Unit Tests**: Testes de negócio (Business/UseCase)
  - `CategoriaBusinessTests`
  - `GastoBusinessTests`
  - `ReceitaBusinessTests`
  - `UsuarioBusinessTests`
  
- **Integration Tests**: Testes de integração da API

### Frameworks de Teste
- **xUnit**: Framework de testes
- **Moq**: Biblioteca para mocks
- **FluentAssertions**: Asserções fluentes para melhor legibilidade
- **Coverlet**: Cobertura de código

## 📋 Instruções para Rodar o Projeto Localmente

### Pré-requisitos
- .NET 9 SDK ou superior
- Oracle Database (ou outro banco de dados Oracle compatível)
- Visual Studio 2022 ou VS Code com extensão C#

### Ambiente de Desenvolvimento

1. **Restaurar pacotes NuGet**
   ```bash
   dotnet restore
   ```

2. **Iniciar a Aplicação**
   ```bash
   dotnet run
   ```

3. **Acessar a Documentação**
   - **Swagger UI**: `http://localhost:5220/swagger` 
   - **Health Check**: `http://localhost:5220/health`
   - **Nota**: Em outros casos, as portas são exibidas no console quando a aplicação inicia

### Portas Padrão
- HTTP: `5000`
- HTTPS: `7018` (quando habilitado)

### Variáveis de Ambiente
- `PORT`: Define a porta de execução (padrão: 5220)
- `ASPNETCORE_ENVIRONMENT`: Define o ambiente

## 🔐 Segurança
- Autenticação segura de usuários
- Validação de entrada em todos os endpoints
- Logging de operações sensíveis
- Suporte a CORS configurável
- Proteção contra ciclos de referência em JSON

## 👥 Equipe
- Barbara Bonome Filipus - RM 560431 | 2TDSPR
- Vinicius Lira Ruggeri - RM 560593 | 2TDSPR
- Yasmin Pereira da Silva - RM 560039 | 2TDSPR

