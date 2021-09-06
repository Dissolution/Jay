

//#define ASYNC

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Jay.Collections.Uber;
using Jay.Debugging;
using Jay.Gambling.Cards;
using Jay.Randomization;
using Jay.Roulette;
using Jay.Roulette.Gamblers;
using Jay.Sandbox.Native;
using Jay.Text;
using Microsoft.Toolkit.HighPerformance;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Serilog.ILogger;

#pragma warning disable 1998

namespace Jay.Sandbox
{
    internal static class Program
    {
        private static readonly ILogger _logger;

        static Program()
        {
            var serilogConfig = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .WriteTo.Console(theme: SystemConsoleTheme.Colored);
            _logger = serilogConfig.CreateLogger();
        }
       
#if ASYNC
        public static async Task<int> Main(params string?[] args)
        {
        }
#else
      


        /* NOTES:
         * IThing that watches a stream of numbers for what it considers 'interesting', can invoke some sortof IProgress<> to signify finding something new
         * Aggregate of those ^ that can be fed very quickly
         * Faster Terminal
         * Clay Event Builder should use weak events
         
         */

        public class CoxelColorBrancher
        {
            private readonly IRandomizer _randomizer;

            public CoxelColorBrancher(IRandomizer randomizer)
            {
                _randomizer = randomizer;
            }

            public CoxelColor Opposite(CoxelColor color)
            {
                return NotSafe.Unmanaged.Not(color);
            }

            public CoxelColor[] Split(CoxelColor color)
            {
                switch (color)
                {
                    case CoxelColor.Black:
                        return new CoxelColor[1] {CoxelColor.Black};
                    case CoxelColor.DarkBlue:
                        return new CoxelColor[1] {CoxelColor.DarkBlue};
                    case CoxelColor.DarkGreen:
                        return new CoxelColor[1] {CoxelColor.DarkGreen};
                    case CoxelColor.DarkCyan:
                    {
                        var array = new CoxelColor[3] {CoxelColor.DarkCyan, CoxelColor.DarkGreen, CoxelColor.DarkBlue};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.DarkRed:
                        return new CoxelColor[1] {CoxelColor.DarkRed};
                    case CoxelColor.DarkMagenta:
                    {
                        var array = new CoxelColor[3] {CoxelColor.DarkMagenta, CoxelColor.DarkRed, CoxelColor.DarkBlue};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.DarkYellow:
                    {
                        var array = new CoxelColor[3] {CoxelColor.DarkYellow, CoxelColor.DarkRed, CoxelColor.DarkGreen};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.Gray:
                    {
                        var array = new CoxelColor[4] {CoxelColor.Gray, CoxelColor.DarkRed, CoxelColor.DarkGreen, CoxelColor.DarkBlue};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.DarkGray:
                        return new CoxelColor[1] {CoxelColor.DarkGray};
                    case CoxelColor.Blue:
                    {
                        var array = new CoxelColor[3] {CoxelColor.Blue, CoxelColor.DarkGray, CoxelColor.DarkBlue};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.Green:
                    {
                        var array = new CoxelColor[3] {CoxelColor.Green, CoxelColor.DarkGray, CoxelColor.DarkGreen};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.Cyan:
                    {
                        var array = new CoxelColor[7] {CoxelColor.Cyan, CoxelColor.Green, CoxelColor.Blue, CoxelColor.DarkGray, CoxelColor.DarkCyan, CoxelColor.DarkGreen, CoxelColor.DarkBlue};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.Red:
                    {
                        var array = new CoxelColor[3] {CoxelColor.Red, CoxelColor.DarkGray, CoxelColor.DarkRed};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.Magenta:
                    {
                        var array = new CoxelColor[7] {CoxelColor.Magenta, CoxelColor.Red, CoxelColor.Blue, CoxelColor.DarkGray, CoxelColor.DarkMagenta, CoxelColor.DarkRed, CoxelColor.DarkBlue};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.Yellow:
                    {
                        var array = new CoxelColor[7] {CoxelColor.Yellow, CoxelColor.Red, CoxelColor.Green, CoxelColor.DarkGray, CoxelColor.DarkYellow, CoxelColor.DarkRed, CoxelColor.DarkGreen};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    case CoxelColor.White:
                    {
                        var array = new CoxelColor[8] {CoxelColor.White, CoxelColor.Red, CoxelColor.Green, CoxelColor.Blue, CoxelColor.DarkGray, CoxelColor.DarkRed, CoxelColor.DarkGreen, CoxelColor.DarkBlue};
                        _randomizer.Shuffle(array);
                        return array;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(color), color, null);
                }
            }
        }

        internal static void BufferTests()
        {
            var buffer = TerminalBuffer.Instance;
            buffer.Read();
            buffer.Clear(new Coxel('╳', CoxelColor.White, CoxelColor.Black));

            char[] curves = "╭╮╯╰".ToCharArray();
            char[] diags = "╱╲".ToCharArray(); //╳
            
            Hold.Debug(curves, diags);
            var rand = new Xoshiro256StarStarRandomizer();
            while (!Console.KeyAvailable)
            {
                var len = buffer.Length;
                for (var i = 0; i < len; i++)
                {
                    ref Coxel coxel = ref buffer[i];
                    coxel.Char = rand.Single(curves);
                    coxel.Color = (CoxelColors) rand.Byte();
                }
                buffer.Flush();
            }


        }

        internal static void RouletteTests()
        {
              var rand = new Xoshiro256StarStarRandomizer();
            var buffer = TerminalBuffer.Instance;
            var terminal = new TerminalText(buffer);
            var wheel = new Wheel(rand);
           
            const int LIMIT = 1_000_000;
            var redColors = new CoxelColors(CoxelColor.Black, CoxelColor.Red);
            var blackColors = new CoxelColors(CoxelColor.White, CoxelColor.Black);
            var greenColors = new CoxelColors(CoxelColor.Black, CoxelColor.Green);

            int startingMoney = 200;
            int minBet = 1;
            
            var watchers = new IWheelWatcher[]
            {
                new StatsWatcher("MinAveMax"),
                new RunWatcher<Pocket.PocketColor>("Pocket Color", p => p.Color),
                new RunWatcher<Pocket>("Pocket", p => p),
                new CountWatcher("House", p => p.IsGreen),
                new CountWatcher("Players", p => !p.IsGreen),
                
                new BetGambler(startingMoney, minBet, Bet.Black),
                new BetGambler(startingMoney, minBet, Bet.Red),
                new BetGambler(startingMoney, minBet, new FuncBet(p => p.Value >= 1 && p.Value <= 12, 3, "Top Third")),
                new BetGambler(startingMoney, minBet, new FuncBet(p => p.Value == 13, 36, "Lucky 13")),
            };
            //var messages = new ConcurrentQueue<string>();
            //watchers.Consume(w => w.Interest += ((sender, msg) => messages.Enqueue(msg)));
            int counter = 0;
            CoxelColors colors;
            wheel.Spin().Take(LIMIT).Consume(forEach: p =>
            {
                counter++;
                watchers.Consume(w => w.Notify(p));
                
                switch (p.Color)
                {
                    case Pocket.PocketColor.Red:
                        colors = redColors;
                        break;
                    case Pocket.PocketColor.Black:
                        colors = blackColors;
                        break;
                    default:
                        colors = greenColors;
                        break; 
                    
                }

                terminal.ColorWrite(colors, p);

                if (counter >= 3 * terminal.Size.Width)
                {
                    terminal.NewLine();
                    counter = 0;

                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey();
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
                else
                {
                    terminal.Append(' ');
                }
            });

            Console.ReadLine();
            buffer.Clear();
            terminal.Index = 1;
            
            foreach (var watcher in watchers)
            {
                var interest = watcher.GetFinalInterest();
                terminal.Append(interest).NewLine();
            }
            buffer.Flush();
            Console.ReadLine();
        }


        public class WarPlayer
        {
            private readonly IRandomizer _randomizer;
            private readonly List<Card> _winnings;
            private readonly List<Card> _deck;

            public int Count => _winnings.Count + _deck.Count;
            
            public WarPlayer(IRandomizer randomizer)
            {
                _randomizer = randomizer;
                _winnings = new List<Card>(52 * 4);
                _deck = Card.GetDeck(Face.Up).ToList();
                _randomizer.Shuffle(_deck);
            }

            public bool TryDrawCard(out Card card)
            {
                int lastIndex = _deck.Count - 1;
                if (lastIndex >= 0)
                {
                    card = _deck[lastIndex];
                    _deck.RemoveAt(lastIndex);
                    return true;
                }
                _deck.AddRange(_winnings);
                _randomizer.Shuffle(_deck);
                _winnings.Clear();
                lastIndex = _deck.Count - 1;
                if (lastIndex >= 0)
                {
                    card = _deck[lastIndex];
                    _deck.RemoveAt(lastIndex);
                    return true;
                }

                card = default;
                return false;
            }
            
         
            public void AddWinnings(params Card[] cards)
            {
                _winnings.AddRange(cards);
            }
        }
        
        
        public static int Main(params string?[] args)
        {
            RouletteTests();
            // var text = new ConsoleText();
            // var rand = Randomizer.New();
            // var playerA = new WarPlayer(rand);
            // var playerB = new WarPlayer(rand);
            //
            // CoxelColor aColor = CoxelColor.Red;
            // CoxelColor bColor = CoxelColor.Blue;
            //
            // WarPlayer GetWinner()
            // {
            //     var cardA = playerA.DrawCard();
            //     var cardARank = cardA.Rank;
            //     var cardB = playerB.DrawCard();
            //     var cardBRank = cardB.Rank;
            //     if (cardARank > cardBRank)
            //     {
            //         text.ColorAppend(aColor, null, "Player A")
            //             .Append(": ")
            //             .ColorAppend(CoxelColor.White, null, cardA)
            //             .Append(" > ")
            //             .ColorAppend(CoxelColor.White, null, cardB);
            //         playerA.AddWinnings(cardA, cardB);
            //         return playerA;
            //     }
            //     else if (cardARank < cardBRank)
            //     {
            //         playerB.AddWinnings(cardA, cardB);
            //         return playerB;
            //     }
            //     else
            //     {
            //         var winner = GetWinner();
            //         winner.AddWinnings(cardA, cardB);
            //         return winner;
            //     } 
            // }
            //
            // ulong round = 0UL;
            // while (true)
            // {
            //     GetWinner();
            //     round += 1UL;
            //     Console.SetCursorPosition(0, 0);
            //     Console.WriteLine($"Round #{round}");
            //     Console.WriteLine($"Player A: {playerA.Count,-3}");
            //     Console.WriteLine($"Player B: {playerB.Count,-3}");
            //     if (Console.KeyAvailable)
            //         Debugger.Break();
            // }
            
            
            // var summary = BenchmarkHelper.RunAndOpen<CharWriterBenchmarks>();
            // Hold.Debug(summary);
            
            Console.WriteLine("Press 'Enter' to quit");
            Console.ReadLine();
            return 0;
        }
#endif


        // public static bool CanBe<T>(object? obj, out T? value)
        // {
        //     if (obj is T typed)
        //     {
        //         value = typed;
        //         return true;
        //     }
        //
        //     value = default(T);
        //     return value is null;
        // }

    }
}