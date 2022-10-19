

using AvtoTest.Models;
using AvtoTest.Options;

namespace AvtoTest.Databases
{
    public class TicketsDatabase
    {
        public Dictionary<long, List<TicketEntity>> Tickets { get; set; }

        public TicketsDatabase()
        {
            Tickets = new Dictionary<long, List<TicketEntity>>();
        }

        public List<TicketEntity> GetOrAddUserTickets(UserEntity user)
        {
            if (Tickets.ContainsKey(user.ChatId))
            {
                return Tickets[user.ChatId];
            }

            var newTickets = new List<TicketEntity>();
            Tickets.Add(user.ChatId, newTickets);
            return newTickets;
        }

        public int GetTicketsCount()
        {
            return Database.DB.QuestionsDb.Questions.Count / TicketsSettings.TicketQuestionsCount;
        }

        public void AddTicket(long chatId, TicketEntity indexTicket)
        {
            var userTickets = Tickets[chatId];
            if (userTickets.Any(t => t.IndexTicket == indexTicket.IndexTicket))
            {

            }
            userTickets.Add(indexTicket);
        }
    }
}
