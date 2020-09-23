using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NCrontab;
using static bbt.enterprise_library.transaction_limit.StatusErrorCode;

namespace bbt.enterprise_library.transaction_limit
{
    public class BusinessService : IBusinessService
    {
        private readonly ILogger<DataService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDataHelperService _dataHelperService;
        private readonly IDataService _dataService;
        private readonly IBusinessHelperService _businessHelperService;

        public BusinessService(ILogger<DataService> logger, IConfiguration configuration, IDataHelperService dataHelperService, IDataService dataService, IBusinessHelperService businessHelperService)
        {
            _logger = logger;
            _configuration = configuration;
            _dataHelperService = dataHelperService;
            _dataService = dataService;
            _businessHelperService = businessHelperService;
        }

        public IEnumerable<LimitDefinition> GetDefinitions(string path, bool includeVariants)
        {
            if (!String.IsNullOrEmpty(path))
            {
                var definitions = _dataService.GetDefinitions(path, includeVariants);
                if (definitions.FirstOrDefault() == null) throw new PathNotDefinedException { };
                foreach (var definition in definitions)
                {
                    _businessHelperService.AutoLimitUpdateOnTheFly(definition);
                    definition.Availability.AvailabilityStatus = _businessHelperService.CheckAvailability(definition.Availability).AvailabilityStatus;
                }
                return definitions;
            }

            else throw new NullPathException { };

        }

        public SearchDefinitionsResponseDefinition SearchDefinitions(string query, int pageIndex, int pageSize, bool isActive)
        {
            if (pageIndex < 0) throw new PageIndexException { };
            if (pageSize <= 0 || pageSize > 1000) throw new PageSizeException { };
            if (String.IsNullOrEmpty(query)) throw new NullQueryException { };

            var definitions = _dataService.SearchDefinitions(query, pageIndex, pageSize, isActive);
            if (definitions == null) return definitions;
            foreach (var definition in definitions.LimitDefinitions)
            {
                _businessHelperService.AutoLimitUpdateOnTheFly(definition);
                definition.Availability.AvailabilityStatus = _businessHelperService.CheckAvailability(definition.Availability).AvailabilityStatus;
            }
            return definitions;

        }

        public LimitDefinition PatchDefinition(string path, string duration, PatchRequestDefinition data)
        {
            if (path == "" || path == null) throw new NullPathException();
            if (duration == "" || duration == null) throw new NullDurationException();
            if (_dataService.GetDefinitions(path, false).Where(x => x.Duration.Span == duration).FirstOrDefault() == null) throw new PathNotDefinedException();
            if (data.NewAmountUtilizedLimit < 0 && data.NewAmountUtilizedLimit != -1) throw new InvalidAmountException();
            if (data.NewTimerUtilizedLimit < 0 && data.NewTimerUtilizedLimit != -1) throw new InvalidTimerLimitException();

            _dataService.PatchLimit(path, duration, data);
            LimitDefinition result = _dataService.GetDefinitions(path, false).Where(x => x.Duration.Span == duration).FirstOrDefault();

            return result;

        }

        public IEnumerable<ExecuteResponseDefinition> ExecuteTransaction(ExecuteRequestDefinition data)
        {
            if (String.IsNullOrEmpty(data.Path)) throw new NullPathException();
            if (data.Path.Contains("*")) throw new ParentPathException();
            var definitions = GetDefinitions(data.Path, true).Where(x => x.isActive == true);
            definitions = _businessHelperService.AddAlsoLookPaths(definitions);
            if (definitions.Count() == 0) throw new PathNotDefinedException();
            var notUpdatedDefinitions = _dataService.GetDefinitions(data.Path, true).Where(x => x.isActive = true);
            var definition = definitions.Where(x => x.Path == data.Path).FirstOrDefault();
            if (definition == null && data.isExactPathRequired == true) throw new PathNotDefinedException();
            if (data.Amount < 0) throw new InvalidAmountException();
            if (!_businessHelperService.CheckCurrencyCode(data.CurrencyCode)) throw new InvalidCurrencyCodeException();

            Dictionary<string, decimal> amounts = new Dictionary<string, decimal>();
            List<ExecuteResponseDefinition> response = new List<ExecuteResponseDefinition>();

            foreach (var def in definitions)
            {
                if (def.AmountLimit.CurrencyCode != data.CurrencyCode)
                    amounts[def.Path + def.Duration.Span] = Decimal.Parse(_dataHelperService.CurrencyConverter(data.Amount, data.CurrencyCode, def.AmountLimit.CurrencyCode).ToString(("0.####")));
                else
                    amounts[def.Path + def.Duration.Span] = Decimal.Parse(data.Amount.ToString(("0.####")));
            }

            if (data.Type == ExecutionType.Utilize || data.Type == ExecutionType.Simulation)
            {
                foreach (var def in definitions)
                {
                    var availabilityStatus = _businessHelperService.CheckAvailability(def.Availability);
                    if (availabilityStatus.AvailabilityStatus == AvailabilityStatusType.NotInRange) throw new AvailabilityException(new AvailabilityRejectDefinition { FirstAvailableDate = _businessHelperService.FirstAvailableTime(def.Availability.Start), Reason = new AvailabilityExceptionDescriptionDefinition[] { new AvailabilityExceptionDescriptionDefinition { Description = "Not inside available time frame.", Language = "EN" }, new AvailabilityExceptionDescriptionDefinition { Description = "Tanımlı işlem yapılabilir zaman dilimi içerisinde değilsiniz.", Language = "TR" } } });
                    if (availabilityStatus.AvailabilityStatus == AvailabilityStatusType.Exception) throw new AvailabilityException(new AvailabilityRejectDefinition { FirstAvailableDate = _businessHelperService.FirstAvailableTime(def.Availability.Start, def.Availability.Exceptions.FirstOrDefault().Finish), Reason = availabilityStatus.Exceptions[0].Descriptions });
                    if (amounts[def.Path + def.Duration.Span] > def.AmountLimit.Remaining && def.AmountLimit.Limit != -1) throw new AmountRemainingLimitException(definitions.Where(x => x.Path == def.Path && x.Duration.Span == def.Duration.Span).FirstOrDefault());
                    if (def.TimerLimit.Remaining <= 0 && def.TimerLimit.Limit != -1) throw new TimerRemainingLimitException(definitions.Where(x => x.Path == def.Path && x.Duration.Span == def.Duration.Span).FirstOrDefault());
                    if (amounts[def.Path + def.Duration.Span] > def.TransactionLimit.MaximumLimit && def.TransactionLimit.MaximumLimit != -1) throw new MaximumLimitException(definitions.Where(x => x.Path == def.Path && x.Duration.Span == def.Duration.Span).FirstOrDefault());
                    if (amounts[def.Path + def.Duration.Span] < def.TransactionLimit.MinimumLimit && amounts[def.Path + def.Duration.Span] != 0 && def.TransactionLimit.MinimumLimit != -1) throw new MinimumLimitException(definitions.Where(x => x.Path == def.Path && x.Duration.Span == def.Duration.Span).FirstOrDefault());

                    var amountLimitDef = new AmountLimitDefinition();
                    amountLimitDef.Remaining = (def.AmountLimit.Limit == -1) ? 0 : def.AmountLimit.Remaining - amounts[def.Path + def.Duration.Span];
                    amountLimitDef.Utilized = def.AmountLimit.Utilized + amounts[def.Path + def.Duration.Span];
                    amountLimitDef.CurrencyCode = def.AmountLimit.CurrencyCode;
                    amountLimitDef.Limit = def.AmountLimit.Limit;
                    var timerLimitDef = new TimerLimitDefinition();
                    timerLimitDef.Limit = def.TimerLimit.Limit;
                    timerLimitDef.Remaining = (def.TimerLimit.Limit == -1) ? 0 : def.TimerLimit.Remaining - 1;
                    timerLimitDef.Utilized = def.TimerLimit.Utilized + 1;
                    timerLimitDef.MaxTimerLimit = def.TimerLimit.MaxTimerLimit;

                    response.Add(new ExecuteResponseDefinition { Path = def.Path, Duration = def.Duration.Span, AmountLimit = amountLimitDef, TransactionLimit = def.TransactionLimit, TimerLimit = timerLimitDef });
                }
            }
            else if (data.Type == ExecutionType.Reversal)
            {
                foreach (var def in definitions)
                {
                    if (def.AmountLimit.Limit < def.AmountLimit.Remaining + amounts[def.Path + def.Duration.Span] && def.AmountLimit.Limit != -1) throw new AmountRemainingLimitException(definitions.Where(x => x.Path == def.Path && x.Duration.Span == def.Duration.Span).FirstOrDefault());
                    if (def.TimerLimit.Limit < def.TimerLimit.Remaining + 1 && def.TimerLimit.Limit != -1) throw new TimerRemainingLimitException(definitions.Where(x => x.Path == def.Path && x.Duration.Span == def.Duration.Span).FirstOrDefault());

                    var amountLimitDef = new AmountLimitDefinition();
                    amountLimitDef.Remaining = (def.AmountLimit.Limit == -1) ? 0 : def.AmountLimit.Remaining + amounts[def.Path + def.Duration.Span];
                    amountLimitDef.Utilized = def.AmountLimit.Utilized - amounts[def.Path + def.Duration.Span];
                    amountLimitDef.CurrencyCode = def.AmountLimit.CurrencyCode;
                    amountLimitDef.Limit = def.AmountLimit.Limit;
                    var timerLimitDef = new TimerLimitDefinition();
                    timerLimitDef.Limit = def.TimerLimit.Limit;
                    timerLimitDef.Remaining = (def.TimerLimit.Limit == -1) ? 0 : def.TimerLimit.Remaining + 1;
                    timerLimitDef.Utilized = def.TimerLimit.Utilized - 1;
                    timerLimitDef.MaxTimerLimit = def.TimerLimit.MaxTimerLimit;

                    response.Add(new ExecuteResponseDefinition { Path = def.Path, Duration = def.Duration.Span, AmountLimit = amountLimitDef, TransactionLimit = def.TransactionLimit, TimerLimit = timerLimitDef });
                }
            }

            int starCount = -1;
            STATUSERRORCODE status;
            List<LimitDefinition> limitList = new List<LimitDefinition>();

            foreach (var def in definitions)
            {
                if (def.Duration.Renewal == RenewalType.elapsed && notUpdatedDefinitions.Where(x => x.Path == def.Path && x.Duration.Span == def.Duration.Span).FirstOrDefault().RenewedAt < CrontabSchedule.Parse(_businessHelperService.GetCron(def.Duration.Span)).GetNextOccurrences(DateTime.Now.AddYears(-1), DateTime.Now).Last())
                    _dataService.AutoUpdateLimits(def.Path, def.Duration.Span, def.RenewedAt);

                if (def.Path == data.Path) status = _dataService.ExecuteTransaction(def.Path, def.Duration.Span, amounts[def.Path + def.Duration.Span], data.Type, "fullPath");
                else status = _dataService.ExecuteTransaction(def.Path, def.Duration.Span, amounts[def.Path + def.Duration.Span], data.Type, "parentPath");

                if (status != STATUSERRORCODE.Success)
                {
                    foreach (var revereDef in limitList)
                    {
                        if (data.Type == ExecutionType.Reversal)
                        {
                            if (def.Path == data.Path) _dataService.ExecuteTransaction(def.Path, def.Duration.Span, amounts[def.Path + def.Duration.Span], ExecutionType.Utilize, "fullPath");
                            else _dataService.ExecuteTransaction(def.Path, def.Duration.Span, amounts[def.Path + def.Duration.Span], ExecutionType.Utilize, "parentPath");
                        }
                        else if (data.Type == ExecutionType.Utilize)
                        {
                            if (def.Path == data.Path) _dataService.ExecuteTransaction(def.Path, def.Duration.Span, amounts[def.Path + def.Duration.Span], ExecutionType.Reversal, "fullPath");
                            else _dataService.ExecuteTransaction(def.Path, def.Duration.Span, amounts[def.Path + def.Duration.Span], ExecutionType.Reversal, "parentPath");
                        }
                    }
                }

                if (status == STATUSERRORCODE.AmountLimitError) throw new AmountRemainingLimitException(def);
                else if (status == STATUSERRORCODE.TimerLimitError) throw new TimerRemainingLimitException(def);
                else if (status == STATUSERRORCODE.DataBaseError) throw new DatabaseException();

                limitList.Add(def);

                if (starCount == -1 || starCount > def.Path.Count(x => x == '*'))
                {
                    starCount = def.Path.Count(x => x == '*');
                    definition = def;
                }
            }

            return response;

        }

        public void UpdateDefinition(UpdateOrCreateLimitDefinitionRequestDefinition data)
        {
            if (String.IsNullOrEmpty(data.Path)) throw new NullPathException { };
            if (
                data.CurrencyCode is null &&
                data.TransactionMinimumLimit is null &&
                data.TransactionMaximumLimit is null &&
                data.AmountLimit is null &&
                data.MaxTimerLimit is null &&
                data.TimerLimit is null &&
                data.Span is null &&
                data.Renewal is null &&
                data.Availability is null &&
                data.DefaultAmountLimit is null &&
                data.DefaultTimerLimit is null &&
                data.isActive is null &&
                data.alsoLook is null) throw new UpdateDefinitionNeedDataException { };
            if (!_businessHelperService.CheckPathFormat(data.Path)) throw new PathFormatException { };
            var def = _dataService.GetDefinitions(data.Path, false).FirstOrDefault();
            if (
                (data.CurrencyCode is null || data.CurrencyCode == string.Empty ||
                data.TransactionMinimumLimit is null ||
                data.TransactionMaximumLimit is null ||
                data.AmountLimit is null ||
                data.TimerLimit is null ||
                data.Span is null || data.Span == string.Empty ||
                data.Renewal is null ||
                data.Availability is null ||
                data.Availability.Start is null ||
                data.Availability.Finish is null ||
                data.MaxAmountLimit is null ||
                data.MaxAmountLimitCurrencyCode is null || data.MaxAmountLimitCurrencyCode == string.Empty ||
                data.MaxTimerLimit is null ||
                data.DefaultAmountLimit is null ||
                data.DefaultTimerLimit is null ||
                data.isActive is null) &&
                def == null
                )
            {
                List<string> missingFields = new List<string>();

                if (data.CurrencyCode is null || data.CurrencyCode == string.Empty) missingFields.Add("currency-code");
                if (data.TransactionMinimumLimit is null) missingFields.Add("transaction-minimum-limit");
                if (data.TransactionMaximumLimit is null) missingFields.Add("transaction-maximum-limit");
                if (data.AmountLimit is null) missingFields.Add("amount-limit");
                if (data.TimerLimit is null) missingFields.Add("timer-limit");
                if (data.Span is null || data.Span == string.Empty) missingFields.Add("duration-span");
                if (data.Renewal is null) missingFields.Add("duration-renewal");
                if (data.MaxAmountLimit is null) missingFields.Add("max-amount-limit");
                if (data.MaxAmountLimitCurrencyCode is null || data.MaxAmountLimitCurrencyCode == string.Empty) missingFields.Add("max-amount-limit-currency-code");
                if (data.DefaultAmountLimit is null) missingFields.Add("default-amount-limit");
                if (data.DefaultTimerLimit is null) missingFields.Add("default-timer-limit");
                if (data.MaxTimerLimit is null) missingFields.Add("max-timer-limit");
                if (data.isActive is null) missingFields.Add("is-active");

                if (data.Availability is null) missingFields.Add("availability");
                else if (data.Availability.Start is null || data.Availability.Finish is null) missingFields.Add("availabilityStartFinish");

                throw new NewDefinitionNeedDataException(missingFields);
            }
            else if (def != null)
            {
                if (data.CurrencyCode is null || data.CurrencyCode == string.Empty) data.CurrencyCode = def.TransactionLimit.CurrencyCode;
                if (data.TransactionMinimumLimit is null) data.TransactionMinimumLimit = def.TransactionLimit.MinimumLimit;
                if (data.TransactionMaximumLimit is null) data.TransactionMaximumLimit = def.TransactionLimit.MaximumLimit;
                if (data.AmountLimit is null) data.AmountLimit = def.AmountLimit.Limit;
                if (data.TimerLimit is null) data.TimerLimit = def.TimerLimit.Limit;
                if (data.Span is null || data.Span == string.Empty) data.Span = def.Duration.Span;
                if (data.Renewal is null) data.Renewal = def.Duration.Renewal;
                if (data.Availability is null) data.Availability = def.Availability;
                if (data.MaxAmountLimit is null) data.MaxAmountLimit = def.MaxAmountLimit;
                if (data.MaxAmountLimitCurrencyCode is null || data.MaxAmountLimitCurrencyCode == string.Empty) data.MaxAmountLimitCurrencyCode = def.MaxAmountLimitCurrencyCode;
                if (data.MaxTimerLimit is null) data.MaxTimerLimit = def.TimerLimit.MaxTimerLimit;
                if (data.DefaultAmountLimit is null) data.DefaultAmountLimit = def.DefaultAmountLimit;
                if (data.DefaultTimerLimit is null) data.DefaultTimerLimit = def.DefaultTimerLimit;
                if (data.isActive is null) data.isActive = def.isActive;
                if (data.alsoLook is null) data.alsoLook = def.alsoLook;
            }

            if (data.Availability.Exceptions != null)
            {
                List<string> missingFields = new List<string>();
                foreach (var ex in data.Availability.Exceptions)
                {
                    if (ex.Start is null || ex.Finish is null)
                    {
                        missingFields.Add("availabilityExceptionStartFinish");
                    }
                    else
                    {
                        _businessHelperService.CheckCrons(new List<string> { ex.Start.Split('?')[0].Trim(), ex.Finish.Split('?')[0].Trim() });

                        if (Int32.Parse(ex.Start.Split('?')[1].Trim()) > Int32.Parse(ex.Finish.Split('?')[1].Trim()))
                        {
                            throw new StartFinishDateException();
                        }
                        else if (Int32.Parse(ex.Start.Split('?')[1].Trim()) == Int32.Parse(ex.Finish.Split('?')[1].Trim()))
                        {
                            if (CrontabSchedule.Parse(ex.Start.Split('?')[0].Trim()).GetNextOccurrence(new DateTime(DateTime.Today.Year, 1, 1)) > CrontabSchedule.Parse(ex.Finish.Split('?')[0].Trim()).GetNextOccurrence(new DateTime(DateTime.Today.Year, 1, 1)))
                                throw new StartFinishDateException();
                        }
                    }

                    if (ex.Descriptions != null)
                    {
                        foreach (var desc in ex.Descriptions)
                        {
                            if (desc.Description is null || desc.Language is null)
                            {
                                missingFields.Add("availabilityExceptionDescriptionsInfo");
                            }
                        }
                    }
                    else missingFields.Add("availabilityExceptionDescriptions");
                }

                if (missingFields.Count != 0) throw new NewDefinitionNeedDataException(missingFields);
            }

            if (data.alsoLook is null) data.alsoLook = "none";
            if (data.AmountLimit < 0 && data.AmountLimit != -1) throw new InvalidAmountLimitException { };
            if (data.TransactionMinimumLimit < 0) throw new InvalidMinimumLimitException { };
            if (data.TransactionMaximumLimit < 0) throw new InvalidMaximumLimitException { };
            if (data.TransactionMaximumLimit < data.TransactionMinimumLimit) throw new MinimumMaximumException { };
            if (data.TimerLimit < 0 && data.TimerLimit != -1) throw new InvalidTimerLimitException { };
            if (data.DefaultAmountLimit < 0 && data.DefaultAmountLimit != -1) throw new InvalidDefaultAmountLimitException { };
            if (data.DefaultTimerLimit < 0 && data.DefaultTimerLimit != -1) throw new InvalidDefaultTimerLimitException { };
            if (!_businessHelperService.CheckCurrencyCode(data.CurrencyCode)) throw new InvalidCurrencyCodeException { };
            if (!_businessHelperService.CheckSpan(data.Span)) throw new InvalidSpanException { };
            if (data.MaxAmountLimit < 0 && data.MaxAmountLimit != -1) throw new InvalidMaxAmountLimitException { };
            if (data.MaxTimerLimit < 0 && data.MaxTimerLimit != -1) throw new InvalidMaxTimerLimitException { };
            if (!_businessHelperService.CheckCurrencyCode(data.MaxAmountLimitCurrencyCode)) throw new InvalidMaxAmountLimitCurrencyCodeException { };
            if (data.CurrencyCode == data.MaxAmountLimitCurrencyCode)
            {
                if (data.MaxAmountLimit < data.AmountLimit && data.MaxAmountLimit != -1) throw new AmountLimitBiggerThanMaxException(null);
            }
            else
            {
                if (_dataHelperService.CurrencyConverter((decimal)data.MaxAmountLimit, data.MaxAmountLimitCurrencyCode, data.CurrencyCode) < data.AmountLimit && data.MaxAmountLimit != -1) throw new AmountLimitBiggerThanMaxException(null);
            }
            if (data.MaxTimerLimit < data.TimerLimit && data.MaxTimerLimit != -1) throw new TimerLimitBiggerThanMaxException(null);
            _businessHelperService.CheckAmountLimits((decimal)data.AmountLimit, data.CurrencyCode, data.Path); // amount limit control for path and parent paths.
            _businessHelperService.CheckTimerLimits((int)data.TimerLimit, data.Path); // timer limit control for path and parent paths.
            _businessHelperService.CheckCrons(new List<string> { data.Availability.Start.Split('?')[0].Trim(), data.Availability.Finish.Split('?')[0].Trim() }); // availability cron controls.

            decimal amountRemainingLimit = 0, amountUtilizedLimit = 0;
            int timerRemainingLimit = 0;

            if (def != null)
            {
                if (def.AmountLimit.CurrencyCode != data.CurrencyCode && def.AmountLimit.Utilized != 0)
                {
                    amountUtilizedLimit = _dataHelperService.CurrencyConverter(def.AmountLimit.Utilized, def.AmountLimit.CurrencyCode, data.CurrencyCode);
                }
                else amountUtilizedLimit = def.AmountLimit.Utilized;

                if (data.AmountLimit - amountUtilizedLimit >= 0 && data.AmountLimit != -1) amountRemainingLimit = (decimal)data.AmountLimit - amountUtilizedLimit;
                if (data.TimerLimit - def.TimerLimit.Utilized >= 0 && data.TimerLimit != -1) timerRemainingLimit = (int)data.TimerLimit - def.TimerLimit.Utilized;
            }

            _dataService.InsertDefinition(
                data.Path,
                data.CurrencyCode,
                data.TransactionMinimumLimit,
                data.TransactionMaximumLimit,
                data.AmountLimit,
                data.TimerLimit,
                data.Span,
                data.Renewal,
                data.Availability,
                amountRemainingLimit,
                amountUtilizedLimit,
                timerRemainingLimit,
                data.MaxAmountLimit,
                data.MaxAmountLimitCurrencyCode,
                data.DefaultAmountLimit,
                data.DefaultTimerLimit,
                data.MaxTimerLimit,
                data.isActive,
                data.alsoLook
                );

        }

        public List<ErrorDefinition> BatchUpdateDefinition(UpdateOrCreateLimitDefinitionRequestDefinition[] datas)
        {
            List<ErrorDefinition> ErrorList = new List<ErrorDefinition>();

            foreach (UpdateOrCreateLimitDefinitionRequestDefinition data in datas)
            {
                try
                {
                    UpdateDefinition(data);
                }
                catch (Exception e)
                {
                    if (e.Data["message"] != null && e.Data["statusCode"] != null) ErrorList.Add(new ErrorDefinition { Path = data.Path.ToString() + " " + data.Span.ToString(), Code = e.Data["statusCode"].ToString() });
                    else if (e.Data["statusCode"] != null) ErrorList.Add(new ErrorDefinition { Path = data.Path.ToString() + " " + data.Span.ToString(), Code = e.Data["statusCode"].ToString() });
                    else ErrorList.Add(new ErrorDefinition { Path = data.Path.ToString() + " " + data.Span.ToString(), Code = "500" });
                }
            }

            if (ErrorList.Count == datas.Count()) throw new BatchFailedException(ErrorList);
            else if (ErrorList.Count != 0) throw new BatchSomeFailedException(ErrorList);
            else return null;

        }

        public byte updateBatch(string query, string duration, int timerLimit, decimal amountLimit, string currencyCode, BatchUpdateLimitDefinition newLimits)
        {
            if (newLimits.AmountLimit < 0 && newLimits.AmountLimit != -1 && newLimits.AmountLimit != -2) throw new InvalidAmountLimitException { };
            if (newLimits.TimerLimit < 0 && newLimits.TimerLimit != -1 && newLimits.TimerLimit != -2) throw new InvalidTimerLimitException { };
            if (newLimits.MaxAmountLimit < 0 && newLimits.MaxAmountLimit != -1 && newLimits.MaxAmountLimit != -2) throw new InvalidMaxAmountLimitException { };
            if (newLimits.MaxTimerLimit < 0 && newLimits.MaxTimerLimit != -1 && newLimits.MaxTimerLimit != -2) throw new InvalidMaxTimerLimitException { };
            if (newLimits.DefaultAmountLimit < 0 && newLimits.DefaultAmountLimit != -1 && newLimits.DefaultAmountLimit != -2) throw new InvalidDefaultAmountLimitException { };
            if (newLimits.DefaultTimerLimit < 0 && newLimits.DefaultTimerLimit != -1 && newLimits.DefaultTimerLimit != -2) throw new InvalidDefaultTimerLimitException { };
            if (newLimits.AmountLimit > newLimits.MaxAmountLimit && newLimits.MaxAmountLimit != -1 && newLimits.MaxAmountLimit != -2) throw new AmountLimitBiggerThanMaxException(null);
            if (newLimits.TimerLimit > newLimits.MaxTimerLimit && newLimits.MaxTimerLimit != -1 && newLimits.MaxTimerLimit != -2) throw new TimerLimitBiggerThanMaxException(null);
            if (!_businessHelperService.CheckCurrencyCode(currencyCode)) throw new InvalidCurrencyCodeException { };

            byte result = _dataService.UpdateBatch(query, duration, timerLimit, amountLimit, currencyCode, newLimits);

            if (result == 2) throw new BatchUpdateFailedException { };
            else return result;
        }

    }
}
