using System;

namespace CryptoWalletDb.Domain
{
    public class UserTransaction
    {
        public int TransactionId { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrencyRate { get; set; }
        public DateTime TransactionDate { get; set; }
        public UserBankAccount FromAccount { get; set; }
        public UserBankAccount ToAccount { get; set; }

    }
}
