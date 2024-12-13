using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCurrencyPWA
{
    public class CurrencyExchangeClass
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        
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