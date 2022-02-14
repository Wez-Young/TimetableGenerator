using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableGenerator.GA.Genetic_Operators
{
    public class MutationOperators
    {

        public static void BlindMutate(Chromosome child)
        {
            if (Settings.rand.NextDouble() > Settings.mutationProbability)
                return;
            //Swap mutate the permutation of the exam IDs
            int index1 = Settings.rand.Next(child.ExamIDs.Count);
            int index2 = Settings.rand.Next(index1, child.ExamIDs.Count);

            int tempGene = child.ExamIDs[index1];

            child.ExamIDs[index1] = child.ExamIDs[index2];
            child.ExamIDs[index2] = tempGene;

            CheckDupeGene(child);
        }

        public static void ReverseMutate(Chromosome child)
        {
            child.ExamIDs.Reverse();
            //child.Timeslots.Reverse();
            //child.ReserveTimeslots.Reverse();

            CheckDupeGene(child);
        }

        public static void ScrambleMutate(Chromosome child)
        {
            if (Settings.rand.NextDouble() > Settings.mutationProbability)
                return;
            //get two points
            int index1 = Settings.rand.Next(child.ExamIDs.Count);
            int index2 = Settings.rand.Next(index1, child.ExamIDs.Count);

            //create list based on items between the two points
            List<int> subset = new();
            for (int i = index1; i < index2; i++)
                subset.Add(child.ExamIDs[i]);
            //randomise the order of the list
            subset = subset.OrderBy(examID => Settings.rand.Next(subset.Count)).ToList();
            //reinsert the list into the child at the same point it was taken
            for (int i = index1; i < index2; i++)
                child.ExamIDs[i] = subset[i - index1];

            CheckDupeGene(child);
        }

        public static void StealMutate(Chromosome child)
        {
            if (Settings.rand.NextDouble() > Settings.mutationProbability)
                return;
 
        }

        public static void CheckDupeGene(Chromosome child)
        {
            if (child.ExamIDs.Count - child.ExamIDs.Distinct().Count() > 0)
                Console.WriteLine("Duplicate genes!! Errors!!");

        }
    }
}
