using CryptoWallet.Models;
using CryptoWalletDb;
using CryptoWalletDb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using CryptoWalletExchange;

namespace CryptoWallet.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            /// Aici se ia lista de MyAccounts
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                /// gasesc user-ul conectat si ii caut conturile bancare
                ExchangeService echangeService = new ExchangeService();
                List<CurrencyRate> rates = echangeService.GetConversionRate(Currency.EUR, new Currency[] { Currency.EUR, Currency.BTC, Currency.GBP, Currency.USD, Currency.XRP });
                List<CurrencyRateViewModel> ratesViewModel = rates.Select(a => new CurrencyRateViewModel
                {
                    Currency = a.Currency.ToString(),
                    Rate = a.Rate
                }).ToList();
                User user = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);
                List<UserBankAccount> userBankAccounts = ctx.UserBankAccounts.Where(u => u.User.UserId == user.UserId).ToList();
                List<BankAccountViewModel> myBankAccountsViewModels = userBankAccounts.Select(a => new BankAccountViewModel
                {
                    AccountId = a.AccountId,
                    Amount = a.Amount,
                    Currency = a.Currency,
                }).ToList();


                foreach (var item in myBankAccountsViewModels)
                {
                    item.CurrencyRate = ratesViewModel.FirstOrDefault(s => s.Currency == item.Currency).Rate;

                }
                BankAccountViewModelTest ViewModel = new BankAccountViewModelTest();
                ViewModel.Lista = myBankAccountsViewModels;
                SetupIndexDeposits(ViewModel, user);
                return View(ViewModel);
            }
        }
        private void SetupIndexDeposits(BankAccountViewModelTest viewModel, User user)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                UserBankAccount bankAccount = ctx.UserBankAccounts.FirstOrDefault(x => x.UserId == user.UserId && x.Currency == "EUR");

                if (bankAccount != null)
                {
                    List<UserTransaction> transactions = ctx.UserTransactions.Where(x => x.FromAccountId == bankAccount.AccountId && x.ToAccountId == bankAccount.AccountId).ToList();
                    if (transactions != null)
                    {
                        foreach (var transaction in transactions)
                        {
                            viewModel.sumDeposits += transaction.Amount;
                        }
                    }
                }
            }
        }

        [HttpGet]
        public ActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit(DepositViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {
                    User user = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);
                    UserBankAccount eurAccount = ctx.UserBankAccounts.FirstOrDefault(a => a.Currency == "EUR" && a.UserId == user.UserId);

                    if (eurAccount == null)
                    {
                        eurAccount = new UserBankAccount
                        {
                            Currency = "EUR",
                            UserId = user.UserId,
                            Amount = 0
                        };
                        ctx.UserBankAccounts.Add(eurAccount);

                    }
                    ctx.SaveChanges();

                    eurAccount.Amount += viewModel.Amount;
                    UserTransaction userTransaction = new UserTransaction
                    {
                        Amount = viewModel.Amount,
                        CurrencyRate = 1,
                        FromAccountId = eurAccount.AccountId,
                        ToAccountId = eurAccount.AccountId,
                        TransactionDate = DateTime.Now,
                        FromAccount = eurAccount,
                        ToAccount = eurAccount
                    };
                    ctx.UserTransactions.Add(userTransaction);
                    ctx.SaveChanges();
                    return RedirectToAction("Index", "BankAccounts");

                }

            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult Send()
        {
            SendViewModel viewModel = new SendViewModel();
            SetupSendViewModel(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(SendViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {

                    User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                    User receiverEmail = ctx.Users.AsNoTracking().FirstOrDefault(x => x.Email == viewModel.ReceiverName);
                    if (receiverEmail == null)
                    {
                        ModelState.AddModelError("", "The email is invalid.");
                        SetupSendViewModel(viewModel);
                        return View(viewModel);
                    }
                    UserBankAccount fromAccount = ctx.UserBankAccounts.FirstOrDefault(a => a.AccountId.ToString() == viewModel.SenderAccountId);
                    UserBankAccount toAccount = ctx.UserBankAccounts.FirstOrDefault(a => a.Currency == fromAccount.Currency && a.UserId == receiverEmail.UserId);
                    if (fromAccount == null)
                    {
                        ModelState.AddModelError("", "The currency of the sender does not match.");
                        SetupSendViewModel(viewModel);
                        return View(viewModel);
                    }

                    if (toAccount == null)
                    {
                        ModelState.AddModelError("", "The currency of the receiver does not match.");
                        SetupSendViewModel(viewModel);
                        return View(viewModel);
                    }


                    if (fromAccount.Amount < viewModel.Amount)
                    {
                        ModelState.AddModelError("", "Insufficient funds.");
                        SetupSendViewModel(viewModel);
                        return View(viewModel);
                    }



                    fromAccount.Amount -= viewModel.Amount;
                    toAccount.Amount += viewModel.Amount;

                    UserTransaction transaction = new UserTransaction
                    {
                        Amount = viewModel.Amount,
                        FromAccountId = fromAccount.AccountId,
                        ToAccountId = toAccount.AccountId,
                        TransactionDate = DateTime.Now,
                        CurrencyRate = 1,
                        FromAccount = fromAccount,
                        ToAccount = toAccount
                    };

                    ctx.UserTransactions.Add(transaction);

                    ctx.SaveChanges();
                    return RedirectToAction("Index");


                }
            }
            return View(viewModel);
        }

        private void SetupSendViewModel(SendViewModel viewModel)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                List<UserBankAccount> currentUserAccounts = ctx.UserBankAccounts.Where(b => b.UserId == currentUser.UserId).ToList();

                List<BankAccountViewModel> accountsViewModels = currentUserAccounts.Select(a => new BankAccountViewModel
                {
                    AccountId = a.AccountId,
                    Amount = a.Amount,
                    Currency = a.Currency
                }).ToList();

                viewModel.SenderAccounts.AddRange(accountsViewModels.Select(a => new SelectListItem
                {
                    Value = a.AccountId.ToString(),
                    Text = a.Currency
                }));


            }
        }

        [HttpGet]
        public ActionResult Transactions()
        {

            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User user = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);
                ExchangeService echangeService = new ExchangeService();
                List<CurrencyRate> rates = echangeService.GetConversionRate(Currency.EUR, new Currency[] { Currency.BTC, Currency.USD, Currency.EUR, Currency.XRP, Currency.GBP });
                List<CurrencyRateViewModel> currentRates = rates.Select(a => new CurrencyRateViewModel
                {
                    Currency = a.Currency.ToString(),
                    Rate = a.Rate
                }).ToList();
                List<UserTransaction> userTransactions = ctx.UserTransactions
                    .Include(ut => ut.FromAccount).Include(ut => ut.FromAccount.User)
                    .Include(ut => ut.ToAccount).Include(ut => ut.ToAccount.User)
                    .Where(u => u.FromAccount.UserId == user.UserId || u.ToAccount.UserId == user.UserId).ToList();
                userTransactions.OrderBy(x => x.TransactionDate);
                List<UserTransactionViewModel> userTransactionsViewModel = userTransactions.Select(a => new UserTransactionViewModel
                {
                    Amount = a.Amount,
                    TransactionDate = a.TransactionDate,
                    CurrencyRate = a.CurrencyRate,
                    From = a.FromAccount.User.Email,
                    To = a.ToAccount.User.Email,
                    FromCurrency = a.FromAccount.Currency,
                    ToCurrency = a.ToAccount.Currency,
                    RateNow = currentRates.FirstOrDefault(x => x.Currency == a.ToAccount.Currency).Rate / (1 / currentRates.FirstOrDefault(x => x.Currency == a.FromAccount.Currency).Rate)
                }).ToList();
                return View(userTransactionsViewModel);
            }
        }

        [HttpGet]
        public ActionResult OpenAccount()
        {
            OpenAccountViewModel viewModel = new OpenAccountViewModel();
            SetupOpenAccount(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OpenAccount(OpenAccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {
                    User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);

                    List<UserBankAccount> currentUserAccounts = ctx.UserBankAccounts.Where(b => b.UserId == currentUser.UserId).ToList();
                    UserBankAccount bankAccount = currentUserAccounts.FirstOrDefault(x => x.Currency == viewModel.NewCurrency);
                    if (bankAccount != null)
                    {
                        ModelState.AddModelError("", "You have already an account in this currency.");
                        SetupOpenAccount(viewModel);
                        return View(viewModel);
                    }
                    bankAccount = new UserBankAccount
                    {
                        Currency = viewModel.NewCurrency,
                        UserId = currentUser.UserId
                    };
                    // bankAccount.User = currentUser;  ----- imi aduce userid gresit
                    bankAccount.UserId = currentUser.UserId;
                    ctx.UserBankAccounts.Add(bankAccount);

                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                SetupOpenAccount(viewModel);
                return View(viewModel);
            }
        }

        private void SetupOpenAccount(OpenAccountViewModel viewModel)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                viewModel.UnopenedAccounts = Enum.GetValues(typeof(Currency)).Cast<Currency>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = v.ToString()
                }).ToList();

            }
        }

        [HttpGet]
        public ActionResult Exchange()
        {
            ExchangeViewModel viewModel = new ExchangeViewModel();
            SetupExchange(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Exchange(ExchangeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
                {
                    User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);
                    UserBankAccount fromAccount = ctx.UserBankAccounts.FirstOrDefault(x => x.AccountId.ToString() == viewModel.CurrencyFrom);
                    UserBankAccount toAccount = ctx.UserBankAccounts.FirstOrDefault(x => x.AccountId.ToString() == viewModel.CurrencyTo);
                    if (toAccount == null)
                    {
                        ModelState.AddModelError("", "Invalid to currency.");
                        SetupExchange(viewModel);
                        return View(viewModel);
                    }
                    if (fromAccount == null)
                    {
                        ModelState.AddModelError("", "Invalid from currency.");
                        SetupExchange(viewModel);
                        return View(viewModel);
                    }
                    if (fromAccount.AccountId == toAccount.AccountId)
                    {
                        ModelState.AddModelError("", "Selected currencies are the same.");
                        SetupExchange(viewModel);
                        return View(viewModel);
                    }
                    if (fromAccount.Amount < viewModel.Amount)
                    {
                        ModelState.AddModelError("", "Insufficient funds.");
                        SetupExchange(viewModel);
                        return View(viewModel);
                    }
                    Currency currencyFrom = (Currency)Enum.Parse(typeof(Currency), fromAccount.Currency, true);
                    Currency currencyTo = (Currency)Enum.Parse(typeof(Currency), toAccount.Currency, true);
                    ExchangeService echangeService = new ExchangeService();
                    List<CurrencyRate> rates = echangeService.GetConversionRate(currencyFrom, new Currency[] { currencyTo });
                    viewModel.Rate = rates[0].Rate;
                    fromAccount.Amount -= viewModel.Amount;
                    toAccount.Amount += (viewModel.Amount * viewModel.Rate);
                    UserTransaction userTransaction = new UserTransaction
                    {
                        FromAccountId = fromAccount.AccountId,
                        ToAccountId = toAccount.AccountId,
                        FromAccount = fromAccount,
                        ToAccount = toAccount,
                        Amount = viewModel.Amount,
                        CurrencyRate = viewModel.Rate * (decimal)1.0000000,
                        TransactionDate = DateTime.Now
                    };
                    ctx.UserTransactions.Add(userTransaction);
                    ctx.SaveChanges();
                    return RedirectToAction("Transactions");
                }
            }
            else
            {
                SetupExchange(viewModel);
                return View(viewModel);
            }

        }

        private void SetupExchange(ExchangeViewModel viewModel)
        {
            using (CryptoWalletDbContext ctx = new CryptoWalletDbContext())
            {
                User currentUser = ctx.Users.AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);
                List<UserBankAccount> currentUserAccounts = ctx.UserBankAccounts.Where(b => b.UserId == currentUser.UserId).ToList();
                List<BankAccountViewModel> accountsViewModels = currentUserAccounts.Select(a => new BankAccountViewModel
                {
                    AccountId = a.AccountId,
                    Amount = a.Amount,
                    Currency = a.Currency
                }).ToList();

                viewModel.Accounts.AddRange(accountsViewModels.Select(a => new SelectListItem
                {
                    Value = a.AccountId.ToString(),
                    Text = a.Currency
                }));


            }
        }

    }
}

