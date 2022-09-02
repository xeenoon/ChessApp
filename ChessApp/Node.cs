using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    internal class Node
    {
        public Bitboard b;
        public Side hasturn;
        Node parent;
        List<Node> children = new List<Node>();
        public Node(Bitboard b, Side hasturn, Node parent)
        {

        }
    }
}
