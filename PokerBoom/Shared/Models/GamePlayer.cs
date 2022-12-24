namespace PokerBoom.Shared.Models
{
    public class GamePlayer
    {
        public string Username { get; set; }

        public bool IsPlaying { get; set; }

        public int SeatNumber { get; set; }

        public int Stack { get; set; }

        public int Bet { get; set; }

        public GamePlayer()
        {
            IsPlaying = false;
        }
    }
}
