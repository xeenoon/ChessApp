﻿using ColorExtensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChessApp
{
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
        public static Filter check_filter = new Filter(250, -150, -150);
        public static Filter danger_filter = new Filter(100, -100, -100);
        public static Filter move_filter = new Filter(0, 50, 0);
        public bool lastmove;

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
        public bool IsCheck()
        {
            if (piece != null && piece.pieceType == PieceType.King)
            {
                if (piece.side == Side.White)
                {
                    var bb = squares.board.bitboard.Copy();
                    bb.SetupSquareAttacks();
                    if ((bb.BlackAttackedSquares & (1ul<<location)) != 0) //Are we in check?
                    {
                        return true;
                    }
                }
                else
                {
                    var bb = squares.board.bitboard.Copy();
                    bb.SetupSquareAttacks();
                    if ((bb.WhiteAttackedSquares & (1ul << location)) != 0) //Are we in check?
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void Paint()
        {
            if (squares.cancelHighlights)
            {
                dangerhighlight = false;
            }
            if (dangerhighlight)
            {
                g.FillRectangle(new Pen(color.AddFilter(danger_filter)).Brush, realworld);
            }
            else if (lastmove)
            {
                g.FillRectangle(new Pen(color.AddFilter(move_filter)).Brush, realworld);
            }
            else if (IsCheck())
            {
                g.FillRectangle(new Pen(color.AddFilter(check_filter)).Brush, realworld);
            }
            else
            {
                g.FillRectangle(new Pen(color).Brush, realworld);
            }
            if (piece != null) 
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawImage(piece.IMG, realworld);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
            if (squares.highlight == this && squares.canshowmove)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawEllipse(new Pen(movecolor,2), realworld.X + 1, realworld.Y + 1, realworld.Width-3, realworld.Height-2);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
            if (squares.moveSquares.Contains(this) && squares.canshowmove)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillEllipse(new Pen(Color.FromArgb(150,movecolor)).Brush, realworld.X + 10, realworld.Y + 10, realworld.Width - 20, realworld.Height - 20);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
        }

        internal void Click()
        {
            squares.cancelHighlights = true;
            squares.arrows.Clear();
            squares.arrowStart = null;
            if (squares.selected_edit != null)
            {
                if (piece != null)
                {

                    if ((squares.selected_edit.pieceType == piece.pieceType && squares.selected_edit.side == piece.side) || squares.selected_edit.pieceType == PieceType.None)
                    {
                        squares.board.Pieces.Remove(this.piece);
                        piece = null;
                        squares.board.bitboard = Bitboard.FromBoard(squares.board);
                        ((Form1)(Form1.ActiveForm)).WriteFEN();
                        return;
                    }
                    else
                    {
                        squares.board.Pieces.Remove(this.piece);
                        piece = null;
                    }
                }
                if (squares.selected_edit.pieceType == PieceType.None)
                {
                    return;
                }
                piece = new Piece(squares.selected_edit.pieceType, squares.selected_edit.side, location);
                squares.board.Pieces.Add(piece);
                squares.board.bitboard = Bitboard.FromBoard(squares.board);
                ((Form1)(Form1.ActiveForm)).WriteFEN();
                return;
            }
            else if (squares.edit)
            {
                if (squares.highlight == this)
                {
                    squares.highlight = null;
                    return;
                }
                else
                {
                    squares.highlight = this;
                }
                return;
            }

            if (squares.moveSquares.Contains(this)) //Are we moving here?
            {
                squares.highlight.Move(location);
                lastmove = true;
                squares.highlight.lastmove = true;
                squares.movehighlights.Add(this);
                squares.movehighlights.Add(squares.highlight);
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

           if (piece != null && (squares.board.hasturn == piece.side || squares.edit) && squares.board.hasturn == piece.side && squares.canshowmove) //Displaying moves?
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

        public void Move(int location)
        {
            squares.ClearMoveHighlights();

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
            squares.board.Reload();

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

            squares.AiMove();
        }

        internal void MoveHighlight()
        {
            squares.moveSquares.Add(this);
        }


        public bool dangerhighlight = false;
        public void RightClick()
        {
            squares.cancelHighlights = false;
            dangerhighlight = !dangerhighlight;
        }

        internal void MouseDown(MouseButtons button)
        {
            if (button == MouseButtons.Right)
            {
                squares.arrowStart = this;
            }
        }
        internal void MouseUp(MouseButtons button)
        {
            if (button == MouseButtons.Right)
            {
                if (squares.arrowStart == null || squares.arrowStart == this)
                {
                    RightClick();
                }
                else
                {
                    squares.AddArrow(this, ((Form1)(Form1.ActiveForm)).arrowColour);
                }
            }
        }
    }
}
