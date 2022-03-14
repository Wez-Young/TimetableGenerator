using System;
using System.Collections.Generic;
using System.Linq;

namespace TimetableGenerator.GA.Genetic_Operators
{
    public class SelectionOperators
    {

        public static void ElitismSelection(Population population, int popSize)
        {
            Population elites = new();
            //Get the x top chromsomes based on the fitness
            decimal percentage = popSize / 100m;
            elites.Chromosomes = population.Chromosomes.OrderBy(x => x.HardConstraintFitness).Take(Convert.ToInt32(percentage * Settings.elitismPercentage)).ToList();
            //Remove the 'elite' chromosomes from the population to avoid duplication
            population.Chromosomes.RemoveAll(x => elites.Chromosomes.Contains(x));
            //Select another x num of chromosomes to go into the next generation
            SelectionOperators.RouletteWheelSelection(population, "selection");
            //Add the elites back into the population
            population.Chromosomes.AddRange(elites.Chromosomes);
        }

        public static void TournamentSelection(Population pop)
        {
            //Create a copy of a random chromosome within the population
            Chromosome winner = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];

            //Iterate until value equals touranament size
            for (int i = 1; i < Settings.tournamentSize; ++i)
            {
                //Create copy of chromosome from population
                Chromosome candidate = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];

                //Check that the candidate is not the same individual as the winner
                while (winner.Equals(candidate))
                    candidate = new(pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)]);

                //If the fitness value of the candidate is lower then set winner to be the candidate
                if (candidate.HardConstraintFitness < winner.HardConstraintFitness)
                {
                    pop.Chromosomes.Remove(winner);
                    return;
                }

                pop.Chromosomes.Remove(candidate);
            }
        }

        public static Population RandomSelection(Population pop)
        {
            //Choose random chromosomes
            Chromosome winner = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];
            Chromosome competitor = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];

            if (winner.Equals(competitor))
                competitor = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];
            //Compare the fitness, if child has lower remove competitor and add child
            if (competitor.HardConstraintFitness > winner.HardConstraintFitness)
                pop.Chromosomes.Remove(competitor);
            else
                pop.Chromosomes.Remove(winner);
            return pop;
        }

        public static Chromosome RouletteWheelSelection(Population pop, string option)
        {
            //Get cumlative fitness
            List<double> cumalativeFitnesses = new(new double[pop.Chromosomes.Count]);
            cumalativeFitnesses[0] = pop.Chromosomes[0].HardConstraintFitness;

            //Add fitness of the next chromsome on top of the previous one and add it to list
            for (int i = 1; i < pop.Chromosomes.Count; i++)
                cumalativeFitnesses[i] = cumalativeFitnesses[i - 1] + pop.Chromosomes[i].HardConstraintFitness;

            switch (option)
            {
                case "survival":
                    //Create empty list with a size of the number of x surviors 
                    List<Chromosome> selection = new(new Chromosome[(pop.Chromosomes.Count + Settings.elitismPercentage) / Settings.elitismPercentage]);

                    for (int i = 0; i < selection.Count; i++)
                    {
                        //();
                        Chromosome individual = SelectIndividual(pop, cumalativeFitnesses);
                        selection[i] = individual;
                    }
                    //Assign the selection list to the population
                    pop.Chromosomes = selection;
                    break;
                case "parent":

                    Chromosome winner = new(SelectIndividual(pop, cumalativeFitnesses));
                    for (int i = 1; i < Settings.tournamentSize; i++)
                    {
                        Chromosome candidate = new(SelectIndividual(pop, cumalativeFitnesses));

                        while (winner.Equals(candidate))
                            candidate = new(SelectIndividual(pop, cumalativeFitnesses));

                        if (candidate.HardConstraintFitness < winner.HardConstraintFitness)
                            winner = candidate;
                    }
                    return winner;

                case "selection":
                    Chromosome firstIndividual = SelectIndividual(pop, cumalativeFitnesses);
                    for (int i = 1; i < Settings.tournamentSize; i++)
                    {
                        Chromosome candidate = SelectIndividual(pop, cumalativeFitnesses);

                        while (firstIndividual.Equals(candidate))
                            candidate = SelectIndividual(pop, cumalativeFitnesses);

                        if (candidate.HardConstraintFitness >= firstIndividual.HardConstraintFitness)
                            pop.Chromosomes.Remove(candidate);
                        else
                            pop.Chromosomes.Remove(firstIndividual);
                    }
                    break;
            }

            return null;
        }

        private static Chromosome SelectIndividual(Population pop, List<double> cumalativeFitnesses)
        {
            //Generate a random fitness value
            double rndFitness = Settings.rand.NextDouble() * cumalativeFitnesses[^1];
            //Find the index of the culmative value that is equal to the random fitness value
            int index = cumalativeFitnesses.BinarySearch(rndFitness);

            //Change index to a positive number
            if (index < 0)
                index = Math.Abs(index + 1);

            return pop.Chromosomes[index];
        }
    }
}
