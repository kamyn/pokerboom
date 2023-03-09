namespace PokerBoom.Server.Repositories
{
    public interface IUserRepostitory
    {
        public Task<int> GetBalance(string userName);
        public Task<bool> ChangeBalance(string userName, int balance);
    }
}
