using Auth.api.Models;

namespace Auth.api.Repository
{
    public interface IUserRepository
    {
        User? GetUser(string email);
        void InsertUser(User user);
    }
}
