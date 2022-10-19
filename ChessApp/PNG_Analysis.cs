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
        public Bitboard finalresult;

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
            ParseDuck();
        }
        public bool failed;
        public void ParseNormal()
        {
            Chessboard startpos = new Chessboard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            textparse = textparse.RemoveChars('+');
            textparse = textparse.RemoveChars('#');
            textparse = textparse.RemoveChars('x');

            textparse = textparse.RemoveCastles();

            List<PieceType> promotions = new List<PieceType>();

            textparse = textparse.RemovePromotions(out promotions);

            string FEN = textparse.Substring(textparse.IndexOf("1"));
            Move normalMove = new Move();

            int v = FEN.LastIndexOf('.');
            string num = FEN[v - 1].ToString();
            if (v >= 2 && char.IsNumber(FEN[v - 2]))
            {
                num = num.Insert(0, FEN[v - 2].ToString());
                if (v >= 3 && char.IsNumber(FEN[v - 3]))
                {
                    num = num.Insert(0, FEN[v - 3].ToString());
                }
            }
            var lastnumber = int.Parse(num); //Find the last number
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
                        PieceType promotion = PieceType.Queen;
                        if (normalMove.pieceType == PieceType.Pawn && normalMove.current / 8 == 0 || normalMove.current / 8 == 7)
                        {
                            //We are promoting
                            promotion = promotions.First();
                            promotions.RemoveAt(0);
                        }
                        startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove == Side.White ? Side.Black : Side.White, promotion);

                        startpos.Reload();

                        if (!secondmove && ((idx + 2 < FEN_data.Length) && ((FEN_data[idx + 2] == 'T' || FEN_data[idx + 2] == '1' || FEN_data[idx + 2] == '0' || FEN_data[idx + 2] == 'R'))))
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

                        if (char.IsNumber(FEN_data[idx]) && (idx >= FEN_data.Length - 2 || FEN_data[idx + 1] == ' ')) //End of move?
                        {
                            while (char.IsNumber(normalData[0]))
                            {
                                normalData = normalData.Substring(1);
                            }

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
                                    normalData = normalData.Substring(1);
                                }
                                else if (char.IsNumber(normalData[0]))
                                {
                                    startrow = int.Parse(normalData[0].ToString()) - 1;
                                    normalData = normalData.Substring(1);
                                }
                            }
                            //We collected the data we know, now try to find the start position
                            var endposition = normalData[0].GetFileNum() + (int.Parse(normalData[1].ToString()) - 1) * 8;
                            List<Move> possibleMoves = new List<Move>();

                            var copy = startpos.bitboard.Copy();
                            copy.SetupSquareAttacks();
                            foreach (var _move in MoveGenerator.CalculateAll(copy, currmove))
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
                            else if (samerow.Count() == 1)
                            {
                                normalMove = samerow[0];
                            }
                            else
                            {
                                failed = true;
                                finalresult = startpos.bitboard;
                                return;
                            }
                            if ((idx + 3 <= FEN_data.Length && FEN_data[idx + 2] == '{')) //Have we missed a comment?
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

                                PieceType promotion = PieceType.Queen;
                                if (normalMove.pieceType == PieceType.Pawn && (normalMove.current / 8 == 0 || normalMove.current / 8 == 7))
                                {
                                    //We are promoting
                                    promotion = promotions.First();
                                    promotions.RemoveAt(0);
                                }
                                startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove == Side.White ? Side.Black : Side.White, promotion);

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
        public void ParseDuck()
        {
            Chessboard startpos = new Chessboard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            textparse = textparse.RemoveChars('+');
            textparse = textparse.RemoveChars('#');
            textparse = textparse.RemoveChars('x');

            textparse = textparse.RemoveCastles();

            List<PieceType> promotions = new List<PieceType>();

            textparse = textparse.RemovePromotions(out promotions);

            string FEN = textparse.Substring(textparse.IndexOf("1"));
            Move normalMove = new Move();
            Move duckMove = new Move();

            int v = FEN.LastIndexOf('.');
            string num = FEN[v - 1].ToString();
            if (v >= 2 && char.IsNumber(FEN[v - 2]))
            {
                num = num.Insert(0, FEN[v - 2].ToString());
                if (v >= 3 && char.IsNumber(FEN[v - 3]))
                {
                    num = num.Insert(0, FEN[v - 3].ToString());
                }
            }
            var lastnumber = int.Parse(num); //Find the last number
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
                string duckData   = "";
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
                        PieceType promotion = PieceType.Queen;
                        if (normalMove.pieceType == PieceType.Pawn && normalMove.current / 8 == 0 || normalMove.current / 8 == 7)
                        {
                            //We are promoting
                            promotion = promotions.First();
                            promotions.RemoveAt(0);
                        }
                        startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove == Side.White ? Side.Black : Side.White, promotion);
                        startpos.bitboard.Move(0, duckMove.current, 0, 1ul << duckMove.current, PieceType.Duck, Side.Animal);

                        startpos.Reload();

                        if (!secondmove && ((idx + 2 < FEN_data.Length) && ((FEN_data[idx + 2] == 'T' || FEN_data[idx + 2] == '1' || FEN_data[idx + 2] == '0' || FEN_data[idx + 2] == 'R'))))
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

                        if (char.IsNumber(FEN_data[idx]) && (idx >= FEN_data.Length - 2 || FEN_data[idx + 1] == ' ')) //End of move?
                        {
                            var duckdata = normalData.Substring(normalData.Length-2);
                            normalData = normalData.Substring(0, normalData.Length-2);
                            while (char.IsNumber(normalData[0]))
                            {
                                normalData = normalData.Substring(1);
                            }

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
                                    normalData = normalData.Substring(1);
                                }
                                else if (char.IsNumber(normalData[0]))
                                {
                                    startrow = int.Parse(normalData[0].ToString()) - 1;
                                    normalData = normalData.Substring(1);
                                }
                            }
                            //We collected the data we know, now try to find the start position
                            var endposition = normalData[0].GetFileNum() + (int.Parse(normalData[1].ToString()) - 1) * 8;
                            List<Move> possibleMoves = new List<Move>();

                            var copy = startpos.bitboard.Copy();
                            copy.SetupSquareAttacks();
                            foreach (var _move in MoveGenerator.CalculateAll(copy, currmove))
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
                            else if (samerow.Count() == 1)
                            {
                                normalMove = samerow[0];
                            }
                            else
                            {
                                failed = true;
                                finalresult = startpos.bitboard;
                                return;
                            }
                            var duckendposition = normalData[0].GetFileNum() + (int.Parse(normalData[1].ToString()) - 1) * 8;
                            duckMove = new Move(0, (byte)duckendposition, PieceType.Duck);

                            if ((idx + 3 <= FEN_data.Length && FEN_data[idx + 2] == '{')) //Have we missed a comment?
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

                                PieceType promotion = PieceType.Queen;
                                if (normalMove.pieceType == PieceType.Pawn && (normalMove.current / 8 == 0 || normalMove.current / 8 == 7))
                                {
                                    //We are promoting
                                    promotion = promotions.First();
                                    promotions.RemoveAt(0);
                                }
                                startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove == Side.White ? Side.Black : Side.White, promotion);
                                startpos.bitboard.Move(0, duckMove.current, 0, 1ul<<duckMove.current, PieceType.Duck, Side.Animal);
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