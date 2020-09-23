using System;
using NCrontab;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace bbt.enterprise_library.transaction_limit
{
    public class BusinessHelperService : IBusinessHelperService
    {
        public readonly IDataHelperService _dataHelperService;
        public readonly IDataService _dataService;
        public BusinessHelperService(IDataHelperService dataHelperService, IDataService dataService)
        {
            _dataHelperService = dataHelperService;
            _dataService = dataService;
        }

        public bool CheckPathFormat(string path)
        {
            if (path[0] == '/' || path[path.Length - 1] == '/' || path.Contains(" ") || path.Contains("//")) return false;
            else return true;
        }

        public void AutoLimitUpdateOnTheFly(LimitDefinition definition)
        {
            if (definition.Duration.Renewal == RenewalType.elapsed)// Periyodik yenilenebilir olup olmadığı kontrol ediliyor.
            {
                var lastExpectedRenewalDate = CrontabSchedule.Parse(GetCron(definition.Duration.Span)).GetNextOccurrences(DateTime.Now.AddYears(-1), DateTime.Now).Last();

                if (definition.RenewedAt < lastExpectedRenewalDate)// yenilenme zamanının gelip gelmediğini kontrol ediyoruz.
                {
                    definition.RenewedAt = lastExpectedRenewalDate;
                    definition.AmountLimit.Remaining = definition.AmountLimit.Limit;
                    definition.AmountLimit.Utilized = 0;
                    definition.TimerLimit.Remaining = definition.TimerLimit.Limit;
                    definition.TimerLimit.Utilized = 0;
                }
            }
        }
        public AvailabilityDefinition CheckAvailability(AvailabilityDefinition definition) //işlemin saat,periyod,tatil gibi uygunlukların kontrolü
        {
            definition.AvailabilityStatus = AvailabilityStatusType.NotCalculated;
            if (definition != null)
            {
                var startFinish = AvailabilityTimeFrame(definition.Start, definition.Finish);

                if (DateTime.Now >= startFinish["start"] && DateTime.Now <= startFinish["finish"])
                {
                    if (definition.Exceptions != null)
                    {
                        foreach (var exception in definition.Exceptions)
                        {
                            var exStartDateCron = exception.Start.Split('?')[0].Trim();
                            var exStartDateYear = int.Parse(exception.Start.Split('?')[1].Trim());
                            if (exStartDateYear > DateTime.Now.Year) continue;

                            var exFinishDateCron = exception.Finish.Split('?')[0].Trim();
                            var exFinishDateYear = int.Parse(exception.Finish.Split('?')[1].Trim());

                            CheckCrons(new List<string>() { exStartDateCron, exFinishDateCron });

                            var exStartDate = CrontabSchedule.Parse(exStartDateCron).GetNextOccurrence(new DateTime(exStartDateYear, 1, 1));
                            var exFinishDate = CrontabSchedule.Parse(exFinishDateCron).GetNextOccurrence(new DateTime(exFinishDateYear, 1, 1));

                            if (DateTime.Now >= exStartDate && DateTime.Now <= exFinishDate)
                            {
                                definition.AvailabilityStatus = AvailabilityStatusType.Exception;
                                definition.Exceptions[0] = exception;
                                break;
                            }
                            else
                            {
                                definition.AvailabilityStatus = AvailabilityStatusType.Available;
                            }
                        }
                    }
                    else
                    {
                        definition.AvailabilityStatus = AvailabilityStatusType.Available;
                    }
                }
                else
                {
                    definition.AvailabilityStatus = AvailabilityStatusType.NotInRange;
                }
            }
            return definition;
        }

        public Dictionary<string, DateTime> AvailabilityTimeFrame(string start, string finish)
        {
            string StartDateCron, FinishDateCron;
            DateTime startDate, finishDate;

            if (start.Contains('?') && finish.Contains('?'))
            {
                StartDateCron = start.Split('?')[0].Trim();
                FinishDateCron = finish.Split('?')[0].Trim();
                startDate = CrontabSchedule.Parse(StartDateCron).GetNextOccurrence(new DateTime(int.Parse(start.Split('?')[1].Trim()), DateTime.Today.Month, DateTime.Today.Day));
                finishDate = CrontabSchedule.Parse(FinishDateCron).GetNextOccurrence(new DateTime(int.Parse(finish.Split('?')[1].Trim()), DateTime.Today.Month, DateTime.Today.Day));
            }
            else
            {
                startDate = CrontabSchedule.Parse(start).GetNextOccurrence(DateTime.Today);
                finishDate = CrontabSchedule.Parse(finish).GetNextOccurrence(DateTime.Today);
            }


            if (start.Split(" ")[0] == "*" && start.Split(" ")[1] == "*" && start != "* * * * *")//24 saat işlem için ve 00.00 yutmaması için
            {
                if (startDate.Minute == 1) startDate = startDate.AddMinutes(-1);
            }

            if (finish.Split(" ")[0] == "*" && finish.Split(" ")[1] == "*" && finish != "* * * * *")
            {
                finishDate = finishDate.AddDays(1);
                if (finishDate.Minute == 1) finishDate = finishDate.AddMinutes(-1);
            }

            return new Dictionary<string, DateTime>() { { "start", startDate }, { "finish", finishDate } };
        }

        public IEnumerable<LimitDefinition> AddAlsoLookPaths(IEnumerable<LimitDefinition> definitions)
        {
            List<LimitDefinition> limits = new List<LimitDefinition>();
            foreach (var def in definitions)
            {
                if (def.alsoLook != "none")
                {
                    if (_dataService.GetDefinitions(def.alsoLook, false).FirstOrDefault() == null) throw new AlsoLookPathNotDefined();
                    else limits.AddRange(_dataService.GetDefinitions(def.alsoLook, false).Where(x => x.isActive == true));
                }
            }
            if (limits.Count() != 0)
                return definitions.Concat(AddAlsoLookPaths(limits));
            else
                return definitions;
        }

        public bool CheckCurrencyCode(string currencyCode)
        {
            Regex currencyCodeRegex = new Regex(@"/^AED|AFN|ALL|AMD|ANG|AOA|ARS|AUD|AWG|AZN|BAM|BBD|BDT|BGN|BHD|BIF|BMD|BND|BOB|BRL|BSD|BTN|BWP|BYR|BZD|CAD|CDF|CHF|CLP|CNY|COP|CRC|CUC|CUP|CVE|CZK|DJF|DKK|DOP|DZD|EGP|ERN|ETB|EUR|FJD|FKP|GBP|GEL|GGP|GHS|GIP|GMD|GNF|GTQ|GYD|HKD|HNL|HRK|HTG|HUF|IDR|ILS|IMP|INR|IQD|IRR|ISK|JEP|JMD|JOD|JPY|KES|KGS|KHR|KMF|KPW|KRW|KWD|KYD|KZT|LAK|LBP|LKR|LRD|LSL|LYD|MAD|MDL|MGA|MKD|MMK|MNT|MOP|MRO|MUR|MVR|MWK|MXN|MYR|MZN|NAD|NGN|NIO|NOK|NPR|NZD|OMR|PAB|PEN|PGK|PHP|PKR|PLN|PYG|QAR|RON|RSD|RUB|RWF|SAR|SBD|SCR|SDG|SEK|SGD|SHP|SLL|SOS|SPL|SRD|STD|SVC|SYP|SZL|THB|TJS|TMT|TND|TOP|TRY|TTD|TVD|TWD|TZS|UAH|UGX|USD|UYU|UZS|VEF|VND|VUV|WST|XAF|XCD|XDR|XOF|XPF|YER|ZAR|ZMW|ZWD$/");
            Match match = currencyCodeRegex.Match(currencyCode);
            if (match.Success && currencyCode.Length == 3) return true;
            else return false;
        }

        public string GetCron(string span)
        {
            try
            {
                CheckCrons(new List<string> { span });
                return span;
            }
            catch (CronFormatException)
            {
                if (new string[] { "@daily", "@weekly", "@monthly", "@yearly" }.Contains(span))
                {
                    switch (span)
                    {
                        case "@daily":
                            return "0 0 * * *";
                        case "@weekly":
                            return "0 0 * * 0";
                        case "@monthly":
                            return "0 0 1 * *";
                        case "@yearly":
                            return "0 0 1 1 *";
                        default:
                            return null;
                    }
                }
                else throw new InvalidSpanException();
            }
        }

        public bool CheckSpan(string span)
        {
            try
            {
                CheckCrons(new List<string> { span });
                return true;
            }
            catch (CronFormatException)
            {
                if (new string[] { "@daily", "@weekly", "@monthly", "@yearly" }.Contains(span))
                {
                    return true;
                }
                else return false;
            }
        }

        public void CheckCrons(List<string> expression)
        {
            foreach (var exp in expression)
            {
                if (CrontabSchedule.TryParse(exp) == null)
                {
                    throw new CronFormatException();
                }
            }
        }

        public DateTime? FirstAvailableTime(string AvailabilityStart, string exceptionFinish)
        {
            DateTime startDate;
            string localStartCron = AvailabilityStart;

            if (exceptionFinish == null && localStartCron.Contains('?'))
            {
                startDate = CrontabSchedule.Parse(localStartCron.Split('?')[0].Trim()).GetNextOccurrence(new DateTime(int.Parse(localStartCron.Split('?')[1].Trim()), 1, 1));
            }
            else if (exceptionFinish == null)
            {
                startDate = CrontabSchedule.Parse(localStartCron).GetNextOccurrence(DateTime.Today);
            }
            else
            {
                string localExceptionCron = exceptionFinish;
                int localExceptionYear = DateTime.Now.Year;

                if (AvailabilityStart.Contains('?'))
                {
                    localStartCron = AvailabilityStart.Split('?')[0].Trim();
                }
                if (exceptionFinish.Contains('?'))
                {
                    localExceptionCron = exceptionFinish.Split('?')[0].Trim();
                    localExceptionYear = int.Parse(exceptionFinish.Split('?')[1].Trim());
                }

                startDate = CrontabSchedule.Parse(localStartCron).GetNextOccurrence(CrontabSchedule.Parse(localExceptionCron).GetNextOccurrence(new DateTime(localExceptionYear, 1, 1)));
            }

            if (localStartCron.Split(" ")[0] == "*" && localStartCron.Split(" ")[1] == "*" && localStartCron != "* * * * *") //24 saat işlem için ve 00.00 yutmaması için
            {
                if (startDate.Minute == 1) startDate = startDate.AddMinutes(-1);
            }

            return startDate;
        }

        public void CheckTimerLimits(int amount, string path)
        {
            IEnumerable<LimitDefinition> list = _dataService.GetDefinitions(path, true);

            foreach (var item in list)
            {
                if (item.Path == path) continue;
                if (item.TimerLimit.MaxTimerLimit != -1)
                    if (item.TimerLimit.MaxTimerLimit < amount) throw new TimerLimitBiggerThanMaxException(item);
            }
        }

        public void CheckAmountLimits(decimal amount, string currencyCode, string path)
        {
            IEnumerable<LimitDefinition> list = _dataService.GetDefinitions(path, true);

            foreach (var item in list)
            {
                if (item.Path == path) continue;
                if (item.MaxAmountLimit != -1)
                {
                    if (item.MaxAmountLimitCurrencyCode == currencyCode)
                    {
                        if (item.MaxAmountLimit < amount)
                            throw new AmountLimitBiggerThanMaxException(item);
                    }
                    else
                    {
                        if (_dataHelperService.CurrencyConverter((decimal)item.MaxAmountLimit, item.MaxAmountLimitCurrencyCode, currencyCode) < amount)
                            throw new AmountLimitBiggerThanMaxException(item);
                    }
                }
            }
        }

        public LimitDefinition RequestToDefinition(UpdateOrCreateLimitDefinitionRequestDefinition request)
        {
            LimitDefinition def = new LimitDefinition
            {
                Path = request.Path,
                DefaultAmountLimit = request.DefaultAmountLimit,
                DefaultTimerLimit = request.DefaultTimerLimit,
                MaxAmountLimit = request.MaxAmountLimit,
                MaxAmountLimitCurrencyCode = request.MaxAmountLimitCurrencyCode,
                TransactionLimit = new TransactionLimitDefinition { MinimumLimit = request.TransactionMinimumLimit ?? default(int), MaximumLimit = request.TransactionMaximumLimit ?? default(int), CurrencyCode = request.CurrencyCode },
                AmountLimit = new AmountLimitDefinition { Limit = request.AmountLimit ?? default(int), Utilized = 0, Remaining = request.AmountLimit ?? default(int), CurrencyCode = request.CurrencyCode },
                TimerLimit = new TimerLimitDefinition { Limit = request.TimerLimit ?? default(int), Utilized = 0, Remaining = request.TimerLimit ?? default(int), MaxTimerLimit = request.MaxTimerLimit ?? default(int) },
                Duration = new DurationDefinition { Span = request.Span, Renewal = request.Renewal ?? default(RenewalType) },
                isActive = request.isActive ?? default(bool),
                alsoLook = request.alsoLook
            };

            return def;
        }

    }
}
