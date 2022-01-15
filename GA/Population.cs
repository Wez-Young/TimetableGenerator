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

        public Population(int popSize, int maxTimeSlot, Dictionary<int, List<int>> exams)
        {
            Chromosomes = new List<Chromosome>();
            GenerateInitialPopulation(popSize, maxTimeSlot, exams);
        }

        //Methods
        //Creates the initial population
        private void GenerateInitialPopulation(int populationSize, int maxTimeSlot, Dictionary<int, List<int>> exams)
        {
            //Creates population equal to populationSize
            for(int i = 0; i < populationSize; i++)
            {
                var newChromosome = new Chromosome(maxTimeSlot, exams);

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
            bool result = false;
            Chromosomes.ForEach(ch =>
            {
                if (ch.ExamIDs.SequenceEqual(chromosome.ExamIDs))
                    result = true;

            });

            return result;
        }

        public Chromosome WorstFitness()
        {
            return Chromosomes.OrderByDescending(ch => ch.Fitness).First();
        }

        public Chromosome BestFitness()
        {
            return Chromosomes.OrderBy(ch => ch.Fitness).First();
        }
    }
}
