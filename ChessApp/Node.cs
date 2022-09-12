using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessApp
{
    public struct Node
    {
        public Bitboard b;
        public Side hasturn;
        public List<Node> children;

        public Node(Bitboard b, Side hasturn)
        {
            this.b = b;
            this.hasturn = hasturn;
            children = new List<Node>();
        }
        public static double squareattacktime = 0;
        public static double copytime = 0;
        private static ulong Populate(int nodes, Bitboard b, Side hasturn, bool first = false)
        {
            Stopwatch stopwatch = new Stopwatch();
            ulong result = 0;
            if (nodes == 0)
            {
                return 1;
            }
            nodes--;
            if (first)
            {
                ++threads_running;
            }
            stopwatch.Start();
            b.SetupSquareAttacks();
            stopwatch.Stop();
            squareattacktime += stopwatch.ElapsedTicks;
            var otherturn = hasturn == Side.White ? Side.Black : Side.White;
            foreach (var move in MoveGenerator.CalculateAll(b, hasturn))
            {
                stopwatch.Restart();
                //Simulating move
                var copy = b.Move(move.last, move.current, 1ul<<move.last, 1ul<<move.current, move.pieceType, hasturn);

                stopwatch.Stop();
                copytime += stopwatch.ElapsedTicks;

                result += Populate(nodes, copy, otherturn);
            }
            if (first)
            {
                --threads_running;
            }
            return result;
        }
        public static int linescalculated = 0;
        public static int threads_running = 0;
        public static ConcurrentBag<ulong> total_nodes = new ConcurrentBag<ulong>();
        public static List<Thread> threads = new List<Thread>();
        public void BasePopulate(int nodes)
        {
            Stopwatch stopwatch = new Stopwatch();

            if (nodes == 0)
            {
                return;
            }
            nodes--;
            stopwatch.Start();
            b.SetupSquareAttacks();
            stopwatch.Stop();
            squareattacktime += stopwatch.ElapsedTicks;
            var otherturn = hasturn == Side.White ? Side.Black : Side.White;
            var options = new ParallelOptions();// { MaxDegreeOfParallelism = int.MaxValue };
            List<Move> source = MoveGenerator.CalculateAll(b, hasturn);
            total_nodes = new ConcurrentBag<ulong>();
            for (int i = 0; i < source.Count; i++)
            {
                Move move = source[i];

                //Simulating move
                var copy = b.Move(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                var t = new Thread(() =>
                {
                    total_nodes.Add(Populate(nodes, copy, otherturn));
                    //total_nodes[i] = v;
                });

                t.Start();
                threads.Add(t);
                
                stopwatch.Stop();
                copytime += stopwatch.ElapsedTicks;
                ++linescalculated;
            }
        }
    }
}
