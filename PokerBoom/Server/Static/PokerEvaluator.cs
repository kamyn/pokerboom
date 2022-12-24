using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Static
{
    public static class PokerEvaluator
    {
        public static int GetHandStrength(List<int> cards) //rank = (c - 1) % 13, suit = (c - 1) / 13
        {
            cards = cards.OrderBy(c => (c - 1) % 13).ToList();
            if (RoyalFlush(cards)) return 9;
            if (StraightFlush(cards)) return 8;
            if (FourOfAKind(cards)) return 7;
            if (FullHouse(cards)) return 6;
            if (Flush(cards)) return 5;
            if (Straight(cards)) return 4;
            if (ThreeOfAKind(cards)) return 3;
            if (TwoPair(cards)) return 2;
            if (OnePair(cards)) return 1;
            return 0;
        }

        private static bool OnePair(List<int> cards) =>
            cards.GroupBy(c => (c - 1) % 13).Count(c => c.Count() == 2) == 1;

        private static bool TwoPair(List<int> cards) =>
            cards.GroupBy(c => (c - 1) % 13).Count(c => c.Count() == 2) == 2;

        private static bool ThreeOfAKind(List<int> cards) =>
            cards.GroupBy(c => (c - 1) % 13).Count(c => c.Count() == 3) == 1;

        private static bool Flush(List<int> cards) =>
            cards.GroupBy(c => (c - 1) % 13).Count(c => c.Count() == 5) == 1;

        private static bool FullHouse(List<int> cards) => 
            cards.GroupBy(c => (c - 1) % 13).All(c => c.Count() == 3 || c.Count() == 2);

        private static bool FourOfAKind(List<int> cards) => 
            cards.GroupBy(c => (c - 1) % 13).Count(c => c.Count() == 4) == 1;

        private static bool StraightFlush(List<int> cards) => 
            Straight(cards) && Flush(cards);

        private static bool RoyalFlush(List<int> cards) =>
            (cards[0] - 1) % 13 == 8 &&
            (cards[1] - 1) % 13 == 9 &&
            (cards[2] - 1) % 13 == 10 &&
            (cards[3] - 1) % 13 == 11 &&
            (cards[4] - 1) % 13 == 12 &&
            Flush(cards);

        private static bool Straight(List<int> cards)
        {
            for (var i = 1; i < cards.Count; i++)
            {
                if (i == 1 && (cards.First() - 1) % 13 == 0 && (cards.Last() - 1) % 13 == 12) continue;
                if ((cards[i] - 1) % 13 - 1 != (cards[i - 1] - 1) % 13)
                    return false;
            }
            return true;
        }
    }
}
