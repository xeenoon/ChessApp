using ChessApp;
using System.Diagnostics;

namespace EngineTester
{
    public class Program
    {
        static System.Timers.Timer timer;
        static bool iswaiting;
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
                iswaiting = false;
                n.BasePopulate(int_depth);
                var nodes = Node.totalnodes;
                var time1 = Node.squareattacktime;
                var time2 = MoveGenerator.TOTALTIME;
                var time3 = Node.copytime;
                stopwatch.Stop();
                iswaiting = true;
                var total = stopwatch.ElapsedTicks;
                timer.Stop();
                lastthreads = 0;
                lastlines = 0;
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
                Console.WriteLine("Nodes per second: " + 1000 * (nodes / stopwatch.ElapsedMilliseconds));
                Console.WriteLine("Nodes searched: " + nodes);

                Bitboard.Pins = 0;
                Bitboard.SlidingAttack = 0;
                Bitboard.StaticAttack = 0;
                Node.squareattacktime = 0;
                Node.totalnodes = 0;
                MoveGenerator.TOTALTIME = 0;
                MoveGenerator.KingMovesCalls = 0;
                MoveGenerator.KingMovesTime = 0;
                MoveGenerator.GetMovesCalls = 0;
                MoveGenerator.GetMovesTime = 0;
                Node.copytime = 0;
            }
        }
        static int lastthreads = 0;
        static int lastlines = 0;
        public static void TimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (iswaiting)
            {
                return;
            }

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