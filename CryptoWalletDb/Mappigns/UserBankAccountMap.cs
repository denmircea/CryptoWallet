using CryptoWalletDb.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWalletDb.Mappigns
{
    public class UserBankAccountMap : EntityTypeConfiguration <UserBankAccount>
    {
        public UserBankAccountMap()
        {
            ToTable("USERSBANKACCOUNTS").HasKey(t => t.AccountId);
            Property(t => t.AccountId).HasColumnName("AccountId").IsRequired();
            Property(t => t.UserId).HasColumnName("UserId").IsRequired();
            Property(t => t.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3) ;
            Property(t => t.Amount).HasColumnName("Amount").HasPrecision(20,10).IsRequired();

            HasRequired(t => t.User)
                .WithMany(t => t.UserBankAccounts)
                .HasForeignKey(f => f.UserId);
        }
    }
}
