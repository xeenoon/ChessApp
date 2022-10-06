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

        public const int SQUARESIZE = 45;

        System.Timers.Timer checkmateDelay;
        bool checkmated = false;
        bool running = false;
        bool reload = false;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (squares == null)
            {
                squares = new Squares(chessboard, new Point(15,55), SQUARESIZE, Color.FromArgb(234,233,210), Color.FromArgb(75,115,153), e.Graphics, Color.FromArgb(0,0,255), Color.FromArgb(50,50,50), this);
                squares.AI_can_move = PlayComputer.Checked;
            }
            else if (reload)
            {
                reload = false;
                squares.Reload(e.Graphics);
            }
            else
            {
                squares.Paint(e.Graphics);
                if (!checkmated && !running)
                {
                    running = true;
                    checkmateDelay = new System.Timers.Timer(100);
                    checkmateDelay.Elapsed += new System.Timers.ElapsedEventHandler(CheckMateTick);
                    checkmateDelay.Start();
                }
            }
        }

        private void CheckMateTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkmateDelay.Stop();
            var copy = squares.board.bitboard.Copy();
            copy.SetupSquareAttacks();
            if (!squares.mustMoveDuck && !squares.edit && (copy.check || copy.doublecheck) && MoveGenerator.MoveCount(copy, squares.board.hasturn) == 0 && checkmated == false) //Checkmate?
            {
                checkmated = true;
                if (squares.gameType == GameType.StandardDuck)
                {
                    MessageBox.Show("Quackmate");
                }
                else
                {
                    MessageBox.Show(String.Format("{0} Checkmated {1}", squares.board.hasturn == Side.White ? Side.Black : Side.White, copy.doublecheck ? "Like a boss" : squares.board.hasturn.ToString()));
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
                    Invalidate();
                }
                SideSquare sidesquare = SideSquare.SquareAt(mouse.Location);
                if (sidesquare != null)
                {
                    sidesquare.Click();
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
            squares.arrows.Clear();
            squares.SetupEdit(checkBox1.Checked);

            bool enabled = checkBox1.Checked;


            comboBox1.SelectedIndex = chessboard.hasturn == Side.White ? 0 : 1;
            W_KingsideCastle.Checked = chessboard.whiteCastles.Kingside;
            W_QueensideCastle.Checked = chessboard.whiteCastles.Queenside;

            B_KingsideCastle.Checked = chessboard.blackCastles.Kingside;
            B_QueensideCastle.Checked = chessboard.blackCastles.Queenside;

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
            reload = true;
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
                    FEN = "rnbqkbnr/pppppppp/7D/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                    squares.board = new Chessboard(FEN);
                    reload = true;
                }
                else if (VariantSelector.SelectedIndex == 2) //Duck duck goose
                {
                    FEN = "rnbqkbnr/pppppppp/7D/8/3G4/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
                    squares.board = new Chessboard(FEN);
                    reload = true;
                }
                squares.gameType = (GameType)VariantSelector.SelectedIndex;
                Invalidate();
            }
        }
    }
}