﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using CryptoPulse.Models;
using Newtonsoft.Json;

namespace CryptoPulse.Infrastructure.CryptoPulseHandler
{
    public class CryptoPulseHandler
    {
        static string BASE_URL = "https://api.CryptoPulse.com/1.0/"; //This is the base URL, method specific URL is appended to this.
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
        public List<Company> GetSymbols()
        {
            string CryptoPulse_API_PATH = BASE_URL + "ref-data/symbols";
            string companyList = "";

            List<Company> companies = null;

            httpClient.BaseAddress = new Uri(CryptoPulse_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(CryptoPulse_API_PATH).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                companyList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            if (!companyList.Equals(""))
            {
                companies = JsonConvert.DeserializeObject<List<Company>>(companyList);
                companies = companies.GetRange(0, 50);
            }
            return companies;
        }

        /****
         * Calls the  Coin Lore API to get 1 year's chart for the supplied symbol. 
        ****/
        public List<Equity> GetChart(string symbol)
        {
            //Using the format method.
            //string CryptoPulse_API_PATH = BASE_URL + "stock/{0}/batch?types=chart&range=1y";
            //CryptoPulse_API_PATH = string.Format(CryptoPulse_API_PATH, symbol);

            string CryptoPulse_API_PATH = BASE_URL + "stock/" + symbol + "/batch?types=chart&range=1y";

            string charts = "";
            List<Equity> Equities = new List<Equity>();
            httpClient.BaseAddress = new Uri(CryptoPulse_API_PATH);
            HttpResponseMessage response = httpClient.GetAsync(CryptoPulse_API_PATH).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                charts = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!charts.Equals(""))
            {
                ChartRoot root = JsonConvert.DeserializeObject<ChartRoot>(charts, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Equities = root.chart.ToList();
            }
            //make sure to add the symbol the chart
            foreach (Equity Equity in Equities)
            {
                Equity.symbol = symbol;
            }

            return Equities;
        }
    }
}