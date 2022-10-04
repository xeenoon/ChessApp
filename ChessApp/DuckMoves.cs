using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    public class DuckMoves
    {
        private static ulong GetMoves(Bitboard b)
        {
            return ~(b.WhitePieces | b.BlackPieces);
        }
        public static List<byte> DuckPositions(Bitboard b)
        {
            List<byte> result = new List<byte>();
            var moves = GetMoves(b);
            while (moves != 0)
            {
                byte lsb = (byte)(BitOperations.TrailingZeros(moves) - 1);
                ulong bitpos = 1ul << lsb;
                moves ^= bitpos; //remove this piece from the ulong of pieces
                result.Add(lsb);
            }
            return result;
        }
    }
}