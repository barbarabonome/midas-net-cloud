using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Midas.API.Logging
{
    public static class LoggerExtensions
    {
        public static void LogEntityOperation(this ILogger logger, string operation, string entityType, int id)
        {
            logger.LogInformation("Operação {Operation} executada em {EntityType} com ID: {EntityId}", operation, entityType, id);
      }

        public static void LogEntityOperationError(this ILogger logger, Exception ex, string operation, string entityType, int id)
        {
            logger.LogError(ex, "Erro ao executar operação {Operation} em {EntityType} com ID: {EntityId}", operation, entityType, id);
        }

        public static void LogDataRetrieved(this ILogger logger, string entityType, int count)
 {
            logger.LogInformation("Total de {EntityType} recuperados: {Count}", entityType, count);
}

        public static void LogValidationWarning(this ILogger logger, string entityType, string property, string value)
        {
    logger.LogWarning("Validação falhou para {EntityType}. Propriedade: {Property}, Valor: {Value}", entityType, property, value);
  }
    }
}
