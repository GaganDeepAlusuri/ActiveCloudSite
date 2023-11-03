using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CryptoPulse.Infrastructure.CryptoPulseHandler;
using CryptoPulse.Models;
using CryptoPulse.DataAccess;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MVCTemplate.Controllers
{
    
    public class HomeController : Controller
    {
        public ApplicationDbContext dbContext;
        private readonly AppSettings _appSettings;
        public const string SessionKeyName = "CoinsData";
        //List<Company> companies = new List<Company>();
        public HomeController(ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            dbContext = context;
            _appSettings = appSettings.Value;
        }

        public IActionResult HelloIndex()
        {
            ViewBag.Hello = _appSettings.Hello;
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }

        /****
         * The Coins action calls the GetCoins method that returns a list of Coins.
         * This list of Coins is passed to the Coins View.
        ****/
        public IActionResult Coins()
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            CryptoPulseHandler webHandler = new CryptoPulseHandler();
            List<Coin> coins = webHandler.GetCoins();
            string coinsData = JsonConvert.SerializeObject(coins);
            HttpContext.Session.SetString(SessionKeyName, coinsData);
            return View(coins);
        }

        public IActionResult Markets(int coinID)
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            CryptoPulseHandler webHandler = new CryptoPulseHandler();
            List<Market> markets = webHandler.GetMarkets(coinID);
            string marketsData = JsonConvert.SerializeObject(markets);
            HttpContext.Session.SetString(SessionKeyName, marketsData);
            var marketsViewModel = new MarketsViewModel
            {
                Coins = webHandler.GetCoins(),
                Markets = markets
            };
            return View("Markets", marketsViewModel);
        }

        public IActionResult Exchanges()
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            CryptoPulseHandler webHandler = new CryptoPulseHandler();
            List<Exchange> exchanges = webHandler.GetExchanges();
            string exchangesData = JsonConvert.SerializeObject(exchanges);
            HttpContext.Session.SetString(SessionKeyName, exchangesData);
            return View("Exchanges", exchanges);
        }


        /****
         * The Refresh action calls the ClearTables method to delete records from a or all tables.
         * Count of current records for each table is passed to the Refresh View.
        ****/
        public IActionResult Refresh(string tableToDel)
        {
            ClearTables(tableToDel);
            Dictionary<string, int> tableCount = new Dictionary<string, int>();
            tableCount.Add("Coins", dbContext.Coins.Count());
            tableCount.Add("Exchanges", dbContext.Exchanges.Count());
            tableCount.Add("Markets", dbContext.Markets.Count());
            return View(tableCount);
        }

        /****
         * Saves the Coins in database.
        ****/
        public IActionResult PopulateCoins()
        {
            string coinsData = HttpContext.Session.GetString(SessionKeyName);
            List<Coin> coins = null;
            if (coinsData != "")
            {
                coins = JsonConvert.DeserializeObject<List<Coin>>(coinsData);
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Enable IDENTITY_INSERT for the "Coins" table
                    dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Coins ON");

                    foreach (Coin coin in coins)
                    {
                        // Database will give PK constraint violation error when trying to insert a record with an existing PK.
                        // So add the coin only if it doesn't exist, check existence using the "Symbol" (PK).
                        if (dbContext.Coins.Where(c => c.Symbol.Equals(coin.Symbol)).Count() == 0)
                        {
                            dbContext.Coins.Add(coin);
                        }
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    ViewBag.dbSuccessComp = 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ViewBag.dbSuccessComp = 0;
                    // Handle the exception as needed (e.g., log the error).
                }
                finally
                {
                    // Disable IDENTITY_INSERT
                    dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Coins OFF");
                }
            }

            return View("Coins", coins);
        }

        /****
         * Saves the Markets in database.
        ****/
        public IActionResult PopulateMarkets()
        {
            string marketsData = HttpContext.Session.GetString(SessionKeyName);
            List<Market> markets = null;
            if (marketsData != "")
            {
                markets = JsonConvert.DeserializeObject<List<Market>>(marketsData);
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Enable IDENTITY_INSERT for the "Coins" table
                    dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Coins ON");

                    foreach (Market market in markets)
                    {
                        // Database will give PK constraint violation error when trying to insert a record with an existing PK.
                        // So add the coin only if it doesn't exist, check existence using the "Symbol" (PK).
                        if (dbContext.Markets.Where(c => c.MarketID.Equals(market.MarketID)).Count() == 0)
                        {
                            dbContext.Markets.Add(market);
                        }
                    }

                    dbContext.SaveChanges();
                    transaction.Commit();
                    ViewBag.dbSuccessComp = 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ViewBag.dbSuccessComp = 0;
                    // Handle the exception as needed (e.g., log the error).
                }
                finally
                {
                    // Disable IDENTITY_INSERT
                    dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Coins OFF");
                }
            }

            return View("Markets", markets);
        }

        /****
         * Deletes the records from tables.
        ****/
        public void ClearTables(string tableToDel)
        {
            if ("all".Equals(tableToDel))
            {
                dbContext.Coins.RemoveRange(dbContext.Coins);
                dbContext.Exchanges.RemoveRange(dbContext.Exchanges);
                dbContext.Markets.RemoveRange(dbContext.Markets);
            }
            else if ("Coins".Equals(tableToDel))
            {
                dbContext.Coins.RemoveRange(dbContext.Coins);
            }
            else if ("Exchanges".Equals(tableToDel))
            {
                dbContext.Exchanges.RemoveRange(dbContext.Exchanges);
            }
            else if ("Markets".Equals(tableToDel))
            {
                dbContext.Markets.RemoveRange(dbContext.Markets);
            }
            dbContext.SaveChanges();
        }
    }
}
