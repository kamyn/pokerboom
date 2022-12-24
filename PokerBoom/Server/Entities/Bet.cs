namespace PokerBoom.Server.Entities
{
    public class Bet
    {
        public int Id { get; set; }
        public int PlayerId { get; set;}
        public int Round { get; set; }
        public int BetAmount { get; set; }
        public Game Game { get; set; }
        public Player Player { get; set; }
    }
}
