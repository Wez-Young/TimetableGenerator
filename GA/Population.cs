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

        public Population(int popSize, int chromSize)
        {
            Chromosomes = new List<Chromosome>();
            GenerateInitialPopulation(populationSize: popSize, chromosomeSize: chromSize);
        }

        //Methods
        private void GenerateInitialPopulation(int populationSize, int chromosomeSize)
        {
            for(int i = 0; i < populationSize; i++)
            {
                Chromosomes.Add(new Chromosome(chromosomeSize));
            }
        }
    }
}
