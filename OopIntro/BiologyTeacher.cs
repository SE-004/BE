namespace OopIntro.School
{
    public class BiologyTeacher
    {
        public static string SchoolName = "Lakewood elementary";

        public string Name; // instance
        private string _subject = "Biology";

        // Constructor
        public BiologyTeacher(string teacherName)
        {
            Name = teacherName;
        }

        public void Teach()
        {
            string message = $"{Name} is teaching {_subject} at {SchoolName}";
            Console.WriteLine(message);
        }
    }
}
