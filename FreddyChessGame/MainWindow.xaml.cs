using FreddyChessGame.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FreddyChessGame
{
    public partial class MainWindow : Window
    {
        private readonly Board chessBoard;
        private readonly GameState gameState;
        private Point? selectedSquare = null;

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
            Title = $"Jeu d'échecs - Au tour de : {gameState.CurrentPlayer}";
        }

        private void OnChessGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(ChessGrid);
            int row = (int)(position.Y / ChessGrid.ActualHeight * 8);
            int col = (int)(position.X / ChessGrid.ActualWidth * 8);

            if (selectedSquare == null)
            {
                // First click: select a piece
                Piece piece = chessBoard.GetPieceAt(row, col);
                if (piece != null && piece.Color == gameState.CurrentPlayer)
                {
                    selectedSquare = new Point(col, row);
                }
            }
            else
            {
                // Second click: try to move the piece
                int fromRow = (int)selectedSquare.Value.Y;
                int fromCol = (int)selectedSquare.Value.X;

                if (chessBoard.IsValidMove(fromRow, fromCol, row, col))
                {
                    chessBoard.MovePiece(fromRow, fromCol, row, col);
                    gameState.SwitchPlayer();
                    RedrawBoard();
                    UpdateTitle();
                }

                // Reset selection regardless of move validity
                selectedSquare = null;
            }
        }

        private void RedrawBoard()
        {
            var pieceVisuals = ChessGrid.Children.OfType<TextBlock>().ToList();
            foreach (var visual in pieceVisuals)
            {
                ChessGrid.Children.Remove(visual);
            }
            DrawPieces();
        }

        private void InitializeBoardUI()
        {
            for (int i = 0; i < 8; i++)
            {
                ChessGrid.ColumnDefinitions.Add(new ColumnDefinition());
                ChessGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var rectangle = new Rectangle
                    {
                        Fill = (r + c) % 2 == 0 ? Brushes.Beige : Brushes.SaddleBrown
                    };
                    Grid.SetRow(rectangle, r);
                    Grid.SetColumn(rectangle, c);
                    ChessGrid.Children.Add(rectangle);
                }
            }
        }

        private void DrawPieces()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = chessBoard.GetPieceAt(r, c);
                    if (piece != null)
                    {
                        TextBlock pieceIcon = new TextBlock
                        {
                            Text = GetPieceUnicode(piece),
                            FontSize = 60,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsHitTestVisible = false
                        };
                        Grid.SetRow(pieceIcon, r);
                        Grid.SetColumn(pieceIcon, c);
                        ChessGrid.Children.Add(pieceIcon);
                    }
                }
            }
        }

        private string GetPieceUnicode(Piece piece)
        {
            switch (piece.Color)
            {
                case PlayerColor.White:
                    switch (piece.Type)
                    {
                        case PieceType.Pawn: return "♙";
                        case PieceType.Rook: return "♖";
                        case PieceType.Knight: return "♘";
                        case PieceType.Bishop: return "♗";
                        case PieceType.Queen: return "♕";
                        case PieceType.King: return "♔";
                    }
                    break;
                case PlayerColor.Black:
                    switch (piece.Type)
                    {
                        case PieceType.Pawn: return "♟";
                        case PieceType.Rook: return "♜";
                        case PieceType.Knight: return "♞";
                        case PieceType.Bishop: return "♝";
                        case PieceType.Queen: return "♛";
                        case PieceType.King: return "♚";
                    }
                    break;
            }
            return "";
        }
    }
}
