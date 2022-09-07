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
            chessboard = new Chessboard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - 0 1");
            webBrowser1.DocumentText = Properties.Resources.html + chessboard.GetHtml();
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


        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection children = webBrowser1.Document.All;

            foreach (HtmlElement child in children)
            {
                if (child.Id == "tablecontainer")
                {
                    var height = (webBrowser1.Height-10).ToString() + "px";

                    var currentStyle = child.Style;
                    child.Style = String.Format("height: {0}; width: {0}; max-height: {0}, max-width: {0}", height) + currentStyle;
                    

                    var size = child.ClientRectangle;

                    //Adding pieces
                    var chessboard = child.FirstChild;
                }
            }

            this.webBrowser1.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown);
        }
        void Body_MouseDown(Object sender, HtmlElementEventArgs e)
        {
            if (e.MouseButtonsPressed == MouseButtons.Left)
            {
                HtmlElement element = this.webBrowser1.Document.GetElementFromPoint(e.ClientMousePosition);
                try
                {
                    if (element.Id == null)
                    {
                        return;
                    }
                    var location = int.Parse(element.Id.Split(new[] { ":" }, StringSplitOptions.None)[1]);
                    chessboard.Click(location, element);
                }
                catch
                {

                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            chessboard = new Chessboard(textBox1.Text);
            webBrowser1.DocumentText = Properties.Resources.html + chessboard.GetHtml();
        }
    }
}