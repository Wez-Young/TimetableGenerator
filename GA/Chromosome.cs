using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TimetableGenerator.GA
{
    public class Chromosome
    {
        //Properties
        public List<int> ExamIDs { get; set; }
        public List<int> Timeslots { get; set; }
        public List<int> ReserveTimeslots { get; set; }
        public double Fitness { get; set; }

        //Constructors
        public Chromosome() {}

        public Chromosome(Chromosome ch)//Make a copy of the chromosome
        {
            ExamIDs = new (ch.ExamIDs);
            Timeslots = new (ch.Timeslots);
            ReserveTimeslots = new (ch.ReserveTimeslots);
            Fitness = ch.Fitness;
        }
        
        public Chromosome(int maxTimeslot, Dictionary<int, List<int>> exams)
        {
            ExamIDs = new ();
            Timeslots = new ();
            ReserveTimeslots = new ();

            GenerateChromosome(exams.Count);
            GenerateTimeslot(Timeslots, maxTimeslot);
            GenerateTimeslot(ReserveTimeslots, maxTimeslot);
        }
        
        //Methods
        private void GenerateTimeslot(List <int> timeslots, int maxTimeslot)
        {
            while(timeslots.Count < ExamIDs.Count)
            {
                int timeslot = Settings.rand.Next(1, maxTimeslot + 1);
                timeslots.Add(timeslot);
            }
        }

        private void GenerateChromosome(int examCount)
        {
            List<int> nums = Enumerable.Range(1, examCount).ToList();
            for(int i = 0; i < examCount; i++)
            {
                int examID = Settings.rand.Next(1, examCount);

                while(!nums.Contains(examID) && nums.Count > 1)
                    examID = Settings.rand.Next(1, examCount);

                if (nums.Count == 1)
                    examID = nums[0];

                ExamIDs.Add(examID);
                nums.Remove(examID);
            }
            Console.Read();
        }
    }
}
