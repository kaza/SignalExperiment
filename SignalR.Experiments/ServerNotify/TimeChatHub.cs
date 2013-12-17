using System;
using System.Threading;
using Microsoft.AspNet.SignalR;

namespace ServerNotify
{
    public class TimeChatHub : Hub
    {
        public TimeChatHub()
        {
            Console.WriteLine();
        }

        public void Send(string name, string message)
        {
            var timeChat = new TimeChatInfo
            {
                Name = name,
                Message = message
            };

            
            Clients.All.broadcastMessage(timeChat);
        }
    }

    public class TimeChatInfo
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }

    public class TimeChatHost
    {
        public static TimeChatTicker Ticker
        {
            get;
            set;
        }
    }

    public class TimeChatTicker
    {
        public TimeChatTicker()
        {
           
        }

        public void StartTicker()
        {
            _updateInterval = TimeSpan.FromMilliseconds(250);
            _timer = new Timer(TickChat, null, _updateInterval, _updateInterval);
        }

        private bool isIn;//mainly help for debuger
        private void TickChat(object state)
        {
            if (isIn)
                return;

            isIn = true;
            InnerTick();
            isIn = false;
        }

        int nrTicks=0;
        
        private  void InnerTick()
        {
            var clients = GlobalHost.ConnectionManager.GetHubContext<TimeChatHub>().Clients;
            var timeChat = new TimeChatInfo
                {
                    Name = Environment.MachineName,
                    Message = DateTime.Now.TimeOfDay.ToString()
                };

            

            if (nrTicks%4 == 0)
                timeChat.Message = "*****";
            
            if (nrTicks%40 == 0)
                timeChat.Message = "clear";

            nrTicks++;
            
            clients.All.broadcastMessage(timeChat);

        }

// ReSharper disable NotAccessedField.Local
        private  Timer _timer;
// ReSharper restore NotAccessedField.Local
        private TimeSpan _updateInterval;
        private int lastClick;
    }
}