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
        public Form1()
        {
            InitializeComponent();
            chessboard = new Chessboard("2bQKb2/nppppppn/r6r/p2pp2p/P2PP2P/R6R/NPPPPPPN/2BqkB2 w - 0 1");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chessboard = new Chessboard(textBox1.Text);
        }

        Point lastclick = new Point(-1,-1);
        Panel tohighlight = null;
        const int SQUARESIZE = 45;
        private void SquareClick(object sender, EventArgs e)
        {
            Panel square = ((Panel)sender);
            var location = square.Location;
            location.X = location.X - 15;
            location.Y = location.Y - 55;

            location.X /= SQUARESIZE;
            location.Y /= SQUARESIZE;
            location.Y = 7 - location.Y;

            bool light = (location.X + location.Y) % 2 != 0;
            if (lastclick != location)
            {
                tohighlight = square;
                square.Refresh();

                if (lastclick.X != -1)
                {
                    Point last = new Point((lastclick.X * SQUARESIZE)+15, ((7-lastclick.Y) * SQUARESIZE) + 55);
                    var panels = Controls.Cast<Control>().Where(p=>p.GetType() == square.GetType());
                    var panel = ((Panel)panels.Where(p => p.Location == last).FirstOrDefault());
                    tohighlight = null;
                    panel.Refresh();
                }
                lastclick = location;
            }
            else
            {
                tohighlight = null;
                square.Refresh();
                lastclick = new Point(-1,-1);
            }
        }

        private void SquarePaint(object sender, PaintEventArgs e)
        {
            Panel _panel = (Panel)(sender);

            var location = _panel.Location;
            location.X = location.X - 15;
            location.Y = location.Y - 55;

            location.X /= SQUARESIZE;
            location.Y /= SQUARESIZE;

            Piece piece = chessboard.Pieces.Where(p => p.position == location).FirstOrDefault();
            if (piece != null)
            {
                e.Graphics.DrawImage(piece.IMG, new Rectangle(0,0,SQUARESIZE, SQUARESIZE));
            }
            if (tohighlight == _panel)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(new Pen(Color.DarkBlue, 2), 1f, 1f, SQUARESIZE - 3f, SQUARESIZE - 3f);
            }
        }
    }
}