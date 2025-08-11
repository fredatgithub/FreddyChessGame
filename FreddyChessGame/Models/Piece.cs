namespace FreddyChessGame.Models
{
    public class Piece
    {
        public PieceType Type { get; }
        public PlayerColor Color { get; }
        public bool HasMoved { get; set; }

        public Piece(PieceType type, PlayerColor color)
        {
            Type = type;
            Color = color;
            HasMoved = false;
        }
    }
}
