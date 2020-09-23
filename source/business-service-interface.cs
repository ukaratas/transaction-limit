using System.Collections.Generic;

namespace bbt.enterprise_library.transaction_limit
{
    public interface IBusinessService
    {
        IEnumerable<LimitDefinition> GetDefinitions(string path, bool includeVariants);
        void UpdateDefinition(UpdateOrCreateLimitDefinitionRequestDefinition data);
        List<ErrorDefinition> BatchUpdateDefinition(UpdateOrCreateLimitDefinitionRequestDefinition[] datas);
        IEnumerable<ExecuteResponseDefinition> ExecuteTransaction(ExecuteRequestDefinition data);
        SearchDefinitionsResponseDefinition SearchDefinitions(string query, int pageIndex, int pageSize, bool isActive);
        LimitDefinition PatchDefinition(string path, string duration, PatchRequestDefinition data);
        byte updateBatch(string query, string duration, int timerLimit, decimal amountLimit, string currencyCode, BatchUpdateLimitDefinition newLimits);
    }
}