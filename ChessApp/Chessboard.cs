using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessApp
{
    public enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King,
        None,
        Duck,
        Goose
    }
    public enum Side
    {
        White,
        Black,
        Animal //Reserved for the one and only, THE DUCK
    }
    public class Piece
    {
        public static readonly Bitmap img_BLACK_PAWN   = Properties.Resources.bP;
        public static readonly Bitmap img_BLACK_ROOK   = Properties.Resources.bR;
        public static readonly Bitmap img_BLACK_KNIGHT = Properties.Resources.bN;
        public static readonly Bitmap img_BLACK_BISHOP = Properties.Resources.bB;
        public static readonly Bitmap img_BLACK_KING   = Properties.Resources.bK;
        public static readonly Bitmap img_BLACK_QUEEN  = Properties.Resources.bQ;
               
        public static readonly Bitmap img_WHITE_PAWN   = Properties.Resources.wP;
        public static readonly Bitmap img_WHITE_ROOK   = Properties.Resources.wR;
        public static readonly Bitmap img_WHITE_KNIGHT = Properties.Resources.wN;
        public static readonly Bitmap img_WHITE_BISHOP = Properties.Resources.wB;
        public static readonly Bitmap img_WHITE_KING   = Properties.Resources.wK;
        public static readonly Bitmap img_WHITE_QUEEN  = Properties.Resources.wQ;

        public static readonly Bitmap img_DUCK  = Properties.Resources.Duck;
        public static readonly Bitmap img_GOOSE = Properties.Resources.Goose;


        public static readonly Bitmap img_NONE = Properties.Resources.Delete;

        public Bitmap IMG
        {
            get
            {
                if (pieceType == PieceType.None)
                {
                    return img_NONE;
                }
                if (pieceType == PieceType.Duck)
                {
                    return img_DUCK;
                }
                if (pieceType == PieceType.Goose)
                {
                    return img_GOOSE;
                }
                if (side == Side.Black) 
                {
                    switch (pieceType)
                    {
                        case PieceType.Pawn:
                            return img_BLACK_PAWN;
                        case PieceType.Rook:
                            return img_BLACK_ROOK;
                        case PieceType.Knight:
                            return img_BLACK_KNIGHT;
                        case PieceType.Bishop:
                            return img_BLACK_BISHOP;
                        case PieceType.Queen:
                            return img_BLACK_QUEEN;
                        case PieceType.King:
                            return img_BLACK_KING;
                    }
                }
                else
                {
                    switch (pieceType)
                    {
                        case PieceType.Pawn:
                            return img_WHITE_PAWN;
                        case PieceType.Rook:
                            return img_WHITE_ROOK;
                        case PieceType.Knight:
                            return img_WHITE_KNIGHT;
                        case PieceType.Bishop:
                            return img_WHITE_BISHOP;
                        case PieceType.Queen:
                            return img_WHITE_QUEEN;
                        case PieceType.King:
                            return img_WHITE_KING;
                    }
                }
                return null;
            }

        }
        public PieceType pieceType;
        public Side side;
        public int position;

        public Piece(PieceType type, Side side, int position)
        {
            this.pieceType = type;
            this.side = side;
            this.position = position;
        }

        public override string ToString()
        {
            if (pieceType == PieceType.Duck)
            {
                return "D";
            }
            if (pieceType == PieceType.Goose)
            {
                return "G";
            }
            if (side == Side.Black)
            {
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return "p";
                    case PieceType.Rook:
                        return "r";
                    case PieceType.Knight:
                        return "n";
                    case PieceType.Bishop:
                        return "b";
                    case PieceType.Queen:
                        return "q";
                    case PieceType.King:
                        return "k";
                }
            }
            else
            {
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return "P";
                    case PieceType.Rook:
                        return "R";
                    case PieceType.Knight:
                        return "N";
                    case PieceType.Bishop:
                        return "B";
                    case PieceType.Queen:
                        return "Q";
                    case PieceType.King:
                        return "K";
                }
            }
            return "";
        }
    }
    public class CastleOptions
    {
        public Side side;
        public bool Queenside;
        public bool Kingside;

        public CastleOptions(Side side, bool queenside, bool kingside)
        {
            this.side = side;
            Queenside = queenside;
            Kingside = kingside;
        }

        public string ToString(CastleOptions prev = null)
        {
            string result = "";
            if (side == Side.Black)
            {
                if (Kingside)
                {
                    result += "k";
                }
                if (Queenside)
                {
                    result += "q";
                }
                if (!Queenside && !Kingside && !prev.Queenside && !prev.Kingside)
                {
                    result = "-";
                }
            }
            else
            {
                if (Kingside)
                {
                    result += "K";
                }
                if (Queenside)
                {
                    result += "Q";
                }
            }
            return result;
        }
    }
    public class Chessboard
    {
        public List<Piece> Pieces = new List<Piece>();
        public Side hasturn = Side.White;
        public CastleOptions blackCastles;
        public CastleOptions whiteCastles;

        string FEN = "";

        public static double totalticks = 0;
        public Bitboard bitboard;
        public Chessboard(string fEN)
        {
            FEN = fEN;
            ParseFEN();
            bitboard = Bitboard.FromBoard(this);
        }
        public void Event(object sender, EventArgs e)
        {

        }
        public void ParseFEN()
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
                    Piece toadd = null;
                    int position = x + (7 - row) * 8;
                    switch (line[i])
                    {
                        //Black pieces
                        case 'p':
                            toadd = new Piece(PieceType.Pawn, Side.Black, position);
                            break;
                        case 'r':
                            toadd = new Piece(PieceType.Rook, Side.Black, position);
                            break;
                        case 'n':
                            toadd = new Piece(PieceType.Knight, Side.Black, position);
                            break;
                        case 'b':
                            toadd = new Piece(PieceType.Bishop, Side.Black, position);
                            break;
                        case 'k':
                            toadd = new Piece(PieceType.King, Side.Black, position);
                            break;
                        case 'q':
                            toadd = new Piece(PieceType.Queen, Side.Black, position);
                            break;

                        //White pieces
                        case 'P':
                            toadd = new Piece(PieceType.Pawn, Side.White, position);
                            break;
                        case 'R':
                            toadd = new Piece(PieceType.Rook, Side.White, position);
                            break;
                        case 'N':
                            toadd = new Piece(PieceType.Knight, Side.White, position);
                            break;
                        case 'B':
                            toadd = new Piece(PieceType.Bishop, Side.White, position);
                            break;
                        case 'K':
                            toadd = new Piece(PieceType.King, Side.White, position);
                            break;
                        case 'Q':
                            toadd = new Piece(PieceType.Queen, Side.White, position);
                            break;
                        //Duck
                        case 'D':
                            toadd = new Piece(PieceType.Duck, Side.Animal, position);
                            break;
                        //Goose
                        case 'G':
                            toadd = new Piece(PieceType.Goose, Side.Animal, position);
                            break;
                    }
                    Pieces.Add(toadd);
                    ++x;
                }
            }
            var hasturn = strs[1];
            if (hasturn == "w")
            {
                this.hasturn = Side.White;
            }
            else
            {
                this.hasturn = Side.Black;
            }
            if (strs.Length == 2)
            {
                return;
            }
            var castles = strs[2];
            whiteCastles = new CastleOptions(Side.White, false, false);
            blackCastles = new CastleOptions(Side.Black, false, false);

            if (castles.Contains("K"))
            {
                whiteCastles.Kingside = true;
            }
            if (castles.Contains("Q"))
            {
                whiteCastles.Queenside = true;
            }
            if (castles.Contains('k'))
            {
                blackCastles.Kingside = true;
            }
            if (castles.Contains('q'))
            {
                blackCastles.Queenside = true;
            }
        }
        public string GetFEN()
        {
            string result = "";
            int curr_row = 7;
            int colsused = 0;
            var set = Pieces.OrderByUpsideGrid();
            for (int i = 0; i < set.Count; i++)
            {
                Piece p = set[i];
                if (p.position / 8 != curr_row)
                {
                    if (8-colsused != 0)
                    {
                        result += (8 - colsused).ToString();
                    }
                    result += "/";
                    for (int j = 0; j < ((curr_row - 1) - (p.position / 8)); ++j)
                    {
                        result += "8/";
                    }
                    curr_row = p.position / 8;
                    if (p.position % 8 != 0)
                    {
                        result += p.position % 8;
                    }
                    colsused = p.position % 8;
                }
                if (p.position % 8 != colsused)
                {
                    int used = ((p.position % 8) - (colsused));
                    if (used != 8)
                    {
                        result += used.ToString();
                        colsused += used;
                    }
                }
                result += p.ToString();
                colsused++;
                if (colsused == 0)
                {
                    colsused = 8;
                    result += "/";
                }
            }
            if (colsused != 8)
            {
                int used = (8 - (colsused));
                if (used != 8)
                {
                    result += used.ToString();
                }
            }
            for (int j = 0; j < (set.Count() == 0 ? 8 : set.Last().position/8); ++j)
            {
                result += "/8";
            }
            if (set.Count() == 0)
            {
                result = result.Substring(1);
            }
            result += String.Format(" {0} ", hasturn == Side.White ? "w" : "b");
            result += whiteCastles.ToString();
            result += blackCastles.ToString(whiteCastles);
            result += " - 0 1";
            return result;
        }

        internal Piece PieceAt(int i)
        {
            var piece = Pieces.Where(p=>p.position == i).FirstOrDefault();
            return piece;
        }

        internal void Reload()
        {
            Pieces.Clear();

            blackCastles.Kingside = bitboard.B_KingsideCastle;
            blackCastles.Queenside = bitboard.B_QueensideCastle;

            whiteCastles.Kingside = bitboard.W_KingsideCastle;
            whiteCastles.Queenside = bitboard.W_QueensideCastle;

            List<PieceType> pieceTypes = new List<PieceType>() {PieceType.Pawn, PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King};
            foreach (var piecetype in pieceTypes) 
            {
                foreach (var position in BitOperations.Bitloop(bitboard.GetBitboard(piecetype, Side.White)))
                {
                    Pieces.Add(new Piece(piecetype, Side.White, position));
                }
                foreach (var position in BitOperations.Bitloop(bitboard.GetBitboard(piecetype, Side.Black)))
                {
                    Pieces.Add(new Piece(piecetype, Side.Black, position));
                }
            }
            if (bitboard.Duck != 0) //Placing a duck?
            {
                Pieces.Add(new Piece(PieceType.Duck, Side.Animal, BitOperations.TrailingZeros(bitboard.Duck)-1));
            }
        }
    }
}