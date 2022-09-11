using System;
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
        public static ulong totalnodes;

        public Node(Bitboard b, Side hasturn)
        {
            this.b = b;
            this.hasturn = hasturn;
            children = new List<Node>();
        }
        public static double squareattacktime = 0;
        public static double copytime = 0;
        private void Populate(int nodes, bool first = false)
        {
            Stopwatch stopwatch = new Stopwatch();

            if (nodes == 0)
            {
                ++totalnodes;
                return;
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
                var resultpos = move.current;
                while (resultpos != 0ul)
                {
                    stopwatch.Restart();

                    byte lsb = (byte)(BitOperations.TrailingZeros(resultpos) - 1);
                    ulong bitpos = 1ul << lsb;

                    //Simulating move
                    var copy = b.Move((byte)(BitOperations.TrailingZeros(move.last) - 1), lsb, move.last, bitpos, move.pieceType, hasturn);


                    Node n = new Node(copy, otherturn);
                    stopwatch.Stop();
                    copytime += stopwatch.ElapsedTicks;

                    n.Populate(nodes);
                    resultpos ^= bitpos; //remove this piece from the ulong of pieces
                }
            }
            if (first)
            {
                --threads_running;
            }
        }
        public static int linescalculated = 0;
        public static int threads_running = 0;
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
            foreach (var move in source) 
            { 
                var resultpos = move.current;
                while (resultpos != 0ul)
                {
                    stopwatch.Restart();

                    byte lsb = (byte)(BitOperations.TrailingZeros(resultpos) - 1);
                    ulong bitpos = 1ul << lsb;
                    resultpos ^= bitpos; //remove this piece from the ulong of pieces

                    //Simulating move
                    var copy = b.Move((byte)(BitOperations.TrailingZeros(move.last) - 1), lsb, move.last, bitpos, move.pieceType, hasturn);
                    Node n = new Node(copy, otherturn);
                    var t = new Thread(() =>
                    {
                        n.Populate(nodes, true);
                    });

                    t.Start();
                    stopwatch.Stop();
                    copytime += stopwatch.ElapsedTicks;
                }
                ++linescalculated;
            }
        }


    }
}
