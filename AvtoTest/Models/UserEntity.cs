using AvtoTest.Enums;
using System;
using System.Collections.Generic;
 
namespace AvtoTest.Models
{
    public class UserEntity
    {
        public long ChatId { get; set; }
        public string Name { get; set; }
        public EUserStep Step { get; set; }
        public int OldMessageId { get; set; }

        public UserEntity (long chatId, string name)
        {
            ChatId = chatId;
            Name = name;
            Step = 0;
        }

        public void SetStep(EUserStep step)
        {
            Step = step;
        }
    }                                                                                          
}
