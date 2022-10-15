using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    internal class Arrow
    {
        public Vector location;
        public double width;
        public double arrow_height; //What percentage of the height is the arrow?
        public Brush colour;

        private PointF[] points = new PointF[7];

        public int startloc;
        public int endloc;

        public Arrow(Vector location, double width, double arrow_height, Brush colour, int startloc, int endloc)
        {
            this.location = location;
            this.width = width;
            this.arrow_height = arrow_height;
            this.colour = colour;
            CalculatePoints();

            this.startloc = startloc;
            this.endloc = endloc;
        }

        public void CalculatePoints()
        {
            //Finding points of base
            var perpindicular_scale = location.GetPerpindicular().UnitVector; //Find the scale-wise value of the unit vector
            if (double.IsNaN(perpindicular_scale.x_scale))
            {
                perpindicular_scale.x_scale = 1;
                perpindicular_scale.y_scale = 0;
            }
            if (double.IsNaN(perpindicular_scale.y_scale))
            {
                perpindicular_scale.x_scale = 0;
                perpindicular_scale.y_scale = 1;
            }
            PointF bottom_right = location.Start.AddScale(perpindicular_scale, width/1.5);
            PointF bottom_left = location.Start.AddScale(perpindicular_scale, -width/1.5); //Go in the opposite direction

            //Now we find the points above
            double scale = location.magnitude - arrow_height; //'1-' means we find the length of the handle of the arrow
            PointF middle_right = bottom_right.AddScale(location.UnitVector, scale);
            PointF middle_left = bottom_left.AddScale(location.UnitVector, scale); //Go in the same direction of the arrowhead for the percentage of the distance specified

            //Find the base points of the arrow
            PointF far_right = middle_right.AddScale(perpindicular_scale, width);
            PointF far_left = middle_left.AddScale(perpindicular_scale, -width); //Again, go perpindicular to create an extrusion for the arrow-head

            PointF head = location.End;

            points = new PointF[7] {bottom_left, bottom_right, middle_right, far_right, head, far_left, middle_left}; //Draw from the bottom left, anticlockwise around the points of the arrow
        }
        public void Draw(Graphics g)
        {
            g.FillPolygon(colour, points);
        }
    }
    public class UnitVector
    {
        public double x_scale;
        public double y_scale;

        public UnitVector(double x_scale, double y_scale)
        {
            this.x_scale = x_scale;
            this.y_scale = y_scale;
        }

        public static UnitVector operator *(UnitVector a, double scale)
        {
            return new UnitVector(a.x_scale * scale, a.y_scale * scale);
        }
    }
    public class Vector
    {
        public double x1;
        public double y1;
        public double x2;
        public double y2;

        public double magnitude;

        public double Length
        {
            get
            {
                return x2 - x1;
            }
        }
        public double Height
        {
            get
            {
                return y2 - y1;
            }
        }

        public PointF Start
        {
            get
            {
                return new PointF((float)x1, (float)y1);
            }
        }
        public PointF End
        {
            get
            {
                return new PointF((float)x2, (float)y2);
            }
        }

        public Vector(Point start, Point end)
        {
            this.x1 = start.X;
            this.y1 = start.Y;

            this.x2 = end.X;
            this.y2 = end.Y;

            magnitude = Math.Sqrt(Length * Length + Height * Height);
        }

        public Vector(double x1, double y1, double x2, double y2, double magnitude)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.magnitude = magnitude;
        }

        public Vector(double x_start, double x_end, double gradient)
        {
            this.x1 = x_start;

            double c = 5;
            this.y1 = gradient * x1 + c;

            this.y2 = gradient * x_end + c;
            this.x2 = x_end;

            this.magnitude = Math.Sqrt(Length * Length + Height * Height);
        }

        public UnitVector UnitVector
        {
            get
            {
                if (double.IsNaN(Height))
                {
                    return new UnitVector(1, double.NaN);
                }
                if (double.IsNaN(Length))
                {
                    return new UnitVector(0, 1);
                }
                return new UnitVector(Length / magnitude, Height / magnitude);
            }
        }
        public Vector GetPerpindicular()
        {
            return new Vector(x1, x2, -(Length/Height)); //To get perpindicular, we just switch the x-values of the end points
        }

        public static bool operator ==(Vector a, Vector b)
        {
            if (a.x1 == b.x1 && a.x2 == b.x2 && a.y1 == b.y1 && a.y2 == b.y2)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Vector a, Vector b)
        {
            if (a.x1 != b.x1 || a.x2 != b.x2 || a.y1 != b.y1 || a.y2 != b.y2)
            {
                return true;
            }
            return false;
        }
    }

    public static class PointExtensions
    {
        public static PointF AddScale(this PointF point, UnitVector unitVector, double scale)
        {
            return new PointF((float)(point.X + unitVector.x_scale * scale), (float)(point.Y + unitVector.y_scale * scale));
        }
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2,
                             rect.Top + rect.Height / 2);
        }
    }
}
