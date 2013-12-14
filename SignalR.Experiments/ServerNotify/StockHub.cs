using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;
using Microsoft.Owin;

namespace ServerNotify
{
    //because Hub instances are transient. A Hub class instance is created 
    //for each operation on the hub, such as connections and calls from the client to the server.
    //So the mechanism that keeps stock data, updates prices, and broadcasts the 
    //price updates has to run in a separate class, which you'll name StockTicker.
    [HubName("stockTickerMini")]
    public class StockHub : Hub
    {
        private readonly StockTicker _stockTicker;

        public StockHub() : this(StockTicker.Instance) { }

        public StockHub(StockTicker stockTicker)
        {
            _stockTicker = stockTicker;
        }

        // If the method had to get the data by doing something that would 
        //involve waiting, such as a database lookup or a web service call, 
        //you would specify Task<IEnumerable<Stock>> as the return value to
        //enable asynchronous processing. For more information, see ASP.NET 
        //SignalR Hubs API Guide - Server - When to execute asynchronously.
        public IEnumerable<Stock> GetAllStocks()
        {
            return _stockTicker.GetAllStocks();
        }
    }

    public class StockTicker
    {
        // Singleton instance
        private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(() =>
            new StockTicker(GlobalHost.ConnectionManager.GetHubContext<StockHub>().Clients));

        private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();

        private readonly object _updateStockPricesLock = new object();

        //stock can go up or down by a percentage of this factor on each change
        private readonly double _rangePercent = .002;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        private readonly Timer _timer;
        private volatile bool _updatingStockPrices = false;

        private StockTicker(IHubConnectionContext clients)
        {
            Clients = clients;

            _stocks.Clear();
            var stocks = new List<Stock>
            {
                new Stock { Symbol = "MSFT", Price = 30.31m },
                new Stock { Symbol = "APPL", Price = 578.18m },
                new Stock { Symbol = "GOOG", Price = 570.30m }
            };
            stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));

            _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

        }

        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext Clients
        {
            get;
            set;
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        private void UpdateStockPrices(object state)
        {
            //lock (_updateStockPricesLock)
            {
                //if (!_updatingStockPrices)
                //{
                //    _updatingStockPrices = true;

                foreach (var stock in _stocks.Values)
                {
                    if (TryUpdateStockPrice(stock))
                    {
                        BroadcastStockPrice(stock);
                    }
                }

                //    _updatingStockPrices = false;
                //}
            }
        }

        private bool TryUpdateStockPrice(Stock stock)
        {
            // Randomly choose whether to update this stock or not


            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * _rangePercent;
            var pos = random.NextDouble() > .51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);
            change = pos ? change : -change;

            stock.Price += change;
            return true;
        }

        private void BroadcastStockPrice(Stock stock)
        {
            Clients.All.updateStockPrice(stock);
        }

    }
}
