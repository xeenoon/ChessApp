using System.Drawing;

namespace ChessApp
{
    internal class EditSquare
    {
        public PieceType pieceType;
        public Side side;
        public Rectangle realworld;
        public bool selected;
        public Squares squares;

        public bool requiresrepaint = true;

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
