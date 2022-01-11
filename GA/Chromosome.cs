using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TimetableGenerator.GA
{
    public class Chromosome
    {
        //Properties
        public List<Gene> Genes { get; }
        public List<int> Timeslots { get; }
        public double Fitness { get; set; }

        //Constructors
        public Chromosome() {}

        public Chromosome(Chromosome ch)//Make a copy of the chromosome
        {
            Genes = new List<Gene>(ch.Genes);
            Timeslots = new List<int>(ch.Timeslots);
            Fitness = ch.Fitness;
        }
        
        public Chromosome(int length, int maxTimeslot, Dictionary<int, List<int>> exams)
        {
            Genes = new List<Gene>();
            Timeslots = new List<int>();

            GenerateChromosome(length, exams);
            GenerateTimeslot(maxTimeslot);
        }
        
        //Methods
        private void GenerateTimeslot(int maxTimeslot)
        {
            while(Timeslots.Count < maxTimeslot)
            {
                int timeslot = Settings.rand.Next(1, maxTimeslot + 1);
                if (!Timeslots.Contains(timeslot))
                    Timeslots.Add(timeslot);
            }
        }

        private void GenerateChromosome(int length, Dictionary<int, List<int>> exams)
        {
            var examIDs = new List<int>(exams.Keys);
            //Adds x amount of genes based on specified length
            for(int i = 0; i < length; i++)
            {
                //Gets random index
                int index = Settings.rand.Next(examIDs.Count);
                //Assigns the value associated to the index to the gene
                Gene gene = new Gene(examEvent: examIDs[index]);
                Genes.Add(gene);
                //Removes the item from list to avoid duplicates
                examIDs.Remove(gene.Event);
            }
        }
    }
}
