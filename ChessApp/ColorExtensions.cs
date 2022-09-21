using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorExtensions
{
    public class Filter
    {
        public int R;
        public int G;
        public int B;

        public Filter(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        internal Color AsColor()
        {
            return Color.FromArgb(R, G, B);
        }
    }

    public static class ColorExtensions
    {
        public static Color AddFilter(this Color color, Filter filter)
        {
            int r = color.R + filter.R;
            int g = color.G + filter.G;
            int b = color.B + filter.B;
            if (r >= 255)
            {
                r = 255;
            }
            if (g >= 255)
            {
                g = 255;
            }
            if (b >= 255)
            {
                b = 255;
            }

            if (r <= 0)
            {
                r = 0;
            }
            if (g <= 0)
            {
                g = 0;
            }
            if (b <= 0)
            {
                b = 0;
            }
            return Color.FromArgb(r,g,b);
        }
    }
}
