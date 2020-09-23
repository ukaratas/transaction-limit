using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static bbt.enterprise_library.transaction_limit.StatusErrorCode;

namespace bbt.enterprise_library.transaction_limit
{
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDataHelperService _dataHelperService;

        public DataService(ILogger<DataService> logger, IConfiguration configuration, IDataHelperService dataHelperService)
        {
            _logger = logger;
            _configuration = configuration;
            _dataHelperService = dataHelperService;
        }

        public IEnumerable<LimitDefinition> GetDefinitions(string path, bool includeVariants)
        {
            List<LimitDefinition> returnValue = new List<LimitDefinition>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand command = new SqlCommand("get-limits-by-path"))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter parameter = command.Parameters.AddWithValue("@paths", _dataHelperService.DecodePaths(path, includeVariants));
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "path-array-type";
                    command.Connection = connection;
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var limitDefinition = new LimitDefinition
                            {
                                CreatedAt = reader.GetDateTime("created-at"),
                                RenewedAt = reader.GetDateTime("Renewed-at"),
                                Path = reader.GetString("path"),
                                MaxAmountLimit = reader.GetDecimal("max-amount-limit"),
                                MaxAmountLimitCurrencyCode = reader.GetString("max-amount-limit-currency-code"),
                                DefaultAmountLimit = reader.GetDecimal("default-amount-limit"),
                                DefaultTimerLimit = reader.GetInt32("default-timer-limit"),
                                TransactionLimit = new TransactionLimitDefinition { MinimumLimit = reader.GetDecimal("transaction-min-limit"), MaximumLimit = reader.GetDecimal("transaction-max-limit"), CurrencyCode = reader.GetString("currency-code") },
                                AmountLimit = new AmountLimitDefinition { Limit = reader.GetDecimal("amount-limit"), Utilized = reader.GetDecimal("amount-utilized-limit"), Remaining = reader.GetDecimal("amount-remaining-limit"), CurrencyCode = reader.GetString("currency-code") },
                                TimerLimit = new TimerLimitDefinition { Limit = reader.GetInt32("timer-limit"), Utilized = reader.GetInt32("timer-utilized-limit"), Remaining = reader.GetInt32("timer-remaining-limit"), MaxTimerLimit = reader.GetInt32("max-timer-limit") },
                                Duration = new DurationDefinition { Span = reader.GetString("duration"), Renewal = (RenewalType)reader.GetByte("renewal") },
                                isActive = reader.GetBoolean("is-active"),
                                alsoLook = reader.GetString("also-look")
                            };

                            if (!reader.IsDBNull("availability"))
                            {
                                limitDefinition.Availability = JsonConvert.DeserializeObject<AvailabilityDefinition>(reader.GetString("availability"));
                            }

                            returnValue.Add(limitDefinition);
                        }
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            return returnValue.ToArray();
        }

        public SearchDefinitionsResponseDefinition SearchDefinitions(string query, int pageIndex, int pageSize, bool isActive)
        {
            query = query.Replace('~', '%');
            List<LimitDefinition> limitDefinitions = new List<LimitDefinition>();
            SearchDefinitionsResponseDefinition returnValue = new SearchDefinitionsResponseDefinition();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {

                using (SqlCommand command = new SqlCommand("search-limits"))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@query", query);
                    command.Parameters.AddWithValue("@isActive", isActive);
                    command.Parameters.AddWithValue("@pageSize", pageSize);
                    command.Parameters.AddWithValue("@pageIndex", pageIndex);
                    command.Connection = connection;
                    connection.Open();


                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var limitDefinition = new LimitDefinition
                        {
                            CreatedAt = reader.GetDateTime("created-at"),
                            RenewedAt = reader.GetDateTime("Renewed-at"),
                            Path = reader.GetString("path"),
                            DefaultAmountLimit = reader.GetDecimal("default-amount-limit"),
                            DefaultTimerLimit = reader.GetInt32("default-timer-limit"),
                            MaxAmountLimit = reader.GetDecimal("max-amount-limit"),
                            MaxAmountLimitCurrencyCode = reader.GetString("max-amount-limit-currency-code"),
                            TransactionLimit = new TransactionLimitDefinition { MinimumLimit = reader.GetDecimal("transaction-min-limit"), MaximumLimit = reader.GetDecimal("transaction-max-limit"), CurrencyCode = reader.GetString("currency-code") },
                            AmountLimit = new AmountLimitDefinition { Limit = reader.GetDecimal("amount-limit"), Utilized = reader.GetDecimal("amount-utilized-limit"), Remaining = reader.GetDecimal("amount-remaining-limit"), CurrencyCode = reader.GetString("currency-code") },
                            TimerLimit = new TimerLimitDefinition { Limit = reader.GetInt32("timer-limit"), Utilized = reader.GetInt32("timer-utilized-limit"), Remaining = reader.GetInt32("timer-remaining-limit"), MaxTimerLimit = reader.GetInt32("max-timer-limit") },
                            Duration = new DurationDefinition { Span = reader.GetString("duration"), Renewal = (RenewalType)reader.GetByte("renewal") },
                            isActive = reader.GetBoolean("is-active"),
                            alsoLook = reader.GetString("also-look")
                        };

                        if (!reader.IsDBNull("availability"))
                        {
                            limitDefinition.Availability = JsonConvert.DeserializeObject<AvailabilityDefinition>(reader.GetString("availability"));
                        }

                        limitDefinitions.Add(limitDefinition);
                    }

                    reader.Close();
                    connection.Close();
                }
                if (limitDefinitions.Count > pageSize)
                {
                    limitDefinitions.RemoveAt(limitDefinitions.Count - 1);
                    returnValue.HasNextPage = true;
                }
                else returnValue.HasNextPage = false;
                returnValue.LimitDefinitions = limitDefinitions;
                return returnValue;
            }
        }

        public void InsertDefinition(string path, string currencyCode, decimal? transactionMinAmount, decimal? transactionMaxAmount, decimal? amountLimit, decimal? timerLimit, string duration, RenewalType? renewalType, AvailabilityDefinition availability, decimal? amountRemainingLimit, decimal amountUtilizedLimit, int timerRemainingLimit, decimal? maxAmountLimit, string maxAmountLimitCurrencyCode, decimal? defaultAmountLimit, int? defaultTimerLimit, int? maxTimerLimit, bool? isActive, string alsoLook)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand command = new SqlCommand("insert-update-limit-definition"))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@amountRemainingLimit", amountRemainingLimit);
                    command.Parameters.AddWithValue("@path", path);
                    command.Parameters.AddWithValue("@amountLimit", amountLimit);
                    command.Parameters.AddWithValue("@timerLimit", timerLimit);
                    command.Parameters.AddWithValue("@duration", duration);
                    command.Parameters.AddWithValue("@isActive", isActive);
                    command.Parameters.AddWithValue("@renewal", renewalType);
                    command.Parameters.AddWithValue("@transactionMin", transactionMinAmount);
                    command.Parameters.AddWithValue("@transactionMax", transactionMaxAmount);
                    command.Parameters.AddWithValue("@currencyCode", currencyCode);
                    command.Parameters.AddWithValue("@renewedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    command.Parameters.AddWithValue("@availability", JsonConvert.SerializeObject(availability));
                    command.Parameters.AddWithValue("@amountUtilizedLimit", amountUtilizedLimit);
                    command.Parameters.AddWithValue("@timerRemainingLimit", timerRemainingLimit);
                    command.Parameters.AddWithValue("@maxAmountLimit", maxAmountLimit);
                    command.Parameters.AddWithValue("@maxAmountLimitCurrencyCode", maxAmountLimitCurrencyCode);
                    command.Parameters.AddWithValue("@maxTimerLimit", maxTimerLimit);
                    command.Parameters.AddWithValue("@defaultTimerLimit", defaultTimerLimit);
                    command.Parameters.AddWithValue("@defaultAmountLimit", defaultAmountLimit);
                    command.Parameters.AddWithValue("@alsoLook", alsoLook);
                    command.Connection = connection;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public STATUSERRORCODE ExecuteTransaction(string path, string duration, decimal amount, ExecutionType type, string pathType)
        {
            string spName = "";
            Byte status = 0;
            if (pathType == "fullPath") spName = "update-limit";
            else if (pathType == "parentPath") spName = "update-limit-queued";
            Stopwatch timer = new Stopwatch();
            int retryInterval = _configuration.GetValue<int>("RetryTimerLimitMS");
            for (int attempted = 0; attempted < _configuration.GetValue<int>("RetryTimesLimit"); attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
                    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        using (SqlCommand command = new SqlCommand(spName))
                        {
                            if (spName == "update-limit")
                                command.Parameters.Add("@returnStatus", SqlDbType.TinyInt).Direction = ParameterDirection.ReturnValue;
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@amount", amount);
                            command.Parameters.AddWithValue("@path", path);
                            command.Parameters.AddWithValue("@duration", duration);
                            command.Parameters.AddWithValue("@type", type);
                            command.Connection = connection;
                            connection.Open();
                            command.ExecuteNonQuery();
                            if (spName == "update-limit")
                                status = Convert.ToByte(command.Parameters["@returnStatus"].Value);
                            connection.Close();

                        }
                    }
                    return (STATUSERRORCODE)status;
                }
                catch (Exception) { }
            }
            return STATUSERRORCODE.DataBaseError;

        }

        public void AutoUpdateLimits(string path, string duration, DateTime renewedAt)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand command = new SqlCommand("auto-update-limits"))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@path", path);
                    command.Parameters.AddWithValue("@duration", duration);
                    command.Parameters.AddWithValue("@renewedat", renewedAt);
                    command.Connection = connection;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void PatchLimit(string path, string duration, PatchRequestDefinition data)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand command = new SqlCommand("patch-limit"))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@path", path);
                    command.Parameters.AddWithValue("@duration", duration);
                    command.Parameters.AddWithValue("@amountutilizedlimit", data.NewAmountUtilizedLimit);
                    command.Parameters.AddWithValue("@timerutilizedlimit", data.NewTimerUtilizedLimit);
                    command.Connection = connection;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public byte UpdateBatch(string query, string duration, int timerLimit, decimal amountLimit, string currencyCode, BatchUpdateLimitDefinition newLimits)
        {
            query = query.Replace('~', '%');
            int rows;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {

                string cmdSelectString = " SELECT COUNT(1) FROM [dbo].[limit-definition]";
                cmdSelectString += " WHERE [path] like @pathQuery";
                if (duration != "-2") cmdSelectString += " AND [duration] like @duration";
                if (amountLimit != -2) cmdSelectString += " AND [amount-limit] = @oldAmountLimit";
                if (timerLimit != -2) cmdSelectString += " AND [timer-limit] = @oldTimerLimit";

                cmdSelectString += " AND [currency-code] = @CurrencyCode";

                connection.Open();
                SqlCommand cmd = new SqlCommand(cmdSelectString, connection);
                cmd.Parameters.AddWithValue("@pathQuery", query);
                if (duration != "-2") cmd.Parameters.AddWithValue("@duration", duration);
                if (amountLimit != -2) cmd.Parameters.AddWithValue("@oldAmountLimit", amountLimit);
                if (timerLimit != -2) cmd.Parameters.AddWithValue("@oldTimerLimit", timerLimit);
                cmd.Parameters.AddWithValue("@CurrencyCode", currencyCode);
                rows = (int)cmd.ExecuteScalar();
                connection.Close();
            }

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                int counter = 0;
                string cmdString;
                cmdString = "Update [dbo].[limit-definition] SET ";
                if (newLimits.AmountLimit != -2)
                {
                    cmdString += "[amount-limit] = @amountLimit";
                    cmdString += ", [amount-remaining-limit] = case when @amountLimit=-1 THEN 0 ELSE  @amountLimit - [amount-utilized-limit] END";
                    counter++;
                }
                if (newLimits.DefaultAmountLimit != -2)
                {
                    if (counter > 0) cmdString += ", [default-amount-limit] = @defaultAmountLimit";
                    else
                    {
                        cmdString += "[default-amount-limit] = @defaultAmountLimit";
                        counter++;
                    }
                }
                if (newLimits.MaxAmountLimit != -2)
                {
                    if (counter > 0) cmdString += ", [max-amount-limit] = @maxAmountLimit";
                    else
                    {
                        cmdString += "[max-amount-limit] = @maxAmountLimit";
                        counter++;
                    }
                }
                if (newLimits.TimerLimit != -2)
                {
                    if (counter > 0)
                    {
                        cmdString += ", [timer-limit] = @timerLimit";
                        cmdString += ", [timer-remaining-limit] = @timerLimit - [timer-utilized-limit]";
                    }
                    else
                    {
                        cmdString += "[timer-limit] = @timerLimit";
                        cmdString += ", [timer-remaining-limit] = @timerLimit - [timer-utilized-limit]";
                        counter++;
                    }
                }
                if (newLimits.DefaultTimerLimit != -2)
                {
                    if (counter > 0) cmdString += ", [default-timer-limit] = @defaultTimerLimit";
                    else
                    {
                        cmdString += "[default-timer-limit] = @defaultTimerLimit";
                        counter++;
                    }
                }
                if (newLimits.MaxTimerLimit != -2)
                {
                    if (counter > 0) cmdString += ", [max-timer-limit] = @maxTimerLimit";
                    else cmdString += "[max-timer-limit] = @maxTimerLimit";
                }
                cmdString += " Where [path] like @pathQuery";
                if (duration != "-2") cmdString += " AND [duration] like @duration";
                if (amountLimit != -2) cmdString += " AND [amount-limit] = @oldAmountLimit";
                if (timerLimit != -2) cmdString += " AND [timer-limit] = @oldTimerLimit";

                cmdString += " AND [currency-code] like @currencyCode";

                connection.Open();
                SqlCommand command = new SqlCommand(cmdString, connection);
                command.Parameters.AddWithValue("@pathQuery", query);

                if (duration != "-2") command.Parameters.AddWithValue("@duration", duration);
                if (currencyCode != "-2") command.Parameters.AddWithValue("@currencyCode", currencyCode);
                if (amountLimit != -2) command.Parameters.AddWithValue("@oldAmountLimit", amountLimit);
                if (timerLimit != -2) command.Parameters.AddWithValue("@oldTimerLimit", timerLimit);

                if (newLimits.AmountLimit != -2) command.Parameters.AddWithValue("@amountLimit", newLimits.AmountLimit);
                if (newLimits.DefaultAmountLimit != -2) command.Parameters.AddWithValue("@defaultAmountLimit", newLimits.DefaultAmountLimit);
                if (newLimits.MaxAmountLimit != -2) command.Parameters.AddWithValue("@maxAmountLimit", newLimits.MaxAmountLimit);
                if (newLimits.TimerLimit != -2) command.Parameters.AddWithValue("@timerLimit", newLimits.TimerLimit);
                if (newLimits.DefaultTimerLimit != -2) command.Parameters.AddWithValue("@defaultTimerLimit", newLimits.DefaultTimerLimit);
                if (newLimits.MaxTimerLimit != -2) command.Parameters.AddWithValue("@maxTimerLimit", newLimits.MaxTimerLimit);

                int result = command.ExecuteNonQuery();

                connection.Close();
                if (result == rows) return 0;
                else if (result < rows) return 1;
                else if (result == 0) return 2;
                else return 3;
            }

        }
    }
}