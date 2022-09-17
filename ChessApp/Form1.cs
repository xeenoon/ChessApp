using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessApp
{
    public partial class Form1 : Form
    {
        Chessboard chessboard;
        Squares squares;

        public Form1()
        {
            InitializeComponent();
            chessboard = new Chessboard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            chessboard = new Chessboard(textBox1.Text);
            squares = null;
            Invalidate();
        }

        const int SQUARESIZE = 45;
        bool reload = false;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (squares == null)
            {
                squares = new Squares(chessboard, new Point(15,55), SQUARESIZE, Color.FromArgb(234,233,210), Color.FromArgb(75,115,153), e.Graphics, Color.FromArgb(0,0,255), Color.FromArgb(50,50,50));
            }
            else if (reload)
            {
                reload = false;
                squares.Reload(e.Graphics);
            }
            else
            {
                squares.Paint(e.Graphics);
            }
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
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            squares.UndoMove();
            this.chessboard = squares.board;
            reload = true;
            Invalidate();
        }
    }
}