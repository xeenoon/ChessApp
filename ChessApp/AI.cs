using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    public class AI
    {
        public static Move GetMove(Bitboard currentState, Side hasturn)
        {
            var bitboard = currentState.Copy();
            bitboard.SetupSquareAttacks();
            var moves = MoveGenerator.CalculateAll(bitboard, hasturn);
            Random r = new Random();
            int idx = r.Next(0,moves.Count-1);

            return moves[idx];
        }
    }
}
