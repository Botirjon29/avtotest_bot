

using AvtoTest.Databases;
using AvtoTest.Models;

namespace AvtoTest.Services
{
    public class UserService
    {
        private readonly TelegramBotService _telegramBotService;
        public UserService(TelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public UserEntity AddUser(Telegram.Bot.Types.User user)
        {
            var name = string.IsNullOrEmpty(user.Username) ? user.FirstName : user.Username;
            return Database.DB.UsersDb.AddUser(user.Id, name);
        }
    }
}
