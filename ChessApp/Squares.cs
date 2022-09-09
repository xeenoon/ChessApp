﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    internal class Squares
    {
        private Square[] squares = new Square[64];
        Point offset;
        int size;

        public Squares(Chessboard board, Point offset, int size, Color light, Color dark, Graphics g)
        {
            this.offset = offset;
            this.size = size;

            for (int i = 0; i < 64; ++i) //Draw all the squares
            {
                int xpos = (i % 8);
                int ypos = (i / 8);

                byte boardpos = (byte)((xpos) + (7-ypos) * 8);

                Rectangle bounds = new Rectangle(offset.X + xpos * size, offset.Y + ypos * size, size, size);
                squares[i] = new Square(bounds, board.PieceAt(boardpos), (i + i/8) % 2 == 0 ? light : dark, (byte)(boardpos), g, this);
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

            int index = location.X + location.Y * 8;
            if (index <= -1 || index >= 64)
            {
                return null;
            }
            return squares[index];
        }
        public Square highlight;
    }
    internal class Square
    {
        Squares squares;

        public Rectangle realworld;
        public byte location;

        public Piece piece;
        public Color color;

        public Graphics g;

        public Square(Rectangle bounds, Piece piece, Color color, byte location, Graphics g, Squares squares)
        {
            this.realworld = bounds;
            this.piece = piece;
            this.color = color;
            this.location = location;
            this.g = g;
            this.squares = squares;
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
                g.DrawEllipse(new Pen(tohighlight,2), realworld.X + 1, realworld.Y + 1, realworld.Width-3, realworld.Height-2);
            }
        }
        Color tohighlight = Color.FromArgb(0,0,0);

        internal void QueueHighlight(Color color)
        {
            if (squares.highlight == this)
            {
                squares.highlight = null;
            }
            else
            {
                squares.highlight = this;
                tohighlight = color;
            }
        }
    }
}
