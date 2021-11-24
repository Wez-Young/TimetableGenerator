using System.Collections.Generic;

namespace TimetableGenerator.GA
{
    public class Gene
    {
        //Properties
        public int Event { get; }
        public List<int> Students { get; }

        //Constructors
        public Gene() { }

        public Gene(int examEvent, List<int> students)
        {
            Event = examEvent;
            Students = students;
        }
    }
}
