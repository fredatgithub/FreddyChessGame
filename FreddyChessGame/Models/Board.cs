using System;
using System.Collections.Generic;

namespace FreddyChessGame.Models
{
    public class Board
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        public Board() { ResetBoard(); }

        public Board(Board other)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (other.pieces[r, c] != null)
                    {
                        this.pieces[r, c] = new Piece(other.pieces[r, c].Type, other.pieces[r, c].Color) { HasMoved = other.pieces[r, c].HasMoved };
                    }
                    else { this.pieces[r, c] = null; }
                }
            }
        }

        public Piece GetPieceAt(int row, int col)
        {
            if (row < 0 || row >= 8 || col < 0 || col >= 8) return null;
            return pieces[row, col];
        }

        public void SetPieceAt(int row, int col, Piece piece)
        {
            if (row < 0 || row >= 8 || col < 0 || col >= 8) return;
            pieces[row, col] = piece;
        }

        public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
        {
            Piece pieceToMove = GetPieceAt(fromRow, fromCol);
            if (pieceToMove == null) return;

            if (pieceToMove.Type == PieceType.Pawn && fromCol != toCol && GetPieceAt(toRow, toCol) == null)
            {
                int capturedPawnRow = toRow + ((pieceToMove.Color == PlayerColor.White) ? 1 : -1);
                SetPieceAt(capturedPawnRow, toCol, null);
            }

            if (pieceToMove.Type == PieceType.King && Math.Abs(fromCol - toCol) == 2)
            {
                if (toCol > fromCol) { Piece rook = GetPieceAt(fromRow, 7); SetPieceAt(fromRow, 5, rook); SetPieceAt(fromRow, 7, null); if (rook != null) rook.HasMoved = true; }
                else { Piece rook = GetPieceAt(fromRow, 0); SetPieceAt(fromRow, 3, rook); SetPieceAt(fromRow, 0, null); if (rook != null) rook.HasMoved = true; }
            }

            pieceToMove.HasMoved = true;
            SetPieceAt(toRow, toCol, pieceToMove);
            SetPieceAt(fromRow, fromCol, null);
        }

        public bool IsValidMove(int fromRow, int fromCol, int toRow, int toCol, (int, int)? enPassantTarget)
        {
            if (fromRow < 0 || fromRow > 7 || fromCol < 0 || fromCol > 7 || toRow < 0 || toRow > 7 || toCol < 0 || toCol > 7) return false;
            if (fromRow == toRow && fromCol == toCol) return false;
            Piece piece = GetPieceAt(fromRow, fromCol);
            if (piece == null) return false;
            Piece destinationPiece = GetPieceAt(toRow, toCol);
            if (destinationPiece != null && destinationPiece.Color == piece.Color) return false;

            bool isPatternValid;
            switch (piece.Type)
            {
                case PieceType.Pawn: isPatternValid = IsValidPawnMove(piece, fromRow, fromCol, toRow, toCol, enPassantTarget); break;
                case PieceType.Rook: isPatternValid = IsValidRookMove(fromRow, fromCol, toRow, toCol); break;
                case PieceType.Bishop: isPatternValid = IsValidBishopMove(fromRow, fromCol, toRow, toCol); break;
                case PieceType.Knight: isPatternValid = IsValidKnightMove(fromRow, fromCol, toRow, toCol); break;
                case PieceType.Queen: isPatternValid = IsValidRookMove(fromRow, fromCol, toRow, toCol) || IsValidBishopMove(fromRow, fromCol, toRow, toCol); break;
                case PieceType.King: isPatternValid = IsValidKingMove(piece, fromRow, fromCol, toRow, toCol); break;
                default: return false;
            }

            if (!isPatternValid) return false;

            Board tempBoard = new Board(this);
            tempBoard.MovePiece(fromRow, fromCol, toRow, toCol);
            if (tempBoard.IsKingInCheck(piece.Color)) return false;

            return true;
        }

        public bool HasLegalMoves(PlayerColor playerColor, (int, int)? enPassantTarget)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = GetPieceAt(r, c);
                    if (piece != null && piece.Color == playerColor)
                    {
                        for (int toR = 0; toR < 8; toR++)
                        {
                            for (int toC = 0; toC < 8; toC++)
                            {
                                if (IsValidMove(r, c, toR, toC, enPassantTarget))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool IsValidPawnMove(Piece pawn, int fromRow, int fromCol, int toRow, int toCol, (int, int)? enPassantTarget)
        {
            int forwardDirection = (pawn.Color == PlayerColor.White) ? -1 : 1;
            Piece destinationPiece = GetPieceAt(toRow, toCol);
            if (toCol == fromCol && toRow == fromRow + forwardDirection && destinationPiece == null) return true;
            bool isStartingRank = (pawn.Color == PlayerColor.White && fromRow == 6) || (pawn.Color == PlayerColor.Black && fromRow == 1);
            if (isStartingRank && toCol == fromCol && toRow == fromRow + 2 * forwardDirection && destinationPiece == null && GetPieceAt(fromRow + forwardDirection, fromCol) == null) return true;
            if (Math.Abs(toCol - fromCol) == 1 && toRow == fromRow + forwardDirection && destinationPiece != null) return true;
            if (enPassantTarget.HasValue && toRow == enPassantTarget.Value.Item1 && toCol == enPassantTarget.Value.Item2 && Math.Abs(toCol - fromCol) == 1 && toRow == fromRow + forwardDirection && destinationPiece == null) return true;
            return false;
        }

        private bool IsValidRookMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (fromRow != toRow && fromCol != toCol) return false;
            if (fromRow == toRow) { for (int c = Math.Min(fromCol, toCol) + 1; c < Math.Max(fromCol, toCol); c++) { if (GetPieceAt(fromRow, c) != null) return false; } }
            else { for (int r = Math.Min(fromRow, toRow) + 1; r < Math.Max(fromRow, toRow); r++) { if (GetPieceAt(r, fromCol) != null) return false; } }
            return true;
        }

        private bool IsValidBishopMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (Math.Abs(fromRow - toRow) != Math.Abs(fromCol - toCol)) return false;
            int rowStep = Math.Sign(toRow - fromRow);
            int colStep = Math.Sign(toCol - fromCol);
            for (int i = 1; i < Math.Abs(fromRow - toRow); i++) { if (GetPieceAt(fromRow + i * rowStep, fromCol + i * colStep) != null) return false; }
            return true;
        }

        private bool IsValidKnightMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            return (Math.Abs(fromRow - toRow) == 2 && Math.Abs(fromCol - toCol) == 1) || (Math.Abs(fromRow - toRow) == 1 && Math.Abs(fromCol - toCol) == 2);
        }

        private bool IsValidKingMove(Piece king, int fromRow, int fromCol, int toRow, int toCol)
        {
            if (Math.Abs(fromRow - toRow) <= 1 && Math.Abs(fromCol - toCol) <= 1) return true;
            if (Math.Abs(fromCol - toCol) == 2 && fromRow == toRow && !king.HasMoved && !IsKingInCheck(king.Color))
            {
                if (toCol == 6) { Piece rook = GetPieceAt(fromRow, 7); if (rook != null && !rook.HasMoved && GetPieceAt(fromRow, 5) == null && GetPieceAt(fromRow, 6) == null && !IsSquareAttackedBy(fromRow, 5, (king.Color == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White)) return true; }
                else if (toCol == 2) { Piece rook = GetPieceAt(fromRow, 0); if (rook != null && !rook.HasMoved && GetPieceAt(fromRow, 1) == null && GetPieceAt(fromRow, 2) == null && GetPieceAt(fromRow, 3) == null && !IsSquareAttackedBy(fromRow, 3, (king.Color == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White)) return true; }
            }
            return false;
        }

        private (int, int) FindKingPosition(PlayerColor kingColor)
        {
            for (int r = 0; r < 8; r++) { for (int c = 0; c < 8; c++) { Piece p = GetPieceAt(r, c); if (p != null && p.Type == PieceType.King && p.Color == kingColor) return (r, c); } }
            return (-1, -1);
        }

        public bool IsSquareAttackedBy(int row, int col, PlayerColor attackerColor)
        {
            for (int r = 0; r < 8; r++) { for (int c = 0; c < 8; c++) { Piece piece = GetPieceAt(r, c); if (piece != null && piece.Color == attackerColor) { bool canAttack; switch (piece.Type) { case PieceType.Pawn: canAttack = Math.Abs(c - col) == 1 && r + ((attackerColor == PlayerColor.White) ? -1 : 1) == row; break; case PieceType.Rook: canAttack = IsValidRookMove(r, c, row, col); break; case PieceType.Bishop: canAttack = IsValidBishopMove(r, c, row, col); break; case PieceType.Knight: canAttack = IsValidKnightMove(r, c, row, col); break; case PieceType.Queen: canAttack = IsValidRookMove(r, c, row, col) || IsValidBishopMove(r, c, row, col); break; case PieceType.King: canAttack = Math.Abs(r - row) <= 1 && Math.Abs(c - col) <= 1; break; default: canAttack = false; break; } if (canAttack) return true; } } }
            return false;
        }

        public bool IsKingInCheck(PlayerColor kingColor)
        {
            var kingPosition = FindKingPosition(kingColor);
            if (kingPosition == (-1, -1)) return false;
            return IsSquareAttackedBy(kingPosition.Item1, kingPosition.Item2, (kingColor == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White);
        }

        public void ResetBoard()
        {
            for (int r = 0; r < 8; r++) { for (int c = 0; c < 8; c++) { pieces[r, c] = null; } }
            pieces[0, 0] = new Piece(PieceType.Rook, PlayerColor.Black);
            pieces[0, 1] = new Piece(PieceType.Knight, PlayerColor.Black);
            pieces[0, 2] = new Piece(PieceType.Bishop, PlayerColor.Black);
            pieces[0, 3] = new Piece(PieceType.Queen, PlayerColor.Black);
            pieces[0, 4] = new Piece(PieceType.King, PlayerColor.Black);
            pieces[0, 5] = new Piece(PieceType.Bishop, PlayerColor.Black);
            pieces[0, 6] = new Piece(PieceType.Knight, PlayerColor.Black);
            pieces[0, 7] = new Piece(PieceType.Rook, PlayerColor.Black);
            for (int c = 0; c < 8; c++) { pieces[1, c] = new Piece(PieceType.Pawn, PlayerColor.Black); }
            pieces[7, 0] = new Piece(PieceType.Rook, PlayerColor.White);
            pieces[7, 1] = new Piece(PieceType.Knight, PlayerColor.White);
            pieces[7, 2] = new Piece(PieceType.Bishop, PlayerColor.White);
            pieces[7, 3] = new Piece(PieceType.Queen, PlayerColor.White);
            pieces[7, 4] = new Piece(PieceType.King, PlayerColor.White);
            pieces[7, 5] = new Piece(PieceType.Bishop, PlayerColor.White);
            pieces[7, 6] = new Piece(PieceType.Knight, PlayerColor.White);
            pieces[7, 7] = new Piece(PieceType.Rook, PlayerColor.White);
            for (int c = 0; c < 8; c++) { pieces[6, c] = new Piece(PieceType.Pawn, PlayerColor.White); }
        }
    }
}
