using CryptoWalletDb.Domain;
using CryptoWalletExchange;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class BankAccountViewModelTest
    {
        public List<BankAccountViewModel> Lista { get; set; }
        public decimal sumDeposits { get; set; }
    }
    public class BankAccountViewModel
    {

        public int AccountId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrencyRate { get; set; }
    }
}