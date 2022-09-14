using ChessApp;
using System.Diagnostics;

namespace EngineTester
{
    public class Program
    {
        static System.Timers.Timer timer;
        static bool iswaiting;
        static Stopwatch stopwatch = new Stopwatch();
        static string input;
        static void Main(string[] args)
        {
            Console.Write("Search depth: ");

            while (true)
            {
                input = Console.ReadLine();
                int int_depth;
                var FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                if (!int.TryParse(input, out int_depth))
                {
                    if (input.Contains(','))
                    {
                        var strs = input.Split(',');
                        FEN = strs[0];
                        int_depth = int.Parse(strs[1]);
                    }
                    else
                    {
                        int_depth = 1;
                        FEN = input;
                    }
                }

                Chessboard chessboard = new Chessboard(FEN);
                Bitboard B = chessboard.bitboard.Copy();
                Node n = new Node(B, chessboard.hasturn);
                stopwatch.Restart();
                timer = new System.Timers.Timer();
                timer.Interval = 100;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerTick);
                timer.Start();
                iswaiting = false;
                n.BasePopulate(int_depth);

                //PrintDebug();
            }
        }

        private static void PrintDebug()
        {
            var time1 = Node.squareattacktime;
            var time2 = MoveGenerator.TOTALTIME + MoveGenerator.MoveCountTime;
            var time3 = Node.populateTime;
            stopwatch.Stop();
            iswaiting = true;
            var total = stopwatch.ElapsedTicks;
            timer.Stop();
            lastthreads = 0;
            lastlines = 0;
            Console.WriteLine(String.Format("Total nodes searched: {0}, Depth: {1}", totalnodes, input));
            Console.WriteLine("---Time Stats---");
            Console.WriteLine(string.Format("SquareAttackCalc() {0} ticks", time1));
            Console.WriteLine();
            Console.WriteLine(string.Format("CalculateAll()     {0} ticks", time2));
            Console.WriteLine(string.Format("   Moves   {0} ticks", MoveGenerator.GetMovesTime));
            Console.WriteLine(string.Format("               {0} calls", MoveGenerator.GetMovesCalls));
            Console.WriteLine(string.Format("   King    {0} ticks", MoveGenerator.KingMovesTime));
            Console.WriteLine(string.Format("               {0} calls", MoveGenerator.KingMovesCalls));
            Console.WriteLine(string.Format("   Count   {0} ticks", MoveGenerator.MoveCountTime));
            Console.WriteLine();
            Console.WriteLine(string.Format("Populate()         {0} ticks", time3));
            Console.WriteLine(string.Format("   Simulation      {0} ticks", Bitboard.MoveTime));
            Console.WriteLine(string.Format("   Copy            {0} ticks", Bitboard.CopyTime));
            Console.WriteLine("---Time Stats---");
            Console.WriteLine();
            Console.WriteLine("Total elapsed ticks: " + total);
            Console.WriteLine("Total time (miliseconds): " + stopwatch.ElapsedMilliseconds);
            if (stopwatch.ElapsedMilliseconds == 0)
            {
                Console.WriteLine("Nodes per second: infinity");
            }
            else 
            {
                Console.WriteLine("Nodes per second: " + 1000 * (totalnodes / (ulong)stopwatch.ElapsedMilliseconds));
            }
            Console.WriteLine();
            Console.WriteLine("---Node Data---");
            Console.WriteLine("Enpassantes: " + Bitboard.enpassantes);
            Console.WriteLine("Captures: "    + Bitboard.captures);
            Console.WriteLine("Castles: "     + Bitboard.castles);
            Console.WriteLine("Promotions: "  + Bitboard.promotions);
            Console.WriteLine("Checks: "      + Bitboard.total_checks);
            Console.WriteLine("Double checks:"+ Bitboard.total_doublechecks);

            Bitboard.Pins = 0;
            Bitboard.SlidingAttack = 0;
            Bitboard.StaticAttack = 0;
            Node.squareattacktime = 0;
            MoveGenerator.TOTALTIME = 0;
            MoveGenerator.KingMovesCalls = 0;
            MoveGenerator.KingMovesTime = 0;
            MoveGenerator.GetMovesCalls = 0;
            MoveGenerator.GetMovesTime = 0;
            Node.populateTime = 0;

            Bitboard.total_checks = 0;
            Bitboard.total_doublechecks = 0;
            Bitboard.MoveTime = 0;

            Bitboard.enpassantes = 0;
            Bitboard.captures = 0;
            Bitboard.castles = 0;
            Bitboard.promotions = 0;

            Console.Write("Search depth: ");
        }

        static int lastthreads = 0;
        static int lastlines = 0;
        static ulong totalnodes;
        public static void TimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (lastthreads != Node.threads_running)
            {
                Console.WriteLine(String.Format("Tasked {0} more threads. Now running {1} threads",Node.threads_running - lastthreads , Node.threads_running));
                lastthreads = Node.threads_running;
            }
            if (lastlines != Node.linescalculated)
            {
                Console.WriteLine(String.Format("Calculated {0} more lines", Node.linescalculated - lastlines));
                lastlines = Node.linescalculated;
            }
            if (Node.threadcount == Node.total_nodes.Count())
            {
                totalnodes = 0;
                foreach (ulong u in Node.total_nodes)
                {
                    totalnodes += u;
                }
                PrintDebug();
                return;
            }
            timer.Start();
        }
    }
}