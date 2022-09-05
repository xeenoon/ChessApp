using ChessApp;
using System.Diagnostics;

namespace EngineTester
{
    public class Program
    {
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
                n.Populate(int_depth);
                var nodes = Node.totalnodes;
                var time1 = Node.squareattacktime;
                var time2 = MoveGenerator.TOTALTIME;
                var time3 = Node.copytime;
                stopwatch.Stop();
                var total = stopwatch.ElapsedTicks;

                Console.WriteLine(String.Format("Total nodes searched: {0}, Depth: {1}", nodes, depth));
                Console.WriteLine("---Time Stats---");
                Console.WriteLine(string.Format("SquareAttackCalc() {0} ticks", time1));
                Console.WriteLine(string.Format("   Pins    {0} ticks", Bitboard.Pins));
                Console.WriteLine(string.Format("   Sliding {0} ticks", Bitboard.SlidingAttack));
                Console.WriteLine(string.Format("   Static  {0} ticks", Bitboard.StaticAttack));
                Console.WriteLine(string.Format("   Timers  {0} ticks", time1 - (Bitboard.Pins+Bitboard.SlidingAttack + Bitboard.StaticAttack)));
                Console.WriteLine(string.Format("CalculateAll()     {0} ticks", time2));
                Console.WriteLine(string.Format("   Moves   {0} ticks", MoveGenerator.GetMovesTime));
                Console.WriteLine(string.Format("               {0} calls", MoveGenerator.GetMovesCalls));
                Console.WriteLine(string.Format("   King    {0} ticks", MoveGenerator.KingMovesTime));
                Console.WriteLine(string.Format("               {0} calls", MoveGenerator.KingMovesCalls));
                Console.WriteLine(string.Format("Populate()         {0} ticks", time3));
                Console.WriteLine("---Time Stats---");
                Console.WriteLine();
                Console.WriteLine("SUM OF MAJOR TIMES:  " + (time1 + time2 + time3));
                Console.WriteLine("Total elapsed ticks: " + total);
                Console.WriteLine("Total time (miliseconds): " + stopwatch.ElapsedMilliseconds);
                Console.WriteLine("Nodes per second: " + 1000*(nodes / stopwatch.ElapsedMilliseconds));
            }
        }
    }
}