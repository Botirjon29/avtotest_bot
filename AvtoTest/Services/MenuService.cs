
using AvtoTest.Enums;
using AvtoTest.Models;

namespace AvtoTest.Services
{
    public class MenuService
    {
        private readonly TelegramBotService _telegramBotService;
        private readonly ExaminationService _examinationService;
        private readonly TickesServices _tickesServices;

        public MenuService(TelegramBotService telegramBotService, ExaminationService examinationService, TickesServices tickesServices)
        {
            _telegramBotService = telegramBotService;
            _examinationService = examinationService;
            _tickesServices = tickesServices;
        }

        public void SendMenu(UserEntity user)
        {
            var menu = new List<string>() { "Tickets", "Examination" };
            _telegramBotService.SendMessage(user.ChatId, "Chosse menu : ", _telegramBotService.GetKeyboard(menu));
            user.SetStep(EUserStep.Menu);
        }

        public void TextFilter(UserEntity user, string message)
        {
            switch (message)
            {
                case "Tickets": _tickesServices.DisplayTickets(user); break;
                case "Examination": _examinationService.StartExam(user); break;
            }
        }

        public void ExamTextFilter(UserEntity user, string message)
        {
            switch (message)
            {
                case "Menu": SendMenu(user); break;
                case "Start":_examinationService.SendTicketQuestion(user); break;
            }
        }public void TicketFilterStarting(UserEntity user, string message, int messageId)
        {
            switch (message)
            {
                case "Menu": SendMenu(user); break;
                default: _tickesServices.StartTicket(user, message, messageId); break;
            }
        }
    }
}
