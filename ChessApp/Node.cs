﻿using System;
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
        public Node parent;
        public List<Node> children = new List<Node>();
        public static int totalnodes = 0;

        public Node(Bitboard b, Side hasturn, Node parent)
        {
            this.b = b;
            this.hasturn = hasturn;
            this.parent = parent;
            ++totalnodes;
        }
        public void Populate(int nodes)
        {
            if (nodes == 0)
            {
                return;
            }
            nodes--;
            b.SetupSquareAttacks();
            var otherturn = hasturn == Side.White ? Side.Black : Side.White;
            Parallel.ForEach(MoveGenerator.CalculateAll(b, hasturn), move =>
            {
                var copy = b.Copy();
                if (hasturn == Side.White)
                {
                    switch (move.pieceType)
                    {
                        case PieceType.Pawn:
                            copy.W_Pawn ^= move.last;
                            copy.W_Pawn ^= move.current;
                            break;
                        case PieceType.Rook:
                            copy.W_Rook ^= move.last;
                            copy.W_Rook ^= move.current;
                            break;
                        case PieceType.Knight:
                            copy.W_Knight ^= move.last;
                            copy.W_Knight ^= move.current;
                            break;
                        case PieceType.Bishop:
                            copy.W_Bishop ^= move.last;
                            copy.W_Bishop ^= move.current;
                            break;
                        case PieceType.Queen:
                            copy.W_Queen ^= move.last;
                            copy.W_Queen ^= move.current;
                            break;
                        case PieceType.King:
                            copy.W_King ^= move.last;
                            copy.W_King ^= move.current;
                            break;
                    }
                }
                else
                {
                    switch (move.pieceType)
                    {
                        case PieceType.Pawn:
                            copy.B_Pawn ^= move.last;
                            copy.B_Pawn ^= move.current;
                            break;
                        case PieceType.Rook:
                            copy.B_Rook ^= move.last;
                            copy.B_Rook ^= move.current;
                            break;
                        case PieceType.Knight:
                            copy.B_Knight ^= move.last;
                            copy.B_Knight ^= move.current;
                            break;
                        case PieceType.Bishop:
                            copy.B_Bishop ^= move.last;
                            copy.B_Bishop ^= move.current;
                            break;
                        case PieceType.Queen:
                            copy.B_Queen ^= move.last;
                            copy.B_Queen ^= move.current;
                            break;
                        case PieceType.King:
                            copy.B_King ^= move.last;
                            copy.B_King ^= move.current;
                            break;
                    }
                }
                //Move has been simulated, now create new node
                Node n = new Node(copy, otherturn, this);
                n.Populate(nodes);
            });
        }
    }
}