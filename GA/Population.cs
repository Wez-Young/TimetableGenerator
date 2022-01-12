using System;
using System.Collections.Generic;
using System.Linq;

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
                var newChromosome = new Chromosome(length, maxTimeSlot, exams);

                if (!CheckPermutationExists(newChromosome))
                {
                    Chromosomes.Add(newChromosome);//Adds new chromsome to population if permutation does not already exist
                    continue;
                }

                --i;//Retry making chromosome            
            }
        }

        private bool CheckPermutationExists(Chromosome chromosome)//Checks if new permutation already exists
        {
            if (Chromosomes.Any(ch => ch == chromosome))
                return true;

            return false;
        }

        public double WorstFitness()
        {
            return Chromosomes.OrderByDescending(ch => ch.Fitness).First().Fitness;
        }

        public double BestFitness()
        {
            return Chromosomes.OrderBy(ch => ch.Fitness).First().Fitness;
        }
    }
}
