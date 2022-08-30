namespace ChessApp
{
    internal class Bitboard
    {
        public ulong W_Pawn = 0ul;   //White Pawns
        public ulong W_Rook = 0ul;   //White Rooks
        public ulong W_Knight = 0ul; //White Knights
        public ulong W_Bishop = 0ul; //White Bishops
        public ulong W_Queen = 0ul;  //White Queens
        public ulong W_King = 0ul;   //White King

        public ulong B_Pawn = 0ul;   //Black Pawns
        public ulong B_Rook = 0ul;   //Black Rooks
        public ulong B_Knight = 0ul; //Black Knights
        public ulong B_Bishop = 0ul; //Black Bishops
        public ulong B_Queen = 0ul;  //Black Queens
        public ulong B_King = 0ul;   //Black King

        public ulong BlackPieces
        {
            get
            {
                return B_Pawn | B_Rook | B_Knight | B_Bishop | B_King | B_Queen;
            }
        }
        public ulong WhitePieces
        {
            get
            {
                return W_Pawn | W_Rook | W_Knight | W_Bishop | W_King | W_Queen;
            }
        }

        public Bitboard(string FEN)
        {
            string[] strs = FEN.Split(' ');
            string[] lines = strs[0].Split('/');
            for (int row = 0; row < lines.Length; ++row)
            {
                var line = lines[row];
                int x = 0;
                for (int i = 0; i < line.Length; ++i)
                {
                    if (char.IsDigit(line[i])) //Jumping foward?
                    {
                        x += int.Parse(line[i].ToString());
                        continue;
                    }
                    ulong position = (1ul << (((7-row) * 8) + x));
                    switch (line[i])
                    {
                        //White pieces
                        case 'p':
                            B_Pawn |= position;
                            break;
                        case 'r':
                            B_Rook |= position;
                            break;
                        case 'n':
                            B_Knight |= position;
                            break;
                        case 'b':
                            B_Bishop |= position;
                            break;
                        case 'k':
                            B_King |= position;
                            break;
                        case 'q':
                            B_Queen |= position;
                            break;

                        //Black pieces
                        case 'P':
                            W_Pawn |= position;
                            break;
                        case 'R':
                            W_Rook |= position; 
                            break;
                        case 'N':
                            W_Knight |= position; 
                            break;
                        case 'W':
                            W_Bishop |= position; 
                            break;
                        case 'K':
                            W_King |= position; 
                            break;
                        case 'Q':
                            W_Queen |= position;
                            break;
                    }
                    ++x;
                }
            }
        }
    }
}