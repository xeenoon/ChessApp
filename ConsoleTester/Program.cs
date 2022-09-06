using ChessApp;
using System.Diagnostics;

namespace EngineTester
{
    public class Program
    {
        static System.Timers.Timer timer;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Search depth: ");
                var depth = Console.ReadLine();
                int int_depth;
                if (!int.TryParse(depth, out int_depth))
                {
                    continue;
                }

                var FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - 0 1";
                Bitboard B = new Bitboard(FEN);
                Node n = new Node(B, Side.White, null);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                timer = new System.Timers.Timer();
                timer.Interval = 100;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerTick);
                timer.Start();
                n.BasePopulate(int_depth);
                var nodes = Node.totalnodes;
                var time1 = Node.squareattacktime;
                var time2 = MoveGenerator.TOTALTIME;
                var time3 = Node.copytime;
                stopwatch.Stop();
                var total = stopwatch.ElapsedTicks;
                timer.Stop();
                Console.WriteLine(String.Format("Total nodes searched: {0}, Depth: {1}", nodes, depth));
                Console.WriteLine("---Time Stats---");
                Console.WriteLine(string.Format("SquareAttackCalc() {0} ticks", time1));
                Console.WriteLine(string.Format("   Pins    {0} ticks", Bitboard.Pins));
                Console.WriteLine(string.Format("   Sliding {0} ticks", Bitboard.SlidingAttack));
                Console.WriteLine(string.Format("   Static  {0} ticks", Bitboard.StaticAttack));
                Console.WriteLine(string.Format("   Timers  {0} ticks", time1 - (Bitboard.Pins+Bitboard.SlidingAttack + Bitboard.StaticAttack)));
                Console.WriteLine();
                Console.WriteLine(string.Format("CalculateAll()     {0} ticks", time2));
                Console.WriteLine(string.Format("   Moves   {0} ticks", MoveGenerator.GetMovesTime));
                Console.WriteLine(string.Format("               {0} calls", MoveGenerator.GetMovesCalls));
                Console.WriteLine(string.Format("   King    {0} ticks", MoveGenerator.KingMovesTime));
                Console.WriteLine(string.Format("               {0} calls", MoveGenerator.KingMovesCalls));
                Console.WriteLine();
                Console.WriteLine(string.Format("Populate()         {0} ticks", time3));
                Console.WriteLine("---Time Stats---");
                Console.WriteLine();
                Console.WriteLine("SUM OF MAJOR TIMES:  " + (time1 + time2 + time3));
                Console.WriteLine("Total elapsed ticks: " + total);
                Console.WriteLine("Total time (miliseconds): " + stopwatch.ElapsedMilliseconds);
                Console.WriteLine("Nodes per second: " + 1000*(nodes / stopwatch.ElapsedMilliseconds));
            }
        }
        static int lastthreads = 0;
        static int lastlines = 0;
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
            timer.Start();
        }
    }
}