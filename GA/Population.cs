using System;
using System.Collections.Generic;
using System.Linq;

namespace TimetableGenerator.GA
{
    public class Population
    {
        //Properties
        public List<Chromosome> Chromosomes { get; set; }

        //Constructors
        public Population() 
        {
            Chromosomes = new ();
        }

        public Population(Population pop)
        {
            Chromosomes = new(pop.Chromosomes);
        }

        public Population(int popSize, int maxTimeSlot, Dictionary<int, List<int>> exams, int[,] conflictMatrix, Dictionary<int, int> conflictTracker)
        {
            Chromosomes = new ();
            GenerateRandomInitialPopulation(popSize, exams);
            //GenerateHeuristicInitialPopulation(popSize, exams, conflictMatrix, conflictTracker);
            
        }

        //Methods
        //Creates the initial population
        private void GenerateRandomInitialPopulation(int populationSize, Dictionary<int, List<int>> exams)
        {
            //Creates population equal to populationSize
            for(int i = 0; i < populationSize; i++)
            {
                Chromosome newChromosome = new (exams);

                if (CheckPermutationExists(Chromosomes, newChromosome) == null)
                    Chromosomes.Add(newChromosome);//Adds new chromsome to population if permutation does not already exist
                else
                    --i;//Retry making chromosome            
            }
        }

        private void GenerateHeuristicInitialPopulation(int popSize, Dictionary<int, List<int>> exams, int[,] conflictMatrix, Dictionary<int, int> conflictTracker)
        {
            for(int i = 0; i < popSize; i++)
            {
                Chromosome ch = new(exams, conflictMatrix, conflictTracker);

                if (CheckPermutationExists(Chromosomes, ch) == null)
                    Chromosomes.Add(ch);//Adds new chromsome to population if permutation does not already exist
                else
                    --i;
            }
        }

        public Chromosome CheckPermutationExists(List<Chromosome> chromosomeList, Chromosome chromosome)//Checks if new permutation already exists
        {
            Chromosome result = null;
            chromosomeList.ForEach(ch =>
            {
                if (chromosomeList.IndexOf(ch) != chromosomeList.IndexOf(chromosome) || !chromosomeList.Contains(chromosome))
                    if (!ch.ExamIDs.SequenceEqual(chromosome.ExamIDs)
                        || !ch.Timeslots.SequenceEqual(chromosome.Timeslots)
                        || !ch.ReserveTimeslots.SequenceEqual(chromosome.ReserveTimeslots))
                        result = null;
                    else
                        result = new(ch);
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
