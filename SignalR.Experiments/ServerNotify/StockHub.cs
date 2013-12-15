using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
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
}
