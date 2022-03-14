using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableGenerator.GA;
using TimetableGenerator.GA.Genetic_Operators;

namespace TimetableGenerator
{
    class HillClimber
    {
        public static void StartHillClimber(Chromosome solution)
        {
            Stopwatch timer = new();
            CheckSoftConstraintFitness(solution);
            timer.Start();
            while (timer.Elapsed.Minutes < 5)
            {
                Chromosome solutionCopy = new(solution);
                MutationOperators.BlindMutateTimeslots(solutionCopy);
                CheckSoftConstraintFitness(solutionCopy);
                if (solutionCopy.SoftConstraintFitness < solution.SoftConstraintFitness)
                    solution = new(solutionCopy);

                Console.WriteLine($"Best Fitness: {solution.SoftConstraintFitness}");
            }
            IO.WriteData(Settings.directory, solution, timer);
            timer.Stop();

        }

        private static void CheckSoftConstraintFitness(Chromosome solution)
        {
            HashSet<int> students = new();
            double fitness = 0;
            foreach (var item in Settings.examStudentList)
                students.UnionWith(item.Value);
            for (int i = 0; i < solution.Timeslots.Count; i++)
            {
                //change to int j = chromosome[i] + 1 for proper fitness
                //j is timeslots
                for (int j = solution.Timeslots[i] + 1; j <= solution.Timeslots[i] + 5; j++)
                {
                    if (j > Settings.maxTimeslot)
                        break;

                    List<int> examsAtTimeSlotJ = new();
                    for (int k = 0; k < solution.Timeslots.Count; k++)
                        //dont add exam i under consideration to this list
                        if (solution.Timeslots[k] == j && k != i)
                            examsAtTimeSlotJ.Add(k + 1);

                    List<int> allStudents = new();
                    //students in exam under consideration
                    allStudents.AddRange(new List<int>(Settings.examStudentList[i + 1]));
                    //students in exam in neighbouring timeslot
                    foreach (var exam in examsAtTimeSlotJ)
                        allStudents.AddRange(new List<int>(Settings.examStudentList[exam]));

                    HashSet<int> uniqueStudents = new(allStudents);
                    int conflicts = allStudents.Count - uniqueStudents.Count;
                    int pow = 5 - (j - solution.Timeslots[i]);
                    fitness += Math.Pow(2, pow) * conflicts;
                }
            }
            if (solution.SoftConstraintFitness == 0)
                solution.OriginalSoftConstraintFitness = fitness / students.Count;

            solution.SoftConstraintFitness = fitness / students.Count;
        }
    }
}
