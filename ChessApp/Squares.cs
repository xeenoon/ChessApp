using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessApp
{
    enum GameType
    {
        Standard,
        StandardDuck,
        DuckDuckGoose
    }
    internal class Squares
    {
        public GameType gameType = GameType.Standard;
        public bool mustMoveDuck;

        private Square[] squares = new Square[64];
        Point offset;
        Point edit_offset;
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
            DrawEdit(g);
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
            DrawEdit(g);
            DrawArrows(g);

            if ((gameType == GameType.StandardDuck || gameType == GameType.DuckDuckGoose) && !edit)
            {
                if (SideSquare.requiresetup)
                {
                    SideSquare.SetupAll(g, SideSquare.DrawOptions.Duck, offset.Y + (size * 8), offset.Y, offset.X, offset.X + (size * 8), this);
                }
                else
                {
                    SideSquare.DrawSquares(g);
                }
            }
        }

        internal void Paint(Graphics g)
        {
            DrawEdit(g);
            foreach (var square in squares)
            {
                square.g = g;
                square.Paint();
            }
            DrawArrows(g);
            if ((gameType == GameType.StandardDuck || gameType == GameType.DuckDuckGoose) && !edit)
            {
                if (SideSquare.requiresetup)
                {
                    SideSquare.SetupAll(g, SideSquare.DrawOptions.Duck, offset.Y + (size * 8), offset.Y, offset.X, offset.X + (size * 8), this);
                }
                else
                {
                    SideSquare.DrawSquares(g);
                }
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

            if (location.Y >= 8)
            {
                return null; //clicked too low
            }

            int left = editSquares[0].realworld.X;
            int right = size + Horizontal_Indent + left;
            int startidx = location.X > left && location.X < right ? 0 : 7;
            if (startidx == 7) //Check if mouse is actually on the right
            {
                int farleft = editSquares[7].realworld.X;
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
                editSquares = new EditSquare[14];
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
                editSquares[6] = new EditSquare(PieceType.None, Side.Black, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;

                bounds.X = bounds.X + 9 * size + Horizontal_Indent * 2;
                bounds.Y = offset.Y;

                editSquares[7] = new EditSquare(PieceType.King, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[8] = new EditSquare(PieceType.Queen, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[9] = new EditSquare(PieceType.Rook, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[10] = new EditSquare(PieceType.Bishop, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[11] = new EditSquare(PieceType.Knight, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[12] = new EditSquare(PieceType.Pawn, Side.White, bounds, this);
                bounds.Y = bounds.Y + size + Vertical_Indent;
                editSquares[13] = new EditSquare(PieceType.None, Side.Black, bounds, this);
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

        internal void GooseChase(int tochase)
        {
            if (gameType != GameType.DuckDuckGoose)
            {
                return;
            }
            Square goose = this[BitOperations.TrailingZeros(board.bitboard.Goose)-1];

            Point gooseloc = new Point(goose.location % 8, goose.location / 8);
            Point chaseloc = new Point(tochase % 8, tochase / 8);

            int xdistance = -(gooseloc.X - chaseloc.X);
            int ydistance = -(gooseloc.Y - chaseloc.Y);
            if (xdistance != 0) 
            {
                xdistance = xdistance <= -1 ? -1 : 1; //Scale down to a 1
            }
            if (ydistance != 0)
            {
                ydistance = ydistance <= -1 ? -1 : 1; //Scale down to a 1
            }

            int newloc = gooseloc.X + xdistance + (gooseloc.Y + ydistance) * 8;
            squares[newloc].piece = goose.piece;
            squares[newloc].piece.position = newloc;
            goose.piece = null;

            board.bitboard.Move((byte)goose.location, (byte)newloc, 1ul<<goose.location, 1ul<<newloc, PieceType.Goose, Side.Animal);
        }

        public List<Square> movehighlights = new List<Square>();
        internal void ClearMoveHighlights()
        {
            foreach (var square in movehighlights)
            {
                square.lastmove = false;
            }
            movehighlights.Clear();
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
        internal void UndoMove()
        {
            if (undomoves.Count == 0)
            {
                return;
            }
            board.bitboard.UndoMove(undomoves.Last());
            if (gameType == GameType.StandardDuck || gameType == GameType.DuckDuckGoose) 
            {
                if (undomoves.Last().pieceType == PieceType.Duck) //Moving duck switches turns
                {
                    board.hasturn = board.hasturn == Side.White ? Side.Black : Side.White;
                    mustMoveDuck = true; //We need to move the duck again
                }
                else
                {
                    mustMoveDuck = false; //We will need to move a piece
                    //Dont change hasturn
                }
            }
            else
            {
                board.hasturn = board.hasturn == Side.White ? Side.Black : Side.White;

            }
            undomoves.Remove(undomoves.Last());

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
        public bool canshowmove = true;
        internal async void AiMove()
        {
            canshowmove = false;
            await Task.Run(() => GetAIMove());
        }

        private void GetAIMove()
        {
            if (AI_can_move)
            {
                var copy = board.bitboard.Copy();
                if (MoveGenerator.MoveCount(board.bitboard, board.hasturn) != 0)
                {
                    var move = AI.GetMove(copy, board.hasturn);
                    undomoves.Add(board.bitboard.Move(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, board.hasturn));
                    board.hasturn = board.hasturn == Side.White ? Side.Black : Side.White;
                    board.Reload();

                    ClearMoveHighlights();

                    squares[move.last].lastmove = true;
                    squares[move.current].lastmove = true;
                    foreach (var square in squares)
                    {
                        square.piece = board.PieceAt(square.location);
                    }
                    movehighlights.Add(squares[move.last]);
                    movehighlights.Add(squares[move.current]); //Highlight the move
                    parent.Invalidate();
                }
            }
            canshowmove = true;
        }

        public bool cancelHighlights = false;

        public Square arrowStart;
        public List<Arrow> arrows = new List<Arrow>();
        public Piece selectedplace;

        private void DrawArrows(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var arrow in arrows)
            {
                arrow.Draw(g);
            }
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        }

        public void AddArrow(Square end, Color arrowcolor)
        {
            //Check pseudo moves
            bool cancontinue = false;
            if (end.location / 8 == arrowStart.location / 8) //Horizontal line
            {
                cancontinue = true;
            }
            if (end.location % 8 == arrowStart.location % 8) //Vertical line
            {
                cancontinue = true;
            }
            ulong pseudobishop = MoveGenerator.upRight[arrowStart.location] | MoveGenerator.upLeft[arrowStart.location] | MoveGenerator.downRight[arrowStart.location] | MoveGenerator.downLeft[arrowStart.location];
            if ((pseudobishop & (1ul<<end.location)) != 0) //Is it a valid diagonal?
            {
                cancontinue = true;
            }
            if ((MoveGenerator.knight[arrowStart.location] & (1ul<<end.location)) != 0) //Valid horsey move
            {
                cancontinue = true;
            }
            if (!cancontinue)
            {
                return;
            }

            Arrow item = new Arrow(new Vector(arrowStart.realworld.Center(), end.realworld.Center()), 8, 25, new Pen(Color.FromArgb(200,arrowcolor)).Brush);
            var duplicate = arrows.Where(a => a.location == item.location).FirstOrDefault();
            if (duplicate != null)
            {
                arrows.Remove(duplicate);
            }
            else
            {
                arrows.Add(item);
            }
            arrowStart = null;
        }
    }
}
