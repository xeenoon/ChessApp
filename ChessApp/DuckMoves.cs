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
            if (b.Goose == 0)
            {
                return ~(b.WhitePieces | b.BlackPieces | b.Animals);
            }
            return ~(b.WhitePieces | b.BlackPieces | b.Animals) & (~MoveGenerator.king[BitOperations.TrailingZeros(b.Goose)-1]);
        }
        public static List<byte> DuckPositions(Bitboard b)
        {
            List<byte> result = new List<byte>();
            var moves = GetMoves(b);
            while (moves != 0)
            {
                byte lsb = (byte)(BitOperations.TrailingZeros(moves) - 1);
                ulong bitpos = 1ul << lsb;
                moves ^= bitpos;
                result.Add(lsb);
            }
            return result;
        }
    }
}