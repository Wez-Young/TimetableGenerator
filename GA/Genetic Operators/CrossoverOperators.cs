using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableGenerator.GA.Genetic_Operators
{
    public class CrossoverOperators
    {
        public static Chromosome PartiallyMapped(Chromosome p1, Chromosome p2)
        {
            //Assign values to represent the 'cutpoints'(indexes)
            int cutpointOne = Settings.rand.Next(p1.Genes.Count);
            int cutpointTwo = Settings.rand.Next(p1.Genes.Count);

            //Stops from an empty child being made when both cutpoints are the same value (can change to split the gene assignment in half?)
            while(cutpointOne == cutpointTwo)
            {
                cutpointTwo = Settings.rand.Next(p1.Genes.Count);
            }
            //Create new child
            Chromosome child = new();
            //Create gene list with x empty slots
            List<Gene> tempGenes = new(new Gene[p1.Genes.Count]);

            //check which cupoint is lower and start with that one
            if (cutpointOne < cutpointTwo)
            {
                //Loop through the second parent and place genes up until the index of the first cutpoint
                for (int i = 0; i < tempGenes.Count; i++)
                {
                    tempGenes[i] = p2.Genes[i];
                    if (i == cutpointOne)
                    {
                        //Loop through the first parent from after the first cutpoint up until the second one
                        for (int j = cutpointOne + 1; j < cutpointTwo; j++)
                        {
                            tempGenes[j] = p1.Genes[j];
                        }
                    }
                    //Loop through the rest of the second parent and assign the final genes
                    else if (i >= cutpointTwo)
                    {
                        tempGenes[i] = p2.Genes[i];
                    }
                }

            }
            else if (cutpointTwo < cutpointOne)
            {
                for (int i = 0; i < tempGenes.Count; i++)
                {
                    tempGenes[i] = p2.Genes[i];
                    if (i == cutpointTwo)
                    {
                        for (int j = cutpointTwo + 1; j < cutpointOne; j++)
                        {
                            tempGenes[j] = p1.Genes[j];
                        }
                    }
                    else if (i >= cutpointOne)
                    {
                        tempGenes[i] = p2.Genes[i];
                    }
                }
            }

            /*
                * Check for duplicate exams within the chromosome
                List<int> examIDList = new();
                foreach (var exam in tempGenes)
                {
                    examIDList.Add(exam.Event);
                }
                Console.WriteLine(examIDList.Distinct().Count());
            */

            //Assign new child the main timeslots from the first parent
            child.Timeslots = new(p1.Timeslots);
            //Assign new child the reserve timeslots from the second parent
            child.ReserveTimeslots = new(p2.ReserveTimeslots);
            //Assign the new genes produced from both parents
            child.Genes = new(tempGenes);

            return child;
        }


    }
}
