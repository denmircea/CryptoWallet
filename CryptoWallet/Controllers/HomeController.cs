using CryptoWallet.Models;
using CryptoWalletDb;
using CryptoWalletExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ExchangeService echangeService = new ExchangeService();
            List<CurrencyRate> rates = echangeService.GetConversionRate(Currency.EUR, new Currency[] { Currency.EUR, Currency.USD, Currency.BTC ,Currency.XRP, Currency.GBP });
            List<CurrencyRateViewModel> viewModel = rates.Select(a => new CurrencyRateViewModel
            {
                Currency = a.Currency.ToString(),
                Rate = a.Rate

            }).ToList();
            return View(viewModel);
        }
    }
}