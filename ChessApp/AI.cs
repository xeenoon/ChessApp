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

            EvaluationData max = new EvaluationData(int.MinValue, int.MaxValue);
            Move best = new Move(0, 0, PieceType.None);

            bitboard.SetupSquareAttacks();

            Parallel.ForEach(MoveGenerator.CalculateAll(bitboard, hasturn), (move, state) =>
            {
                var copy = bitboard.CopyMove(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                EvaluationData score = new EvaluationData(0,0);

                if (hasturn == Side.Black)
                {
                    score = alphaBetaMin(int.MinValue, int.MaxValue, 4, copy, Side.White);
                }
                else
                {
                    score = alphaBetaMax(int.MinValue, int.MaxValue, 4, copy, Side.Black);
                }
                if (score.eval >= max.eval)
                {
                    if (score.eval == max.eval) //Same eval?
                    {
                        if (score.checkmate_depth_left > max.checkmate_depth_left)
                        {
                            max = score;
                            best = move;
                        }
                    }
                    else
                    {
                        max = score;
                        best = move;
                    }
                }
            });
            return best;
        }
        static EvaluationData alphaBetaMax(int alpha, int beta, int depthleft, Bitboard b, Side hasturn)
        {
            if (depthleft == 0)
            {
                return new EvaluationData(Evaluation.Evaluate(b, hasturn), depthleft);
            }
            b.SetupSquareAttacks();
            bool didnothing = true;
            foreach (var move in MoveGenerator.CalculateAll(b, hasturn))
            {
                didnothing = false;
                var data = b.Move(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                var score = alphaBetaMin(alpha, beta, depthleft - 1, b, hasturn == Side.White ? Side.Black : Side.White);
                b.UndoMove(data);
                if (score.eval >= beta)
                    return new EvaluationData(beta, depthleft);   // fail hard beta-cutoff
                if (score.eval > alpha)
                    alpha = score.eval; // alpha acts like max in MiniMax
            }
            if (didnothing)
            {
                return new EvaluationData(Evaluation.Evaluate(b, hasturn, true), depthleft);
            }
            return new EvaluationData(alpha, 0);
        }
        
        static EvaluationData alphaBetaMin(int alpha, int beta, int depthleft, Bitboard b, Side hasturn)
        {
            if (depthleft == 0)
            {
                int v = Evaluation.Evaluate(b, hasturn);
                if (v == int.MinValue)
                {
                    return new EvaluationData(v - 1, depthleft);
                }

                return new EvaluationData(-1 * v, 0);
            }
            b.SetupSquareAttacks();
            bool didnothing = true;
            foreach (var move in MoveGenerator.CalculateAll(b, hasturn))
            {
                didnothing = false;
                var data = b.Move(move.last, move.current, 1ul << move.last, 1ul << move.current, move.pieceType, hasturn);
                var score = alphaBetaMax(alpha, beta, depthleft - 1, b, hasturn == Side.White ? Side.Black : Side.White);
                b.UndoMove(data);
                if (score.eval <= alpha)
                    return new EvaluationData(alpha, depthleft); // fail hard alpha-cutoff
                if (score.eval < beta)
                    beta = score.eval; // beta acts like min in MiniMax
            }
            if (didnothing)
            {
                int v = Evaluation.Evaluate(b, hasturn, true);
                if (v == int.MinValue)
                {
                    return new EvaluationData(v - 1, depthleft);
                }
                else
                {
                    return new EvaluationData(-v, 0);
                }
            }
            return new EvaluationData(beta, 0);
        }
    }
}
