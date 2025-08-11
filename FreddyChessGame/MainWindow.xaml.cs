using FreddyChessGame.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FreddyChessGame
{
  public partial class MainWindow: Window
  {
    private readonly Board chessBoard;
    private readonly GameState gameState;
    private Point? selectedSquare = null;
    private bool isGameOver = false;

    public MainWindow()
    {
      InitializeComponent();
      chessBoard = new Board();
      gameState = new GameState();
      InitializeBoardUI();
      DrawPieces();
      UpdateTitle();

      ChessGrid.MouseDown += OnChessGridMouseDown;
    }

    private void UpdateTitle()
    {
      if (isGameOver) return;
      string playerColor = gameState.CurrentPlayer == PlayerColor.White ? "Blanc" : "Noir";
      Title = $"Jeu d'échecs - Au tour de : {playerColor}";
    }

    private void OnChessGridMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (isGameOver) return;

      Point position = e.GetPosition(ChessGrid);
      int row = (int)(position.Y / ChessGrid.ActualHeight * 8);
      int col = (int)(position.X / ChessGrid.ActualWidth * 8);

      if (selectedSquare == null)
      {
        Piece piece = chessBoard.GetPieceAt(row, col);
        if (piece != null && piece.Color == gameState.CurrentPlayer)
        {
          selectedSquare = new Point(col, row);
        }
      }
      else
      {
        int fromRow = (int)selectedSquare.Value.Y;
        int fromCol = (int)selectedSquare.Value.X;
        Piece movingPiece = chessBoard.GetPieceAt(fromRow, fromCol);

        if (chessBoard.IsValidMove(fromRow, fromCol, row, col, gameState.EnPassantTargetSquare))
        {
          bool wasTwoSquarePawnMove = movingPiece.Type == PieceType.Pawn && Math.Abs(row - fromRow) == 2;

          chessBoard.MovePiece(fromRow, fromCol, row, col);

          gameState.ClearEnPassantTarget();
          if (wasTwoSquarePawnMove)
          {
            gameState.SetEnPassantTarget(fromRow + ((movingPiece.Color == PlayerColor.White) ? -1 : 1), fromCol);
          }

          gameState.SwitchPlayer();
          RedrawBoard();
          UpdateTitle();
          CheckForGameOver();
        }

        selectedSquare = null;
      }
    }

    private void CheckForGameOver()
    {
      if (!chessBoard.HasLegalMoves(gameState.CurrentPlayer, gameState.EnPassantTargetSquare))
      {
        isGameOver = true;
        if (chessBoard.IsKingInCheck(gameState.CurrentPlayer))
        {
          MessageBox.Show($"Échec et mat ! {gameState.CurrentPlayer} a perdu.", "Partie terminée");
        }
        else
        {
          MessageBox.Show("Pat ! La partie est nulle.", "Partie terminée");
        }
      }
    }

    private void RedrawBoard()
    {
      var pieceVisuals = ChessGrid.Children.OfType<TextBlock>().ToList();
      foreach (var visual in pieceVisuals) { ChessGrid.Children.Remove(visual); }
      DrawPieces();
    }

    private void InitializeBoardUI()
    {
      for (int i = 0; i < 8; i++) { ChessGrid.ColumnDefinitions.Add(new ColumnDefinition()); ChessGrid.RowDefinitions.Add(new RowDefinition()); }
      for (int r = 0; r < 8; r++) { for (int c = 0; c < 8; c++) { var rect = new Rectangle { Fill = (r + c) % 2 == 0 ? Brushes.Beige : Brushes.SaddleBrown }; Grid.SetRow(rect, r); Grid.SetColumn(rect, c); ChessGrid.Children.Add(rect); } }
    }

    private void DrawPieces()
    {
      for (int r = 0; r < 8; r++) { for (int c = 0; c < 8; c++) { Piece piece = chessBoard.GetPieceAt(r, c); if (piece != null) { TextBlock pieceIcon = new TextBlock { Text = GetPieceUnicode(piece), FontSize = 60, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, IsHitTestVisible = false }; Grid.SetRow(pieceIcon, r); Grid.SetColumn(pieceIcon, c); ChessGrid.Children.Add(pieceIcon); } } }
    }

    private string GetPieceUnicode(Piece piece)
    {
      switch (piece.Color)
      {
        case PlayerColor.White:
          switch (piece.Type) { case PieceType.Pawn: return "♙"; case PieceType.Rook: return "♖"; case PieceType.Knight: return "♘"; case PieceType.Bishop: return "♗"; case PieceType.Queen: return "♕"; case PieceType.King: return "♔"; }
          break;
        case PlayerColor.Black:
          switch (piece.Type) { case PieceType.Pawn: return "♟"; case PieceType.Rook: return "♜"; case PieceType.Knight: return "♞"; case PieceType.Bishop: return "♝"; case PieceType.Queen: return "♛"; case PieceType.King: return "♚"; }
          break;
      }
      return "";
    }
  }
}
