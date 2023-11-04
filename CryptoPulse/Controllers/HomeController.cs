using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
        public const string SessionKeyName = "CoinsData";
        //List<Company> companies = new List<Company>();
        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
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

        public IActionResult WatchList()
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            CryptoPulseHandler webHandler = new CryptoPulseHandler();
            List<Coin> watchListcoins = GetWatchList();
            string coinsData = JsonConvert.SerializeObject(watchListcoins);
            HttpContext.Session.SetString(SessionKeyName, coinsData);
            return View("WatchList", watchListcoins);
        }

        public IActionResult AddToWatchList(string coinJson)
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            // Deserialize the JSON data back to a list of Coin objects
            CoinWatchList coin = JsonConvert.DeserializeObject<CoinWatchList>(coinJson);
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Enable IDENTITY_INSERT for the "Coins" table
                    dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Coins ON");

                    // Check if a coin with the same symbol already exists in the table
                    var existingCoin = dbContext.Coins.FirstOrDefault(c => c.Symbol.Equals(coin.Symbol));

                    if (existingCoin == null)
                    {
                        // Coin does not exist, so add it
                        // Create a new Coin object and set its properties
                        Coin newCoin = new Coin
                        {
                            Symbol = coin.Symbol,
                            Name = coin.Name,
                            Rank = coin.Rank,
                            PriceUSD = coin.PriceUSD, // Replace with the actual price
                            ID = coin.ID, // Replace with the actual ID value
                            MarketCapUSD = coin.MarketCapUSD, // Replace with the actual market cap
                            Volume24h = coin.Volume24h, // Replace with the actual 24-hour volume
                            SupplyCurrent = coin.SupplyCurrent, // Replace with the actual current supply
                            SupplyTotal = coin.SupplyTotal, // Replace with the actual total supply
                            SupplyMax = coin.SupplyMax, // Replace with the actual maximum supply
                            PercentChange1h = coin.PercentChange1h, // Replace with the actual 1-hour percentage change
                            PercentChange24h = coin.PercentChange24h, // Replace with the actual 24-hour percentage change
                            PercentChange7d = coin.PercentChange7d // Replace with the actual 7-day percentage change
                        };

                        // Add the newCoin object to the dbContext
                        dbContext.Coins.Add(newCoin);

                        // Save changes to the database
                        dbContext.SaveChanges();

                        dbContext.SaveChanges();
                        transaction.Commit();
                        ViewBag.dbSuccessComp = 1;
                    }
                    else
                    {
                        // Coin with the same symbol already exists, you can handle it as needed
                        // For example, update the existing record or skip the duplicate
                        transaction.Rollback();
                        ViewBag.dbSuccessComp = 0;
                        // Handle the duplicate coin here
                    }
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
            CryptoPulseHandler webHandler = new CryptoPulseHandler();
            List<Coin> coins = webHandler.GetCoins();
            return View("Coins", coins);
        }

        public IActionResult DeleteFromWatchList(int coinID)
        {
            // Set ViewBag variable first
            ViewBag.dbSuccessComp = 0;
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Find the coin with the given ID in the table
                    var coinToDelete = dbContext.Coins.FirstOrDefault(c => c.ID == coinID);

                    if (coinToDelete != null)
                    {
                        // Coin exists, so delete it
                        dbContext.Coins.Remove(coinToDelete);
                        dbContext.SaveChanges();

                        transaction.Commit();
                        ViewBag.dbSuccessComp = 1;
                    }
                    else
                    {
                        // Coin with the given ID does not exist, handle it as needed
                        transaction.Rollback();
                        ViewBag.dbSuccessComp = 0;
                        // Handle the case when the coin does not exist in the table
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ViewBag.dbSuccessComp = 0;
                    // Handle the exception as needed (e.g., log the error).
                }
            }

            List<Coin> watchListcoins = GetWatchList();
            string coinsData = JsonConvert.SerializeObject(watchListcoins);
            HttpContext.Session.SetString(SessionKeyName, coinsData);
            return View("WatchList", watchListcoins);
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

        public List<Coin> GetWatchList()
        {
            List<Coin> coins = new List<Coin>(); // Initialize to an empty list

            try
            {
                // Query the database to select all coins
                coins = dbContext.Coins.ToList();

                ViewBag.dbSuccessComp = 1;
            }
            catch (Exception ex)
            {
                ViewBag.dbSuccessComp = 0;
                // Handle the exception as needed (e.g., log the error).
            }
            return coins;
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
