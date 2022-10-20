using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessApp
{
    public class PGN_Analysis
    {
        public PGN gamedata;
        public Bitmap bitmap;
        public string PGN_data;

        public PGN_Analysis(string PGN, int height, int width)
        {
            bitmap = new Bitmap(width, height);
            this.PGN_data = PGN;
            gamedata = new PGN(PGN);
        }
        public void Paint()
        {
            var graphics = Graphics.FromImage(bitmap);
            graphics.DrawString(PGN_data, new Font("Arial", 10, FontStyle.Regular), new Pen(Color.Black).Brush, new Point(0,0));
        }
    }
    public class PGN
    {
        string textparse;
        string strvalue;

        public List<PGNMove> data = new List<PGNMove>();
        public Bitboard finalresult;
        public override string ToString()
        {
            Chessboard startpos = new Chessboard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            Bitboard b = startpos.bitboard;

            string pgn = "";
            for (int i = 0; i< (data.Count()/2); ++i)
            {
                pgn += string.Format("{0}. ", i+1); //Add the indexer

                for (int j = 0; j < 2; ++j) //switch the sides
                {
                    Side hasturn = j == 0 ? Side.White : Side.Black;
                    if (data.Count() == i*2+j)
                    {
                        break;
                    }
                    var move = data[i * 2 + j];
                    var copy = b.Copy();
                    copy.SetupSquareAttacks();
                    List<int> possibleStartPositions = new List<int>();
                    
                    foreach (var b_move in MoveGenerator.CalculateAll(copy, hasturn))
                    {
                        if (b_move.pieceType == move.normalmove.pieceType && b_move.last == move.normalmove.last)
                        {
                            possibleStartPositions.Add(b_move.last);
                        }
                    }
                    string movestring = "";
                    string rowcol = "";
                    if (possibleStartPositions.Distinct().Count() >= 2)
                    {
                        int startcol = move.normalmove.last % 8;
                        int startrow = move.normalmove.last / 8;

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
                    string endposition = (move.normalmove.current % 8).GetFileLetter().ToString() + ((move.normalmove.current / 8) + 1).ToString();
                    switch (move.normalmove.pieceType)
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
                    if (move.normalmove.pieceType == PieceType.Pawn && (move.normalmove.current/8==0 || move.normalmove.current/8==7))
                    {
                        string promotionletter = "";
                        switch (move.normalmove.promotion)
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
                        if ((b.BlackPieces & (1ul<<move.normalmove.current)) != 0) //Taking a piece
                        {
                            istaking = "x";
                        }
                    }
                    if (hasturn == Side.Black)
                    {
                        if ((b.WhitePieces & (1ul << move.normalmove.current)) != 0) //Taking a piece
                        {
                            istaking = "x";
                        }
                    }
                    if (istaking == "x" && move.normalmove.pieceType == PieceType.Pawn) //Pawn taking
                    {
                        //We must specify row-col
                        rowcol = (move.normalmove.last % 8).GetFileLetter().ToString();
                    }
                    movestring = string.Format("{0}{1}{4}{2}{3} ", piecetypestring, rowcol, endposition, promotionstring, istaking);
                    pgn += movestring;
                    b.Move(move.normalmove.last, move.normalmove.current, 1ul << move.normalmove.last, 1ul << move.normalmove.current, move.normalmove.pieceType, hasturn, move.normalmove.promotion);
                }
            }
            return pgn + gameresult;
        }
        public struct PGNMove
        {
            public Move normalmove;
            Move duckmove;
            string comment;

            public PGNMove(Move normalmove, Move duckmove, string comment)
            {
                this.normalmove = normalmove;
                this.duckmove = duckmove;
                this.comment = comment;
            }
            public PGNMove(Move normalmove, string comment)
            {
                this.normalmove = normalmove;
                this.duckmove = new Move(100, 100, PieceType.None);
                this.comment = comment;
            }

            public override string ToString()
            {
                var startpos = (normalmove.last % 8).GetFileLetter() + ((normalmove.last / 8) + 1).ToString();
                var endpos = (normalmove.current % 8).GetFileLetter() + ((normalmove.current / 8) + 1).ToString();
                if (duckmove.current == 100)
                {
                    return startpos + endpos + " " + comment;
                }
                var duckpos = (duckmove.current % 8).GetFileLetter() + ((duckmove.current / 8) + 1).ToString();
                return startpos + endpos + "-@" + duckpos + " " + comment;
            }
        }

        public PGN(string textparse)
        {
            textparse = Regex.Replace(textparse, "\n", " ");
            textparse = Regex.Replace(textparse, "\r", "");
            this.textparse = textparse;
            if (textparse.Contains('D')) //My duck piece identifier
            {
                ParseNormalDuck();
            }
            else if (textparse.Contains(" .. ")) //Weird PGN4 notation
            {
                this.textparse = textparse.Shift3();
                ParseDuck();
            }
            else //Just a normal game?
            {
                ParseNormal();
            }
        }
        public bool failed;
        string gameresult = "";
        public void ParseNormal()
        {
            Chessboard startpos = new Chessboard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            textparse = textparse.RemoveChars('+');
            textparse = textparse.RemoveChars('#');
            textparse = textparse.RemoveChars('x');
            textparse = Regex.Replace(textparse, "\r", " ");
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
                int nextidx = FEN.IndexOf((i+1).ToString() + ".");
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
                        PGNMove m = new PGNMove(normalMove, comment);
                        currmove = currmove == Side.White ? Side.Black : Side.White;

                        comment = "";
                        normalData = "";
                        PieceType promotion = PieceType.Queen;
                        if (normalMove.pieceType == PieceType.Pawn && normalMove.current / 8 == 0 || normalMove.current / 8 == 7)
                        {
                            //We are promoting
                            promotion = promotions.First();
                            promotions.RemoveAt(0);
                            m.normalmove.promotion = promotion;
                        }
                        startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove == Side.White ? Side.Black : Side.White, promotion);

                        startpos.Reload();
                        data.Add(m);

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
                            if (normalMove.current == 30)
                            {

                            }
                            if ((idx + 3 <= FEN_data.Length && FEN_data[idx + 2] == '{')) //Have we missed a comment?
                            {
                                continue;
                            }
                            PGNMove move = new PGNMove(normalMove, comment);
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
                                    move.normalmove.promotion = promotion;
                                }
                                data.Add(move);
                                startpos.bitboard.Move(normalMove.last, normalMove.current, 1ul << normalMove.last, 1ul << normalMove.current, normalMove.pieceType, currmove == Side.White ? Side.Black : Side.White, promotion);

                                startpos.Reload();
                            }
                            else
                            {
                                data.Add(move);
                                if ((FEN_data[idx + 2] == '{'))
                                {
                                    continue;
                                }
                                else
                                {
                                    gameresult = FEN_data.Substring(idx+2);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void ParseNormalDuck()
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
                string duckData = "";
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
                        PGNMove m = new PGNMove(normalMove, comment);
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
                            var duckdata = normalData.Substring(normalData.Length - 2);
                            normalData = normalData.Substring(0, normalData.Length - 2);
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
                            PGNMove move = new PGNMove(normalMove, comment);
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
                                startpos.bitboard.Move(0, duckMove.current, 0, 1ul << duckMove.current, PieceType.Duck, Side.Animal);
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

            textparse = textparse.Replace(".. O-O-O", ".. Ke8-c8");
            textparse = textparse.Replace(".. O-O", ".. Ke8-g8");
            textparse = textparse.Replace(". O-O-O", ". Ke1-c1");
            textparse = textparse.Replace(". O-O", ". Ke1-g1"); //Removing castling

            List<PieceType> promotions = new List<PieceType>();

            textparse = textparse.RemovePromotions(out promotions);

            string FEN = textparse.Substring(textparse.IndexOf("1"));

            int lastnumber = FEN.LastIndexor();

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

                string normalData = "";
                string duckData = "";
                Side currmove = Side.White;

                var datas = FEN_data.Split(new string[] { " .. " }, StringSplitOptions.None);

                //Whites move
                var data = datas[0];
                if (data == "R" || data == "T" || data == "")
                {
                    break;
                }
                data = data.Substring(data.IndexOf('.')+2);
                char piecedata = data[0];
                PieceType pieceType = PieceType.Pawn;
                if (char.IsUpper(piecedata))
                {
                    switch (piecedata)
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
                    data = data.Substring(1); //Remove the piecetype from the string
                }
                string normalmovedata = data.Substring(0,5);
                string[] movedata = normalmovedata.Split('-');
                int startposition = movedata[0][0].GetFileNum() + (int.Parse(movedata[0][1].ToString())*8);
                int endposition = movedata[1][0].GetFileNum() + (int.Parse(movedata[1][1].ToString()) * 8);

                string duckdata = data.Substring(data.Length-2);
                int duckposition = duckdata[0].GetFileNum() + (int.Parse(duckdata[1].ToString()) * 8);

                PieceType promotion = PieceType.None;
                if (pieceType == PieceType.Pawn && (endposition/8 == 0||endposition/8==7)) //Promoting?
                {
                    promotion = promotions.First();
                    promotions.RemoveAt(0);
                }

                startpos.bitboard.Move((byte)startposition, (byte)endposition, 1ul << startposition, 1ul << endposition, pieceType, Side.White, promotion);
                startpos.bitboard.Move(0, (byte)duckposition, 0, 1ul << duckposition, PieceType.Duck, Side.Animal);
                this.data.Add(new PGNMove(new Move((byte)startposition, (byte)endposition, pieceType, promotion), new Move(0, (byte)duckposition, PieceType.Duck), ""));
                startpos.Reload();





                //Blacks move now
                data = datas[1];
                if (data == "R" || data == "T" || data == "")
                {
                    break;
                }
                piecedata = data[0];
                pieceType = PieceType.Pawn;
                if (char.IsUpper(piecedata))
                {
                    switch (piecedata)
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
                    data = data.Substring(1); //Remove the piecetype from the string
                }
                normalmovedata = data.Substring(0, 5);
                movedata = normalmovedata.Split('-');
                startposition = movedata[0][0].GetFileNum() + (int.Parse(movedata[0][1].ToString()) * 8);
                endposition = movedata[1][0].GetFileNum() + (int.Parse(movedata[1][1].ToString()) * 8);

                duckdata = data.Substring(data.Length - 3, 2);
                duckposition = duckdata[0].GetFileNum() + (int.Parse(duckdata[1].ToString()) * 8);

                promotion = PieceType.None;
                if (pieceType == PieceType.Pawn && (endposition / 8 == 0 || endposition / 8 == 7)) //Promoting?
                {
                    promotion = promotions.First();
                    promotions.RemoveAt(0);
                }

                startpos.bitboard.Move((byte)startposition, (byte)endposition, 1ul << startposition, 1ul << endposition, pieceType, Side.Black, promotion);
                startpos.bitboard.Move(0, (byte)duckposition, 0, 1ul << duckposition, PieceType.Duck, Side.Animal);
                this.data.Add(new PGNMove(new Move((byte)startposition, (byte)endposition, pieceType, promotion), new Move(0, (byte)duckposition, PieceType.Duck), ""));
                startpos.Reload();
            }

            foreach (var move in data)
            {
                Console.WriteLine(move.ToString());
            }
        }
    }
}
