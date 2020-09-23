using System;
using System.Collections.Generic;

namespace bbt.enterprise_library.transaction_limit
{
    public interface IBusinessHelperService
    {
        AvailabilityDefinition CheckAvailability(AvailabilityDefinition definition);
        bool CheckCurrencyCode(string currencyCode);
        string GetCron(string span);
        void CheckCrons(List<string> expression);
        void AutoLimitUpdateOnTheFly(LimitDefinition definition);
        bool CheckPathFormat(string path);
        Dictionary<string, DateTime> AvailabilityTimeFrame(string start, string finish);
        bool CheckSpan(string span);
        DateTime? FirstAvailableTime(string AvailabilityStart, string exceptionFinish = null);
        void CheckTimerLimits(int amount, string path);
        void CheckAmountLimits(decimal amount, string currencyCode, string path);
        IEnumerable<LimitDefinition> AddAlsoLookPaths(IEnumerable<LimitDefinition> definitions);
        LimitDefinition RequestToDefinition(UpdateOrCreateLimitDefinitionRequestDefinition request);
    }
}