using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Models
{
    public class Player
    {
        public string Name { get; set; }
        public List<int> HandCards { get; set; } = new List<int>();
        public int HandStrength { get; set; } = 0;
        public bool IsPlaying { get; set; } = true;
        public int RoundBet { get; set; }
        public bool HasMadeMove { get; set; } = false;
        public int SeatNumber { get; set; }
        public int Stack { get; set; }
    }
}
