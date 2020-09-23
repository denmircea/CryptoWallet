using CryptoWallet.Models;
using CryptoWalletDb;
using CryptoWalletDb.Domain;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace CryptoWallet.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "BankAccounts");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {

                    CryptoWalletDb.Domain.User user = ctx.Users.FirstOrDefault(u => u.Email == viewModel.Username && u.Password == viewModel.Password);
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(viewModel.Username, true);
                        return RedirectToAction("Index", "BankAccounts");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid credentials.");
                        return View(viewModel);
                    }
                }
            }
            else
            {
                return View(viewModel);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {

                    CryptoWalletDb.Domain.User user = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == viewModel.Email);
                    if (user == null)
                    {
                        user = new User
                        {
                            Email = viewModel.Email,
                            Name = viewModel.Name,
                            Password = viewModel.Password

                        };
                        ctx.Users.Add(user);
                        ctx.SaveChanges();
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email is already registered.");
                        return View(viewModel);
                    }
                }
            }
            else
            {
                return View(viewModel);
            }
        }
    }
}