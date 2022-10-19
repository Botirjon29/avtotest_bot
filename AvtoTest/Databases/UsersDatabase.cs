
using AvtoTest.Models;

namespace AvtoTest.Databases
{
    public class UsersDatabase
    {
        public List<UserEntity> Users { get; set; }
        public UsersDatabase(List<UserEntity> users)
        {
            Users = users;
        }

        public UserEntity AddUser(long chatId, string name)
        {
            if (Users.Any(user => user.ChatId == chatId))
            {
                return Users.First(user => user.ChatId == chatId);
            }
            var user = new UserEntity(chatId, name);
            Users.Add(user);
            return user;
        }
    }
}
