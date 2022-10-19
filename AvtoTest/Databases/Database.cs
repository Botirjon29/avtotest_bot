using Newtonsoft.Json;
using AvtoTest.Models;

namespace AvtoTest.Databases
{
     public class Database
    {
        private const string UsersJsonPath = "JsonData/users.json";
        private const string QuestionsJsonPath = @"C:\\Users\\Admin\\OneDrive\\Рабочий стол\\Projects\\C#\\Darsliklar\\AvtoTest\\JsonData\\uzlotin.json";
        private const string ImagesPath = @"C:\\Users\\Admin\\OneDrive\\Рабочий стол\\Projects\\C#\\Darsliklar\\AvtoTest\\Images";


        private static Database _database;
        public static Database DB
        {
            get
            {
                if (_database == null)
                {
                    _database = new Database();
                }
                return _database;
            }
        }
            
        public   QuestionsDatabase QuestionsDb;
        public   UsersDatabase UsersDb;
        public   TicketsDatabase TicketDb;
        public Database()
        {
            QuestionsDb = new QuestionsDatabase(ReadQuestionsJson());
            UsersDb = new UsersDatabase(ReadUsersJson());
            TicketDb = new TicketsDatabase();
        }


        private  List<UserEntity> ReadUsersJson()
        {
            if (!File.Exists(UsersJsonPath)) return new List<UserEntity>();
            var json = File.ReadAllText(UsersJsonPath);
            try
            {
                return  JsonConvert.DeserializeObject <List<UserEntity>>(json)!;
            }
            catch  
            {
                Console.WriteLine("Canot deserialize users json.");
                return new List<UserEntity>();
            }
        }
        private   List<QuestionEntity> ReadQuestionsJson()
        {
            if (!File.Exists(QuestionsJsonPath)) return new List<QuestionEntity>();
            var json = File.ReadAllText(QuestionsJsonPath);
            try
            {
                return JsonConvert.DeserializeObject<List<QuestionEntity>>(json)!;
            }
            catch 
            {
                Console.WriteLine("Cannot deserialize questions json");
                return new List<QuestionEntity>();
            }
        }


        public void SaveUsersToJsonFile()
        {
            var json = JsonConvert.SerializeObject(UsersDb.Users);
            File.WriteAllText(UsersJsonPath,json);
        }
        public static Stream? GetQuestionMedia(string imageName)
        {
            var path = Path.Combine(ImagesPath, $"{imageName}.png");
            if (!File.Exists(path)) return null;

            var bytes = File.ReadAllBytes(path);
            return new MemoryStream(bytes);
        }

    }
}
