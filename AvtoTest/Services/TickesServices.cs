
using AvtoTest.Databases;
using AvtoTest.Enums;
using AvtoTest.Models;
using AvtoTest.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AvtoTest.Services
{
    public class TickesServices
    {
        private TelegramBotService _telegramBotService;
        public TickesServices(TelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }
        public void DisplayTickets(UserEntity user)
        {
            var ticketsCount = Database.DB.TicketDb.GetTicketsCount();
            var ticketsButtons = new List<string>();

            var tickets = Database.DB.TicketDb.GetOrAddUserTickets(user);

            for (var i = 0; i < ticketsCount; i++)
            {
                var ticketText = $"Ticket {i + 1}";
                if (tickets.Any(t => t.IndexTicket == i))
                {
                    var ticket = tickets.First(t => t.IndexTicket == i);
                    if (ticket.IsCompleted)
                    {
                        ticketText +=" ✅";
                    }
                    else
                    {
                        ticketText += $" - [{ticket.CorrectAnswerCount}/{ticket.QuestionsCount}]";
                    }
                }
                ticketsButtons.Add(ticketText);
            }
            _telegramBotService.SendMessage(chatId: user.ChatId,
                message: "Tickets list ",
                _telegramBotService.GetInlineTicketsKeyboard(ticketsButtons));
            user.SetStep(EUserStep.TicketList);
        }

        public void TextFilter(UserEntity user, string message, int messageId)
        {
            Database.DB.TicketDb.GetOrAddUserTickets(user);
            var from = int.Parse(message) * TicketsSettings.TicketQuestionsCount;
            var questions = Database.DB.QuestionsDb.CreateTicket(from, TicketsSettings.TicketQuestionsCount);

            var ticket = new TicketEntity(user.ChatId, int.Parse(message), new Queue<QuestionEntity>(questions));

            Database.DB.TicketDb.AddTicket(user.ChatId, ticket);

            DisplayTicket(user, ticket);

            user.SetStep(EUserStep.TicketStarting);
        }

        private void DisplayTicket(UserEntity user, TicketEntity ticket)
        {
            string message = $"Ticket : {ticket.IndexTicket + 1} \n Questions count : {ticket.QuestionsCount}";

            var buttons = _telegramBotService.GetInlineTicketsKeyboard(new List<string>() { "Start" }, ticket.IndexTicket);
            _telegramBotService.SendMessage(user.ChatId, message, buttons);
        }

        public void StartTicket(UserEntity user, string message, int messageId)
        {
            SendTicketQuestion(user, int.Parse(message));
        }

        private void SendTicketQuestion(UserEntity user, int ticketIndex)
        {
            var ticketQuestions = Database.DB.TicketDb.GetOrAddUserTickets(user);
        var ticket = ticketQuestions.First(t => t.IndexTicket == ticketIndex);

            if (ticket.Questions.Count == 0)
            {
                string message = $"Questions end. \n Result : [{ticket.CorrectAnswerCount}/{ticket.QuestionsCount}]";
                _telegramBotService.SendMessage(user.ChatId, message);
                user.SetStep(EUserStep.Menu);
                return;
            }

            var question = ticket.Questions.Dequeue();

            DisplayQuestion(user,question,ticket);
        }

        public void DisplayQuestion(UserEntity user, QuestionEntity question, TicketEntity ticket)
        {
            var choices = question.Choices.Select(choice => choice.Text).ToList();
            var correctChoice = question.Choices.First(choice => choice.Answer);
            var correctAnswerIndex = question.Choices.IndexOf(correctChoice);

            var questionIndex = $"[{ticket.CurrentQuestion}/{ticket.QuestionsCount}]";

            if (question.Media.Exisit)
            {
                _telegramBotService.SendMessage(
                    chatId: user.ChatId,
                    message: $"{questionIndex} {question.Question}",
                    image: Database.GetQuestionMedia(question.Media.Name),
                    reply: _telegramBotService.GetInlineKeyboard(choices, correctAnswerIndex, ticket.CurrentQuestion, ticket.IndexTicket));
                return;
            }

            _telegramBotService.SendMessage(
                chatId: user.ChatId,
                message: $"{questionIndex} {question.Question}",
                reply: _telegramBotService.GetInlineKeyboard(choices, correctAnswerIndex, ticket.CurrentQuestion, ticket.IndexTicket));
        }

        public void CheckAnswer(UserEntity user, string message, int messageId, InlineKeyboardMarkup reply)
        {
            int[] answer;
            try
            {
                answer = message.Split(',').Select(int.Parse).ToArray();
            }
            catch
            {
                return;
            }
            var answerResult = " ❌"; ;
            var ticketIndex = answer[3];

            if (answer[0] == answer[1])
            {
                var tickets = Database.DB.TicketDb.GetOrAddUserTickets(user);
                var ticket = tickets.First(t => t.IndexTicket == ticketIndex);
                ticket.CorrectAnswerCount++;
                answerResult = " ✅";
            }

            var _reply = reply.InlineKeyboard.ToArray();
            _reply[answer[1]].ToArray()[0].Text += answerResult;
            _telegramBotService.EditMessageButtons(user.ChatId, messageId, new InlineKeyboardMarkup(_reply));

            System.Threading.Thread.Sleep(1000);
            SendTicketQuestion(user, ticketIndex);
        }
    }
}
