
using AvtoTest.Databases;
using AvtoTest.Enums;
using AvtoTest.Models;
using AvtoTest.Options;
using Telegram.Bot.Types.ReplyMarkups;

namespace AvtoTest.Services
{
    public class ExaminationService
    {
        private readonly TelegramBotService _telegramBotService;
        private Dictionary<long, TicketEntity> _exams = new();
        public ExaminationService(TelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public void StartExam(UserEntity user)
        {
            var ticket = CreatTicket(user.ChatId);
            DisplayTicket(user, ticket);
        }

        private void DisplayTicket(UserEntity user, TicketEntity ticket)
        {
            var message = $"Exam Started \nQuestions Count : {ticket.QuestionsCount}";
            var buttons = _telegramBotService.GetInlineKeyboard(new List<string> { "Start" });
            _telegramBotService.SendMessage(user.ChatId, message, buttons);
            user.SetStep(EUserStep.Exam);
        }

        private TicketEntity CreatTicket(long chatId)
        {
            var ticket = new TicketEntity(chatId, CreatExamTicket());
            _exams.Add(chatId, ticket);
            return ticket;
        }

        public Queue<QuestionEntity> CreatExamTicket()
        {
            int randomNumber = new Random().Next(0, Database.DB.QuestionsDb.Questions.Count / TicketsSettings.TicketQuestionsCount);
            var questions = Database.DB.QuestionsDb.CreateTicket(randomNumber * TicketsSettings.TicketQuestionsCount, TicketsSettings.TicketQuestionsCount);
            return new Queue<QuestionEntity>(questions);
        }

        public void SendTicketQuestion(UserEntity user)
        {
            var ticket = _exams[user.ChatId];
            Queue<QuestionEntity> ticketQueue = ticket.Questions;
            if (ticketQueue == null || ticketQueue.Count == 0)
            {
                TicketFinished(user);
                return;
            }

            var question = ticketQueue.Dequeue();
            SendQuestion(user, question);
            user.SetStep(EUserStep.ExamStarted);
        }

        private void SendQuestion(UserEntity user, QuestionEntity question)
        {
            var choices = question.Choices!.Select(choice => choice.Text).ToList();
            var correctChoice = question.Choices!.First(choice => choice.Answer);
            var correctAnswerQuestion = question.Choices!.IndexOf(correctChoice);

            var ticket = _exams[user.ChatId];
            var questionIndex = $"[{ticket.CurrentQuestion}/{ticket.QuestionsCount}]";

            if (question.Media.Exisit)
            {
                _telegramBotService.SendMessage(
                    chatId: user.ChatId,
                    message: $"{questionIndex} {question.Question}",
                    image: Database.GetQuestionMedia(question.Media.Name),
                    reply: _telegramBotService.GetInlineKeyboard(choices, correctAnswerQuestion, ticket.CurrentQuestion));
            }
            _telegramBotService.SendMessage(
                chatId: user.ChatId,
                message: $"{questionIndex} {question.Question}",
                reply: _telegramBotService.GetInlineKeyboard(choices, correctAnswerQuestion, ticket.CurrentQuestion));
        }

        public void CheckAnswer(UserEntity user, string messsage, int messageId, InlineKeyboardMarkup reply)
        {
            int[] answer;
            try
            {
                answer = messsage.Split(',').Select(int.Parse).ToArray();
            }
            catch
            {
                return;
            }

            var answerResult = "❌";
            if (answer[0] == answer[1])
            {
                answerResult = "✅";
                var ticket = _exams[user.ChatId];
                ticket.CorrectAnswerCount++;
            }

            var _reply = reply.InlineKeyboard.ToArray();
            _reply[answer[1]].ToArray()[0].Text = answerResult;
            _telegramBotService.EditMessageButtons(user.ChatId, messageId, new InlineKeyboardMarkup(_reply));
            SendTicketQuestion(user);
        }

        public void TicketFinished(UserEntity user)
        {
            var ticket = _exams[user.ChatId];
            _telegramBotService.SendMessage(user.ChatId, $"Exam finished. \n Result: {ticket.CorrectAnswerCount}/{ticket.QuestionsCount}");
            _exams.Remove(user.ChatId);

            user.SetStep(EUserStep.Menu);
        }

    }
}
