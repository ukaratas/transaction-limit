using System.Data;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace bbt.enterprise_library.transaction_limit
{
    public class DataHelperService : IDataHelperService
    {
        public DataTable DecodePaths(string path, bool includeVariants)
        {
            DataTable pathList = new DataTable();
            pathList.Columns.Add("path", typeof(string));
            pathList.Rows.Add(path);

            if (includeVariants)
            {
                var variants = path.Split('/');
                for (int i = variants.Length; i >= 0 ; i--)
                {
                    string variantPath = variants[0];
                    for (int y = 0; y < variants.Length - 1; y++)
                    {
                        if (y < i)
                        {
                            variantPath = variantPath + "/" + variants[y + 1];
                        }
                        else
                        {
                            variantPath = variantPath + "/*";
                        }
                    }
                    if (variantPath != path) pathList.Rows.Add(variantPath);
                }
            }
            return pathList;
        }

        public decimal CurrencyConverter(decimal amount, string sourceCurrencyCode, string targetCurrencyCode)
        {
            try
            {
                if (sourceCurrencyCode != "TRY" && targetCurrencyCode != "TRY")
                {
                    WebRequest req = WebRequest.Create("https://ibgwapi.burgan.com.tr/ib/exchange/utilities/converter?amount= " + amount + "&sourceCurrencyCode=" + sourceCurrencyCode + "&targetCurrencyCode=TRY");
                    req.Credentials = CredentialCache.DefaultCredentials;
                    WebResponse res = req.GetResponse();
                    CurrencyConverterResponseDefinition resFromServer = new CurrencyConverterResponseDefinition();
                    using (Stream dataStream = res.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        resFromServer = JsonConvert.DeserializeObject<CurrencyConverterResponseDefinition>(reader.ReadToEnd());
                    }
                    res.Close();
                    amount = resFromServer.Amount;
                    sourceCurrencyCode = "TRY";
                }

                WebRequest request = WebRequest.Create("https://ibgwapi.burgan.com.tr/ib/exchange/utilities/converter?amount= " + amount + "&sourceCurrencyCode=" + sourceCurrencyCode + "&targetCurrencyCode=" + targetCurrencyCode);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                CurrencyConverterResponseDefinition responseFromServer = new CurrencyConverterResponseDefinition();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseFromServer = JsonConvert.DeserializeObject<CurrencyConverterResponseDefinition>(reader.ReadToEnd());
                }
                response.Close();
                return responseFromServer.Amount;
            }
            catch (System.Exception)
            {
                throw new CurrencyConverterException();
            }
        }
    }
}
