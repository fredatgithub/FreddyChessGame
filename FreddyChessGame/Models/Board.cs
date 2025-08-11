using System;

namespace FreddyChessGame.Models
{
    public class Board
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        public Board()
        {
            ResetBoard();
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
            if (pieceToMove != null)
            {
                SetPieceAt(toRow, toCol, pieceToMove);
                SetPieceAt(fromRow, fromCol, null);
            }
        }

        public bool IsValidMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (fromRow < 0 || fromRow > 7 || fromCol < 0 || fromCol > 7 || toRow < 0 || toRow > 7 || toCol < 0 || toCol > 7) return false;
            if (fromRow == toRow && fromCol == toCol) return false;
            Piece piece = GetPieceAt(fromRow, fromCol);
            if (piece == null) return false;
            Piece destinationPiece = GetPieceAt(toRow, toCol);
            if (destinationPiece != null && destinationPiece.Color == piece.Color) return false;

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    return IsValidPawnMove(piece, fromRow, fromCol, toRow, toCol);
                case PieceType.Rook:
                    return IsValidRookMove(fromRow, fromCol, toRow, toCol);
                case PieceType.Bishop:
                    return IsValidBishopMove(fromRow, fromCol, toRow, toCol);
                case PieceType.Knight:
                    return IsValidKnightMove(fromRow, fromCol, toRow, toCol);
                case PieceType.Queen:
                    return IsValidRookMove(fromRow, fromCol, toRow, toCol) || IsValidBishopMove(fromRow, fromCol, toRow, toCol);
                case PieceType.King:
                    return IsValidKingMove(fromRow, fromCol, toRow, toCol);
                default:
                    return false;
            }
        }

        private bool IsValidPawnMove(Piece pawn, int fromRow, int fromCol, int toRow, int toCol)
        {
            int forwardDirection = (pawn.Color == PlayerColor.White) ? -1 : 1;
            Piece destinationPiece = GetPieceAt(toRow, toCol);

            if (toCol == fromCol && toRow == fromRow + forwardDirection && destinationPiece == null) return true;

            bool isStartingRank = (pawn.Color == PlayerColor.White && fromRow == 6) || (pawn.Color == PlayerColor.Black && fromRow == 1);
            if (isStartingRank && toCol == fromCol && toRow == fromRow + 2 * forwardDirection && destinationPiece == null && GetPieceAt(fromRow + forwardDirection, fromCol) == null) return true;

            if (Math.Abs(toCol - fromCol) == 1 && toRow == fromRow + forwardDirection && destinationPiece != null) return true;

            return false;
        }

        private bool IsValidRookMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (fromRow != toRow && fromCol != toCol) return false;

            if (fromRow == toRow)
            {
                int startCol = Math.Min(fromCol, toCol) + 1;
                int endCol = Math.Max(fromCol, toCol);
                for (int c = startCol; c < endCol; c++)
                {
                    if (GetPieceAt(fromRow, c) != null) return false;
                }
            }
            else
            {
                int startRow = Math.Min(fromRow, toRow) + 1;
                int endRow = Math.Max(fromRow, toRow);
                for (int r = startRow; r < endRow; r++)
                {
                    if (GetPieceAt(r, fromCol) != null) return false;
                }
            }

            return true;
        }

        private bool IsValidBishopMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (Math.Abs(fromRow - toRow) != Math.Abs(fromCol - toCol)) return false;

            int rowStep = (toRow > fromRow) ? 1 : -1;
            int colStep = (toCol > fromCol) ? 1 : -1;
            int currentRow = fromRow + rowStep;
            int currentCol = fromCol + colStep;

            while (currentRow != toRow && currentCol != toCol)
            {
                if (GetPieceAt(currentRow, currentCol) != null) return false;
                currentRow += rowStep;
                currentCol += colStep;
            }

            return true;
        }

        private bool IsValidKnightMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            int rowDiff = Math.Abs(fromRow - toRow);
            int colDiff = Math.Abs(fromCol - toCol);
            return (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
        }

        private bool IsValidKingMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            int rowDiff = Math.Abs(fromRow - toRow);
            int colDiff = Math.Abs(fromCol - toCol);
            return rowDiff <= 1 && colDiff <= 1;
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
