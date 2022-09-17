﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessApp
{
    internal class Squares
    {
        private Square[] squares = new Square[64];
        Point offset;
        int size;
        public Chessboard board;

        Color light;
        Color dark;
        Color select;
        Color move;

        Form1 parent;

        public List<Bitboard.BoardData> undomoves = new List<Bitboard.BoardData>();
        public Squares(Chessboard board, Point offset, int size, Color light, Color dark, Graphics g, Color select, Color move, Form1 parent)
        {
            this.offset = offset;
            this.size = size;
            this.board = board;
            this.light = light;
            this.dark = dark;
            this.select = select;
            this.move = move;
            this.parent = parent;

            Reload(g);
        }

        public void Reload(Graphics g)
        {
            for (int i = 0; i < 64; ++i) //Draw all the squares
            {
                int xpos = (i % 8);
                int ypos = 7 - (i / 8);

                Rectangle bounds = new Rectangle(offset.X + xpos * size, offset.Y + ypos * size, size, size);
                squares[i] = new Square(bounds, board.PieceAt(i), (i + i / 8) % 2 == 1 ? light : dark, i, 1ul << i, g, this, select, move);
                squares[i].Paint();
            }
        }

        internal void Paint(Graphics graphics)
        {
            foreach (var square in squares)
            {
                square.g = graphics;
                square.Paint();
            }
        }

        internal Square SquareAt(Point location)
        {
            location.X = location.X - offset.X;
            location.Y = location.Y - offset.Y;

            location.X /= size;
            location.Y /= size;

            location.Y = 7 - location.Y;

            int index = location.X + location.Y * 8;
            if (index <= -1 || index >= 64)
            {
                return null;
            }
            return squares[index];
        }
        public Square highlight;
        public List<Square> moveSquares = new List<Square>();

        internal void UndoMove()
        {
            if (undomoves.Count == 0)
            {
                return;
            }
            board.bitboard.UndoMove(undomoves.Last());
            undomoves.Remove(undomoves.Last());

            board.hasturn = board.hasturn == Side.White ? Side.Black : Side.White;
            board.Reload();
        }

        public Square this[int index]
        {
            get
            {
                return squares[index];
            }
        }
        public bool AI_can_move;
        internal void AiMove()
        {
            if (AI_can_move)
            {
                board.bitboard.SetupSquareAttacks();
                if (MoveGenerator.MoveCount(board.bitboard, board.hasturn) == 0)
                {
                    return;
                }
                var move = AI.GetMove(board.bitboard, board.hasturn);
                undomoves.Add(board.bitboard.Move(move.last, move.current, 1ul<<move.last, 1ul<<move.current, move.pieceType, board.hasturn));
                board.Reload();
                parent.Reload();
                board.hasturn = board.hasturn == Side.White ? Side.Black : Side.White;
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
            if (squares.moveSquares.Contains(this)) //Are we moving here?
            {
                squares.highlight.Move(location);
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
           if (piece != null && squares.board.hasturn == piece.side) //Displaying moves?
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

            squares.undomoves.Add(squares.board.bitboard.Move((byte)this.location, (byte)location, 1ul << this.location, 1ul << location, piece.pieceType, piece.side));

            if (piece.pieceType == PieceType.Pawn && (location / 8 == 7 || location / 8 == 0))
            {
                piece.pieceType = PieceType.Queen;
            }

            piece.position = location;
            squares[location].piece = piece;

            squares.board.hasturn = piece.side == Side.White ? Side.Black : Side.White;

            piece = null;

            squares.AiMove();
        }

        internal void MoveHighlight()
        {
            squares.moveSquares.Add(this);
        }
    }
}
