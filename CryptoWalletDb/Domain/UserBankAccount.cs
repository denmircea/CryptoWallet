using System.Collections.Generic;

namespace CryptoWalletDb.Domain
{
    public class UserBankAccount

    {
        public UserBankAccount()
        {
            FromTransactions = new List<UserTransaction>();
            ToTransactions = new List<UserTransaction>();
        }

        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public User User { get; set; }
        public virtual ICollection<UserTransaction> FromTransactions { get; set; }
        public virtual ICollection<UserTransaction> ToTransactions { get; set; }
    }
}
