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
        DuckDuckGoose,
        Crazyhouse,
        CrazyDuck
    }
    internal class Squares
    {
        public GameType gameType = GameType.Standard;
        public bool mustMoveDuck;

        public Square[] squares = new Square[64];
        Point offset;
        Point originaloffset;
        Point edit_offset;
        int size;
        public Chessboard board;

        Color light;
        Color dark;
        Color select;
        Color move;

        Form1 parent;

        public List<Bitboard.BoardData> undomoves = new List<Bitboard.BoardData>();
        public Squares(Chessboard board, Point offset, int size, Color light, Color dark, Graphics g, Color select, Color move, Form1 parent, Graphics editgraphics)
        {
            this.offset = offset;
            this.originaloffset = offset;
            DrawEdit(editgraphics);
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
            if ((gameType == GameType.Crazyhouse || gameType == GameType.CrazyDuck) && offset.Y == originaloffset.Y && !edit)
            {
                offset.Y += Form1.SQUARESIZE;
            }
            else if (edit && gameType != GameType.Standard && offset.Y != originaloffset.Y)
            {
                offset.Y -= Form1.SQUARESIZE;
                offset.X -= (size + Horizontal_Indent);
            }


            for (int i = 0; i < 64; ++i) //Draw all the squares
            {
                int xpos = (i % 8);
                int ypos = 7 - (i / 8);

                Rectangle bounds = new Rectangle(xpos * size, ypos * size, size, size);
                squares[i] = new Square(bounds, board.PieceAt(i), (i + i / 8) % 2 == 1 ? light : dark, i, 1ul << i, g, this, select, move);
                squares[i].Paint();
            }

            if ((gameType != GameType.Standard) && !edit)
            {
                if (SideSquare.requiresetup)
                {
                    if (gameType == GameType.Crazyhouse)
                    {
                        SideSquare.SetupAll(g, SideSquare.DrawOptions.Crazyhouse, offset.Y + (size * 8), offset.Y, offset.X, offset.X + (size * 8), this);
                    }
                    else if (gameType == GameType.CrazyDuck)
                    {
                        SideSquare.SetupAll(g, SideSquare.DrawOptions.CrazyDuck, offset.Y + (size * 8), offset.Y, offset.X, offset.X + (size * 8), this);
                    }
                    else
                    {
                        SideSquare.SetupAll(g, SideSquare.DrawOptions.Duck, offset.Y + (size * 8), offset.Y, offset.X, offset.X + (size * 8), this);
                    }
                }
                else
                {
                    SideSquare.DrawSquares(g);
                }
            }
        }

        internal void Paint(Graphics boardGraphics, Graphics arrowGraphics, Graphics editGraphics)
        {
            if ((gameType == GameType.Crazyhouse || gameType == GameType.CrazyDuck) && offset.Y == originaloffset.Y && !edit)
            {
                offset.Y += Form1.SQUARESIZE;
            }

            DrawEdit(editGraphics);
            foreach (var square in squares)
            {
                square.g = boardGraphics;
                if (square.requiresPaint)
                {
                    square.Paint();
                    square.requiresPaint = false;
                }
                if (square.IsCheck())
                {
                    square.Paint();
                    square.requiresPaint = true;
                }
            }
            DrawArrows(arrowGraphics);
            if ((gameType != GameType.Standard) && !edit)
            {
                if (SideSquare.requiresetup)
                {
                    if (gameType == GameType.Crazyhouse)
                    {
                        SideSquare.SetupAll(boardGraphics, SideSquare.DrawOptions.Crazyhouse, offset.Y + (size * 8), offset.Y, offset.X, offset.X + (size * 8), this);
                    }
                    else if (gameType == GameType.CrazyDuck)
                    {
                        SideSquare.SetupAll(boardGraphics, SideSquare.DrawOptions.CrazyDuck, offset.Y + (size * 8), offset.Y, offset.X, offset.X + (size * 8), this);
                    }
                    else
                    {
                        SideSquare.SetupAll(boardGraphics, SideSquare.DrawOptions.Duck, offset.Y + (size * 8), offset.Y, offset.X, offset.X + (size * 8), this);
                    }
                }
                else
                {
                    SideSquare.DrawSquares(boardGraphics);
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
            if (edit)
            {
                location.X -= (Form1.SQUARESIZE + 5);
            }
            if (location.Y < 0 || location.X < 0)
            {
                return null;
            }
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
            if (editSquares == null || !edit)
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
        public void ClearMoves()
        {
            foreach (var move in moveSquares)
            {
                move.requiresPaint = true;
            }
            moveSquares.Clear();
        }
        public bool edit;
        public static int Horizontal_Indent = 5;
        public static int Vertical_Indent = 5;
        internal void SetupEdit(bool indented, Graphics editGraphics)
        {
            edit = indented;

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

            foreach (var square in editSquares)
            {
                square.Paint(editGraphics);
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
                square.requiresPaint = true;
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
                    if (sqr.requiresrepaint)
                    {
                        sqr.Paint(g);
                        sqr.requiresrepaint = false;
                    }
                }
            }
        }
        internal void UndoMove()
        {
            if (undomoves.Count == 0)
            {
                return;
            }
            var last = undomoves.Last();
            if (last.isplacing)
            {
                board.bitboard.UndoPlace(undomoves.Last());
                SideSquare.AddPiece(last.pieceType, last.side);
            }
            else
            {
                board.bitboard.UndoMove(undomoves.Last());
            }
            if (gameType == GameType.StandardDuck || gameType == GameType.DuckDuckGoose || gameType == GameType.CrazyDuck) 
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
        public void ClearArrows()
        {
            arrows.Clear();
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
            Arrow item = new Arrow(new Vector(arrowStart.realworld.Center(), end.realworld.Center()), 8, 25, new Pen(Color.FromArgb(200,arrowcolor)).Brush, arrowStart.location, end.location);
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
     /*   public void RepaintEffected(int start, int end)
        {
            if ((MoveGenerator.upRight[start] & (1ul << end)) != 0)
            {
                ulong torepaint = MoveGenerator.upRight[start] | 1ul<<start;
                if (start >= 8)
                {
                    torepaint |= MoveGenerator.upRight[start - 8];
                    torepaint |= 1ul << (start - 8);
                }
                if (start <= 56)
                {
                    torepaint |= MoveGenerator.upRight[start + 8];
                    torepaint |= 1ul << (start + 8);
                }
                foreach (var position in BitOperations.Bitloop(torepaint))
                {
                    squares[position].requiresPaint = true;
                }
            }
            else if ((MoveGenerator.upLeft[start] & (1ul << end)) != 0)
            {
                ulong torepaint = MoveGenerator.upLeft[start] | 1ul << start;
                if (start >= 8)
                {
                    torepaint |= MoveGenerator.upLeft[start - 8];
                    torepaint |= 1ul << (start - 8);
                }
                if (start <= 56)
                {
                    torepaint |= MoveGenerator.upLeft[start + 8];
                    torepaint |= 1ul << (start + 8);
                }
                foreach (var position in BitOperations.Bitloop(torepaint))
                {
                    squares[position].requiresPaint = true;
                }
            }
            else if ((MoveGenerator.downLeft[start] & (1ul << end)) != 0)
            {
                ulong torepaint = MoveGenerator.downLeft[start] | 1ul << start;
                if (start >= 8)
                {
                    torepaint |= MoveGenerator.downLeft[start - 8];
                    torepaint |= 1ul << (start - 8);
                }
                if (start <= 56)
                {
                    torepaint |= MoveGenerator.downLeft[start + 8];
                    torepaint |= 1ul << (start + 8);
                }
                foreach (var position in BitOperations.Bitloop(torepaint))
                {
                    squares[position].requiresPaint = true;
                }
            }
            else if ((MoveGenerator.downRight[start] & (1ul << end)) != 0)
            {
                ulong torepaint = MoveGenerator.downRight[start] | 1ul << start;
                if (start >= 8)
                {
                    torepaint |= MoveGenerator.downRight[start - 8];
                    torepaint |= 1ul << (start - 8);
                }
                if (start <= 56)
                {
                    torepaint |= MoveGenerator.downRight[start + 8];
                    torepaint |= 1ul << (start + 8);
                }
                foreach (var position in BitOperations.Bitloop(torepaint))
                {
                    squares[position].requiresPaint = true;
                }
            }
            else if ((MoveGenerator.down[start] & (1ul << end)) != 0)
            {
                ulong torepaint = MoveGenerator.down[start] | 1ul << start;
                foreach (var position in BitOperations.Bitloop(torepaint))
                {
                    squares[position].requiresPaint = true;
                }
            }
            else if ((MoveGenerator.up[start] & (1ul << end)) != 0)
            {
                ulong torepaint = MoveGenerator.up[start] | 1ul << start;
                foreach (var position in BitOperations.Bitloop(torepaint))
                {
                    squares[position].requiresPaint = true;
                }
            }
            else if ((MoveGenerator.left[start] & (1ul << end)) != 0)
            {
                ulong torepaint = MoveGenerator.left[start] | 1ul << start;
                foreach (var position in BitOperations.Bitloop(torepaint))
                {
                    squares[position].requiresPaint = true;
                }
            }
            else if ((MoveGenerator.right[start] & (1ul << end)) != 0)
            {
                ulong torepaint = MoveGenerator.right[start] | 1ul << start;
                foreach (var position in BitOperations.Bitloop(torepaint))
                {
                    squares[position].requiresPaint = true;
                }
            }
        }*/
    }
}
