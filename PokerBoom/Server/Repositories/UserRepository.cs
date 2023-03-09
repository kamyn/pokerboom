using PokerBoom.Server.Data;
using PokerBoom.Shared.Models;

namespace PokerBoom.Server.Repositories
{
    public class UserRepository : IUserRepostitory
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> GetBalance(string userName)
        {
            var user = _db.Users.First(u => u.UserName == userName);
            if (user != null)
                return user.Currency;
            return 0;
        }

        public async Task<bool> ChangeBalance(string userName, int balance)
        {
            var user = _db.Users.First(u => u.UserName == userName);
            if (user != null)
            {
                user.Currency = balance;
                _db.SaveChanges();
            }
            return true;
        }
    }
}
