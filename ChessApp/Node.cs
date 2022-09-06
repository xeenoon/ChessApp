using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    public class Node
    {
        public Bitboard b;
        public Side hasturn;
        public Node parent;
        public List<Node> children = new List<Node>();
        public static int totalnodes = 0;

        public Node(Bitboard b, Side hasturn, Node parent)
        {
            this.b = b;
            this.hasturn = hasturn;
            this.parent = parent;
            ++totalnodes;
        }
        public static double squareattacktime = 0;
        public static double copytime = 0;
        private void Populate(int nodes)
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
            foreach (var move in MoveGenerator.CalculateAll(b, hasturn))
            {
                
                var resultpos = move.current;
                while (resultpos != 0ul)
                {
                    stopwatch.Restart();

                    var copy = b.Copy();
                    byte lsb = (byte)(BitOperations.TrailingZeros(resultpos) - 1);
                    ulong bitpos = 1ul << lsb;

                    //Simulating move
                    if (hasturn == Side.White)
                    {
                        switch (move.pieceType)
                        {
                            case PieceType.Pawn:
                                copy.W_Pawn ^= move.last;
                                copy.W_Pawn ^= bitpos;
                                break;
                            case PieceType.Rook:
                                copy.W_Rook ^= move.last;
                                copy.W_Rook ^= bitpos;
                                break;
                            case PieceType.Knight:
                                copy.W_Knight ^= move.last;
                                copy.W_Knight ^= bitpos;
                                break;
                            case PieceType.Bishop:
                                copy.W_Bishop ^= move.last;
                                copy.W_Bishop ^= bitpos;
                                break;
                            case PieceType.Queen:
                                copy.W_Queen ^= move.last;
                                copy.W_Queen ^= bitpos;
                                break;
                            case PieceType.King:
                                copy.W_King ^= move.last;
                                copy.W_King ^= bitpos;
                                break;
                        }
                    }
                    else
                    {
                        switch (move.pieceType)
                        {
                            case PieceType.Pawn:
                                copy.B_Pawn ^= move.last;
                                copy.B_Pawn ^= bitpos;
                                break;
                            case PieceType.Rook:
                                copy.B_Rook ^= move.last;
                                copy.B_Rook ^= bitpos;
                                break;
                            case PieceType.Knight:
                                copy.B_Knight ^= move.last;
                                copy.B_Knight ^= bitpos;
                                break;
                            case PieceType.Bishop:
                                copy.B_Bishop ^= move.last;
                                copy.B_Bishop ^= bitpos;
                                break;
                            case PieceType.Queen:
                                copy.B_Queen ^= move.last;
                                copy.B_Queen ^= bitpos;
                                break;
                            case PieceType.King:
                                copy.B_King ^= move.last;
                                copy.B_King ^= bitpos;
                                break;
                        }
                    }
                    Node n = new Node(copy, otherturn, this);
                    n.Populate(nodes);
                    resultpos ^= bitpos; //remove this piece from the ulong of pieces

                    stopwatch.Stop();
                    copytime += stopwatch.ElapsedTicks;
                }
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
            Parallel.ForEach(source, options , move =>
            {
                ++threads_running;
                var resultpos = move.current;
                while (resultpos != 0ul)
                {
                    stopwatch.Restart();

                    var copy = b.Copy();
                    byte lsb = (byte)(BitOperations.TrailingZeros(resultpos) - 1);
                    ulong bitpos = 1ul << lsb;
                    resultpos ^= bitpos; //remove this piece from the ulong of pieces

                    //Simulating move
                    if (hasturn == Side.White)
                    {
                        switch (move.pieceType)
                        {
                            case PieceType.Pawn:
                                copy.W_Pawn ^= move.last;
                                copy.W_Pawn ^= bitpos;
                                break;
                            case PieceType.Rook:
                                copy.W_Rook ^= move.last;
                                copy.W_Rook ^= bitpos;
                                break;
                            case PieceType.Knight:
                                copy.W_Knight ^= move.last;
                                copy.W_Knight ^= bitpos;
                                break;
                            case PieceType.Bishop:
                                copy.W_Bishop ^= move.last;
                                copy.W_Bishop ^= bitpos;
                                break;
                            case PieceType.Queen:
                                copy.W_Queen ^= move.last;
                                copy.W_Queen ^= bitpos;
                                break;
                            case PieceType.King:
                                copy.W_King ^= move.last;
                                copy.W_King ^= bitpos;
                                break;
                        }
                    }
                    else
                    {
                        switch (move.pieceType)
                        {
                            case PieceType.Pawn:
                                copy.B_Pawn ^= move.last;
                                copy.B_Pawn ^= bitpos;
                                break;
                            case PieceType.Rook:
                                copy.B_Rook ^= move.last;
                                copy.B_Rook ^= bitpos;
                                break;
                            case PieceType.Knight:
                                copy.B_Knight ^= move.last;
                                copy.B_Knight ^= bitpos;
                                break;
                            case PieceType.Bishop:
                                copy.B_Bishop ^= move.last;
                                copy.B_Bishop ^= bitpos;
                                break;
                            case PieceType.Queen:
                                copy.B_Queen ^= move.last;
                                copy.B_Queen ^= bitpos;
                                break;
                            case PieceType.King:
                                copy.B_King ^= move.last;
                                copy.B_King ^= bitpos;
                                break;
                        }
                    }
                    Node n = new Node(copy, otherturn, this);
                    n.Populate(nodes);

                    stopwatch.Stop();
                    copytime += stopwatch.ElapsedTicks;
                }
                ++linescalculated;
                --threads_running;
            });
        }
    }
}
