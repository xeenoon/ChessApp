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
            if (options == DrawOptions.Crazyhouse)
            {
                allsquares = new SideSquare[2, 5]; //Allow a duck for white and black
                int offset = Form1.SQUARESIZE/2;

                allsquares[0, 0] = new SideSquare(PieceType.Pawn, Side.Black, 0, false, squares, new Rectangle(boardleft + offset, boardtop- Form1.SQUARESIZE, Form1.SQUARESIZE, Form1.SQUARESIZE));
                offset += (int)(Form1.SQUARESIZE*1.5f);
                allsquares[0, 1] = new SideSquare(PieceType.Rook, Side.Black, 0, false, squares, new Rectangle(boardleft + offset, boardtop - Form1.SQUARESIZE, Form1.SQUARESIZE, Form1.SQUARESIZE));
                offset += (int)(Form1.SQUARESIZE * 1.5f);
                allsquares[0, 2] = new SideSquare(PieceType.Knight, Side.Black, 0, false, squares, new Rectangle(boardleft + offset, boardtop - Form1.SQUARESIZE, Form1.SQUARESIZE, Form1.SQUARESIZE));
                offset += (int)(Form1.SQUARESIZE * 1.5f);
                allsquares[0, 3] = new SideSquare(PieceType.Bishop, Side.Black, 0, false, squares, new Rectangle(boardleft + offset, boardtop - Form1.SQUARESIZE, Form1.SQUARESIZE, Form1.SQUARESIZE));
                offset += (int)(Form1.SQUARESIZE * 1.5f);
                allsquares[0, 4] = new SideSquare(PieceType.Queen, Side.Black, 0, false, squares, new Rectangle(boardleft + offset, boardtop - Form1.SQUARESIZE, Form1.SQUARESIZE, Form1.SQUARESIZE));

                offset = Form1.SQUARESIZE/2;
                allsquares[1, 0] = new SideSquare(PieceType.Pawn, Side.White, 0, false, squares, new Rectangle(boardleft + offset, boardbottom, Form1.SQUARESIZE, Form1.SQUARESIZE));
                offset += (int)(Form1.SQUARESIZE * 1.5f);
                allsquares[1, 1] = new SideSquare(PieceType.Rook, Side.White, 0, false, squares, new Rectangle(boardleft + offset, boardbottom, Form1.SQUARESIZE, Form1.SQUARESIZE));
                offset += (int)(Form1.SQUARESIZE * 1.5f);
                allsquares[1, 2] = new SideSquare(PieceType.Knight, Side.White, 0, false, squares, new Rectangle(boardleft + offset, boardbottom, Form1.SQUARESIZE, Form1.SQUARESIZE));
                offset += (int)(Form1.SQUARESIZE * 1.5f);
                allsquares[1, 3] = new SideSquare(PieceType.Bishop, Side.White, 0, false, squares, new Rectangle(boardleft + offset, boardbottom, Form1.SQUARESIZE, Form1.SQUARESIZE));
                offset += (int)(Form1.SQUARESIZE * 1.5f);
                allsquares[1, 4] = new SideSquare(PieceType.Queen, Side.White, 0, false, squares, new Rectangle(boardleft + offset, boardbottom, Form1.SQUARESIZE, Form1.SQUARESIZE));
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
            g.DrawString(amount.ToString(), new Font("Arial", 10, FontStyle.Regular), new Pen(Color.Black).Brush, realLocation);
        }

        public static void AddPiece(PieceType pieceType, Side side) //Add sidepiece to the board. This can be a duck or a crazyhouse capture
        {
            allsquares.Cast<SideSquare>().ToList().Where(s => s.side == side && s.toplace == pieceType).FirstOrDefault().amount++;
        }

        public void Click()
        {
            if (amount == 0)
            {
                selected = false;
            }
            else
            {
                selected = !selected;
            }
        }

        public static SideSquare SquareAt(Point mouseposition)
        {
            if (allsquares == null)
            {
                return null;
            }
            foreach (var square in allsquares)
            {
                if (square.realLocation.Contains(mouseposition))
                {
                    return square;
                }
            }
            return null;
        }

        internal static void DeselectAll()
        {
            foreach (var square in allsquares)
            {
                square.selected = false;
            }
        }
        internal static void Place(PieceType pieceType, Side original)
        {
            foreach (var square in allsquares)
            {
                if (square.toplace == pieceType && square.side == original)
                {
                    square.amount--;
                }
            }
        }
    }
}
