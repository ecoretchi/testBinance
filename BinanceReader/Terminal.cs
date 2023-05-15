using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceReader
{
    public class Terminal
    {
        public Action<string>? onSubsribeToTrades = null;
        public Func<int>? TotalCached = null;
        public Func<int>? SubscribedPairs = null;

        bool IsOnline { get; set; } = false;

        private void ShowHelp()
        {
            Console.WriteLine("------HELP-------");
            Console.WriteLine("?|h|help - show this help");
            Console.WriteLine("x|exit|q|quit - exit");
            Console.WriteLine("m - show main menu");
            Console.WriteLine("o - on/off online mode");
            Console.WriteLine("s,status - show status");

            Console.WriteLine("");
        }

        private void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("------MAIN-SELECTION-MENU-------");
            Console.WriteLine("1. SUBSCRIBE to TRADES ");
        }

        private void ShowStatus()
        {
            Console.WriteLine();
            Console.WriteLine("------Status-------");
            Console.WriteLine("Online mode is: " + (IsOnline ? "On" : "Off"));
            Console.WriteLine($"Total cached pairs: {TotalCached?.Invoke()}");
            Console.WriteLine($"Subscribed pairs: {SubscribedPairs?.Invoke()}");
        }

        public void WriteLine(string s)
        {
            if (IsOnline)
            {
                Console.WriteLine(s);
            }

        }

        public void WriteLine(ConsoleColor c, string s)
        {
            if (IsOnline)
            {
                var tmp = Console.ForegroundColor;
                Console.ForegroundColor = c;
                Console.WriteLine(s);
                Console.ForegroundColor = tmp;
            }

        }

        public void Run()
        {
            Console.WriteLine("BinanceReader v0.1");
            Console.WriteLine("");
            ShowHelp();
            ShowMenu();

            var isRun = true;
            while (isRun)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("> ");
                var ln = Console.ReadLine();

                switch (ln)
                {
                    case "eixt":
                    case "x":
                    case "quit":
                    case "q":
                        isRun = false;
                        break;
                    case "m":
                        ShowMenu();
                        break;
                    case "?":
                    case "h":
                    case "help":
                        ShowHelp();
                        break;
                    case "o":
                        IsOnline^= true;
                        Console.WriteLine("Online mode is: " + (IsOnline ? "On" : "Off"));
                        break;
                    case "status":
                    case "s":
                        ShowStatus();
                        break;
                    case "1":
                        {
                            Console.WriteLine("Please enter pair in format: usdttry,ethbtc,ethusdt");
                            ln = Console.ReadLine();
                            var trades = ln?.Split(',').ToList();
                            if (trades != null)
                            {
                                foreach (var tr in trades)
                                {
                                    onSubsribeToTrades?.Invoke(tr);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid trades");
                            }
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Invalid input");
                        }
                        break;

                }
            }
        }
    }
}
