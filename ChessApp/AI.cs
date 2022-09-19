using System;
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

            Parallel.ForEach(MoveGenerator.CalculateAll(bitboard, hasturn), move =>
            {
                var copy = bitboard.CopyMove(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                int score = 0;
                
                if (hasturn == Side.Black)
                {
                    score = alphaBetaMin(int.MinValue, int.MaxValue, 4, copy, Side.White);
                }
                else
                {
                    score = alphaBetaMax(int.MinValue, int.MaxValue, 4, copy, Side.Black);
                }
                if (score > max)
                {
                    max = score;
                    best = move;
                }
            });
            return best;
        }


        static int alphaBetaMax(int alpha, int beta, int depthleft, Bitboard b, Side hasturn)
        {
            if (depthleft == 0)
            {
                return Evaluation.Evaluate(b);
            }
            b.SetupSquareAttacks();
            foreach (var move in MoveGenerator.CalculateAll(b, hasturn))
            {
                var data = b.Move(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                var score = alphaBetaMin(alpha, beta, depthleft - 1, b, hasturn == Side.White ? Side.Black : Side.White);
                b.UndoMove(data);
                if (score >= beta)
                    return beta;   // fail hard beta-cutoff
                if (score > alpha)
                    alpha = score; // alpha acts like max in MiniMax
            }
            return alpha;
        }
        
        static int alphaBetaMin(int alpha, int beta, int depthleft, Bitboard b, Side hasturn)
        {
            if (depthleft == 0)
            {
                return -Evaluation.Evaluate(b);
            }
            b.SetupSquareAttacks();
            foreach (var move in MoveGenerator.CalculateAll(b, hasturn))
            {
                var data = b.Move(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                var score = alphaBetaMax(alpha, beta, depthleft - 1, b, hasturn == Side.White ? Side.Black : Side.White);
                b.UndoMove(data);
                if (score <= alpha)
                    return alpha; // fail hard alpha-cutoff
                if (score < beta)
                    beta = score; // beta acts like min in MiniMax
            }
            return beta;
        }
    }
}
