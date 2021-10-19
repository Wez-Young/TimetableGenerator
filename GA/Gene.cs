namespace TimetableGenerator.GA
{
    public class Gene
    {
        //Properties
        private int _Slot { get; }

        //Constructors
        public Gene() { }

        public Gene(int slot)
        {
            _Slot = slot;
        }
    }
}
