namespace QuickCurrencyPWA
{
    [Serializable]
    public class CurrencyExchanges
    {
        public List<CurrencyExchangeClass> CurrencyExchangeList { get; set; }

        public CurrencyExchanges()
        {
            CurrencyExchangeList = [];
        }

        public void Update(string fromCurrency, string toCurrency, decimal exchangeRate, DateTime updateTime)
        {
            if (fromCurrency == toCurrency)
            {
                throw new ArgumentException("Cannot update exchange rate for the same currency");
            }
            if (exchangeRate <= 0)
            {
                throw new ArgumentException("Exchange rate must be greater than 0");
            }
            if (fromCurrency != "SEK")
            {
                throw new ArgumentException("From currency must be SEK");
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

            if (fromCurrency != "SEK" && toCurrency != "SEK")
            {
                var fromRate1 = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == fromCurrency);
                var toRate2 = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == toCurrency);
                if (fromRate1 == null || toRate2 == null)
                {
                    throw new Exception("Exchange rate not found");
                }
                return toRate2.ExchangeRate / fromRate1.ExchangeRate;
            }
            if (fromCurrency == "SEK")
            {
                var toRate = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == toCurrency);
                return toRate == null ? throw new Exception("Exchange rate not found") : toRate.ExchangeRate;
            }
            if (toCurrency == "SEK")
            {
                var fromRate = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == fromCurrency);
                return fromRate == null ? throw new Exception("Exchange rate not found") : 1 / fromRate.ExchangeRate;
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
                var exchange = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == fromCurrency);
                return exchange == null ? throw new Exception("Exchange rate not found") : exchange.Inverted();
            }
            var toCurrencyExchange = CurrencyExchangeList.FirstOrDefault(x => x.ToCurrency == toCurrency);
            return toCurrencyExchange == null
                ? throw new Exception("Exchange rate not found")
                : new CurrencyExchangeClass
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                ExchangeRate = GetExchangeRate(fromCurrency, toCurrency),
                LastUpdated = toCurrencyExchange.LastUpdated
            };
        }
    }

    [Serializable]
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
                ToCurrency = this.FromCurrency,
                LastUpdated = this.LastUpdated
            };
        }
    }
}
