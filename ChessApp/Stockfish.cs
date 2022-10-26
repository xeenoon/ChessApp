using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Drawing;

namespace ChessApp
{
    public class Stockfish
    {
        public int depth;
        public Form1 mainform;
        public List<Move> bestmoves = new List<Move>();
        public Bitmap data;

        Process process = new Process();

        public Stockfish(Form1 mainform)
        {
            this.mainform = mainform;
        }

        public void Start()
        {
            bestmoves = new List<Move>();

            process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = @"C:\Users\ccw10\Downloads\stockfish_15_win_x64\stockfish_15_x64.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            Thread asyncread = new Thread(new ThreadStart(ReadData));
            asyncread.Start();

            Thread asyncwrite = new Thread(new ThreadStart(WriteData));
            asyncwrite.Start();
        }

        string lasteval = "0";
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
                    var startrow = int.Parse(movedata[1].ToString()) - 1;
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
                    try
                    {
                        var movedata = line.Substring(9, 4);
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
                    catch
                    {

                    }
                }
                if (line.Contains("score cp"))
                {
                    string s = line.Substring(line.IndexOf("score cp ") + 9).Split(' ')[0];
                    if (s[0] == '+')
                    {
                        s = s.Substring(1);
                    }
                    var score = float.Parse(s);
                    score = score / 100;
                    lasteval = score.ToString();
                }
                else if (line.Contains("score mate "))
                {
                    string s = line.Substring(line.IndexOf("score mate ") + 11).Split(' ')[0];
                    if (s[0] == '-')
                    {
                        lasteval = "-M" + s.Substring(1);
                    }
                    else
                    {
                        lasteval = "M" + s;
                    }
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
            process.StandardInput.WriteLine("eval");
            mainform.Invalidate();
            process.StandardInput.WriteLine("go infinite");
        }

        public Move GetMove()
        {
            return new Move(4, 12, PieceType.Pawn);
        }
        Side hasturn = Side.White;
        public Bitboard b;
        internal void UpdateFEN(Chessboard board)
        {
            if (lastFEN != board.GetFEN())
            {
                if (board.bitboard.W_King == 0 || board.bitboard.B_King == 0)
                {
                    return;
                }

                hasturn = board.hasturn;
                b = board.bitboard.Copy();
                bestmoves.Clear();
                lastFEN = board.GetFEN();

                Thread asyncwrite = new Thread(new ThreadStart(WriteData));
                asyncwrite.Start();
            }
        }

        public Bitmap Draw(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            if (b.W_King == 0 || b.B_King == 0)
            {
                return bmp;
            }
            Graphics g = Graphics.FromImage(bmp);
            //Draw top move at 1/3 of the height
            var movesize = height / 3;
            for (int i = 0; i < bestmoves.Count(); ++i)
            {
                var eval = lasteval;
                if (hasturn == Side.Black)
                {
                    if (eval[0] != '-')
                    {
                        eval = eval.Insert(0, "-");
                    }
                    else
                    {
                        eval = eval.Substring(1);
                    }
                }
                Color boxcolour = eval[0] == '-' ? Color.Black : Color.White;
                g.FillRectangle(new Pen(boxcolour).Brush, new Rectangle(0, 0, width, 20));

                var move = bestmoves[0];
                var copy = b.Copy();
                foreach (var piecetype in new List<PieceType>() { PieceType.Pawn, PieceType.Knight, PieceType.Rook, PieceType.Bishop, PieceType.Queen, PieceType.King })
                {
                    var bb = copy.GetBitboard(piecetype, hasturn);
                    if (((1ul << move.last) & bb) != 0)
                    {
                        move.pieceType = piecetype;
                        break;
                    }
                }

                copy.SetupSquareAttacks();
                List<int> possibleStartPositions = new List<int>();

                foreach (var b_move in MoveGenerator.CalculateAll(copy, hasturn))
                {
                    if (b_move.pieceType == move.pieceType && b_move.current == move.current)
                    {
                        possibleStartPositions.Add(b_move.last);
                    }
                }
                string movestring = "";
                string rowcol = "";
                possibleStartPositions = possibleStartPositions.Distinct().ToList();
                if (possibleStartPositions.Count() >= 2)
                {
                    int startcol = move.last % 8;
                    int startrow = move.last / 8;

                    List<int> samecol = possibleStartPositions.Where(m => m % 8 == startcol).ToList();
                    List<int> samerow = possibleStartPositions.Where(m => m / 8 == startrow).ToList();
                    List<int> same = possibleStartPositions.Where(m => m / 8 == startrow && m % 8 == startcol).ToList();
                    if (samecol.Count() == 1) //Just need to specify column?
                    {
                        rowcol = (samecol[0] / 8).GetFileLetter().ToString();
                    }
                    else if (samerow.Count() == 1) //Just need to specify column?
                    {
                        rowcol = ((samerow[0] % 8) + 1).ToString();
                    }
                    else if (same.Count() == 1) //Just need to specify column?
                    {
                        rowcol = (same[0] / 8).GetFileLetter().ToString() + ((same[0] % 8) + 1).ToString();
                    }
                }

                var piecetypestring = "";
                string endposition = (move.current % 8).GetFileLetter().ToString() + ((move.current / 8) + 1).ToString();
                switch (move.pieceType)
                {
                    case PieceType.Pawn:
                        piecetypestring = "";
                        break;
                    case PieceType.Rook:
                        piecetypestring = "R";
                        break;
                    case PieceType.Knight:
                        piecetypestring = "N";
                        break;
                    case PieceType.Bishop:
                        piecetypestring = "B";
                        break;
                    case PieceType.Queen:
                        piecetypestring = "Q";
                        break;
                    case PieceType.King:
                        piecetypestring = "K";
                        break;
                }
                string promotionstring = "";
                if (move.pieceType == PieceType.Pawn && (move.current / 8 == 0 || move.current / 8 == 7))
                {
                    string promotionletter = "";
                    switch (move.promotion)
                    {
                        case PieceType.Rook:
                            promotionletter = "R";
                            break;
                        case PieceType.Knight:
                            promotionletter = "N";
                            break;
                        case PieceType.Bishop:
                            promotionletter = "B";
                            break;
                        case PieceType.Queen:
                            promotionletter = "Q";
                            break;
                    }
                    promotionstring = "=" + promotionletter;
                }
                string istaking = "";
                if (hasturn == Side.White)
                {
                    if ((b.BlackPieces & (1ul << move.current)) != 0) //Taking a piece
                    {
                        istaking = "x";
                    }
                }
                if (hasturn == Side.Black)
                {
                    if ((b.WhitePieces & (1ul << move.current)) != 0) //Taking a piece
                    {
                        istaking = "x";
                    }
                }
                if (istaking == "x" && move.pieceType == PieceType.Pawn) //Pawn taking
                {
                    //We must specify row-col
                    rowcol = (move.last % 8).GetFileLetter().ToString();
                }
                movestring = string.Format("{0}{1}{4}{2}{3}", piecetypestring, rowcol, endposition, promotionstring, istaking);
                switch (movestring)
                {
                    case "Kg1":
                        movestring = "O-O";
                        break;
                    case "Kg8":
                        movestring = "O-O";
                        break;
                    case "Kc1":
                        movestring = "O-O-O";
                        break;
                    case "Kc8":
                        movestring = "O-O-O";
                        break;
                }


                g.DrawString(eval.ToString() + " " + movestring, new Font("Arial", 10, FontStyle.Bold), new Pen(boxcolour == Color.White ? Color.Black : Color.White).Brush, new PointF(0, 0));
            }
            return bmp;
        }
    }
    public class StockfishMove
    {
        public Move move;
        public Chessboard board;
        public Bitboard b;

        public string movestring;
        public string evaluation;
        
        Process stockfish;

        public StockfishMove(string movedata, Chessboard board)
        {
            this.movestring = movedata;

            var startcol = movedata[0].GetFileNum();
            var startrow = int.Parse(movedata[1].ToString()) - 1;
            int startpos = startrow * 8 + startcol;

            var endcol = movedata[2].GetFileNum();
            var endrow = int.Parse(movedata[3].ToString()) - 1;
            int endpos = endrow * 8 + endcol;

            move = new Move((byte)startpos, (byte)endpos, PieceType.None);
            b = board.bitboard;

            //Start stockfish evaluation of the move
            stockfish = new Process();
            // Configure the process using the StartInfo properties.
            stockfish.StartInfo.FileName = @"C:\Users\ccw10\Downloads\stockfish_15_win_x64\stockfish_15_x64.exe";
            stockfish.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            stockfish.StartInfo.RedirectStandardInput = true;
            stockfish.StartInfo.RedirectStandardOutput = true;
            stockfish.StartInfo.UseShellExecute = false;
            stockfish.StartInfo.CreateNoWindow = true;
            stockfish.Start();

            Thread asyncread = new Thread(new ThreadStart(ReadData));
            asyncread.Start();

            stockfish.StandardInput.WriteLine("position fen " + board.GetFEN());
            stockfish.StandardInput.WriteLine("go searchmoves " + movedata);
        }
        private void ReadData()
        {
            while (!stockfish.StandardOutput.EndOfStream)
            {
                string line = stockfish.StandardOutput.ReadLine();
                if (line.Contains("score cp"))
                {
                    string s = line.Substring(line.IndexOf("score cp ") + 9).Split(' ')[0];
                    if (s[0] == '+')
                    {
                        s = s.Substring(1);
                    }
                    var score = float.Parse(s);
                    score = score / 100;
                    evaluation = score.ToString();
                }
                else if (line.Contains("score mate "))
                {
                    string s = line.Substring(line.IndexOf("score mate ") + 11).Split(' ')[0];
                    if (s[0] == '-')
                    {
                        evaluation = "-M" + s.Substring(1);
                    }
                    else
                    {
                        evaluation = "M" + s;
                    }
                }
            }
        }
        public override string ToString()
        {
            var copy = b.Copy();
            foreach (var piecetype in new List<PieceType>() { PieceType.Pawn, PieceType.Knight, PieceType.Rook, PieceType.Bishop, PieceType.Queen, PieceType.King })
            {
                var bb = copy.GetBitboard(piecetype, board.hasturn);
                if (((1ul << move.last) & bb) != 0)
                {
                    move.pieceType = piecetype;
                    break;
                }
            }

            copy.SetupSquareAttacks();
            List<int> possibleStartPositions = new List<int>();

            foreach (var b_move in MoveGenerator.CalculateAll(copy, board.hasturn))
            {
                if (b_move.pieceType == move.pieceType && b_move.current == move.current)
                {
                    possibleStartPositions.Add(b_move.last);
                }
            }
            string movestring = "";
            string rowcol = "";
            possibleStartPositions = possibleStartPositions.Distinct().ToList();
            if (possibleStartPositions.Count() >= 2)
            {
                int startcol = move.last % 8;
                int startrow = move.last / 8;

                List<int> samecol = possibleStartPositions.Where(m => m % 8 == startcol).ToList();
                List<int> samerow = possibleStartPositions.Where(m => m / 8 == startrow).ToList();
                List<int> same = possibleStartPositions.Where(m => m / 8 == startrow && m % 8 == startcol).ToList();
                if (samecol.Count() == 1) //Just need to specify column?
                {
                    rowcol = (samecol[0] / 8).GetFileLetter().ToString();
                }
                else if (samerow.Count() == 1) //Just need to specify column?
                {
                    rowcol = ((samerow[0] % 8) + 1).ToString();
                }
                else if (same.Count() == 1) //Just need to specify column?
                {
                    rowcol = (same[0] / 8).GetFileLetter().ToString() + ((same[0] % 8) + 1).ToString();
                }
            }

            var piecetypestring = "";
            string endposition = (move.current % 8).GetFileLetter().ToString() + ((move.current / 8) + 1).ToString();
            switch (move.pieceType)
            {
                case PieceType.Pawn:
                    piecetypestring = "";
                    break;
                case PieceType.Rook:
                    piecetypestring = "R";
                    break;
                case PieceType.Knight:
                    piecetypestring = "N";
                    break;
                case PieceType.Bishop:
                    piecetypestring = "B";
                    break;
                case PieceType.Queen:
                    piecetypestring = "Q";
                    break;
                case PieceType.King:
                    piecetypestring = "K";
                    break;
            }
            string promotionstring = "";
            if (move.pieceType == PieceType.Pawn && (move.current / 8 == 0 || move.current / 8 == 7))
            {
                string promotionletter = "";
                switch (move.promotion)
                {
                    case PieceType.Rook:
                        promotionletter = "R";
                        break;
                    case PieceType.Knight:
                        promotionletter = "N";
                        break;
                    case PieceType.Bishop:
                        promotionletter = "B";
                        break;
                    case PieceType.Queen:
                        promotionletter = "Q";
                        break;
                }
                promotionstring = "=" + promotionletter;
            }
            string istaking = "";
            if (board.hasturn == Side.White)
            {
                if ((b.BlackPieces & (1ul << move.current)) != 0) //Taking a piece
                {
                    istaking = "x";
                }
            }
            if (board.hasturn == Side.Black)
            {
                if ((b.WhitePieces & (1ul << move.current)) != 0) //Taking a piece
                {
                    istaking = "x";
                }
            }
            if (istaking == "x" && move.pieceType == PieceType.Pawn) //Pawn taking
            {
                //We must specify row-col
                rowcol = (move.last % 8).GetFileLetter().ToString();
            }
            movestring = string.Format("{0}{1}{4}{2}{3}", piecetypestring, rowcol, endposition, promotionstring, istaking);
            switch (movestring)
            {
                case "Kg1":
                    movestring = "O-O";
                    break;
                case "Kg8":
                    movestring = "O-O";
                    break;
                case "Kc1":
                    movestring = "O-O-O";
                    break;
                case "Kc8":
                    movestring = "O-O-O";
                    break;
            }
            return movestring;
        }
    }
    public class StockfishMoveHandler
    {
        public List<StockfishMove> stockfishmoves = new List<StockfishMove>();
        public Chessboard boardstate;
        public StockfishMoveHandler(Chessboard boardstate)
        {
            this.boardstate = boardstate;
        }
        public void Add(string movedata, int idx)
        {
            if (stockfishmoves.Any(s=>s.movestring == movedata)) //Already in it?
            {
                if (stockfishmoves[idx].movestring != movedata) //Are we going to be swapping around moves
                {
                    StockfishMove move = stockfishmoves.FirstOrDefault(s => s.movestring == movedata);

                    int oldidx = stockfishmoves.IndexOf(move);
                    stockfishmoves.RemoveAt(oldidx);
                    stockfishmoves.Insert(idx, move);
                }
            }
            else
            {
                stockfishmoves.Add(new StockfishMove(movedata, boardstate));
            }
        }
    }
}
