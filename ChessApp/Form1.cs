using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace ChessApp
{
    public partial class Form1 : Form
    {
        Chessboard chessboard;
        Squares squares;

        public Form1()
        {
            InitializeComponent();
            SQUARESIZE = (this.Size.Height - 60) / 11;

            string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            chessboard = new Chessboard(FEN);
            FEN_TEXT.Text = FEN;
        }
        public void WriteFEN()
        {
            FEN_TEXT.Text = chessboard.GetFEN();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            chessboard = new Chessboard(FEN_TEXT.Text);
            squares = null;

            Invalidate();
        }

        public static int SQUARESIZE = 45;

        System.Timers.Timer checkmateDelay;
        bool checkmated = false;
        bool running = false;
        bool reload = false;
        bool reset = false;
        Bitmap boardIMG;
        Bitmap arrowsIMG;
        Bitmap editIMG;
        Bitmap placeIMG;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (resizing)
            {
                e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                if (squares.edit)
                {
                    e.Graphics.DrawImage(editIMG, 0, 0, Size.Width, Size.Height);
                    e.Graphics.DrawImage(boardIMG, 20 + SQUARESIZE, SQUARESIZE+10, Size.Width, Size.Height);
                }
                else if (squares.gameType == GameType.Crazyhouse || squares.gameType == GameType.CrazyDuck)
                {
                    e.Graphics.DrawImage(placeIMG, 0, 0, Size.Width, Size.Height);
                    e.Graphics.DrawImage(boardIMG, 15, Form1.SQUARESIZE+55, Size.Width, Size.Height);
                }
                else if (squares.gameType == GameType.StandardDuck || squares.gameType == GameType.DuckDuckGoose)
                {
                    e.Graphics.DrawImage(placeIMG, 0, 0, Size.Width, Size.Height);
                    e.Graphics.DrawImage(boardIMG, 15, SQUARESIZE+10, Size.Width, Size.Height);
                }
                else
                {
                    e.Graphics.DrawImage(boardIMG, 15, SQUARESIZE+10, Size.Width, Size.Height);
                }
                e.Graphics.DrawImage(arrowsIMG, 15, SQUARESIZE + 10, Size.Width, Size.Height);
                e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                return;
            }

            arrowsIMG = new Bitmap(Size.Width, Size.Height);
            if (squares == null)
            {
                boardIMG = new Bitmap(Size.Width, Size.Height);
                editIMG = new Bitmap(Size.Width, Size.Height);
                placeIMG = new Bitmap(Size.Width, Size.Height);
            }

            Graphics boardGraphics = Graphics.FromImage(boardIMG);
            Graphics arrowGraphics = Graphics.FromImage(arrowsIMG);
            Graphics editGraphics = Graphics.FromImage(editIMG);
            Graphics placeGraphics = Graphics.FromImage(placeIMG);

            if (squares == null)
            {
                squares = new Squares(chessboard, new Point(15,55), SQUARESIZE, Color.FromArgb(234,233,210), Color.FromArgb(75,115,153), boardGraphics, Color.FromArgb(0,0,255), Color.FromArgb(50,50,50), this, editGraphics);
                squares.SetupEdit(checkBox1.Checked, editGraphics);

                squares.AI_can_move = PlayComputer.Checked;
                comboBox1.SelectedIndex = comboBox1.SelectedIndex;
            }
            else if (reload)
            {
                reload = false;
                squares.Reload(boardGraphics);
            }
            else if (reset)
            {
                reset = false;

                boardIMG  = new Bitmap(Size.Width, Size.Height);
                editIMG   = new Bitmap(Size.Width, Size.Height);
                placeIMG  = new Bitmap(Size.Width, Size.Height);
                arrowsIMG = new Bitmap(Size.Width, Size.Height);

                boardGraphics = Graphics.FromImage(boardIMG);
                arrowGraphics = Graphics.FromImage(arrowsIMG);
                editGraphics  = Graphics.FromImage(editIMG);
                placeGraphics = Graphics.FromImage(placeIMG);

                foreach (var editsquare in squares.editSquares)
                {
                    editsquare.requiresrepaint = true;
                }
                if (SideSquare.allsquares != null) 
                {
                    foreach (var sidesquare in SideSquare.allsquares)
                    {
                        if (sidesquare != null) 
                        {
                            sidesquare.requiresRepaint = true;
                        }
                    }
                }

                squares.size = SQUARESIZE;
                squares.offset = squares.originaloffset;
                squares.Reload(boardGraphics); //Re-write the squares with the new squaresize
                squares.SetupEdit(squares.edit, editGraphics);
                SideSquare.requiresetup = true;
                squares.ClearArrows();
                squares.Paint(boardGraphics, arrowGraphics, editGraphics, placeGraphics);
            }
            else
            {
                squares.Paint(boardGraphics, arrowGraphics, editGraphics, placeGraphics);
                if (!checkmated && !running)
                {
                    running = true;
                    checkmateDelay = new System.Timers.Timer(100);
                    checkmateDelay.Elapsed += new System.Timers.ElapsedEventHandler(CheckMateTick);
                    checkmateDelay.Start();
                }
            }
            
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            if (squares.edit)
            {
                e.Graphics.DrawImage(editIMG , 0, 0);
                e.Graphics.DrawImage(boardIMG, 15 + SQUARESIZE + 5, 55);
            }
            else if (squares.gameType == GameType.Crazyhouse || squares.gameType == GameType.CrazyDuck)
            {
                e.Graphics.DrawImage(placeIMG, 0, 0);
                e.Graphics.DrawImage(boardIMG, 15, 55 + Form1.SQUARESIZE);
            }
            else if (squares.gameType == GameType.StandardDuck || squares.gameType == GameType.DuckDuckGoose)
            {
                e.Graphics.DrawImage(placeIMG, 0, 0);
                e.Graphics.DrawImage(boardIMG, 15, 55);
            }
            else
            {
                e.Graphics.DrawImage(boardIMG, 15, 55);
            }
            e.Graphics.DrawImage(arrowsIMG, 15, 55);
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        }

        private void CheckMateTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkmateDelay.Stop();
            if (squares == null)
            {
                running = false;
                return;
            }
            var copy = squares.board.bitboard.Copy();
            copy.SetupSquareAttacks();
            if (!squares.edit) //an we run
            {
                if ((copy.check || copy.doublecheck) && MoveGenerator.MoveCount(copy, squares.board.hasturn) == 0 && checkmated == false) //Checkmate
                {
                    if (squares.gameType == GameType.Standard)
                    {
                        checkmated = true;
                        MessageBox.Show(String.Format("{0} Checkmated {1}", squares.board.hasturn == Side.White ? Side.Black : Side.White, copy.doublecheck ? "Like a boss" : squares.board.hasturn.ToString()));
                    }
                }
                else if (copy.W_King == 0 || copy.B_King == 0 || (copy.W_King&copy.BlackPieces) != 0 || (copy.B_King&copy.WhitePieces) != 0) //King captured
                {
                    checkmated = true;
                    if (squares.gameType == GameType.StandardDuck || squares.gameType == GameType.DuckDuckGoose || squares.gameType == GameType.CrazyDuck)
                    {
                        MessageBox.Show("Quackmate");
                    }
                    if (squares.gameType  == GameType.Crazyhouse)
                    {
                        MessageBox.Show(string.Format("{0}'s king was captured", squares.board.hasturn));
                    }
                }
            }
            running = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            VariantSelector.SelectedIndex = 0;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = (MouseEventArgs)(e);
            MouseButtons button = mouse.Button;
            if (button == MouseButtons.Left)
            {
                Square clicked = squares.SquareAt(mouse.Location);
                if (clicked != null)
                {
                    clicked.Click();
                }
                SideSquare sidesquare = SideSquare.SquareAt(mouse.Location);
                if (sidesquare != null)
                {
                    sidesquare.Click();
                    if (sidesquare.selected)
                    {
                        squares.selectedplace = new Piece(sidesquare.toplace, sidesquare.side, -1);
                    }
                    else
                    {
                        squares.selectedplace = null;
                    }
                    Invalidate();
                }

                EditSquare editSquare = squares.EditSquareAt(mouse.Location);
                if (editSquare != null)
                {
                    editSquare.Click();
                    Invalidate();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            squares.ClearArrows();
            arrowsIMG = new Bitmap(Size.Width, Size.Height);

            bool enabled = checkBox1.Checked;


            comboBox1.SelectedIndex = chessboard.hasturn == Side.White ? 0 : 1;
            W_KingsideCastle.Checked = chessboard.whiteCastles.Kingside;
            W_QueensideCastle.Checked = chessboard.whiteCastles.Queenside;

            B_KingsideCastle.Checked = chessboard.blackCastles.Kingside;
            B_QueensideCastle.Checked = chessboard.blackCastles.Queenside;

            squares.edit = checkBox1.Checked;
            Invalidate();

            panel1.Visible = enabled;
            
            button2.Enabled = !checkBox1.Checked;
            FEN_TEXT.ReadOnly = checkBox1.Checked;
            PlayComputer.Enabled = !checkBox1.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                chessboard.hasturn = comboBox1.SelectedIndex == 0 ? Side.White : Side.Black;
                FEN_TEXT.Text = chessboard.GetFEN();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            squares.UndoMove();
            this.chessboard = squares.board;
            Invalidate();
        }

        private void PlayComputerCheckChange(object sender, EventArgs e)
        {
            squares.AI_can_move = PlayComputer.Checked;
        }

        internal void Reload()
        {
            reload = true;
            Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            var button = e.Button;
            Square clicked = squares.SquareAt(e.Location);
            if (clicked != null)
            {
                clicked.MouseDown(button);
                Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            var button = e.Button;
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                arrowColour = Color.Red;
            }
            else if ((ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                arrowColour = Color.Orange;
            }
            else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                arrowColour = Color.Yellow;
            }
            else
            {
                arrowColour = Color.Blue;
            }
            Square clicked = squares.SquareAt(e.Location);
            if (clicked != null)
            {
                clicked.MouseUp(button);
                Invalidate();
            }
        }
        public Color arrowColour = Color.Blue;

        private void CastleOptionChanged(object sender, EventArgs e)
        {
            if (!panel1.Visible) //Are we inactive?
            {
                return;
            }
            chessboard.whiteCastles = new CastleOptions(Side.White, W_QueensideCastle.Checked, W_KingsideCastle.Checked);
            chessboard.blackCastles = new CastleOptions(Side.Black, B_QueensideCastle.Checked, B_KingsideCastle.Checked);
            FEN_TEXT.Text = chessboard.GetFEN();
            chessboard.bitboard = Bitboard.FromBoard(chessboard);
        }

        private void VariantSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (squares != null)
            {
                string FEN = "";
                if (VariantSelector.SelectedIndex == 0) //Standard
                {
                    FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                    squares.board = new Chessboard(FEN);
                    reload = true;
                }
                else if (VariantSelector.SelectedIndex == 1) //Standard duck
                {
                    FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                    squares.board = new Chessboard(FEN);
                    reload = true;
                    placeIMG = new Bitmap(Size.Width, Size.Height);

                }
                else if (VariantSelector.SelectedIndex == 2) //Duck duck goose
                {
                    FEN = "rnbqkbnr/pppppppp/8/8/3G4/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                    squares.board = new Chessboard(FEN);
                    reload = true;
                    placeIMG = new Bitmap(Size.Width, Size.Height);

                }
                else if (VariantSelector.SelectedIndex == 3) //Crazyhouse
                {
                    FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                    squares.board = new Chessboard(FEN);
                    reload = true;
                    placeIMG = new Bitmap(Size.Width, Size.Height);

                }
                else if (VariantSelector.SelectedIndex == 3) //CrazyDuck
                {
                    FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                    squares.board = new Chessboard(FEN);
                    reload = true;
                    placeIMG = new Bitmap(Size.Width, Size.Height);

                }
                SideSquare.requiresetup = true;
                squares.gameType = (GameType)VariantSelector.SelectedIndex;
                Invalidate();
            }
        }
        bool resizing = false;
        int lastwidth = 0;
        int lastheight = 0;
        private bool _capturingMoves;

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            resizing = true;
            lastwidth = 0;
            lastheight = 0;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            resizing = false;
            reset = true;
            Invalidate();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (Size.Width <= lastwidth - 5 || Size.Width >= lastwidth + 5)
            {
                //Change the width of the graphics
                lastwidth = this.Size.Width;
            }
            if (Size.Height <= lastheight - 5 || Size.Height >= lastheight + 5)
            {
                if (Size.Height <= Size.Width)
                {
                    SQUARESIZE = (this.Size.Height - 60) / 11;
                }
                else if (Size.Width <= Size.Height)
                {
                    SQUARESIZE = (this.Size.Width - 60) / 11;
                }
                Invalidate();

                lastheight = this.Size.Height;
            }
            if (!resizing)
            {
                reset = true;
                Invalidate();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _capturingMoves)
            {
                return;
            }

            // Might want to pad these values a bit if the line is only 1px,
            // might be hard for the user to hit directly


            _capturingMoves = true;
            lastpos = Cursor.Position;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _capturingMoves = false;
        }

        Point lastpos = new Point(0,0);
        private void MovingPanel(object sender, MouseEventArgs e)
        {
            if (_capturingMoves)
            {
                var difference = new Point(Cursor.Position.X - lastpos.X, Cursor.Position.Y-lastpos.Y);
                if (difference.X >= 10)
                {

                }
                panel1.Location = new Point(difference.X+panel1.Location.X, difference.Y + panel1.Location.Y);
                lastpos = Cursor.Position;
            }
        }
    }
}