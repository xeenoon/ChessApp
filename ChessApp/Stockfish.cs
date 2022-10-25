using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace ChessApp
{
    public class Stockfish
    {
        public int depth;
        public Form1 mainform;
        public List<Move> bestmoves = new List<Move>(20);

        Process process = new Process();

        public Stockfish(Form1 mainform)
        {
            this.mainform = mainform;
        }

        public void Start()
        {
            bestmoves = new List<Move>(20);

            process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = @"C:\Users\ccw10\Downloads\stockfish_15_win_x64\stockfish_15_x64.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();

            Thread asyncread = new Thread(new ThreadStart(ReadData));
            asyncread.Start();

            Thread asyncwrite = new Thread(new ThreadStart(WriteData));
            asyncwrite.Start();
        }

        private void ReadData()
        {
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.StartsWith("info depth ") && line.Contains("currmove"))
                {
                    line = line.Substring(11);
                    int depth = int.Parse(line.Split(' ')[0]);
                    line = line.Substring(depth.ToString().Length + 1 + "currmove ".Length);
                    var movedata = line.Split(' ')[0];
                    var startcol = movedata[0].GetFileNum();
                    var startrow = int.Parse(movedata[1].ToString())-1;
                    int startpos = startrow * 8 + startcol;

                    var endcol = movedata[2].GetFileNum();
                    var endrow = int.Parse(movedata[3].ToString()) - 1;
                    int endpos = endrow * 8 + endcol;

                    Move move = new Move((byte)startpos, (byte)endpos, PieceType.None);
                    int index = int.Parse(line.Split(' ').Last()) - 1;
                    if (index >= 3)
                    {
                        mainform.DrawStockfish(bestmoves);
                        continue;
                    }
                    if (bestmoves.Count() <= index)
                    {
                        bestmoves.Add(move);
                    }
                    else
                    {
                        bestmoves[index] = move;
                    }
                }
                if (line.StartsWith("bestmove "))
                {
                    var movedata = line.Substring(9,4);
                    var startcol = movedata[0].GetFileNum();
                    var startrow = int.Parse(movedata[1].ToString()) - 1;
                    int startpos = startrow * 8 + startcol;

                    var endcol = movedata[2].GetFileNum();
                    var endrow = int.Parse(movedata[3].ToString()) - 1;
                    int endpos = endrow * 8 + endcol;
                    Move move = new Move((byte)startpos, (byte)endpos, PieceType.None);

                    if (bestmoves.Count() == 0)
                    {
                        bestmoves.Add(move);
                    }
                    else
                    {
                        bestmoves[0] = move;
                    }
                    mainform.DrawStockfish(bestmoves);
                }
                // do something with line
            }   
        }
        string lastFEN = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private void WriteData()
        {
            process.StandardInput.WriteLine("stop");
            process.StandardInput.WriteLine("position fen " + lastFEN);
            process.StandardInput.WriteLine("go movetime 100");
            Thread.Sleep(200);
            process.StandardInput.WriteLine("go infinite");
        }

        public Move GetMove()
        {
            return new Move(4, 12, PieceType.Pawn);
        }

        internal void UpdateFEN(Chessboard board)
        {
            if (lastFEN != board.GetFEN())
            {
                bestmoves.Clear();
                lastFEN = board.GetFEN();

                Thread asyncwrite = new Thread(new ThreadStart(WriteData));
                asyncwrite.Start();
            }
        }
    }
}
