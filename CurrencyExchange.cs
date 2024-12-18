using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace QuickCurrencyPWA
{

    public class CurrencyExchanges
    {
        private List<CurrencyExchangeClass> CurrencyExchangeList { get; set; }
        public CurrencyExchanges()
        {
            CurrencyExchangeList = [];
        }

        public void Update(string fromCurrency,  string toCurrency, decimal exchangeRate, DateTime updateTime)
        { 
            if (fromCurrency == toCurrency)
            {
                throw new Exception("Cannot update exchange rate for the same currency");
            }
            if (exchangeRate <= 0)
            {
                throw new Exception("Exchange rate must be greater than 0");
            }
            if (fromCurrency != "SEK")
            {
                throw new Exception("From currency must be SEK");
            }
            var exchange = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == toCurrency);
            if (exchange == null)
            {
                CurrencyExchangeList.Add(new CurrencyExchangeClass
                {
                    FromCurrency = fromCurrency,
                    ToCurrency = toCurrency,
                    ExchangeRate = exchangeRate,
                    LastUpdated = updateTime
                });
            }
            else
            {
                exchange.ExchangeRate = exchangeRate;
                exchange.LastUpdated = updateTime;
            }
        }

        public decimal GetExchangeRate(string fromCurrency, string toCurrency)
        {
            if (fromCurrency == toCurrency)
            {
                return 1;
            }
            
            if (fromCurrency !=  "SEK" && toCurrency != "SEK")
            {
                CurrencyExchangeClass fromRate1 = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == fromCurrency);
                CurrencyExchangeClass toRate2 = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == toCurrency);
                if (fromRate1 == null || toRate2 == null)
                {
                    throw new Exception("Exchange rate not found");
                }
                return fromRate1.ExchangeRate * toRate2.ExchangeRate;
            }
            if(fromCurrency == "SEK")
            {
                CurrencyExchangeClass toRate = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == toCurrency);
                if (toRate == null)
                {
                    throw new Exception("Exchange rate not found");
                }
                return toRate.ExchangeRate;
            }
            if (toCurrency == "SEK")
            {
                CurrencyExchangeClass fromRate = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == fromCurrency);
                if (fromRate == null)
                {
                    throw new Exception("Exchange rate not found");
                }
                return 1 / fromRate.ExchangeRate;
            }
            return 0;
        }

        public bool IsStale()
        {
            if (CurrencyExchangeList.Count == 0)
            {
                return true;
            }
            foreach (var exchange in CurrencyExchangeList)
            {
                if (exchange.LastUpdated.AddHours(24) < DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }

        public CurrencyExchangeClass GetExchange(string fromCurrency, string toCurrency)
        {
            if (fromCurrency == "SEK")
            {
                return CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == toCurrency);
            }
            if (toCurrency == "SEK")
            {
                return CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == fromCurrency).Inverted();
            }
            return new CurrencyExchangeClass
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                ExchangeRate = GetExchangeRate(fromCurrency, toCurrency),
                LastUpdated = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == toCurrency).LastUpdated
            };
        }
    }

        public class CurrencyExchangeClass
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Inverts the CurrencyExchangeRate object. Use with CAUTION!
        /// </summary>
        public void Invert()
        {
            string temp = FromCurrency;
            FromCurrency = ToCurrency;
            ToCurrency = temp;
            ExchangeRate = 1 / ExchangeRate;
        }

        /// <summary>
        /// Returns an inverted copy of the current exchange rate
        /// </summary>
        /// <returns>This exchangerate inverted</returns>
        public CurrencyExchangeClass Inverted()
        {
            return new CurrencyExchangeClass
            {
                ExchangeRate = 1 / this.ExchangeRate,
                FromCurrency = this.ToCurrency,
                ToCurrency = this.FromCurrency
            };
        }
    }
    
}