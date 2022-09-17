﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp
{
    public class AI
    {
        public static Move GetMove(Bitboard currentState, Side hasturn)
        {
            var bitboard = currentState.Copy();
            bitboard.SetupSquareAttacks();
            
            int max = int.MinValue;
            Move best = new Move(0,0,PieceType.None);

            bitboard.SetupSquareAttacks();
            foreach (var move in MoveGenerator.CalculateAll(bitboard, hasturn))
            {
                var data = bitboard.Move(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                var score = mini(3, bitboard, hasturn == Side.White ? Side.Black : Side.White);
                bitboard.UndoMove(data);
                if (score > max)
                {
                    max = score;
                    best = move;
                }
            }
            return best;
        }
        static int maxi(int depth, Bitboard b, Side hasturn)
        {
            if (depth == 0)
            {
                return b.evaluate();
            }
            int max = int.MinValue;
            b.SetupSquareAttacks();

            foreach (var move in MoveGenerator.CalculateAll(b, hasturn))
            {
                var data = b.Move(move.last, move.current, 1ul<<move.last, 1ul<<move.current, move.pieceType, hasturn);
                var score = mini(depth - 1, b, hasturn == Side.White ? Side.Black : Side.White);
                b.UndoMove(data);
                if (score > max)
                    max = score;
            }
            return max;
        }

        static int mini(int depth, Bitboard b, Side hasturn)
        {
            if (depth == 0)
            {
                return -b.evaluate();
            }
            int min = int.MaxValue;
            b.SetupSquareAttacks();
            foreach (var move in MoveGenerator.CalculateAll(b, hasturn))
            {
                var data = b.Move(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                var score = maxi(depth - 1, b, hasturn == Side.White ? Side.Black : Side.White);
                b.UndoMove(data);
                if (score < min)
                    min = score;
            }
            return min;
        }
    }
}
