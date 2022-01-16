using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableGenerator.GA.Genetic_Operators
{
    public class MutationOperators
    {

        public static Chromosome SwapMutate(Chromosome child)
        {
            int index1 = Settings.rand.Next(child.ExamIDs.Count);
            int index2 = Settings.rand.Next(child.ExamIDs.Count);

            while(index1 == index2)
                index2 = Settings.rand.Next(child.ExamIDs.Count);

            int tempGene = child.ExamIDs[index1];

            child.ExamIDs[index1] = child.ExamIDs[index2];
            child.ExamIDs[index2] = tempGene;

            return child;
        }

        public static Chromosome ReverseMutate(Chromosome child)
        {
            child.ExamIDs.Reverse();
            child.Timeslots.Reverse();
            child.ReserveTimeslots.Reverse();

            return child;
        }
    }
}
