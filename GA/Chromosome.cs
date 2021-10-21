using System;
using System.Collections.Generic;

namespace TimetableGenerator.GA
{
    public class Chromosome
    {
        //Properties
        public List<Gene> Genes { get; }

        //Constructors
        public Chromosome() 
        {
            Genes = new List<Gene>();
        }
        
        public Chromosome(int length, int maxTimeslot, List<int> examIDs)
        {
            Genes = new List<Gene>();
            GenerateChromosome(length, maxTimeslot, examIDs);
        }
        
        //Methods
        //Creates a chromosome filled with 'genes'
        private void GenerateChromosome(int length, int maxTimeSlot, List<int> examIDs)
        {
            //Adds x amount of genes based on specified length
            for(int i = 0; i < length; i++)
            {
                //Gets random index
                int index = Settings.rand.Next(examIDs.Count);
                //Assigns the value associated to the index to the gene
                Gene gene = new Gene(examIDs[index]);
                Genes.Add(gene);
                //Removes the item from list to avoid duplicates
                examIDs.Remove(gene.Event);
            }
        }
    }
}
