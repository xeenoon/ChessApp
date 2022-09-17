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
        const int KNIGHT_VALUE = 310;
        const int BISHOP_VALUE = 320;
        const int ROOK_VALUE = 500;
        const int QUEEN_VALUE = 900;

        internal static int Evaluate(Bitboard bitboard)
        {
            int whitevalue = 0;
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Pawn) * PAWN_VALUE);
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Knight) * KNIGHT_VALUE);
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Bishop) * BISHOP_VALUE);
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Rook) * ROOK_VALUE);
            whitevalue += (int)(BitOperations.NumberOfSetBits(bitboard.W_Queen) * QUEEN_VALUE);


            int blackvalue = 0;
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Pawn) * PAWN_VALUE);
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Knight) * KNIGHT_VALUE);
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Bishop) * BISHOP_VALUE);
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Rook) * ROOK_VALUE);
            blackvalue += (int)(BitOperations.NumberOfSetBits(bitboard.B_Queen) * QUEEN_VALUE);

            return whitevalue - blackvalue;
        }
    }
}
