using CryptoWalletDb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class UserTransactionViewModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrencyRate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal RateNow { get; set; }


    }
}