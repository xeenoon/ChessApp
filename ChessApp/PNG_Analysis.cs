using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessApp
{
    public class PNG_Analysis
    {
        public string PNG;
        public Graphics graphics;

        public PNG_Analysis(string PNG, Graphics graphics)
        {
            this.PNG = PNG;
            this.graphics = graphics;
        }
        public void Paint(Graphics graphics = null)
        {
            if (graphics == null)
            {
                graphics = this.graphics; //Allow specific graphics to be set by the thing calling it, or be automatically set
            }
        }
    }
    public class PNG
    {
        string textparse;
        string strvalue;

        public List<PNGMove> data = new List<PNGMove>();

        public struct PNGMove
        {
            Move normalmove;
            Move duckmove;
            string comment;

            public PNGMove(Move normalmove, Move duckmove, string comment)
            {
                this.normalmove = normalmove;
                this.duckmove = duckmove;
                this.comment = comment;
            }
            public PNGMove(Move normalmove, string comment)
            {
                this.normalmove = normalmove;
                this.duckmove = new Move(100, 100, PieceType.None);
                this.comment = comment;
            }

            public override string ToString()
            {
                var startpos = (normalmove.last % 8).GetFileLetter() + ((normalmove.last / 8) + 1).ToString();
                var endpos   = (normalmove.current % 8).GetFileLetter() + ((normalmove.current / 8) + 1).ToString();
                if (duckmove.current == 100)
                {
                    return startpos + endpos + " " + comment;
                }
                var duckpos = (duckmove.current % 8).GetFileLetter() + ((duckmove.current / 8) + 1).ToString();
                return startpos + endpos + "-@" + duckpos + " " + comment;
            }
        }

        public PNG(string textparse)
        {
            this.textparse = textparse;
            this.textparse = Regex.Replace(textparse, "\n", "");
            ParseNormal();
        }
        public void ParseDuck()
        {
            Chessboard startpos = new Chessboard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            string FEN = textparse.Substring(textparse.IndexOf("1"));
            var lastnumber = int.Parse(FEN[FEN.LastIndexOf('.') - 1].ToString()); //Find the last number
            for (int i = 1; i < lastnumber + 1; ++i)
            {
                int nextidx = FEN.IndexOf((i + 1).ToString() + ".");
                if (nextidx == -1)
                {
                    nextidx = FEN.Length;
                }
                int curridx = FEN.IndexOf(i.ToString() + ".");
                string FEN_data = FEN.Substring(curridx, nextidx - curridx);

                bool secondmove;
                bool readingcomment = false;
                string comment = "";

                string normalData = "";
                bool readingduck = false;
                char duckFile = '\0';

                int duckrow = 0;
                Side currmove = Side.White;
                for (int idx = 0; idx < FEN_data.Length; ++idx)
                {
                    if (FEN_data[idx] == '{')
                    {
                        readingcomment = true;
                        continue;
                    }
                    else if (FEN_data[idx] == '}')
                    {
                        readingcomment = false;
                        continue;
                    }
                    else if (readingcomment)
                    {
                        comment += FEN_data[idx];
                        continue;
                    }
                    else
                    {
                        if (FEN_data[idx] == 'D' || FEN_data[idx] == 'Θ') //Duck?
                        {
                            readingduck = true;
                        }

                        if (!readingduck && (idx >= FEN_data.Length - 2 || !(FEN_data[idx + 1] == '.')) && FEN_data[idx] != '.' && FEN_data[idx] != ' ') //Just a normal move?
                        {
                            normalData += FEN_data[idx];
                        }
                        else if (readingduck && char.IsLetter(FEN_data[idx])) //Is this a file?
                        {
                            if (FEN_data[idx] == 'D' || FEN_data[idx] == 'Θ') //Duck?
                            {
                                continue; //Dont include the identifier letter
                            }
                            if (duckFile == '\0')
                            {
                                duckFile = FEN_data[idx];
                            }
                        }
                        else if (readingduck && char.IsNumber(FEN_data[idx])) //Reading a row
                        {
                            if (duckrow == '\0')
                            {
                                duckrow = int.Parse(FEN_data[idx].ToString());
                            }
                        } //Duck moves are much simpler as you know exactly where it was and where it will be

                        if (char.IsNumber(FEN_data[idx]) && FEN_data[idx + 1] == ' ') //End of move?
                        {
                            char piecetype_text = normalData[0];
                            PieceType pieceType = PieceType.None;
                            if (char.IsLower(piecetype_text)) //Pawn?
                            {
                                pieceType = PieceType.Pawn;
                            }
                            else
                            {
                                switch (piecetype_text)
                                {
                                    case 'K':
                                        pieceType = PieceType.King;
                                        break;
                                    case 'Q':
                                        pieceType = PieceType.Queen;
                                        break;
                                    case 'B':
                                        pieceType = PieceType.Bishop;
                                        break;
                                    case 'N':
                                        pieceType = PieceType.Knight;
                                        break;
                                    case 'R':
                                        pieceType = PieceType.Rook;
                                        break;
                                }
                                normalData = normalData.Substring(1); //Remove the leading letter
                            }
                            //Piecetype has been assigned, lets see what we can find out about the original location
                            int startrow = -1;
                            int startcol = -1;
                            if (normalData.Length == 4) //FileRowFileRow
                            {
                                startcol = normalData[0].GetFileNum();
                                startrow = int.Parse(normalData[1].ToString()) - 1;
                                normalData = normalData.Substring(2);
                            }
                            else if (normalData.Length == 3)
                            {
                                if (char.IsLetter(normalData[0])) //Reading file
                                {
                                    startcol = normalData[0].GetFileNum();
                                }
                                else if (char.IsNumber(normalData[0]))
                                {
                                    startrow = int.Parse(normalData[0].ToString()) - 1;
                                }
                            }
                            //We collected the data we know, now try to find the start position
                            var endposition = normalData[0].GetFileNum() + (int.Parse(normalData[1].ToString()) - 1) * 8;
                            List<Move> possibleMoves = new List<Move>();
                            foreach (var _move in MoveGenerator.CalculateAll(startpos.bitboard, currmove))
                            {
                                if (_move.pieceType == pieceType && _move.current == endposition) //Is this the right move?
                                {
                                    possibleMoves.Add(_move);
                                }
                            }
                            Move normalMove = new Move();
                            List<Move> samecol = possibleMoves.Where(m => m.last % 8 == startcol).ToList();
                            List<Move> samerow = possibleMoves.Where(m => m.last / 8 == startrow).ToList();
                            List<Move> same = possibleMoves.Where(m => m.last / 8 == startrow && m.last % 8 == startcol).ToList();
                            if (possibleMoves.Count == 1)
                            {
                                normalMove = possibleMoves[0];
                            }
                            else if (samecol.Count() == 1)
                            {
                                normalMove = samecol[0];
                            }
                            else if (same.Count() == 1)
                            {
                                normalMove = same[0];
                            }
                            int duck_file = duckFile.GetFileNum();
                            int duck_row = int.Parse(duckrow.ToString()) - 1;
                            var lastpos = BitOperations.TrailingZeros(startpos.bitboard.Duck) - 1;
                            Move duckmove = new Move((byte)lastpos, (byte)(duck_file + duck_row * 8), PieceType.Duck);

                            PNGMove move = new PNGMove(normalMove, duckmove, comment);
                            data.Add(move);
                            currmove = currmove == Side.White ? Side.Black : Side.White;
                            if (idx + 2 >= FEN_data.Length || char.IsLetter(FEN_data[idx + 2])) //Is there another move?
                            {
                                readingduck = false;
                                comment = "";
                                secondmove = true;
                                normalData = "";
                                duckFile = '\0';

                                duckrow = 0;

                                startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove);
                                startpos.bitboard.Move(duckmove.last, duckmove.current, 1ul << duckmove.last, 1ul << duckmove.current, duckmove.pieceType, currmove);

                                startpos.Reload();
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
        public void ParseNormal()
        {
            Chessboard startpos = new Chessboard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            string FEN = textparse.Substring(textparse.IndexOf("1"));
            Move normalMove = new Move();

            var lastnumber = int.Parse(FEN[FEN.LastIndexOf('.') - 1].ToString()); //Find the last number
            for (int i = 1; i < lastnumber + 1; ++i)
            {
                int nextidx = FEN.IndexOf((i + 1).ToString() + ".");
                if (nextidx == -1)
                {
                    nextidx = FEN.Length;
                }
                int curridx = FEN.IndexOf(i.ToString() + ".");
                string FEN_data = FEN.Substring(curridx, nextidx - curridx);

                bool secondmove = false;
                bool readingcomment = false;
                string comment = "";

                string normalData = "";
                Side currmove = Side.White;
                for (int idx = 0; idx < FEN_data.Length; ++idx)
                {
                    if (FEN_data[idx] == '{')
                    {
                        readingcomment = true;
                        continue;
                    }
                    else if (FEN_data[idx] == '}')
                    {
                        readingcomment = false;
                        PNGMove m = new PNGMove(normalMove, comment);
                        data.Add(m);
                        currmove = currmove == Side.White ? Side.Black : Side.White;

                        comment = "";
                        normalData = "";

                        startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove == Side.White ? Side.Black : Side.White);

                        startpos.Reload();

                        if (!secondmove && ((idx+2 < FEN_data.Length)&&((FEN_data[idx+2] == 'T' || FEN_data[idx + 2] == '1' || FEN_data[idx + 2] == '0' || FEN_data[idx + 2] == 'R'))))
                        {
                            return; ///Game over
                        }
                        secondmove = true;
                        continue;
                    }
                    else if (readingcomment)
                    {
                        comment += FEN_data[idx];
                        continue;
                    }
                    else
                    {
                        if ((idx >= FEN_data.Length - 2 || !(FEN_data[idx + 1] == '.')) && FEN_data[idx] != '.' && FEN_data[idx] != ' ') //Just a normal move?
                        {
                            normalData += FEN_data[idx];
                        }

                        if (char.IsNumber(FEN_data[idx]) && FEN_data[idx + 1] == ' ') //End of move?
                        {
                            char piecetype_text = normalData[0];
                            PieceType pieceType = PieceType.None;
                            if (char.IsLower(piecetype_text)) //Pawn?
                            {
                                pieceType = PieceType.Pawn;
                            }
                            else
                            {
                                switch (piecetype_text)
                                {
                                    case 'K':
                                        pieceType = PieceType.King;
                                        break;
                                    case 'Q':
                                        pieceType = PieceType.Queen;
                                        break;
                                    case 'B':
                                        pieceType = PieceType.Bishop;
                                        break;
                                    case 'N':
                                        pieceType = PieceType.Knight;
                                        break;
                                    case 'R':
                                        pieceType = PieceType.Rook;
                                        break;
                                }
                                normalData = normalData.Substring(1); //Remove the leading letter
                            }
                            if (pieceType == PieceType.Pawn && normalData[1] == 'x') //Are we taking?
                            {
                                normalData = normalData.Substring(2); //Taking is autosimulated, just ignore it
                            }
                            else if(normalData[0] == 'x') //Just something normal taking?
                            {
                                normalData = normalData.Substring(1); //Again, taking is done automatically, ignore the X
                            }
                            //Piecetype has been assigned, lets see what we can find out about the original location
                            int startrow = -1;
                            int startcol = -1;
                            if (normalData.Length == 4) //FileRowFileRow
                            {
                                startcol = normalData[0].GetFileNum();
                                startrow = int.Parse(normalData[1].ToString()) - 1;
                                normalData = normalData.Substring(2);
                            }
                            else if (normalData.Length == 3)
                            {
                                if (char.IsLetter(normalData[0])) //Reading file
                                {
                                    startcol = normalData[0].GetFileNum();
                                }
                                else if (char.IsNumber(normalData[0]))
                                {
                                    startrow = int.Parse(normalData[0].ToString()) - 1;
                                }
                            }
                            //We collected the data we know, now try to find the start position
                            var endposition = normalData[0].GetFileNum() + (int.Parse(normalData[1].ToString()) - 1) * 8;
                            List<Move> possibleMoves = new List<Move>();
                            foreach (var _move in MoveGenerator.CalculateAll(startpos.bitboard, currmove))
                            {
                                if (_move.pieceType == pieceType && _move.current == endposition) //Is this the right move?
                                {
                                    possibleMoves.Add(_move);
                                }
                            }
                            normalMove = new Move();
                            List<Move> samecol = possibleMoves.Where(m => m.last % 8 == startcol).ToList();
                            List<Move> samerow = possibleMoves.Where(m => m.last / 8 == startrow).ToList();
                            List<Move> same = possibleMoves.Where(m => m.last / 8 == startrow && m.last % 8 == startcol).ToList();
                            if (possibleMoves.Count == 1)
                            {
                                normalMove = possibleMoves[0];
                            }
                            else if (samecol.Count() == 1)
                            {
                                normalMove = samecol[0];
                            }
                            else if (same.Count() == 1)
                            {
                                normalMove = same[0];
                            }
                            else
                            {

                            }
                            if ((idx+2 <= FEN_data.Length && FEN_data[idx+2] == '{')) //Have we missed a comment?
                            {
                                continue;
                            }
                            PNGMove move = new PNGMove(normalMove, comment);
                            data.Add(move);
                            bool contains = FEN_data.Substring(idx).Contains('{');
                            currmove = currmove == Side.White ? Side.Black : Side.White;
                            if (idx + 2 >= FEN_data.Length || char.IsLetter(FEN_data[idx + 2])) //Is there another move?
                            {
                                comment = "";
                                secondmove = true;
                                normalData = "";

                                startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove == Side.White ? Side.Black : Side.White);

                                startpos.Reload();
                            }
                            else
                            {
                                if ((FEN_data[idx + 2] == '{'))
                                {
                                    continue;
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}