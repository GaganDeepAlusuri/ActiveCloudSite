using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using CryptoPulse.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptoPulse.Infrastructure.CryptoPulseHandler
{
    public class CryptoPulseHandler
    {
        static string BASE_URL = "https://api.coinlore.net/api/"; //This is the base URL, method specific URL is appended to this.
        HttpClient httpClient;

        public CryptoPulseHandler()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        /****
         * Calls the Coin Lore reference API to get the list of coins. 
        ****/
        public List<Coin> GetCoins()
        {
            try
            {
                string CryptoPulse_API_PATH = BASE_URL + "tickers/";

                // Create a new HttpClient instance or configure it during initialization
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(CryptoPulse_API_PATH);

                HttpResponseMessage response = httpClient.GetAsync(CryptoPulse_API_PATH).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var coinInfo = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (!string.IsNullOrWhiteSpace(coinInfo))
                    {
                        JObject jsonObject = JObject.Parse(coinInfo);
                        JArray data = (JArray)jsonObject["data"];

                        if (data != null)
                        {
                            List<Coin> coins = data.ToObject<List<Coin>>();
                            return coins;
                        }
                    }
                }

                // Handle the case where the response is not successful or the coin data is empty.
                return new List<Coin>();
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log them, and possibly return a default value or throw.
                // For example:
                // Log.Error("Error in GetCoins method: " + ex.Message);
                // throw;
                return new List<Coin>();
            }
        }

        public List<Market> GetMarkets(int coinId)
        {
            try
            {
                string CryptoPulse_API_PATH = $"{BASE_URL}/coin/markets/?id={coinId}";
                httpClient.BaseAddress = new Uri(CryptoPulse_API_PATH);
                HttpResponseMessage response = httpClient.GetAsync(CryptoPulse_API_PATH).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var marketInfo = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (!string.IsNullOrWhiteSpace(marketInfo))
                    {
                        List<Market> markets = JsonConvert.DeserializeObject<List<Market>>(marketInfo);

                        if (markets != null)
                        {
                            return markets;
                        }
                    }
                }

                // Handle the case where the response is not successful or the coin data is empty.
                return new List<Market>();
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log them, and possibly return a default value or throw.
                // For example:
                // Log.Error("Error in GetMarkets method: " + ex.Message);
                // throw;
                return new List<Market>();
            }
        }

        public List<Exchange> GetExchanges()
        {
            try
            {
                string CryptoPulse_API_PATH = BASE_URL + "exchanges/";

                // Create a new HttpClient instance or configure it during initialization
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(CryptoPulse_API_PATH);

                HttpResponseMessage response = httpClient.GetAsync(CryptoPulse_API_PATH).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    var exchangeInfo = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (!string.IsNullOrWhiteSpace(exchangeInfo))
                    {
                        JObject jsonObject = JObject.Parse(exchangeInfo);

                        List<Exchange> exchanges = jsonObject.Values().Select(subObject =>
                        {
                            return JsonConvert.DeserializeObject<Exchange>(subObject.ToString());
                        }).ToList();

                        // Now, the 'exchanges' list contains the converted Exchange objects
                        return exchanges;
                    }
                }

                // Handle the case where the response is not successful or the coin data is empty.
                return new List<Exchange>();
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log them, and possibly return a default value or throw.
                // For example:
                // Log.Error("Error in GetCoins method: " + ex.Message);
                // throw;
                return new List<Exchange>();
            }
        }
    }
}
