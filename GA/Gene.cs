namespace TimetableGenerator.GA
{
    public class Gene
    {
        //Properties
        private int _Timeslot { get; }

        //Constructors
        public Gene() { }

        public Gene(int timeslot)
        {
            _Timeslot = timeslot;
        }
    }
}
