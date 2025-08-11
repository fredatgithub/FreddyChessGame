namespace FreddyChessGame.Models
{
    public class GameState
    {
        public PlayerColor CurrentPlayer { get; private set; }

        public GameState()
        {
            CurrentPlayer = PlayerColor.White;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer == PlayerColor.White) ? PlayerColor.Black : PlayerColor.White;
        }
    }
}
