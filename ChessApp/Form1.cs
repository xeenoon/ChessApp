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
            textBox1.Text = FEN;
        }
        public void WriteFEN()
        {
            textBox1.Text = chessboard.GetFEN();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            chessboard = new Chessboard(textBox1.Text);
            squares = null;

            Invalidate();
        }

        const int SQUARESIZE = 45;
        System.Timers.Timer checkmateDelay;
        bool checkmated = false;
        bool running = false;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (squares == null)
            {
                squares = new Squares(chessboard, new Point(15,55), SQUARESIZE, Color.FromArgb(234,233,210), Color.FromArgb(75,115,153), e.Graphics, Color.FromArgb(0,0,255), Color.FromArgb(50,50,50));
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
            if (!squares.edit && copy.check || copy.doublecheck && MoveGenerator.MoveCount(copy, squares.board.hasturn) == 0 && checkmated == false) //Checkmate?
            {
                checkmated = true;
                MessageBox.Show(String.Format("{0} Checkmated {1}", squares.board.hasturn == Side.White ? Side.Black : Side.White, copy.doublecheck ? "Like a boss" : squares.board.hasturn.ToString()));
            }
            running = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
            squares.SetupEdit(checkBox1.Checked);
            comboBox1.Visible = checkBox1.Checked;
            comboBox1.SelectedIndex = chessboard.hasturn == Side.White ? 0 : 1;
            Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                chessboard.hasturn = comboBox1.SelectedIndex == 0 ? Side.White : Side.Black;
                textBox1.Text = chessboard.GetFEN();
            }
        }
    }
}