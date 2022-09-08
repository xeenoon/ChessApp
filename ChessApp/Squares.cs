using System;
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

        public Squares(Chessboard board, Point offset, int size, Color light, Color dark, Graphics g)
        {
            for (int i = 0; i < 64; ++i) //Draw all the squares
            {
                int xpos = (i % 8);
                int ypos = (i / 8);
                Rectangle bounds = new Rectangle(offset.X + xpos * size, offset.Y + ypos * size, size, size);
                squares[i] = new Square(bounds, board.PieceAt(63 - i), (i + i/8) % 2 == 0 ? light : dark, (byte)(63 - i), g);
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
    }
    internal class Square
    {
        public Rectangle realworld;
        public byte location;

        public Piece piece;
        public Color color;

        public Graphics g;

        public Square(Rectangle bounds, Piece piece, Color color, byte location, Graphics g)
        {
            this.realworld = bounds;
            this.piece = piece;
            this.color = color;
            this.location = location;
            this.g = g;
        }

        public void Paint()
        {
            g.FillRectangle(new Pen(color).Brush,realworld);
            if (piece != null) 
            {
                g.DrawImage(piece.IMG, realworld);
            }
        }
    }
}
