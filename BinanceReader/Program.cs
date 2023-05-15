// See https://aka.ms/new-console-template for more information
using BinanceReader;

Terminal terminal = new Terminal();

terminal.onSubsribeToTrades += pair => new TradePairWorker(terminal, pair);
terminal.TotalCached += () => TradePairWorker.TotalCached;
terminal.SubscribedPairs  += () => TradePairWorker.SubscribedPairs;

terminal.Run();

Console.WriteLine($"terminated");