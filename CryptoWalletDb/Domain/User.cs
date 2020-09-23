using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWalletDb.Domain
{
    public  class User
    {
        public User()
        {
            UserBankAccounts = new List<UserBankAccount>();
        }
       
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public virtual ICollection<UserBankAccount>UserBankAccounts { get; set; }
    }
}
