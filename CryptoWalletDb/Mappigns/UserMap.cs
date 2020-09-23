using CryptoWalletDb.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWalletDb.Mappigns
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("USERS").HasKey(t => t.UserId);
            Property(t => t.UserId).HasColumnName("UserId").IsRequired();
            Property(t => t.Email).HasColumnName("Email").IsRequired().HasMaxLength(64);
            Property(t => t.Password).HasColumnName("Password").IsRequired().HasMaxLength(32);
            Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(32);
        }
    }
    
}
