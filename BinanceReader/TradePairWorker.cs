namespace BinanceReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WebSocketSharp;
    using System.Diagnostics;
    using Newtonsoft.Json;

    public class TradePairWorker
    {
        Terminal terminal;
        static List<WebSocket> webSockets = new List<WebSocket>();
        static List<Trade> cachedTrades = new List<Trade>();
        static Dictionary<string, int> tradesCount = new Dictionary<string, int>();
        static int MaxCachedTrades = 10000;

        public static int TotalCached => cachedTrades.Count;
        public static int SubscribedPairs => tradesCount.Count;

        
        string pair;

        public TradePairWorker(Terminal terminal, string pair)
        {
            this.terminal = terminal;
            this.pair = pair;

            var th = new Thread(Run);
            th.Start();
        }

        void Run()
        {
            string SocketUrl = $"wss://stream.binance.com:9443/ws/{pair}@trade";

            WebSocket ws = new WebSocket(SocketUrl);

            terminal.WriteLine($"SubsribeToTrade Pair: {pair}");

            ws.OnOpen += (sender, j) =>
            {
                terminal.WriteLine($"{pair}: websocket opened");
            };

            ws.OnMessage += (sender, j) =>
            {
                //terminal.WriteLine($"{pair}: websocket message");
                //terminal.WriteLine($"{j.Data}");

                var trade = JsonConvert.DeserializeObject<Trade>(j.Data);
                if (trade != null)
                {
                    var color = trade.BuyerMaker == true ? ConsoleColor.Green : ConsoleColor.Red;
                    terminal.WriteLine(color, $"Pair={trade.Pair}, Currency={trade.Currency}, EventID={trade.EventID}, BuyerID={trade.BuyerID}");
                    cachedTrades.Add(trade);
                    //check max trade pair at cahce and remove if exceeded
                    int tradeCount = 0;
                    if (tradesCount.TryGetValue(trade.Pair, out tradeCount))
                    {
                        ++tradeCount;
                    }
                    else
                    {
                        tradesCount.Add(trade.Pair, 1);
                    }

                    if (tradeCount > MaxCachedTrades)
                    {
                        foreach (var cachedTrade in cachedTrades)
                        {
                            if (trade.Pair == cachedTrade.Pair)
                            {
                                cachedTrades.Remove(cachedTrade);
                                break;
                            }
                        }
                    }

                }
            };

            ws.OnError += (sender, j) =>
            {
                terminal.WriteLine($"{pair}: websocket error");
                terminal.WriteLine($"{j.Message}");
            };

            ws.OnClose += (sender, j) =>
            {
                terminal.WriteLine($"{pair}: websocket closed");
                webSockets.Remove(sender as WebSocket);
            };

            ws.Connect();

            webSockets.Add(ws);
        }
    }
}
