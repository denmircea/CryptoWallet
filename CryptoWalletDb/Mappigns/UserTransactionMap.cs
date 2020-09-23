using CryptoWalletDb.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWalletDb.Mappigns
{
    public class UserTransactionMap : EntityTypeConfiguration <UserTransaction>
    {
        public UserTransactionMap()
        {
            ToTable("USERSTRANSACTIONS").HasKey(t => t.TransactionId);
            Property(t => t.TransactionId).HasColumnName("TransactionId").IsRequired();
            Property(t => t.FromAccountId).HasColumnName("FromAccountId").IsRequired();
            Property(t => t.ToAccountId).HasColumnName("ToAccountId").IsRequired();
            Property(t => t.Amount).HasColumnName("Amount").HasPrecision(20,10).IsRequired();
            Property(t => t.CurrencyRate).HasColumnName("CurrencyRate").HasPrecision(20,10).IsRequired();
            Property(t => t.TransactionDate).HasColumnName("TransactionDate").IsRequired();

            HasRequired(t => t.FromAccount)
                .WithMany(t => t.FromTransactions)
                .HasForeignKey(f => f.FromAccountId);
            HasRequired(t => t.ToAccount)
                .WithMany(t => t.ToTransactions)
                .HasForeignKey(f => f.ToAccountId);

        }
    }
}
