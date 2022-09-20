using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    internal static class Evaluation
    {

        const int PAWN_VALUE = 100;
        const int KNIGHT_VALUE = 320;
        const int BISHOP_VALUE = 330;
        const int ROOK_VALUE = 500;
        const int QUEEN_VALUE = 900;

        public static int[] BlackPawnVal = new int[64]
        {
             0,  0,  0,  0,  0,  0,  0,  0,
             50, 50, 50, 50, 50, 50, 50, 50,
             10, 10, 20, 30, 30, 20, 10, 10,
             5,  5, 10, 25, 25, 10,  5,  5,
             0,  0,  0, 20, 20,  0,  0,  0,
             5, -5,-10,  0,  0,-10, -5,  5,
             5, 10, 10,-20,-20, 10, 10,  5,
             0,  0,  0,  0,  0,  0,  0,  0
        };
        public static int[] WhitePawnVal = new int[64]
        {
             0,  0,  0,  0,  0,  0,  0,  0,
             5, 10, 10,-20,-20, 10, 10,  5,
             5, -5,-10,  0,  0,-10, -5,  5,
             0,  0,  0, 20, 20,  0,  0,  0,
             5,  5, 10, 25, 25, 10,  5,  5,
             10, 10, 20, 30, 30, 20, 10, 10,
             50, 50, 50, 50, 50, 50, 50, 50,
             0,  0,  0,  0,  0,  0,  0,  0
        };

        public static int[] KnightVal = new int[64]
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50,
        };

        public static int[] BlackBishopVal = new int[64]
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 15, 15,  5,  5,-10,
            -10,  0, 10, 15, 15, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10, 15,  0,  0,  0,  0, 15,-10,
            -20,-10,-10,-10,-10,-10,-10,-20,
        };

        public static int[] WhiteBishopVal = new int[64]
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10, 15,  0,  0,  0,  0, 15,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  0, 10, 15, 15, 10,  0,-10,
            -10,  5,  5, 15, 15,  5,  5,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -20,-10,-10,-10,-10,-10,-10,-20,
        };

        public static int[] BlackRookVal = new int[64]
        {
             0,  0,  0,  0,  0,  0,  0,  0,
             5, 30, 30, 30, 30, 30, 30,  5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
             0,  0,  0,  10,  10, 0,  0,  0
        };
        public static int[] WhiteRookVal = new int[64]
        {
             0,  0,  0,  10,  10, 0,  0,  0,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
             5, 30, 30, 30, 30, 30, 30,  5,
             0,  0,  0,  0,  0,  0,  0,  0
        };
        public static int[] BlackQueenVal = new int[64]
        {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5,  5,  5,  5,  0,-10,
             -5,  0,  5,  5,  5,  5,  0, -5,
              0,  0,  5,  5,  5,  5,  0, -5,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        };
        public static int[] WhiteQueenVal = new int[64]
        {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -10,  5,  5,  5,  5,  5,  0,-10,
              0,  0,  5,  5,  5,  5,  0, -5,
             -5,  0,  5,  5,  5,  5,  0, -5,
            -10,  0,  5,  5,  5,  5,  0,-10,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        };

        public static int[] BlackKingVal = new int[64]
        {
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -10,-20,-20,-20,-20,-20,-20,-10,
             20, 20,-10,-10,-10,-10, 20, 20,
             20, 50, 10,-10,-10,-10, 50, 20
        };
        public static int[] WhiteKingVal = new int[64]
        {
             20, 50,-10,-10,-10, 10, 50, 20,
             20, 20,-10,-10,-10,-10, 20, 20,
            -10,-20,-20,-20,-20,-20,-20,-10,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
        };

        internal static int Evaluate(Bitboard bitboard, Side hasmove, bool nomoves = false)
        {
            //we are only checking to see if hasmove is checkmated
            Side opposite = hasmove == Side.White ? Side.Black : Side.White;
            bitboard.SetupSquareAttacks(opposite);
        
            if (nomoves)
            {
                if ((bitboard.doublecheck || bitboard.check)) //Checkmate?
                {
                    if (hasmove == Side.White)
                    {
                        return int.MinValue;
                    }
                    else
                    {
                        return int.MaxValue;
                    }
                }
                else //Stalemate?
                {
                    return 0;
                }
            }

            else if (bitboard.doublecheck)
            {
                bitboard.xrays.Clear();
                bitboard.SetupPins();
                byte position = (byte)(BitOperations.TrailingZeros(hasmove == Side.White ? bitboard.W_King : bitboard.B_King) - 1);

                var kingmoves = MoveGenerator.Moves(PieceType.King, hasmove, position, bitboard);
                if (BitOperations.NumberOfSetBits(kingmoves) == 0) //Can the king not move anywhere?
                {
                    //Checkmate
                    if (hasmove == Side.White)
                    {
                        return int.MinValue;
                    }
                    else
                    {
                        return int.MaxValue;
                    }
                }
            }
            else if (bitboard.check) //Check for a checkmate
            {
                bitboard.xrays.Clear();
                bitboard.SetupPins();

                var moves = MoveGenerator.MoveCount(bitboard, hasmove);
                if (moves == 0)
                {
                    //Checkmate
                    if (hasmove == Side.White)
                    {
                        return int.MinValue;
                    }
                    else
                    {
                        return int.MaxValue;
                    }
                }
            }

            
            

            int whitevalue = 0;
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Pawn) * PAWN_VALUE);
            whitevalue += MultiplyValues(bitboard.W_Pawn, WhitePawnVal);
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Knight) * KNIGHT_VALUE);
            whitevalue += MultiplyValues(bitboard.W_Knight, KnightVal);
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Bishop) * BISHOP_VALUE);
            whitevalue += MultiplyValues(bitboard.W_Bishop, WhiteBishopVal);
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Rook) * ROOK_VALUE);
            whitevalue += MultiplyValues(bitboard.W_Rook, WhiteRookVal);
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Queen) * QUEEN_VALUE);
            whitevalue += MultiplyValues(bitboard.W_Queen, WhiteQueenVal);
            whitevalue += MultiplyValues(bitboard.W_King, WhiteKingVal);


            int blackvalue = 0;
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Pawn) * PAWN_VALUE);
            blackvalue += MultiplyValues(bitboard.B_Pawn, BlackPawnVal);
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Knight) * KNIGHT_VALUE);
            blackvalue += MultiplyValues(bitboard.B_Knight, KnightVal);
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Bishop) * BISHOP_VALUE);
            blackvalue += MultiplyValues(bitboard.B_Bishop, BlackBishopVal);
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Rook) * ROOK_VALUE);
            blackvalue += MultiplyValues(bitboard.B_Rook, BlackRookVal);
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Queen) * QUEEN_VALUE);
            blackvalue += MultiplyValues(bitboard.B_Queen, BlackQueenVal);
            blackvalue += MultiplyValues(bitboard.B_King, BlackKingVal);

            return whitevalue - blackvalue;
        }

        public static int MultiplyValues(ulong bitboard, int[] values)
        {
            int result = 0;
            foreach (var lsb in BitOperations.Bitloop(bitboard))
            {
                result += values[lsb];
            }
            return result;
        }
    }
}