using ColorExtensions;
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

        public bool requiresPaint = true;

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
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

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
                g.DrawImage(piece.IMG, realworld);
            }
            if (squares.highlight == this && squares.canshowmove)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawEllipse(new Pen(movecolor,2), realworld.X + 3, realworld.Y + 3, realworld.Width-7, realworld.Height-7);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
            if (squares.moveSquares.Contains(this) && squares.canshowmove)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.FillEllipse(new Pen(Color.FromArgb(150,movecolor)).Brush, realworld.X + Form1.SQUARESIZE/6, realworld.Y + Form1.SQUARESIZE / 6, realworld.Width - Form1.SQUARESIZE / 3, realworld.Height - Form1.SQUARESIZE / 3);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
        }

        internal void Click()
        {
            requiresPaint = true;
            squares.cancelHighlights = true;
            squares.ClearArrows();
            squares.arrowStart = null;
            if (squares.selected_edit != null)
            {
                if (piece != null && piece.pieceType != PieceType.Duck)
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

            if (squares.moveSquares.Contains(this)) //Are we moving here?
            {
                if (piece != null && (squares.gameType == GameType.Crazyhouse || squares.gameType == GameType.CrazyDuck))
                {
                    SideSquare.AddPiece(piece.pieceType, piece.side == Side.White ? Side.Black : Side.White);
                }
                if (squares.gameType == GameType.StandardDuck || squares.gameType == GameType.DuckDuckGoose || squares.gameType == GameType.CrazyDuck)
                {
                    squares.mustMoveDuck = true;
                }
                if (squares.highlight.piece.pieceType == PieceType.Duck)
                {
                    squares.mustMoveDuck = false;
                    squares.GooseChase(location);
                }
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
                squares.highlight.requiresPaint = true;
            }
            if (squares.selectedplace != null) //Are we moving here?
            {
                if ((squares.selectedplace.pieceType == PieceType.Duck && !squares.mustMoveDuck) || ((DuckMoves.GetMoves(squares.board.bitboard) & (1ul<<location)) == 0))
                {

                }
                else
                {
                    SideSquare.Place(squares.selectedplace.pieceType, squares.selectedplace.side);
                    if (squares.selectedplace.pieceType == PieceType.Duck)
                    {
                        squares.selectedplace.side = Side.Animal;
                    }
                    Piece toplace = new Piece(squares.selectedplace.pieceType, squares.selectedplace.side, location);
                    squares.board.Pieces.Add(toplace);
                    squares.board.bitboard.Add(squares.selectedplace.pieceType, squares.selectedplace.side, location); //Add the piece to the bitboard

                    piece = toplace;
                    lastmove = true;
                    squares.movehighlights.Add(this);
                    if (squares.selected_edit == null && squares.edit)
                    {
                        squares.board.bitboard = Bitboard.FromBoard(squares.board);
                        ((Form1)(Form1.ActiveForm)).WriteFEN();
                    }
                    squares.board.hasturn = squares.board.hasturn == Side.White ? Side.Black : Side.White;
                    if (squares.mustMoveDuck)
                    {
                        squares.mustMoveDuck = false;
                        squares.GooseChase(location);
                    }
                    squares.undomoves.Add(new Bitboard.BoardData((byte)location, piece.pieceType, piece.side, squares.board.bitboard.W_KingsideCastle, squares.board.bitboard.B_KingsideCastle, squares.board.bitboard.W_QueensideCastle, squares.board.bitboard.B_QueensideCastle));
                    squares.board.Reload();
                }
                squares.selectedplace = null;
                SideSquare.DeselectAll();
            }
            squares.ClearMoves();

            if (squares.highlight == this)
            {
                squares.highlight = null;
                return;
            }
            else
            {
                if (squares.highlight != null)
                {
                    squares.highlight.requiresPaint = true;
                }
                squares.highlight = this;
            }

            if (piece != null && (squares.board.hasturn == piece.side || piece.side == Side.Animal) && (squares.canshowmove || piece.pieceType == PieceType.Duck)) //Displaying moves?
            {
                Bitboard board;
                ulong moves;
                switch (squares.gameType)
                {
                    case GameType.Standard:
                        board = squares.board.bitboard.Copy();
                        board.SetupSquareAttacks();

                        moves = MoveGenerator.Moves(piece.pieceType, piece.side, (byte)location, board);
                        while (moves != 0ul)
                        {
                            byte lsb = (byte)(BitOperations.TrailingZeros(moves) - 1);
                            moves ^= 1ul << lsb;
                            squares[lsb].MoveHighlight();
                        }
                        break;
                    case GameType.DuckDuckGoose:
                    case GameType.StandardDuck:
                        if (squares.mustMoveDuck || squares.edit)
                        {
                            if (piece.pieceType == PieceType.Duck)
                            {
                                foreach (var move in DuckMoves.DuckPositions(squares.board.bitboard))
                                {
                                    squares[move].MoveHighlight();
                                }
                            }
                        }
                        else //Normal move
                        {
                            board = squares.board.bitboard.Copy(); //No square attacks cos thats how duck-chess works
                            board.squares_to_block_check = ulong.MaxValue;

                            moves = MoveGenerator.Moves(piece.pieceType, piece.side, (byte)location, board);
                            while (moves != 0ul)
                            {
                                byte lsb = (byte)(BitOperations.TrailingZeros(moves) - 1);
                                moves ^= 1ul << lsb;
                                squares[lsb].MoveHighlight();
                            }
                        }
                        break;
                    case GameType.Crazyhouse:
                        board = squares.board.bitboard.Copy();
                        board.squares_to_block_check = ulong.MaxValue;

                        moves = MoveGenerator.Moves(piece.pieceType, piece.side, (byte)location, board);
                        while (moves != 0ul)
                        {
                            byte lsb = (byte)(BitOperations.TrailingZeros(moves) - 1);
                            moves ^= 1ul << lsb;
                            squares[lsb].MoveHighlight();
                        }
                        break;
                    case GameType.CrazyDuck:
                        if (squares.mustMoveDuck || squares.edit)
                        {
                            if (piece.pieceType == PieceType.Duck)
                            {
                                foreach (var move in DuckMoves.DuckPositions(squares.board.bitboard))
                                {
                                    squares[move].MoveHighlight();
                                }
                            }
                        }
                        else //Normal move
                        {
                            board = squares.board.bitboard.Copy();
                            board.squares_to_block_check = ulong.MaxValue;

                            moves = MoveGenerator.Moves(piece.pieceType, piece.side, (byte)location, board);
                            while (moves != 0ul)
                            {
                                byte lsb = (byte)(BitOperations.TrailingZeros(moves) - 1);
                                moves ^= 1ul << lsb;
                                squares[lsb].MoveHighlight();
                            }
                        }
                        break;
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
            if (!squares.edit && !squares.mustMoveDuck) //Not editing, and has finished duck move
            {
                squares.board.hasturn = squares.board.hasturn == Side.White ? Side.Black : Side.White;
            }
            piece = null;

            squares.AiMove();
        }

        internal void MoveHighlight()
        {
            squares.moveSquares.Add(this);
            requiresPaint = true;
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
                    requiresPaint = true;
                }
                else
                {
                    squares.AddArrow(this, ((Form1)(Form1.ActiveForm)).arrowColour);
                }
            }
        }
    }
}
