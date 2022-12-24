namespace PokerBoom.Server.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int BoardId { get; set; }

        public List<Player> Players { get; set; }
        public Board Board { get; set; }
        public Table Table { get; set; }
        public List<Bet> Bets { get; set; }
    }
}
