namespace AvtoTest.Models
{
    public class TicketEntity
    {
        public long ChatId { get; set; }
        public int QuestionsCount { get; set; }
        public int CorrectAnswerCount { get; set; }
        public Queue<QuestionEntity> Questions { get; set; }
        public int IndexTicket { get; set; }

        public int CurrentQuestion
        {
            get { return QuestionsCount - Questions.Count; }
        }

        public bool IsCompleted
        {
            get
            {
                return QuestionsCount == CorrectAnswerCount;
            }
        }

        public TicketEntity(long chatId, Queue<QuestionEntity> questions)
        {
            ChatId = chatId;
            QuestionsCount = questions.Count;
            CorrectAnswerCount = 0;
            Questions = questions;
        }

        public TicketEntity(long chatId, int indexTicket, Queue<QuestionEntity> questions)
        {
            ChatId = chatId;
            IndexTicket = indexTicket;
            Questions = questions;
            CorrectAnswerCount = 0;
            QuestionsCount = questions.Count;
        }
    }
}
