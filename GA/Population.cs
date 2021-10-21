using System.Collections.Generic;

namespace TimetableGenerator.GA
{
    public class Population
    {
        //Properties
        public List<Chromosome> Chromosomes { get; }

        //Constructors
        public Population() 
        {
            Chromosomes = new List<Chromosome>();
        }

        public Population(int popSize, int length, int maxTimeSlot, Dictionary<int, List<int>> exams)
        {
            Chromosomes = new List<Chromosome>();
            GenerateInitialPopulation(popSize, length, maxTimeSlot, exams);
        }

        //Methods
        //Creates the initial population
        private void GenerateInitialPopulation(int populationSize, int length, int maxTimeSlot, Dictionary<int, List<int>> exams)
        {
            //Creates population equal to populationSize
            for(int i = 0; i < populationSize; i++)
            {
                //Adds new chromsome to population
                Chromosomes.Add(new Chromosome(length, maxTimeSlot, exams));
            }
        }
    }
}
