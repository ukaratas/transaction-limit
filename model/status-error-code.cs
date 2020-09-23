namespace bbt.enterprise_library.transaction_limit
{
    public class StatusErrorCode
    {
        public enum STATUSERRORCODE
        {
            Success = 0,
            AmountLimitError = 1,
            TimerLimitError = 2,
            DataBaseError = 3
        }
    }
}
