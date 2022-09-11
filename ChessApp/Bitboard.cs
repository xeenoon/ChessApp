using System.Collections.Generic;
using System.Diagnostics;

namespace ChessApp
{
    public struct Bitboard
    {
        public ulong W_Pawn;   //White Pawns
        public ulong W_Rook;   //White Rooks
        public ulong W_Knight; //White Knights
        public ulong W_Bishop; //White Bishops
        public ulong W_Queen;  //White Queens
        public ulong W_King;   //White King

        public ulong B_Pawn;   //Black Pawns
        public ulong B_Rook;   //Black Rooks
        public ulong B_Knight; //Black Knights
        public ulong B_Bishop; //Black Bishops
        public ulong B_Queen;  //Black Queens
        public ulong B_King;   //Black King

        public ulong xrays;
        //Legal moves allowed because of pins for every place on the board
        //Default is everything

        public bool check;
        public bool doublecheck;
        public ulong squares_to_block_check; //Squares that pieces can move to to block checks
        //If king is in check, pieces should only be able to move to squares that block the attack

        public int enpassent;

        public ulong BlackPieces
        {
            get
            {
                return B_Pawn | B_Rook | B_Knight | B_Bishop | B_King | B_Queen;
            }
        }
        public ulong WhitePieces
        {
            get
            {
                return W_Pawn | W_Rook | W_Knight | W_Bishop | W_King | W_Queen;
            }
        }
        public ulong WhiteAttackedSquares;
        public ulong BlackAttackedSquares;
        
        public static double StaticAttack = 0;
        public static double SlidingAttack = 0;
        public static double Pins = 0;

        public bool B_KingsideCastle;
        public bool B_QueensideCastle;
        public bool W_KingsideCastle;
        public bool W_QueensideCastle;

        public static ulong total_checks;
        public static ulong total_doublechecks;
        public void SetupSquareAttacks()
        {
            squares_to_block_check = ulong.MaxValue;
            WhiteAttackedSquares = WhiteAttacks();
            BlackAttackedSquares = BlackAttacks();
            SetupPins();

            if (check)
            {
                ++total_checks;
            }
            if (doublecheck)
            {
                ++total_doublechecks;
            }
        }
        public void SetupPins()
        {
            SET_XRAY_Pins(W_Rook, PieceType.Rook, Side.White, B_King);
            SET_XRAY_Pins(W_Bishop, PieceType.Bishop, Side.White, B_King);
            SET_XRAY_Pins(W_Queen, PieceType.Queen, Side.White, B_King);
            
            SET_XRAY_Pins(B_Rook, PieceType.Rook, Side.Black, W_King);
            SET_XRAY_Pins(B_Bishop, PieceType.Bishop, Side.Black, W_King);
            SET_XRAY_Pins(B_Queen, PieceType.Queen, Side.Black, W_King);
        }

        int checks;
        ulong WhiteAttacks()
        {
            checks = 0;

            ulong attacks = 0ul;
            attacks |= StaticPieceAttacks(W_Pawn, PieceType.Pawn     , Side.White, B_King);
            attacks |= SlidingPieceAttacks(attacks, W_Rook, PieceType.Rook     , Side.White, B_King);
            attacks |= SlidingPieceAttacks(attacks, W_Bishop, PieceType.Bishop , Side.White, B_King);
            attacks |= StaticPieceAttacks(W_King, PieceType.King     , Side.White, B_King);
            attacks |= SlidingPieceAttacks(attacks, W_Queen, PieceType.Queen   , Side.White, B_King);
            attacks |= StaticPieceAttacks(W_Knight, PieceType.Knight , Side.White, B_King);
            if (checks == 1)
            {
                check = true;
            }
            if (checks >= 2)
            {
                doublecheck = true;
            }
            return attacks;
        }
        ulong BlackAttacks()
        {
            checks = 0;

            ulong attacks = 0ul;
            attacks |= StaticPieceAttacks(B_Pawn, PieceType.Pawn     , Side.Black, W_King);
            attacks |= SlidingPieceAttacks(attacks, B_Rook, PieceType.Rook     , Side.Black, W_King);
            attacks |= SlidingPieceAttacks(attacks, B_Bishop, PieceType.Bishop , Side.Black, W_King);
            attacks |= StaticPieceAttacks(B_King, PieceType.King     , Side.Black, W_King);
            attacks |= SlidingPieceAttacks(attacks, B_Queen, PieceType.Queen   , Side.Black, W_King);
            attacks |= StaticPieceAttacks(B_Knight, PieceType.Knight , Side.Black, W_King);
            if (checks == 1)
            {
                check = true;
            }
            if (checks >= 2)
            {
                doublecheck = true;
            }
            return attacks;
        }

        private ulong SlidingPieceAttacks(ulong attacks, ulong piece_bitboard, PieceType pieceType, Side s, ulong oppositeKing)
        {
            while (piece_bitboard != 0)
            {
                byte lsb = (byte)(BitOperations.TrailingZeros(piece_bitboard)-1);
                ulong bitpos = 1ul << lsb;
                piece_bitboard ^= bitpos; //remove this pawn from the ulong of pieces

                ulong[] moves = MoveGenerator.SlidingAttackRays(pieceType, s, lsb, this);
                foreach (var attackray in moves)
                {
                    if ((attackray & oppositeKing) != 0) //King is in check
                    {
                        ++checks;
                        squares_to_block_check = (attackray ^ oppositeKing) | bitpos; //Find all places a piece could move to block
                    }
                    attacks |= attackray; //Get all the attacking moves and add them to the attacks bitboard
                }
            }
            return attacks;
        }
        private ulong StaticPieceAttacks(ulong piece_bitboard, PieceType pieceType, Side s, ulong oppositeKing)
        {
            if (piece_bitboard == 0)
            {
                return 0ul;
            }
            ulong result = 0ul;
            byte lsb;
            switch (pieceType)
            {
                case PieceType.Pawn:
                    var pawnattacks = MoveGenerator.PawnAttackRays(s, piece_bitboard);
                    if ((pawnattacks & oppositeKing) != 0) //Is in check
                    {
                        ++checks;
                        if (s == Side.White) //<<7 <<9 to attack
                        {
                            if (((oppositeKing >> 9) & piece_bitboard) != 0)
                            {
                                //Is a pawn attacking up and left?
                                squares_to_block_check = oppositeKing >> 9; //We can only take this pawn
                            }
                            if (((oppositeKing >> 7) & piece_bitboard) != 0)
                            {
                                //Is a pawn attacking up and right?
                                squares_to_block_check = oppositeKing >> 7; //We can only take this pawn
                            }
                        }
                        else //>>7 and  >>9 to attack
                        {
                            if (((oppositeKing << 9) & piece_bitboard) != 0)
                            {
                                //Is a pawn attacking up and left?
                                squares_to_block_check = oppositeKing << 9; //We can only take this pawn
                            }
                            if (((oppositeKing << 7) & piece_bitboard) != 0)
                            {
                                //Is a pawn attacking up and right?
                                squares_to_block_check = oppositeKing << 7; //We can only take this pawn
                            }
                        }
                    }
                    result |= pawnattacks;
                    break;
                case PieceType.King:
                    lsb = (byte)(BitOperations.TrailingZeros(piece_bitboard) - 1);
                    result |= MoveGenerator.king[lsb];
                    break;
                case PieceType.Knight:
                    lsb = (byte)(BitOperations.TrailingZeros(piece_bitboard) - 1);
                    var knightattacks = MoveGenerator.KnightAttackRays(s, piece_bitboard);
                    if ((knightattacks & oppositeKing) != 0) //Is in check
                    {
                        ++checks;
                        squares_to_block_check = piece_bitboard & MoveGenerator.knight[(BitOperations.TrailingZeros(oppositeKing) - 1)];
                    }

                    result |= knightattacks;
                    break;
            }
            return result;
        }

        private void SET_XRAY_Pins(ulong piece_bitboard, PieceType pieceType, Side s, ulong oppositeKing)
        {
            var oppositeside = s == Side.White ? BlackPieces : WhitePieces;

            while (piece_bitboard != 0)
            {
                byte lsb = (byte)(BitOperations.TrailingZeros(piece_bitboard) - 1);
                ulong bitpos = 1ul << lsb;
                piece_bitboard ^= bitpos; //remove this pawn from the ulong of pieces

                ulong[] moves = MoveGenerator.XRAY(lsb, this, s, pieceType);
                foreach (var attackray in moves)
                {
                    if ((attackray & oppositeKing) != 0) //King is in check
                    {
                        var pinned = attackray & oppositeside ^ oppositeKing; //Find all the pinned pieces

                        if ((pinned & (pinned-1)) == 0 && pinned != 0) //Only one bit set
                        {
                            xrays |= attackray | bitpos; //Add this piece to the pinned list, it will not be able to move next turn
                        }
                    }
                }
            }
        }

        public static Bitboard FromFEN(string FEN)
        {
            Chessboard board = new Chessboard(FEN);
            return board.bitboard.Copy();
        }
        public Bitboard Copy()
        {
            return new Bitboard()
            {
                W_Pawn = this.W_Pawn,
                W_Bishop = this.W_Bishop,
                W_King = this.W_King,
                W_Knight = this.W_Knight,
                W_Queen = this.W_Queen,
                W_Rook = this.W_Rook,

                B_Pawn = this.B_Pawn,
                B_Bishop = this.B_Bishop,
                B_King = this.B_King,
                B_Knight = this.B_Knight,
                B_Queen = this.B_Queen,
                B_Rook = this.B_Rook,

                B_KingsideCastle = this.B_KingsideCastle,
                W_KingsideCastle = this.W_KingsideCastle,
                B_QueensideCastle = this.B_QueensideCastle,
                W_QueensideCastle = this.W_QueensideCastle,

                enpassent = this.enpassent,
                xrays = 0,
            };
        }
        public static Bitboard FromBoard(Chessboard board)
        {
            var bitboard = new Bitboard();

            foreach (var piece in board.Pieces)
            {
                if (piece.side == Side.White)
                {
                    switch (piece.pieceType)
                    {
                        case PieceType.Pawn:
                            bitboard.W_Pawn |= 1ul << piece.position;
                            break;
                        case PieceType.Rook:
                            bitboard.W_Rook |= 1ul << piece.position;
                            break;
                        case PieceType.Knight:
                            bitboard.W_Knight |= 1ul << piece.position;
                            break;
                        case PieceType.Bishop:
                            bitboard.W_Bishop |= 1ul << piece.position;
                            break;
                        case PieceType.Queen:
                            bitboard.W_Queen |= 1ul << piece.position;
                            break;
                        case PieceType.King:
                            bitboard.W_King |= 1ul << piece.position;
                            break;
                    }
                }
                else
                {
                    switch (piece.pieceType)
                    {
                        case PieceType.Pawn:
                            bitboard.B_Pawn |= 1ul << piece.position;
                            break;
                        case PieceType.Rook:
                            bitboard.B_Rook |= 1ul << piece.position;
                            break;
                        case PieceType.Knight:
                            bitboard.B_Knight |= 1ul << piece.position;
                            break;
                        case PieceType.Bishop:
                            bitboard.B_Bishop |= 1ul << piece.position;
                            break;
                        case PieceType.Queen:
                            bitboard.B_Queen |= 1ul << piece.position;
                            break;
                        case PieceType.King:
                            bitboard.B_King |= 1ul << piece.position;
                            break;
                    }
                }
            }
            bitboard.B_KingsideCastle = board.blackCastles.Kingside;
            bitboard.B_QueensideCastle = board.blackCastles.Queenside;

            bitboard.W_KingsideCastle  = board.whiteCastles.Kingside;
            bitboard.W_QueensideCastle = board.whiteCastles.Queenside;
            bitboard.xrays = 0;
            bitboard.squares_to_block_check = ulong.MaxValue;
            bitboard.enpassent = -2;
            return bitboard;
        }

        public static ulong captures;
        public static ulong enpassantes;
        public static ulong castles;
        public static ulong promotions;

        public static double MoveTime;
        public Bitboard Move(byte startlocation, byte endlocation, ulong startpos, ulong endpos, PieceType pieceType, Side side)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Bitboard copy = Copy();
            //stopwatch.Stop();
            //MoveTime += stopwatch.ElapsedTicks;
            //return copy;
            var othersidepieces = side == Side.White ? BlackPieces : WhitePieces;
            
            bool istaking = (endpos & othersidepieces) != 0;
            if (istaking) //Are we taking a piece?
            {
                ++captures;
                if (side == Side.White) //Taking piece will be black
                {
                    if ((B_Pawn & endpos) != 0) //Taking a pawn?
                    {
                        copy.B_Pawn ^= endpos;
                    }
                    else if ((B_Rook & endpos) != 0) //Taking a Rook?
                    {
                        copy.B_Rook ^= endpos;
                    }
                    else if ((B_Knight & endpos) != 0) //Taking a knight?
                    {
                        copy.B_Knight ^= endpos;
                    }
                    else if ((B_Bishop & endpos) != 0) //Taking a Bishop?
                    {
                        copy.B_Bishop ^= endpos;
                    }
                    else if ((B_Queen & endpos) != 0) //Taking a Queen?
                    {
                        copy.B_Queen ^= endpos;
                    }
                }
                else
                {
                    if ((W_Pawn & endpos) != 0) //Taking a pawn?
                    {
                        copy.W_Pawn ^= endpos;
                    }
                    else if ((W_Rook & endpos) != 0) //Taking a Rook?
                    {
                        copy.W_Rook ^= endpos;
                    }
                    else if ((W_Knight & endpos) != 0) //Taking a knight?
                    {
                        copy.W_Knight ^= endpos;
                    }
                    else if ((W_Bishop & endpos) != 0) //Taking a Bishop?
                    {
                        copy.W_Bishop ^= endpos;
                    }
                    else if ((W_Queen & endpos) != 0) //Taking a Queen?
                    {
                        copy.W_Queen ^= endpos;
                    }
                }
            }

            if (pieceType == PieceType.Pawn && !istaking) //En passante?
            {
                if (side == Side.White && startpos << 8 != endpos && startlocation / 8 == 4 && enpassent == endlocation % 8)
                {
                    copy.B_Pawn ^= endpos >> 8;
                    ++enpassantes;
                }
                else if (startpos >> 8 != endpos && startlocation / 8 == 3 && enpassent == endlocation % 8)
                {
                    copy.W_Pawn ^= endpos << 8;
                    ++enpassantes;
                }
            }
            if (pieceType == PieceType.King && (endlocation - startlocation) == 2) //Kingside castle?
            {
                ++castles;
                if (side == Side.White)
                {
                    copy.W_Rook ^= (1ul << 7);
                    copy.W_Rook ^= (1ul << 5);
                }
                else
                {
                    copy.B_Rook ^= (1ul << 63);
                    copy.B_Rook ^= (1ul << 61);
                }
            }
            if (pieceType == PieceType.King && (endlocation - startlocation) == -2) //Queenside castle?
            {
                ++castles;
                if (side == Side.White)
                {
                    copy.W_Rook ^= (1ul << 0);
                    copy.W_Rook ^= (1ul << 3);
                }
                else
                {
                    copy.B_Rook ^= (1ul << 56);
                    copy.B_Rook ^= (1ul << 50);
                }
            }

            if (pieceType == PieceType.Pawn && ((endlocation == (startlocation - 16)) || (endlocation == (startlocation + 16)))) //Just moved foward two?
            {
                copy.enpassent = startlocation % 8;
            }
            else
            {
                copy.enpassent = -2;
            }
            if (pieceType == PieceType.Pawn && (endlocation / 8 == 7 || endlocation / 8 == 0))
            {
                ++promotions;
                if (side == Side.White)
                {
                    copy.W_Pawn ^= startpos;
                    copy.W_Queen ^= endpos;
                }
                else
                {
                    copy.B_Pawn ^= startpos;
                    copy.B_Queen ^= endpos;
                }
            }
            else if (side == Side.White)
            {
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        copy.W_Pawn ^= startpos;
                        copy.W_Pawn ^= endpos;
                        break;
                    case PieceType.Rook:
                        copy.W_Rook ^= startpos;
                        copy.W_Rook ^= endpos;
                        break;
                    case PieceType.Knight:
                        copy.W_Knight ^= startpos;
                        copy.W_Knight ^= endpos;
                        break;
                    case PieceType.Bishop:
                        copy.W_Bishop ^= startpos;
                        copy.W_Bishop ^= endpos;
                        break;
                    case PieceType.Queen:
                        copy.W_Queen ^= startpos;
                        copy.W_Queen ^= endpos;
                        break;
                    case PieceType.King:
                        copy.W_King ^= startpos;
                        copy.W_King ^= endpos;
                        break;
                }
            }
            else
            {
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        copy.B_Pawn ^= startpos;
                        copy.B_Pawn ^= endpos;
                        break;
                    case PieceType.Rook:
                        copy.B_Rook ^= startpos;
                        copy.B_Rook ^= endpos;
                        break;
                    case PieceType.Knight:
                        copy.B_Knight ^= startpos;
                        copy.B_Knight ^= endpos;
                        break;
                    case PieceType.Bishop:
                        copy.B_Bishop ^= startpos;
                        copy.B_Bishop ^= endpos;
                        break;
                    case PieceType.Queen:
                        copy.B_Queen ^= startpos;
                        copy.B_Queen ^= endpos;
                        break;
                    case PieceType.King:
                        copy.B_King ^= startpos;
                        copy.B_King ^= endpos;
                        break;
                }
            }

            if (pieceType == PieceType.King)
            {
                if (side == Side.White)
                {
                    copy.W_KingsideCastle = false;
                    copy.W_QueensideCastle = false;
                }
                else
                {
                    copy.B_KingsideCastle = false;
                    copy.B_QueensideCastle = false;
                }
            }
            if (pieceType == PieceType.Rook && startlocation == 0) //Queenside rook
            {
                copy.W_QueensideCastle = false;
            }
            if (pieceType == PieceType.Rook && startlocation == 7) //Kingside rook
            {
                copy.W_KingsideCastle = false;
            }
            if (pieceType == PieceType.Rook && startlocation == 56) //Queenside rook
            {
                copy.B_QueensideCastle = false;
            }
            if (pieceType == PieceType.Rook && startlocation == 63) //Queenside rook
            {
                copy.B_KingsideCastle = false;
            }

            
            stopwatch.Stop();
            MoveTime += stopwatch.ElapsedTicks;
            return copy;
        }
    }
}