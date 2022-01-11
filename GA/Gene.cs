using System.Collections.Generic;

namespace TimetableGenerator.GA
{
    public class Gene
    {
        //Properties
        public int Event { get; }

        //Constructors
        public Gene() { }

        public Gene(int examEvent)
        {
            Event = examEvent;
        }
    }
}
