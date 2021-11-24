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
        public Dictionary<int, int> ExamSizes { get; }

        //Constructors
        public Chromosome() {}
        
        public Chromosome(int length, int maxTimeslot, Dictionary<int, List<int>> exams)
        {
            Genes = new List<Gene>();
            Timeslots = new List<int>();
            ExamSizes = new Dictionary<int, int>();

            GenerateChromosome(length, maxTimeslot, exams);
            GenerateTimeslot(maxTimeslot);
            InitialiseExamSizes();
        }
        
        //Methods
        private void GenerateTimeslot(int maxTimeslot)
        {
            while(Timeslots.Count < maxTimeslot)
            {
                int timeslot = Settings.rand.Next(maxTimeslot);
                if (!Timeslots.Contains(timeslot))
                    Timeslots.Add(timeslot);
            }
        }

        private void InitialiseExamSizes()
        {
            for (int i = 1; i <= Genes.Count; i++)
            {
                ExamSizes.Add(Genes[i-1].Event, Genes[i-1].Students.Count);
            }
        }

        private void GenerateChromosome(int length, int maxTimeSlot, Dictionary<int, List<int>> exams)
        {
            var examIDs = new List<int>(exams.Keys);
            //Adds x amount of genes based on specified length
            for(int i = 0; i < length; i++)
            {
                //Gets random index
                int index = Settings.rand.Next(examIDs.Count);
                //Assigns the value associated to the index to the gene
                Gene gene = new Gene(examEvent: examIDs[index], students: exams[examIDs[index]]);
                Genes.Add(gene);
                //Removes the item from list to avoid duplicates
                examIDs.Remove(gene.Event);
            }
        }
    }
}
