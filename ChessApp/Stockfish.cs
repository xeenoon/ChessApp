using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChessApp
{
    internal static class Stockfish
    {
        public static void Start()
        {
            Process.Start(@"C:\Users\ccw10\Downloads\stockfish_15_win_x64\stockfish_15_x64.exe");
        }
        public static Move GetMove()
        {
            return new Move(4, 12, PieceType.Pawn);
        }
    }
}
