namespace FreddyChessGame.Models
{
    public class GameState
    {
        public PlayerColor CurrentPlayer { get; private set; }
        public (int, int)? EnPassantTargetSquare { get; private set; }

        public GameState()
        {
            CurrentPlayer = PlayerColor.White;
            EnPassantTargetSquare = null;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
        }

        public void SetEnPassantTarget(int row, int col)
        {
            EnPassantTargetSquare = (row, col);
        }

        public void ClearEnPassantTarget()
        {
            EnPassantTargetSquare = null;
        }
    }
}
