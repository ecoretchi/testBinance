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

    class PairInfo
    {
        public int cachedCount;
    }

    public class TradePairWorker
    {
        Terminal terminal;
        static List<WebSocket> webSockets = new List<WebSocket>();
        static List<Trade> cachedTrades = new List<Trade>();
        static Dictionary<string, PairInfo> pairInfo = new Dictionary<string, PairInfo>();
        static int MaxCachedTrades = 4;
        static object tradesLocker = new();

        public static int TotalCached => cachedTrades.Count;
        public static int SubscribedPairs => pairInfo.Count;


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
                    lock (tradesLocker)
                    {
                        cachedTrades.Add(trade);
                    }

                    //check max trade pair at cahce and remove if exceeded

                    if (pairInfo.TryGetValue(trade.Pair, out var pairInf))
                    {
                        pairInf.cachedCount++;

                        if (pairInf.cachedCount > MaxCachedTrades)
                        {
                            lock (tradesLocker)
                            {

                                Trade? toRemove = null;
                                foreach (var cachedTrade in cachedTrades)
                                {
                                    if (trade.Pair == cachedTrade.Pair)
                                    {
                                        toRemove = cachedTrade;
                                        break;
                                    }
                                }
                                if (toRemove != null)
                                {
                                    cachedTrades.Remove(toRemove);
                                }
                            }
                        }
                    }
                    else
                    {
                        pairInfo.Add(trade.Pair, new PairInfo() { cachedCount = 1 });
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
