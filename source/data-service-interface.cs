using System.Collections.Generic;
using static bbt.enterprise_library.transaction_limit.StatusErrorCode;

namespace bbt.enterprise_library.transaction_limit
{
    public interface IDataService
    {
        IEnumerable<LimitDefinition> GetDefinitions(string path, bool includeVariants);
        void InsertDefinition(string path, string currencyCode, decimal? transactionMinAmount, decimal? transactionMaxAmount, decimal? amountLimit, decimal? timerLimit, string duration, RenewalType? renewalType, AvailabilityDefinition availability, decimal? amountRemainingLimit, decimal amountUtilizedLimit, int timerRemainingLimit, decimal? maxAmountLimit, string maxAmountLimitCurrencyCode, decimal? defaultAmountLimit, int? defaultTimerLimit, int? maxTimerLimit, bool? isActive, string alsoLook);
        STATUSERRORCODE ExecuteTransaction(string path, string duration, decimal amount, ExecutionType type, string pathType);
        void AutoUpdateLimits(string path, string duration, System.DateTime renewedAt);
        SearchDefinitionsResponseDefinition SearchDefinitions(string query, int pageIndex, int pageSize, bool isActive);
        void PatchLimit(string path, string duration, PatchRequestDefinition data);
        byte UpdateBatch(string query, string duration, int timerLimit, decimal amountLimit, string currencyCode, BatchUpdateLimitDefinition newLimits);
    }
}



