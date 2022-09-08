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
        public const string BLACK_PAWN   = @"https://upload.wikimedia.org/wikipedia/commons/thumb/c/c7/Chess_pdt45.svg/45px-Chess_pdt45.svg.png";
        public const string BLACK_ROOK   = @"https://upload.wikimedia.org/wikipedia/commons/thumb/f/ff/Chess_rdt45.svg/45px-Chess_rdt45.svg.png";
        public const string BLACK_KNIGHT = @"https://upload.wikimedia.org/wikipedia/commons/thumb/e/ef/Chess_ndt45.svg/45px-Chess_ndt45.svg.png";
        public const string BLACK_BISHOP = @"https://upload.wikimedia.org/wikipedia/commons/thumb/9/98/Chess_bdt45.svg/45px-Chess_bdt45.svg.png";
        public const string BLACK_KING   = @"https://upload.wikimedia.org/wikipedia/commons/thumb/f/f0/Chess_kdt45.svg/45px-Chess_kdt45.svg.png";
        public const string BLACK_QUEEN  = @"https://upload.wikimedia.org/wikipedia/commons/thumb/4/47/Chess_qdt45.svg/45px-Chess_qdt45.svg.png";

        public const string WHITE_PAWN   = @"https://upload.wikimedia.org/wikipedia/commons/thumb/4/45/Chess_plt45.svg/45px-Chess_plt45.svg.png";
        public const string WHITE_ROOK   = @"https://upload.wikimedia.org/wikipedia/commons/thumb/7/72/Chess_rlt45.svg/45px-Chess_rlt45.svg.png";
        public const string WHITE_KNIGHT = @"https://upload.wikimedia.org/wikipedia/commons/thumb/7/70/Chess_nlt45.svg/45px-Chess_nlt45.svg.png";
        public const string WHITE_BISHOP = @"https://upload.wikimedia.org/wikipedia/commons/thumb/b/b1/Chess_blt45.svg/45px-Chess_blt45.svg.png";
        public const string WHITE_KING   = @"https://upload.wikimedia.org/wikipedia/commons/thumb/4/42/Chess_klt45.svg/45px-Chess_klt45.svg.png";
        public const string WHITE_QUEEN  = @"https://upload.wikimedia.org/wikipedia/commons/thumb/1/15/Chess_qlt45.svg/45px-Chess_qlt45.svg.png";

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