using ChessApp;

namespace Precompile
{
    class Program
    {
        static string LocalPath = @"C:\Users\" + Environment.UserName.ToString() + @"\Downloads\masks.txt";
        static void Main(string[] args)
        {
            Console.WriteLine("Printing data to: " + LocalPath);
            string towrite = "public static ulong[] Up = new ulong[512] { ";

            for (int rook_loc = 0; rook_loc < 64; ++rook_loc)
            {
                for (int lowestblocker = 0; lowestblocker < 8; ++lowestblocker)
                {
                    Bitboard bitboard = new Bitboard();
                    bitboard.squares_to_block_check = ulong.MaxValue;
                    bitboard.W_Rook |= 1ul << rook_loc; //Add the rook
                    bitboard.B_Pawn |= 1ul << (rook_loc % 8) + lowestblocker * 8;
                    if (bitboard.W_Rook == bitboard.B_Pawn)
                    {
                        towrite += "0UL,";
                    }
                    else
                    {
                        var moves = MoveGenerator.RookMoves((byte)rook_loc, bitboard, Side.White);
                        moves &= MoveGenerator.up[rook_loc];
                        towrite += (moves + "UL,");
                    }
                }
            }
            towrite = towrite.Substring(0,towrite.Length-1); //Remove last character
            towrite += "};\n";
            if (!File.Exists(LocalPath))
            {
                var file = File.Create(LocalPath);
                file.Close();
            }
            File.WriteAllText(LocalPath, towrite);
            Console.WriteLine("Finished printing data");
            Console.ReadLine();
        }
    }
}