using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableGenerator.GA.Genetic_Operators
{
    public class CrossoverOperators
    {

        public static List<Chromosome> PartiallyMappedCrossover(Chromosome p1, Chromosome p2)
        {
            //create new children
            Chromosome c1 = new Chromosome(p1.ExamIDs.Count);
            Chromosome c2 = new Chromosome(p1.ExamIDs.Count);
            c1.Timeslots = new(p1.Timeslots);
            c2.Timeslots = new(p1.Timeslots);
            c1.ReserveTimeslots = new(p2.ReserveTimeslots);
            c2.ReserveTimeslots = new(p2.ReserveTimeslots);
            //select two index points
            int cutpointOne = Settings.rand.Next(p1.ExamIDs.Count);
            int cutpointTwo = Settings.rand.Next(cutpointOne + 1, p1.ExamIDs.Count);
            //initialise children genes with parent genes
            c1.ExamIDs = new(p1.ExamIDs);
            c2.ExamIDs = new(p2.ExamIDs);
            //Create dictionary to hold gene mapping
            Dictionary<int, int> map1 = new();
            Dictionary<int, int> map2 = new();

            //Change only between the cutpoints
            for (int i = cutpointOne; i < cutpointTwo; i++)
            {
                //child gene now equals parent gene at that index
                c1.ExamIDs[i] = p2.ExamIDs[i];
                //Add the gene mapping
                map1.Add(p2.ExamIDs[i], p1.ExamIDs[i]);

                c2.ExamIDs[i] = p1.ExamIDs[i];
                map2.Add(p1.ExamIDs[i], p2.ExamIDs[i]);
            }

            //Check for duplicate genes before the first cutpoint
            for (int i = 0; i < cutpointOne; i++)
            {
                //Check map contains a item with that gene
                while (map1.ContainsKey(c1.ExamIDs[i]))
                {
                    c1.ExamIDs[i] = map1[c1.ExamIDs[i]];
                }

                while (map2.ContainsKey(c2.ExamIDs[i]))
                {
                    c2.ExamIDs[i] = map2[c2.ExamIDs[i]];
                }
            }
            //Check for duplicate genes after the second cutpoint
            for (int i = cutpointTwo; i < p1.ExamIDs.Count; i++)
            {
                while (map1.ContainsKey(c1.ExamIDs[i]))
                {
                    c1.ExamIDs[i] = map1[c1.ExamIDs[i]];
                }

                while (map2.ContainsKey(c2.ExamIDs[i]))
                {
                    c2.ExamIDs[i] = map2[c2.ExamIDs[i]];
                }
            }

            CheckDupeGene(c1);
            CheckDupeGene(c2);

            return new List<Chromosome> {
                c1, c2
            };
        }

        public static Chromosome OrderedCrossover(Chromosome p1, Chromosome p2)
        {
            //set cut points
            int cutpointOne = Settings.rand.Next(p1.ExamIDs.Count);
            int cutpointTwo = Settings.rand.Next(cutpointOne, p1.ExamIDs.Count);

            //initialise child
            Chromosome child = new(p1.ExamIDs.Count);
            child.Timeslots = new(p1.Timeslots);
            child.ReserveTimeslots = new(p2.ReserveTimeslots);

            //create copy of parent 2 genes
            List<int> tempGenes = new(p2.ExamIDs);

            for (int i = cutpointOne; i < cutpointTwo; i++)
            {
                //add parent one genes to child between the two cutpoints
                child.ExamIDs[i] = p1.ExamIDs[i];
                //Remove those genes from the copy of parent2 genes
                p2.ExamIDs.Remove(child.ExamIDs[i]);
            }
            //For each remaining gene
            for (int j = 0; j < tempGenes.Count; j++)
            {
                //Go through the child's entire gene list
                for (int i = 0; i < p1.ExamIDs.Count; i++)
                {
                    //if the gene is 'empty' (0) and does not already contain the same gene
                    if (child.ExamIDs[i] == 0 && !child.ExamIDs.Contains(tempGenes[j]))
                    {
                        //Add gene to child
                        child.ExamIDs[i] = tempGenes[j];
                        break;
                    }
                }

            }

            CheckDupeGene(child);

            return child;
        }

        //Check if children contain duplicate genes
        public static void CheckDupeGene(Chromosome child)
        {
            if (child.ExamIDs.Count - child.ExamIDs.Distinct().Count() > 0)
                Console.WriteLine("Duplicate genes!! Errors!!");

        }
    }
}
