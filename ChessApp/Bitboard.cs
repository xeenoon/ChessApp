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

        public ulong pinnedPieces = 0ul;

        public bool check = false;
        public bool doublecheck = false;
        public ulong squares_to_block_check = ulong.MaxValue; //Squares that pieces can move to to block checks
        //If king is in check, pieces should only be able to move to squares that block the attack

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
        
        public void SetupSquareAttacks()
        {
            squares_to_block_check = ulong.MaxValue;
            WhiteAttackedSquares = WhiteAttacks();
            BlackAttackedSquares = BlackAttacks();
            SetupPins();
        }
        public void SetupPins()
        {
            pinnedPieces |= XRAY_Pins(W_Rook, PieceType.Rook, Side.White, B_King);
            pinnedPieces |= XRAY_Pins(W_Bishop, PieceType.Bishop, Side.White, B_King);
            pinnedPieces |= XRAY_Pins(W_Queen, PieceType.Queen, Side.White, B_King);

            pinnedPieces |= XRAY_Pins(B_Rook, PieceType.Rook, Side.Black, B_King);
            pinnedPieces |= XRAY_Pins(B_Bishop, PieceType.Bishop, Side.Black, B_King);
            pinnedPieces |= XRAY_Pins(B_Queen, PieceType.Queen, Side.Black, B_King);
        }

        int checks = 0;
        ulong WhiteAttacks()
        {
            checks = 0;

            ulong attacks = 0ul;
            attacks |= StaticPieceAttacks(attacks, W_Pawn, PieceType.Pawn     , Side.White, B_King);
            attacks |= SlidingPieceAttacks(attacks, W_Rook, PieceType.Rook     , Side.White, B_King);
            attacks |= SlidingPieceAttacks(attacks, W_Bishop, PieceType.Bishop , Side.White, B_King);
            attacks |= StaticPieceAttacks(attacks, W_King, PieceType.King     , Side.White, B_King);
            attacks |= SlidingPieceAttacks(attacks, W_Queen, PieceType.Queen   , Side.White, B_King);
            attacks |= StaticPieceAttacks(attacks, W_Knight, PieceType.Knight , Side.White, B_King);
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
            attacks |= StaticPieceAttacks(attacks, B_Pawn, PieceType.Pawn     , Side.Black, W_King);
            attacks |= SlidingPieceAttacks(attacks, B_Rook, PieceType.Rook     , Side.Black, W_King);
            attacks |= SlidingPieceAttacks(attacks, B_Bishop, PieceType.Bishop , Side.Black, W_King);
            attacks |= StaticPieceAttacks(attacks, B_King, PieceType.King     , Side.Black, W_King);
            attacks |= SlidingPieceAttacks(attacks, B_Queen, PieceType.Queen   , Side.Black, W_King);
            attacks |= StaticPieceAttacks(attacks, B_Knight, PieceType.Knight , Side.Black, W_King);
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
            stopwatch.Stop();
            SlidingAttack += stopwatch.ElapsedTicks;
            return attacks;
        }
        private ulong StaticPieceAttacks(ulong attacks, ulong piece_bitboard, PieceType pieceType, Side s, ulong oppositeKing)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (piece_bitboard != 0)
            {
                byte lsb = (byte)(BitOperations.TrailingZeros(piece_bitboard) - 1);
                ulong bitpos = 1ul << lsb;
                piece_bitboard ^= bitpos; //remove this pawn from the ulong of pieces

                ulong attackray = MoveGenerator.StaticAttackRays(pieceType, s, lsb, this);
                if ((attackray & oppositeKing) != 0) //King is in check
                {
                    ++checks;
                    squares_to_block_check = bitpos; //Piece must be taken to block
                }
                attacks |= attackray; //Get all the attacking moves and add them to the attacks bitboard
            }
            stopwatch.Stop();
            StaticAttack += stopwatch.ElapsedTicks;
            return attacks;
        }

        private ulong XRAY_Pins(ulong piece_bitboard, PieceType pieceType, Side s, ulong oppositeKing)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ulong result = 0ul;

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
                        var pinned = attackray & oppositeside; //Find all the pinned pieces

                        if ((pinned & (pinned-1)) == 0) //Only one bit set
                        {
                            pinnedPieces |= pinned; //Add this piece to the pinned list, it will not be able to move next turn
                        }
                    }
                    result |= attackray; //Get all the attacking moves and add them to the attacks bitboard
                }
            }
            stopwatch.Stop();
            Pins += stopwatch.ElapsedTicks;
            return result;
        }

        public Bitboard(string FEN)
        {
            string[] strs = FEN.Split(' ');
            string[] lines = strs[0].Split('/');
            for (int row = 0; row < lines.Length; ++row)
            {
                var line = lines[row];
                int x = 0;
                for (int i = 0; i < line.Length; ++i)
                {
                    if (char.IsDigit(line[i])) //Jumping foward?
                    {
                        x += int.Parse(line[i].ToString());
                        continue;
                    }
                    ulong position = (1ul << (((7-row) * 8) + x));
                    switch (line[i])
                    {
                        //White pieces
                        case 'p':
                            B_Pawn |= position;
                            break;
                        case 'r':
                            B_Rook |= position;
                            break;
                        case 'n':
                            B_Knight |= position;
                            break;
                        case 'b':
                            B_Bishop |= position;
                            break;
                        case 'k':
                            B_King |= position;
                            break;
                        case 'q':
                            B_Queen |= position;
                            break;

                        //Black pieces
                        case 'P':
                            W_Pawn |= position;
                            break;
                        case 'R':
                            W_Rook |= position; 
                            break;
                        case 'N':
                            W_Knight |= position; 
                            break;
                        case 'W':
                            W_Bishop |= position; 
                            break;
                        case 'K':
                            W_King |= position; 
                            break;
                        case 'Q':
                            W_Queen |= position;
                            break;
                    }
                    ++x;
                }
            }
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
            };
        }
        public Bitboard()
        {

        }
    }
}