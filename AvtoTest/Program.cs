using AvtoTest.Enums;
using AvtoTest.Models;
using AvtoTest.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botService = new TelegramBotService();
var userService = new UserService(botService);
var examService = new ExaminationService(botService);
var ticketService = new TickesServices(botService);
var menuService = new MenuService(botService, examService, ticketService);
botService.GetUpdate((_, update, _) => Task.Run(() => GetUpdate(update)));
Console.ReadKey();

void GetUpdate(Update update)
{
    var (from, messageId, message, reply, isSuccess) = GetValues(update);
    if (!isSuccess) return;

    var user = userService.AddUser(from);
    if (messageId == user.OldMessageId) return;
    user.OldMessageId = messageId;
    StepFilter(user, messageId, message, reply);

}

void StepFilter(UserEntity user, int messageId, string message, InlineKeyboardMarkup reply)
{
    switch (user.Step)
    {
        case EUserStep.NewUser: menuService.SendMenu(user); break;
        case EUserStep.Menu: menuService.TextFilter(user, message); break;
        case EUserStep.Exam: menuService.ExamTextFilter(user, message); break;
        case EUserStep.ExamStarted: examService.CheckAnswer(user, message, messageId, reply); break;
        case EUserStep.TicketList: ticketService.TextFilter(user, message, messageId); break;
        case EUserStep.TicketStarting: menuService.TicketFilterStarting(user, message, messageId); break;
        case EUserStep.TicketStarted: ticketService.CheckAnswer(user, message, messageId, reply); break;
    }
}

Tuple<User, int, string, InlineKeyboardMarkup, bool> GetValues(Update update)
{
    User from;
    string message;
    int messageId;
    InlineKeyboardMarkup messageMarkup = null;
    if (update.Type == UpdateType.CallbackQuery)
    {
        from = update.CallbackQuery.From;
        message = update.CallbackQuery.Data;
        messageId = update.CallbackQuery.Message.MessageId;
        messageMarkup = update.CallbackQuery.Message.ReplyMarkup;
    }
    else if (update.Type == UpdateType.Message)
    {
        from = update.Message.From;
        message = update.Message.Text;
        messageId = update.Message.MessageId;
    }
    else return new(null, 0, null, null, false);

    return new(from, messageId, message, messageMarkup, true);
}





