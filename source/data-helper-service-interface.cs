using System.Data;

namespace bbt.enterprise_library.transaction_limit
{
    public interface IDataHelperService
    {
        public DataTable DecodePaths(string path, bool includeVariants);
        public decimal CurrencyConverter(decimal amount, string sourceCurrencyCode, string targetCurrencyCode);
    }
}