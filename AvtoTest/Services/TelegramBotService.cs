using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
namespace AvtoTest.Services
{
    public class TelegramBotService
    {
        private const string BotToken = "5612341440:AAHA31yB0D0WBLFa1P0NOUWGFtB8Cd81pPc";

        private TelegramBotClient bot;

        public TelegramBotService()
        {
            bot = new TelegramBotClient(BotToken);
        }

        public void GetUpdate(Func<ITelegramBotClient, Update, CancellationToken, Task> update)
        {
            bot.StartReceiving(
                updateHandler: update,
                errorHandler: (_, ex, _) =>
                {
                    Console.WriteLine(ex.Message);
                    return Task.CompletedTask;
                }
                );

        }

        public void SendMessage(long chatId, string message, IReplyMarkup reply = null)
        {
            bot.SendTextMessageAsync(chatId, message, replyMarkup: reply);
        }

        public void SendMessage(long chatId, Stream image, string message, IReplyMarkup reply = null)
        {
            bot.SendPhotoAsync(chatId, new InputOnlineFile(image), message, replyMarkup: reply);
        }

        public void EditMessageButtons(long chatId, int messageId, InlineKeyboardMarkup reply)
        {
            bot.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: reply);
        }

        public InlineKeyboardMarkup GetInlineKeyboard(List<string> buttonsText, int? correctAnswerIndex = null, int? questionIndex = null)
        {
            var buttons = new InlineKeyboardButton[buttonsText.Count][];

            for (var i = 0; i < buttonsText.Count; i++)
            {
                buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(
                text: buttonsText[i],
                callbackData: correctAnswerIndex == null ? buttonsText[i] : $"{correctAnswerIndex},{i},{questionIndex}")};

            }
            return new InlineKeyboardMarkup(buttons);
        }

        public InlineKeyboardMarkup GetInlineTicketsKeyboard(List<string> buttonText, int? ticketIndex = null)
        {
            var buttons = new InlineKeyboardButton[buttonText.Count][];
            for (int i = 0; i < buttonText.Count; i++)
            {
                buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(
                    text: buttonText[i],
                    callbackData: ticketIndex == null ? i.ToString() : ticketIndex.ToString()!)};
            }
            return new InlineKeyboardMarkup(buttons);
        }

        public InlineKeyboardMarkup GetInlineKeyboard(List<string> buttonText, int correctAnswerIndex, int questionIndex, int ticketIndex)
        {
            var buttons = new InlineKeyboardButton[buttonText.Count][];
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new InlineKeyboardButton[] {InlineKeyboardButton.WithCallbackData(
                    text: buttonText[i],
                    callbackData: $"{correctAnswerIndex},{i},{questionIndex},{ticketIndex}")};
            }
            return new InlineKeyboardMarkup(buttons);
        }

        public ReplyKeyboardMarkup GetKeyboard(List<string> buttonsText)
        {
            var buttons = new KeyboardButton[buttonsText.Count][];
            for (int i = 0; i < buttonsText.Count; i++)
            {
                buttons[i] = new KeyboardButton[] { new (buttonsText[i]) };
            }
            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true};
        }
    }
}
