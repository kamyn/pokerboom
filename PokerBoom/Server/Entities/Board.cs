namespace PokerBoom.Server.Entities
{
    public class Board
    {
        public int Id { get; set; }
        public int Card1 { get; set; }
        public int Card2 { get; set; }
        public int Card3 { get; set; }
        public int Card4 { get; set; }
        public int Card5 { get; set; }
        public Game Game { get; set; }
    }
}
