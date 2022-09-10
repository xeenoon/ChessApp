﻿using System.Collections.Generic;
using System.Diagnostics;

namespace ChessApp
{
    public class Bitboard
    {
        public ulong W_Pawn = 0ul;   //White Pawns
        public ulong W_Rook = 0ul;   //White Rooks
        public ulong W_Knight = 0ul; //White Knights
        public ulong W_Bishop = 0ul; //White Bishops
        public ulong W_Queen = 0ul;  //White Queens
        public ulong W_King = 0ul;   //White King

        public ulong B_Pawn = 0ul;   //Black Pawns
        public ulong B_Rook = 0ul;   //Black Rooks
        public ulong B_Knight = 0ul; //Black Knights
        public ulong B_Bishop = 0ul; //Black Bishops
        public ulong B_Queen = 0ul;  //Black Queens
        public ulong B_King = 0ul;   //Black King

        public ulong[] pinnedPieces = new ulong[64] { ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue }; 
        //Legal moves allowed because of pins for every place on the board
        //Default is everything

        public bool check = false;
        public bool doublecheck = false;
        public ulong squares_to_block_check = ulong.MaxValue; //Squares that pieces can move to to block checks
        //If king is in check, pieces should only be able to move to squares that block the attack

        public int enpassent = -2;

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
        public ulong WhiteAttackedSquares = 0ul;
        public ulong BlackAttackedSquares = 0ul;
        
        public static double StaticAttack = 0;
        public static double SlidingAttack = 0;
        public static double Pins = 0;

        public bool B_KingsideCastle;
        public bool B_QueensideCastle;
        public bool W_KingsideCastle;
        public bool W_QueensideCastle;

        public void SetupSquareAttacks()
        {
            squares_to_block_check = ulong.MaxValue;
            WhiteAttackedSquares = WhiteAttacks();
            BlackAttackedSquares = BlackAttacks();
            SetupPins();
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

        int checks = 0;
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
                            int v = BitOperations.TrailingZeros(pinned) - 1;
                            pinnedPieces[v] = attackray | bitpos; //Add this piece to the pinned list, it will not be able to move next turn
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
                W_Pawn = W_Pawn,
                W_Bishop = W_Bishop,
                W_King = W_King,
                W_Knight = W_Knight,
                W_Queen = W_Queen,
                W_Rook = W_Rook,

                B_Pawn = B_Pawn,
                B_Bishop = B_Bishop,
                B_King = B_King,
                B_Knight = B_Knight,
                B_Queen = B_Queen,
                B_Rook = B_Rook,

                B_KingsideCastle = B_KingsideCastle,
                W_KingsideCastle = W_KingsideCastle,
                B_QueensideCastle = B_QueensideCastle,
                W_QueensideCastle = W_QueensideCastle,

                enpassent = enpassent,
            };
        }
        public Bitboard(Chessboard board)
        {
            foreach (var piece in board.Pieces)
            {
                if (piece.side == Side.White)
                {
                    switch (piece.pieceType)
                    {
                        case PieceType.Pawn:
                            W_Pawn |= 1ul << piece.position;
                            break;
                        case PieceType.Rook:
                            W_Rook |= 1ul << piece.position;
                            break;
                        case PieceType.Knight:
                            W_Knight |= 1ul << piece.position;
                            break;
                        case PieceType.Bishop:
                            W_Bishop |= 1ul << piece.position;
                            break;
                        case PieceType.Queen:
                            W_Queen |= 1ul << piece.position;
                            break;
                        case PieceType.King:
                            W_King |= 1ul << piece.position;
                            break;
                    }
                }
                else
                {
                    switch (piece.pieceType)
                    {
                        case PieceType.Pawn:
                            B_Pawn |= 1ul << piece.position;
                            break;
                        case PieceType.Rook:
                            B_Rook |= 1ul << piece.position;
                            break;
                        case PieceType.Knight:
                            B_Knight |= 1ul << piece.position;
                            break;
                        case PieceType.Bishop:
                            B_Bishop |= 1ul << piece.position;
                            break;
                        case PieceType.Queen:
                            B_Queen |= 1ul << piece.position;
                            break;
                        case PieceType.King:
                            B_King |= 1ul << piece.position;
                            break;
                    }
                }
            }
            B_KingsideCastle = board.blackCastles.Kingside;
            B_QueensideCastle = board.blackCastles.Queenside;

            W_KingsideCastle  = board.whiteCastles.Kingside;
            W_QueensideCastle = board.whiteCastles.Queenside;
        }
        public Bitboard()
        {

        }
        public ulong PieceBB(Side side, PieceType pieceType)
        {
            if (side == Side.White)
            {
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return W_Pawn;
                    case PieceType.Rook:
                        return W_Rook;
                    case PieceType.Knight:
                        return W_Knight;
                    case PieceType.Bishop:
                        return W_Bishop;
                    case PieceType.Queen:
                        return W_Queen;
                    case PieceType.King:
                        return W_King;
                }
            }
            else
            {
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return B_Pawn;
                    case PieceType.Rook:
                        return B_Rook;
                    case PieceType.Knight:
                        return B_Knight;
                    case PieceType.Bishop:
                        return B_Bishop;
                    case PieceType.Queen:
                        return B_Queen;
                    case PieceType.King:
                        return B_King;
                }
            }
            return 0ul; //wot
        }
        public static ulong captures;
        public static ulong enpassantes;
        public static ulong castles;
        public static ulong promotions;
        public Bitboard Move(byte startlocation, byte endlocation, ulong startpos, ulong endpos, PieceType pieceType, Side side)
        {
            Bitboard copy = Copy();

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

            return copy;
        }
    }
}