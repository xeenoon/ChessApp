using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessApp
{
    internal static class Extensions
    {
        public static string RemovePretext(this string s)
        {
            string result = "";
            bool insidepretext = false;
            for (int i = 0; i < s.Length; ++i)
            {
                var c = s[i];
                if (c == '[')
                {
                    insidepretext = true;
                    continue;
                }
                if (c == ']')
                {
                    insidepretext = false;
                    continue;
                }
                if (!insidepretext)
                {
                    result += c;
                }
            }
            return result;
        }
        public static string RemoveChars(this string s, char toremove)
        {
            string result = "";
            bool insidecomment = false;
            for (int i = 0; i < s.Length; ++i)
            {
                if (s[i] == '{')
                {
                    insidecomment = true;
                }
                else if (s[i] == '}')
                {
                    insidecomment = false;
                }
                else if (s[i] != toremove && !insidecomment)
                {
                    result += s[i];
                }
                else if (insidecomment)
                {
                    result += s[i];
                }
            }
            return result;
        }
        public static string RemoveCastles(this string s)
        {
            string result = "";
            bool insidecomment = false;
            int skiptimes = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                if (skiptimes >= 1)
                {
                    --skiptimes;
                    continue;
                }
                if (s[i] == '{')
                {
                    insidecomment = true;
                }
                else if (s[i] == '}')
                {
                    insidecomment = false;
                }
                else if (!insidecomment)
                {
                    var c = s[i];
                    if (c == 'O') //Castling
                    {
                        if (s[i - 2] == '.') //White?
                        {
                            if (i + 5 <= s.Length && s[i + 4] == 'O' && s[i+3] == '-') //-O-O Last O will be 4 indexes after the first one
                            {
                                skiptimes = 5;
                                result += "Kc1 ";
                            }
                            else //Kingside
                            {
                                skiptimes = 3; //-O Last O will be two indexes after
                                result += "Kg1 ";
                            }
                        }
                        else
                        {
                            if (i + 5 <= s.Length && s[i + 4] == 'O' && s[i + 3] == '-') //-O-O Last O will be 4 indexes after the first one
                            {
                                skiptimes = 5;
                                result += "Kc8 ";
                            }
                            else //Kingside
                            {
                                skiptimes = 3; //-O Last O will be two indexes after
                                result += "Kg8 ";

                            }
                        }
                        continue;
                    }
                    result += s[i];
                }
                else if (insidecomment)
                {
                    result += s[i];
                }
            }
            return result;
        }
        public static string RemovePromotions(this string s, out List<PieceType> promotions)
        {
            promotions = new List<PieceType>();

            string result = "";
            bool insidecomment = false;
            int skiptimes = 0;
            for (int i = 0; i < s.Length; ++i)
            {
                if (skiptimes >= 1)
                {
                    switch (s[i])
                    {
                        case 'Q':
                            promotions.Add(PieceType.Queen);
                            break;
                        case 'R':
                            promotions.Add(PieceType.Rook);
                            break;
                        case 'B':
                            promotions.Add(PieceType.Bishop);
                            break;
                        case 'N':
                            promotions.Add(PieceType.Knight);
                            break;
                    }
                    --skiptimes;
                    continue;
                }
                if (s[i] == '{')
                {
                    insidecomment = true;
                }
                else if (s[i] == '}')
                {
                    insidecomment = false;
                }
                else if (s[i] == '=' && !insidecomment)
                {
                    skiptimes = 1; //a1=Q, Remove the 'Q' at the end. Automatically cancelled
                    continue;
                }
                else
                {
                    result += s[i];
                }
            }
            return result;
        }
        public static int GetFileNum(this char c)
        {
            switch (c)
            {
                case 'a':
                    return 0;
                case 'b':
                    return 1;
                case 'c':
                    return 2;
                case 'd':
                    return 3;
                case 'e':
                    return 4;
                case 'f':
                    return 5;
                case 'g':
                    return 6;
                case 'h':
                    return 7;
            }
            return -1;
        }
        public static char GetFileLetter(this int c)
        {
            switch (c)
            {
                case 0:
                    return 'a';
                case 1:
                    return 'b';
                case 2:
                    return 'c';
                case 3:
                    return 'd';
                case 4:
                    return 'e';
                case 5:
                    return 'f';
                case 6:
                    return 'g';
                case 7:
                    return 'h';
            }
            return '\0';
        }
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }
        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static List<Piece> OrderByUpsideGrid(this List<Piece> list)
        {
            List<Piece> result = new List<Piece>();
            int lastrow = 7;
            List<Piece> cache = new List<Piece>();
            foreach (var item in list.OrderByDescending(i => i.position))
            {
                if (item.position / 8 != lastrow)
                {
                    cache.Reverse();
                    result.AddRange(cache);
                    cache = new List<Piece>();
                    lastrow = item.position / 8;
                }
                cache.Add(item);
            }
            cache.Reverse();
            result.AddRange(cache);
            return result;
        }
    }
}
