﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessApp
{
    internal class Squares
    {
        private Square[] squares = new Square[64];
        Point offset;
        Point edit_offset;
        int size;
        public Chessboard board;

        public Squares(Chessboard board, Point offset, int size, Color light, Color dark, Graphics g, Color select, Color move)
        {
            this.offset = offset;
            DrawEdit(g);
            this.size = size;
            this.board = board;

            for (int i = 0; i < 64; ++i) //Draw all the squares
            {
                int xpos = (i % 8);
                int ypos = 7 - (i / 8);

                Rectangle bounds = new Rectangle(offset.X + xpos * size, offset.Y + ypos * size, size, size);
                squares[i] = new Square(bounds, board.PieceAt(i), (i + i/8) % 2 == 1 ? light : dark, i, 1ul<<i, g, this, select, move);
                squares[i].Paint();
            }
        }

        internal void Paint(Graphics graphics)
        {
            DrawEdit(graphics);
            foreach (var square in squares)
            {
                square.g = graphics;
                square.Paint();
            }
        }

        internal Square SquareAt(Point location)
        {
            if (EditSquareAt(location) != null)
            {
                return null;
            }
            location.X = location.X - offset.X;
            location.Y = location.Y - offset.Y;

            location.X /= size;
            location.Y /= size;

            location.Y = 7 - location.Y;
            if (location.X <= -1 || location.X >= 8 || location.Y <= -1 || location.Y >= 8)
            {
                return null;
            }
            int index = location.X + location.Y * 8;
            if (index <= -1 || index >= 64)
            {
                return null;
            }
            return squares[index];
        }
        public EditSquare selected_edit;
        internal EditSquare EditSquareAt(Point location)
        {
            if (editSquares == null)
            {
                return null;
            }
            location.X = location.X - edit_offset.X;
            location.Y = location.Y - edit_offset.Y;

            location.Y /= (size+Vertical_Indent);

            if (location.Y >= 7)
            {
                return null; //clicked too low
            }

            int left = editSquares[0].realworld.X;
            int right = size + Horizontal_Indent + left;
            int startidx = location.X > left && location.X < right ? 0 : 6;
            if (startidx == 6) //Check if mouse is actually on the right
            {
                int farleft = editSquares[6].realworld.X;
                int farright = farleft + size + Horizontal_Indent;
                if (!(location.X > farleft && location.X < farright))
                {
                    return null;
                }
            }

            startidx += location.Y;
            return editSquares[startidx];
        }
        public Square highlight;
        public List<Square> moveSquares = new List<Square>();
        public bool edit;
        public static int Horizontal_Indent = 5;
        public static int Vertical_Indent = 5;
        internal void SetupEdit(bool indented)
        {
            edit = indented;
            moveSquares.Clear();

            foreach (var square in squares)
            {
                square.realworld = new Rectangle(square.realworld.X + (indented ? size+Horizontal_Indent : -(size+Horizontal_Indent)), square.realworld.Y, size, size);
            }
            if (indented) 
            {
                edit_offset = offset;
                editSquares = new EditSquare[12];
                Rectangle bounds = new Rectangle(this.offset.X, this.offset.Y, size, size);
                editSquares[0] = new EditSquare(PieceType.King, Side.Black, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[1] = new EditSquare(PieceType.Queen, Side.Black, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[2] = new EditSquare(PieceType.Rook, Side.Black, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[3] = new EditSquare(PieceType.Bishop, Side.Black, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[4] = new EditSquare(PieceType.Knight, Side.Black, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[5] = new EditSquare(PieceType.Pawn, Side.Black, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;

                bounds.X = bounds.X + 9 * size + Horizontal_Indent * 2;
                bounds.Y = offset.Y;

                editSquares[6] = new EditSquare(PieceType.King, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[7] = new EditSquare(PieceType.Queen, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[8] = new EditSquare(PieceType.Rook, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[9] = new EditSquare(PieceType.Bishop, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[10] = new EditSquare(PieceType.Knight, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[11] = new EditSquare(PieceType.Pawn, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;

                offset = new Point(offset.X + Horizontal_Indent + size, offset.Y);
            }
            else
            {
                editSquares = null;
                selected_edit = null;
                offset = new Point(offset.X - (Horizontal_Indent + size), offset.Y);
            }
        }
        EditSquare[] editSquares;
        public void DrawEdit(Graphics g)
        {
            if (edit)
            {
                foreach (var sqr in editSquares)
                {
                    sqr.Paint(g);
                }
            }
        }
        public Square this[int index]
        {
            get
            {
                return squares[index];
            }
        }
    }
    internal class Square
    {
        Squares squares;

        public Rectangle realworld;
        public int location;
        public ulong bitboard_location;

        public Piece piece;
        public Color color;

        public Graphics g;

        public Color selectcolor;
        public Color movecolor;

        public Square(Rectangle bounds, Piece piece, Color color, int location, ulong bitboard_location, Graphics g, Squares squares, Color selectcolor, Color movecolor)
        {
            this.realworld = bounds;
            this.piece = piece;
            this.color = color;
            this.location = location;
            this.g = g;
            this.squares = squares;
            this.bitboard_location = bitboard_location;

            this.movecolor = movecolor;
            this.selectcolor = selectcolor;
        }

        public void Paint()
        {
            g.FillRectangle(new Pen(color).Brush,realworld);
            if (piece != null) 
            {
                g.DrawImage(piece.IMG, realworld);
            }
            if (squares.highlight == this)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawEllipse(new Pen(movecolor,2), realworld.X + 1, realworld.Y + 1, realworld.Width-3, realworld.Height-2);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
            if (squares.moveSquares.Contains(this))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillEllipse(new Pen(movecolor).Brush, realworld.X + 5, realworld.Y + 5, realworld.Width - 10, realworld.Height - 10);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
        }

        internal void Click()
        {
            if (squares.selected_edit != null)
            {
                if (piece != null && squares.selected_edit.pieceType == piece.pieceType)
                {
                    squares.board.Pieces.Remove(this.piece);
                    piece = null;
                    squares.board.bitboard = Bitboard.FromBoard(squares.board);
                    ((Form1)(Form1.ActiveForm)).WriteFEN();
                    return;
                }

                piece = new Piece(squares.selected_edit.pieceType, squares.selected_edit.side, location);
                squares.board.Pieces.Add(piece);
                squares.board.bitboard = Bitboard.FromBoard(squares.board);
                ((Form1)(Form1.ActiveForm)).WriteFEN();
                return;
            }

            if (squares.moveSquares.Contains(this)) //Are we moving here?
            {
                squares.highlight.Move(location);
                if (squares.selected_edit == null && squares.edit)
                {
                    squares.board.bitboard = Bitboard.FromBoard(squares.board);
                    ((Form1)(Form1.ActiveForm)).WriteFEN();
                }
            }
            squares.moveSquares.Clear();

            if (squares.highlight == this)
            {
                squares.highlight = null;
                return;
            }
            else
            {
                squares.highlight = this;
            }
           if (piece != null && (squares.board.hasturn == piece.side || squares.edit)) //Displaying moves?
           {

               var board = squares.board.bitboard.Copy();
               board.SetupSquareAttacks();
        
               var moves = MoveGenerator.Moves(piece.pieceType, piece.side, (byte)location, board);
               while (moves != 0ul)
               {
                   byte lsb = (byte)(BitOperations.TrailingZeros(moves) - 1);
                   moves ^= 1ul<<lsb;
                   squares[lsb].MoveHighlight();
               }
           }
        }

        private void Move(int location)
        {
            squares.board.Pieces.Remove(squares.board.PieceAt(location));

            if (piece.pieceType == PieceType.Pawn && Math.Abs(location - this.location) % 8 != 0 && squares.board.PieceAt(location) == null) //En passante?
            {
                if (piece.side == Side.White && this.location /8 == 4 && squares.board.bitboard.enpassent == location % 8)
                {
                    squares.board.Pieces.Remove(squares.board.PieceAt(location - 8));
                    squares[location - 8].piece = null;
                }
                else if(this.location / 8 == 3 && squares.board.bitboard.enpassent == location % 8)
                {
                    squares.board.Pieces.Remove(squares.board.PieceAt(location + 8));
                    squares[location + 8].piece = null;
                }
            }
            if (piece.pieceType == PieceType.King && (location - this.location) == 2) //Kingside castle?
            {
                var kingsiderook = squares.board.PieceAt(location + 1);
                squares[location + 1].piece = null;
                squares[location - 1].piece = kingsiderook;
                kingsiderook.position = location - 1;
            }
            if (piece.pieceType == PieceType.King && (location - this.location) == -2) //Queenside castle?
            {
                var kingsiderook = squares.board.PieceAt(location - 2);
                squares[location - 2].piece = null;
                squares[location + 1].piece = kingsiderook;
                kingsiderook.position = location + 1;
            }

            squares.board.bitboard = squares.board.bitboard.Move((byte)this.location, (byte)location, 1ul << this.location, 1ul << location, piece.pieceType, piece.side);

            if (piece.pieceType == PieceType.Pawn && (location / 8 == 7 || location / 8 == 0))
            {
                piece.pieceType = PieceType.Queen;
            }

            piece.position = location;
            squares[location].piece = piece;
            if (!squares.edit)
            {
                squares.board.hasturn = piece.side == Side.White ? Side.Black : Side.White;
            }
            piece = null;
        }

        internal void MoveHighlight()
        {
            squares.moveSquares.Add(this);
        }
    }
    internal class EditSquare
    {
        public PieceType pieceType;
        public Side side;
        public Rectangle realworld;
        public bool selected;
        public Squares squares;

        public EditSquare(PieceType pieceType, Side side, Rectangle rectangle, Squares squares)
        {
            this.pieceType = pieceType;
            this.side = side;
            this.realworld = rectangle;
            this.squares = squares;
        }

        public void Paint(Graphics g)
        {
            if (selected)
            {
                g.FillRectangle(new Pen(Color.Gray).Brush, realworld);
            }
            else
            {
                g.DrawRectangle(new Pen(Color.Gray), realworld);
            }
            g.DrawImage(new Piece(pieceType, side, -1).IMG, realworld);
        }
        public void Click()
        {
            if (squares.selected_edit != this)
            {
                if (squares.selected_edit != null)
                {
                    squares.selected_edit.Click();
                }
                selected = true;
                squares.selected_edit = this;
            }
            else
            {
                selected = false;
                squares.selected_edit = null;
            }
        }
    }
}
