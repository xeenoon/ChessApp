using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    internal class SideSquare
    {
        public enum DrawOptions
        {
            NoExtras, //Dont draw any extra pieces
            Duck, //Only draw the duck for the first players turn, in the centre
            MultiDuck, //Have a greyed out duck when no duck is available to place, and a count of ducks you can place on your turn
            Crazyhouse, //Show all pieces, no duck
            Duckhouse, //Like crazyhouse, except with ducks!
        }
        public static void SetupAll(Graphics g, DrawOptions options, int boardbottom, int boardtop, int boardleft, int boardright, Squares squares)
        {
            if (options == DrawOptions.NoExtras)
            {
                return;
            }
            if (options == DrawOptions.Duck)
            {
                allsquares = new SideSquare[1,1]; //Only ever one square, first player placing the duck
                allsquares[0, 0] = new SideSquare(PieceType.Duck, Side.White, 1, false, squares, new Rectangle((boardleft + boardright)/2 - Form1.SQUARESIZE/2, boardbottom + 5, Form1.SQUARESIZE, Form1.SQUARESIZE));
                //Place a duck at the bottom of the board, in the middle
            }
            if (options == DrawOptions.MultiDuck)
            {
                allsquares = new SideSquare[1,2]; //Allow a duck for white and black
                allsquares[0, 0] = new SideSquare(PieceType.Duck, Side.White, 1, false, squares, new Rectangle((boardleft + boardright) / 2, boardbottom, Form1.SQUARESIZE, Form1.SQUARESIZE));
                allsquares[0, 1] = new SideSquare(PieceType.Duck, Side.Black, 1, false, squares, new Rectangle((boardleft + boardright) / 2, boardtop, Form1.SQUARESIZE, Form1.SQUARESIZE));
                //Create a duck for white and a duck for black
            }
            DrawSquares(g);
            requiresetup = false;
        }

        public static void DrawSquares(Graphics g)
        {
            foreach (var square in allsquares)
            {
                square.Draw(g);
            }
        }


        public PieceType toplace;
        public Side side;
        public int amount;
        public bool selected;
        Squares squares;

        public static SideSquare[,] allsquares;
        //Looks like this for duckhouse
        //p n b r q d
        //P N B R Q d
        //
        //Looks like this for notmal duck
        //     d
        //     d
        //Squares will be greyed out when duck is unavailable to place


        public Rectangle realLocation;
        internal static bool requiresetup = true;

        private SideSquare(PieceType toplace, Side side, int amount, bool selected, Squares squares, Rectangle realLocation)
        {
            this.toplace = toplace;
            this.side = side;
            this.amount = amount;
            this.selected = selected;
            this.squares = squares;
            this.realLocation = realLocation;
        }


        private void Draw(Graphics g)
        {
            g.DrawImage(new Piece(toplace, side, -1).IMG, new Rectangle(realLocation.X + 5, realLocation.Y + 5, realLocation.Width - 8, realLocation.Height - 8));
            if (selected) 
            {
                g.FillRoundedRectangle(new Pen(Color.FromArgb(100, Color.LightGreen)).Brush, realLocation, 5);
                g.DrawRoundedRectangle(new Pen(Color.Black), realLocation, 5);
            }
            else
            {
                g.FillRoundedRectangle(new Pen(Color.FromArgb(50, Color.Gray)).Brush, realLocation, 5);
            }
        }

        private void AddPiece(PieceType pieceType, Side side) //Add sidepiece to the board. This can be a duck or a crazyhouse capture
        {
            
        }

        public void Click()
        {
            selected = !selected;
            
        }

        public static SideSquare SquareAt(Point mouseposition)
        {
            foreach (var square in allsquares)
            {
                if (square.realLocation.Contains(mouseposition))
                {
                    return square;
                }
            }
            return null;
        }
    }
}
