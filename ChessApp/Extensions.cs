using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    internal static class Extensions
    {
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
