using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWalletExchange
{
    public class ExchangeService
    {
        private const string BASE_URL = "https://min-api.cryptocompare.com";
        public List<CurrencyRate> GetConversionRate(Currency from, Currency[] to)
        {
            if (to == null || to.Length == 0)
            {
                throw new ArgumentException("to");
            }
            string url = $"{BASE_URL}/data/price?fsym={from}&tsyms={string.Join(",", to)}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            List<CurrencyRate> returnValues = new List<CurrencyRate>();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        string content = reader.ReadToEnd();
                        Dictionary<string,decimal> conversionRates = JsonConvert.DeserializeObject<Dictionary<string,decimal>>(content);
                        foreach(KeyValuePair<string,decimal> rate in conversionRates)
                        {
                            returnValues.Add(new CurrencyRate
                            {
                                Rate = rate.Value,
                                Currency=(Currency)Enum.Parse(typeof(Currency),rate.Key)
                            });

                        }
                        return returnValues;
                    }
                }
            }
        }
    }
}

