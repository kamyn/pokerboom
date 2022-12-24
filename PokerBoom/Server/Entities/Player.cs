namespace PokerBoom.Server.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int GameId { get; set; }
        public int Stack { get; set; }
        public int SeatPlace { get; set; }
        public int FirstCard { get; set; }
        public int SecondCard { get; set; }
        public Game Game { get; set; }
        public List<Bet> Bets { get; set; }
        public ApplicationUser User { get; set; }
    }
}
