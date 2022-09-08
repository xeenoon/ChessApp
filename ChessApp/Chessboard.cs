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
    }
    public enum Side
    {
        White,
        Black
    }
    public struct Position
    {
        public int x;
        public int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public byte val
        {
            get
            {
                return (byte)(((7 - y) * 8) + x);
            }
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.x == b.x && a.y== b.y;
        }
        public static bool operator !=(Position a, Position b)
        { 
            return a.x != b.x || a.y != b.y;
        }

        public static bool operator ==(Position a, System.Drawing.Point b)
        {
            return a.x == b.X && a.y == b.Y;
        }
        public static bool operator !=(Position a, System.Drawing.Point b)
        {
            return a.x != b.X || a.y != b.Y;
        }
    }
    public class Piece
    {
        public static readonly Bitmap img_BLACK_PAWN   = Properties.Resources.BlackPawn;
        public static readonly Bitmap img_BLACK_ROOK   = Properties.Resources.BlackRook;
        public static readonly Bitmap img_BLACK_KNIGHT = Properties.Resources.BlackKnight;
        public static readonly Bitmap img_BLACK_BISHOP = Properties.Resources.BlackBishop;
        public static readonly Bitmap img_BLACK_KING   = Properties.Resources.BlackKing;
        public static readonly Bitmap img_BLACK_QUEEN  = Properties.Resources.BlackQueen;
               
        public static readonly Bitmap img_WHITE_PAWN   = Properties.Resources.WhitePawn;
        public static readonly Bitmap img_WHITE_ROOK   = Properties.Resources.WhiteRook;
        public static readonly Bitmap img_WHITE_KNIGHT = Properties.Resources.WhiteKnight;
        public static readonly Bitmap img_WHITE_BISHOP = Properties.Resources.WhiteBishop;
        public static readonly Bitmap img_WHITE_KING   = Properties.Resources.WhiteKing;
        public static readonly Bitmap img_WHITE_QUEEN  = Properties.Resources.WhiteQueen;
        public Bitmap IMG
        {
            get
            {
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
        public Position position;

        public Piece(PieceType type, Side side, Position position)
        {
            this.pieceType = type;
            this.side = side;
            this.position = position;
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
    }
    public class Chessboard
    {
        public List<Piece> Pieces = new List<Piece>();
        Side hasturn;
        CastleOptions blackCastles;
        CastleOptions whiteCastles;

        string FEN = "";

        public static double totalticks = 0;
        public Chessboard(string fEN)
        {
            FEN = fEN;
            ParseFEN();
            Bitboard B = new Bitboard(FEN);
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
                    switch (line[i])
                    {
                        //Black pieces
                        case 'p':
                            toadd = new Piece(PieceType.Pawn, Side.Black, new Position(x, row));
                            break;
                        case 'r':
                            toadd = new Piece(PieceType.Rook, Side.Black, new Position(x, row));
                            break;
                        case 'n':
                            toadd = new Piece(PieceType.Knight, Side.Black, new Position(x, row));
                            break;
                        case 'b':
                            toadd = new Piece(PieceType.Bishop, Side.Black, new Position(x, row));
                            break;
                        case 'k':
                            toadd = new Piece(PieceType.King, Side.Black, new Position(x, row));
                            break;
                        case 'q':
                            toadd = new Piece(PieceType.Queen, Side.Black, new Position(x, row));
                            break;

                        //White pieces
                        case 'P':
                            toadd = new Piece(PieceType.Pawn, Side.White, new Position(x, row));
                            break;
                        case 'R':
                            toadd = new Piece(PieceType.Rook, Side.White, new Position(x, row));
                            break;
                        case 'N':
                            toadd = new Piece(PieceType.Knight, Side.White, new Position(x, row));
                            break;
                        case 'B':
                            toadd = new Piece(PieceType.Bishop, Side.White, new Position(x, row));
                            break;
                        case 'K':
                            toadd = new Piece(PieceType.King, Side.White, new Position(x, row));
                            break;
                        case 'Q':
                            toadd = new Piece(PieceType.Queen, Side.White, new Position(x, row));
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
            whiteCastles = new CastleOptions(Side.Black, false, false);
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
    }
}