using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TimetableGenerator.GA
{
    public class Chromosome
    {
        //Properties
        public List<Gene> Genes { get; set; }
        public List<int> Timeslots { get; set; }
        public List<int> ReserveTimeslots { get; set; }
        public double Fitness { get; set; }

        //Constructors
        public Chromosome() {}

        public Chromosome(Chromosome ch)//Make a copy of the chromosome
        {
            Genes = new List<Gene>(ch.Genes);
            Timeslots = new List<int>(ch.Timeslots);
            ReserveTimeslots = new List<int>(ch.ReserveTimeslots);
            Fitness = ch.Fitness;
        }
        
        public Chromosome(int maxTimeslot, Dictionary<int, List<int>> exams)
        {
            Genes = new List<Gene>();
            Timeslots = new List<int>();
            ReserveTimeslots = new List<int>();

            GenerateChromosome(exams);
            GenerateTimeslot(Timeslots, maxTimeslot);
            GenerateTimeslot(ReserveTimeslots, maxTimeslot);
        }
        
        //Methods
        private void GenerateTimeslot(List <int> timeslots, int maxTimeslot)
        {
            while(timeslots.Count < Genes.Count)
            {
                int timeslot = Settings.rand.Next(1, maxTimeslot + 1);
                timeslots.Add(timeslot);
            }
        }

        private void GenerateChromosome(Dictionary<int, List<int>> exams)
        {
            //Use list to ensure there are no duplicate exams placed in the chromosome
            List<int> examTracker = new();
            for(int i = 0; i < exams.Count(); i++)
            {
                int examID = Settings.rand.Next(1, exams.Count+1);

                if (!examTracker.Contains(examID))
                {
                    examTracker.Add(examID);
                    Gene g = new(examID);
                    Genes.Add(g);
                }

            }
        }
    }
}
