namespace PokerBoom.Server.Entities
{
    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SmallBlind { get; set; }

        public List<Game> Games { get; set; }
    }
}
