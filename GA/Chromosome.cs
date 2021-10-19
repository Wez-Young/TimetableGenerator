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
        
        public Chromosome(int length)
        {
            Genes = new List<Gene>();
            GenerateChromosome(length);
        }
        
        //Methods
        private void GenerateChromosome(int length)
        {
            for(int i = 0; i < length; i++)
            {
                Gene gene = new Gene(/*Add value in here later*/);
                Genes.Add(gene);
            }
        }
    }
}
