
namespace PokerBoom.Server.Models
{
    public class Deck
    {
        private List<int> _cards = Enumerable.Range(1, 52).ToList();
        private Random _random = new Random(Environment.TickCount);
        private void Shuffle()
        {
            int n = _cards.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                int tmp = _cards[k];
                _cards[k] = _cards[n];
                _cards[n] = tmp;
            }
        }

        public Deck()
        {
            Shuffle();
        }

        public int NextCard()
        {
            int card = _cards.ElementAt(0);
            _cards.RemoveAt(0);
            return card;
        }

        public List<int> NextCards(int count)
        {
            List<int> cards = _cards.GetRange(0, count);
            _cards.RemoveRange(0, count);
            return cards;
        }
    }
}
