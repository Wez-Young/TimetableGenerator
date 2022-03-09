using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

using TimetableGenerator.GA;
using System.Runtime.CompilerServices;
using TimetableGenerator.GA.Genetic_Operators;

namespace TimetableGenerator
{
    class Program
    {
        private static Dictionary<int, List<int>> examStudentList = new();
        private static int count;

        static void Main()
        {
            /*
            examStudentList = new(new SortedDictionary<int, List<int>>(IO.ReadFile()));
            Chromosome solution = new Chromosome(new List<int> { 21, 33, 5, 15, 4, 17, 14, 30, 18, 8, 23, 34, 0, 28, 0, 8, 13, 20, 9, 9, 27, 8, 30, 5,
                13, 26, 23, 11, 5, 30, 32, 21, 22, 1, 33, 3, 18, 10, 14, 13, 21, 27, 21, 12, 13, 21, 13, 22, 32, 25, 18,
                23, 22, 4, 23, 9, 18, 30, 31, 15, 23, 25, 34, 7, 25, 1, 23, 3, 19, 10, 18, 17, 12, 19, 18, 5, 0, 9, 20,
                24, 3, 3, 34, 24, 29, 16, 12, 22, 28, 10, 32, 33, 27, 26, 26, 10, 34, 27, 6, 9, 25, 19, 18, 33, 3, 8, 4,
                10, 33, 27, 11, 14, 14, 4, 4, 12, 0, 28, 30, 4, 19, 11, 7, 20, 16, 1, 32, 34, 25, 32, 13, 7, 19, 2, 3,
                34, 30, 0, 1, 8, 31, 30, 25, 10, 0, 25, 5, 30, 30, 31, 31, 4, 30, 2, 30, 32, 25, 26, 11, 1, 0, 8, 29,
                21, 32, 15, 18, 22, 4, 10, 31, 25, 6, 2, 13, 29, 24, 11, 34, 17, 2, 33, 8, 27, 23, 33, 16, 20, 32, 7, 0,
                13, 18, 19, 12, 17, 20, 30, 0, 34, 23, 11, 2, 19, 33, 13, 27, 23, 7, 10, 25, 6, 22, 27, 14, 34, 32, 19,
                2, 12, 28, 0, 29, 3, 18, 24, 30, 5, 0, 10, 4, 12, 27, 0, 34, 5, 18, 6, 12, 19, 10, 14, 23, 34, 20, 33,
                12, 0, 14, 7, 29, 12, 23, 4, 30, 30, 23, 12, 0, 10, 25, 4, 13, 13, 12, 6, 6, 15, 34, 8, 29, 9, 13, 30,
                30, 21, 1, 11, 28, 34, 7, 6, 33, 21, 13, 25, 24, 12, 20, 0, 16, 0, 31, 30, 0, 12, 25, 21, 20, 20, 5, 3,
                2, 21, 19, 19, 19, 17, 20, 18, 18, 18, 23, 24, 10, 30, 9, 1, 34, 14, 34, 17, 19, 1, 8, 9, 25, 28, 28, 5,
                10, 30, 19, 24, 22, 28, 29, 10, 4, 31, 33, 4, 28, 34, 25, 18, 11, 0, 17, 16, 16, 16, 16, 16, 16, 16, 16,
                15, 28, 30, 17, 1, 29, 28, 9, 9, 9, 34, 15, 13, 15, 6, 33, 27, 22, 12, 22, 9, 32, 18, 0, 24, 18, 2, 34,
                16, 8, 29, 11, 16, 25, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 2, 0, 1, 1, 8, 9, 11, 29, 4, 24, 23, 25, 32, 32,
                32, 15, 17, 25, 11, 26, 1, 34, 29, 10, 16, 21, 12, 20, 0, 11, 28, 6, 34, 17, 18, 8, 5, 22, 6, 2, 0, 7,
                7, 7, 7, 7, 20, 20, 19, 11, 21, 19, 12, 26, 33, 3, 33, 33, 33, 24, 24, 24, 25, 3, 8, 32, 34, 26, 17, 18,
                29, 1, 34, 22, 12, 6, 10, 5, 16, 4, 21, 34, 34, 34, 34, 34, 1, 1, 2, 30, 1, 10, 10, 11, 27, 22, 5, 20,
                5, 5, 0, 15, 31, 17, 11, 28, 30, 26, 13, 19, 13, 34, 25, 27, 7, 0, 25, 22, 14, 19, 6, 24, 2, 29, 7, 8,
                28, 14, 34, 4, 29, 17, 20, 5, 11, 16, 33, 34, 34, 33, 31, 31, 16, 33, 1, 27, 34, 5, 17, 31, 18, 29, 19,
                13, 19, 9, 34, 30, 25, 13, 17, 32, 26, 25, 26, 27, 7, 33, 27, 15, 23, 25, 18, 5, 11, 28, 18, 24, 8, 22,
                34, 25, 34, 0, 15, 33, 15, 28, 9, 0, 34, 6, 0, 6, 12, 17, 11, 10, 10, 3, 23, 15, 9, 20, 29, 6, 23, 34,
                18, 7, 5, 0, 24, 13, 30, 6, 29, 11, 27, 23, 9, 10, 27, 21, 4, 29, 26, 17, 23, 25, 8, 32, 14, 0, 34, 3,
                4, 28, 7, 2, 17, 16, 25, 10, 1, 16, 13, 27, 17, 24, 0, 32, 17, 15, 23, 32, 23, 31, 21, 2, 4, 2, 34, 28,
                12, 2, 20, 7, 27, 11, 30, 24, 21, 13, 13, 0, 19, 10, 23, 6, 16});

            CheckSoftConstraintFitness(solution);

            Console.WriteLine($"Best Fitness: {solution.SoftConstraintFitness}");
            */
            int popSize = 50;
            examStudentList = new(new SortedDictionary<int, List<int>>(IO.ReadFile()));
            for (int test = 0; test < Settings.testNum; test++)
            {
                count = 0;
                int gen = 0;
                Stopwatch timer = new();
                Dictionary<int, int> conflictTracker = new();

                int[,] conflictMatrix = new int[examStudentList.Count, examStudentList.Count];
                CreateConflictMatrix(conflictMatrix, conflictTracker);

                Population population = new(popSize, Settings.maxTimeslot, examStudentList, conflictMatrix, conflictTracker);
                population.Chromosomes.ForEach(ch => ch.HardConstraintFitness = CheckHardConstraintFitness(conflictMatrix, ch));

                bool run = true;
                timer.Start();
                while (run)
                {
                    gen++;
                    Console.WriteLine($" Generation: {gen} Best Fitness: {population.BestFitness().HardConstraintFitness} No. unplaced exams: {Math.Round(population.BestFitness().HardConstraintFitness * examStudentList.Count)}\n Time Elapsed: {timer.Elapsed}\tNo. calculated fitness: {count}");
                    //Console.WriteLine($" Generation: {gen} Worst Fitness: {population.WorstFitness().Fitness} No. unplaced exams: {Math.Round(population.WorstFitness().Fitness * examStudentList.Count)}");

                    SelectionOperators.ElitismSelection(population, popSize);

                    List<Chromosome> children = new();
                    //Create the rest of the population through children of the surviving population
                    for (int i = population.Chromosomes.Count; population.Chromosomes.Count + children.Count < popSize; i++)
                    {
                        //Create child based on two randomly selected chromosomes using a Crossover method
                        CrossoverOperators.PartiallyMappedCrossover(children, SelectionOperators.RouletteWheelSelection(population, "parent"), SelectionOperators.RouletteWheelSelection(population, "parent"));
                        //CrossoverOperators.OrderedCrossover(children, new Chromosome(RouletteWheelSelection(population, "parent")), new Chromosome(RouletteWheelSelection(population, "parent")));

                        //CrossoverOperators.PartiallyMappedCrossover(children, SelectionOperators.TournamentSelection(population), SelectionOperators.TournamentSelection(population));
                        //CrossoverOperators.OrderedCrossover(children, TournamentSelection(population), TournamentSelection(population));

                        children.ForEach(child => MutationOperators.BlindMutate(child));
                        //children.ForEach(child => MutationOperators.ScrambleMutate(child));
                        children.ForEach(child => { child.HardConstraintFitness = CheckHardConstraintFitness(conflictMatrix, child); });

                        List<Chromosome> removeDupesList = new();
                        children.ForEach(ch =>
                        {
                            var check = population.CheckPermutationExists(population.Chromosomes, ch);
                            if (check == null)
                            {
                                check = population.CheckPermutationExists(children, ch);
                                if (check != null)
                                    removeDupesList.Add(ch);
                            }
                            else
                                removeDupesList.Add(ch);
                        });
                        if (removeDupesList.Count > 0)
                            children.RemoveAll(child => removeDupesList.Contains(child));
                    }
                    //Add newly created children to the population
                    population.Chromosomes.AddRange(children);

                    while (population.Chromosomes.Count > popSize)
                        SelectionOperators.RouletteWheelSelection(population, "selection");
                    //RandomSelection(population);

                    if (population.BestFitness().HardConstraintFitness == 0 || timer.Elapsed.Minutes == 5)
                    {
                        while (timer.Elapsed.Minutes <= 4)
                        {
                            CheckSoftConstraintFitness(population.BestFitness());
                            Chromosome solutionCopy = new(population.BestFitness());
                            MutationOperators.BlindMutateTimeslots(solutionCopy);
                            CheckSoftConstraintFitness(solutionCopy);
                            if (solutionCopy.SoftConstraintFitness < population.BestFitness().SoftConstraintFitness)
                                population.Chromosomes[population.Chromosomes.IndexOf(population.BestFitness())] = new(solutionCopy);

                            Console.WriteLine($"Best Fitness: {population.BestFitness().SoftConstraintFitness}");
                        }
                        run = false;
                        timer.Stop();
                    }
                }
                population.BestFitness().ExamIDs = population.BestFitness().ExamIDs.OrderBy(x => x).ToList();
                IO.PrintInfo(population, gen, timer, examStudentList);
                IO.WriteFiles(population.BestFitness(), timer, gen, test, count);
            }
        }
        //Initialises the exam conflicts matrix
        private static void CreateConflictMatrix(int[,] matrix, Dictionary<int, int> conflictTracker)
        {
            foreach (var firstExam in examStudentList)//Start with one exam
            {
                //Add exam to tracker with initial conflicts of 0
                conflictTracker.Add(firstExam.Key, 0);
                foreach (var nextExam in examStudentList)//Check against every other exam
                {
                    int conflicts = FindConflicts(firstExam, nextExam);
                    matrix[firstExam.Key - 1, nextExam.Key - 1] = conflicts;//Add the number of conflicts to the matrix

                    if (conflicts != 0 && conflictTracker.ContainsKey(firstExam.Key))
                    {
                        int examConflicts = conflictTracker[firstExam.Key];
                        conflictTracker[firstExam.Key] = examConflicts += 1;
                    }

                }
            }
        }
        //Check the number of conflicts between two exams
        private static int FindConflicts(KeyValuePair<int, List<int>> firstExam, KeyValuePair<int, List<int>> nextExam)
        {
            List<int> allStudents = new(firstExam.Value);
            //Add students from both exams into one list
            allStudents.AddRange(nextExam.Value);
            //return the number of duplicate student IDs within the list
            return allStudents.Count - allStudents.Distinct().Count();
        }
        //Calculate the fitness of a solution within the population
        private static double CheckHardConstraintFitness(int[,] conflictMatrix, Chromosome ch)
        {
            count++;
            double fitness = 0;
            Dictionary<int, List<int>> timetable = new();
            //Decodes the chromosome into a timetable
            CreateTimetable(timetable, conflictMatrix, ch);
            //Calculate fitness based onthe number of unplaced exams divided by the total number of exams e.g., 33/190 = fitness
            fitness = Math.Round((double)(ch.ExamIDs.Count - timetable.Values.Sum(list => list.Count)) / ch.ExamIDs.Count, 4);
            return fitness;
        }
        //Places exams into a 'timetable' based on their given timeslots
        private static void CreateTimetable(Dictionary<int, List<int>> timetable, int[,] conflictMatrix, Chromosome ch)
        {
            //Populate timetable with timeslot number (ID) and an empty assigned exam list
            for (int i = 1; i <= Settings.maxTimeslot; i++)
                timetable.Add(i, new());

            AssignMainTimeslot(timetable, conflictMatrix, ch);

            ch.Timetable = new(timetable);
        }
        //Attempt to assign exam to its main timeslot
        private static void AssignMainTimeslot(Dictionary<int, List<int>> timetable, int[,] conflictMatrix, Chromosome ch)
        {
            int index = -1;
            //Go through the permutation of exams
            foreach (var exam in ch.ExamIDs)
            {
                index++;
                List<int> visitedTimeslots = new();
                //Get the next timeslot in the permutation at the same index as the exam
                int currentTimeslot = ch.Timeslots[index];
                visitedTimeslots.Add(currentTimeslot);
                //Create a copy of the current assigned exams in the timeslot
                List<int> assignedExams = new(timetable[currentTimeslot]);
                //Add exam to list in timeslot if there are no other assigned exams
                if (assignedExams.Count == 0)
                {
                    assignedExams.Add(exam);
                    timetable[currentTimeslot] = assignedExams;
                }
                else
                {
                    List<int> conflicts = new();
                    //Check the conflicts between the current exam and the assigned exams
                    foreach (var assigned in assignedExams)
                    {
                        //Do nothing if there is no conflict
                        if (conflictMatrix[exam - 1, assigned - 1] == 0)
                            continue;
                        else
                            //Add the conflicting assigned exam to the conflicts list
                            conflicts.Add(assigned);
                    }

                    //Do an action based on the number of conflicts, 0, 1, 1<X
                    switch (conflicts.Count)
                    {
                        case 0://Assign the current exam as there is no conflicts
                            assignedExams.Add(exam);
                            timetable[currentTimeslot] = new(assignedExams);
                            break;

                        default:
                            //If the number of conflicting exams is greater than one, directly attempt to assign exam to back up timeslot 
                            AssignBackup(timetable, conflictMatrix, ch, exam, visitedTimeslots);
                            break;
                    }
                }
            }
        }
        //Attempt to assign exam to its reserve timeslot
        private static void AssignBackup(Dictionary<int, List<int>> timetable, int[,] conflictMatrix, Chromosome ch, int examID, List<int> visitedTimeslots)
        {
            int index = ch.ExamIDs.FindIndex(id => id == examID);

            //Get the next timeslot in the permutation
            int currentTimeslot = ch.ReserveTimeslots[index];
            visitedTimeslots.Add(currentTimeslot);
            //Create a copy of the current assigned exams in the timeslot
            List<int> assignedExams = new(timetable[currentTimeslot]);
            if (!visitedTimeslots.Contains(currentTimeslot))
            {
                //Add exam to list in timeslot if there are no other assigned exams
                if (timetable[currentTimeslot].Count == 0)
                {
                    assignedExams.Add(examID);
                    timetable[currentTimeslot] = assignedExams;
                    ch.Timeslots[ch.ExamIDs.IndexOf(examID)] = currentTimeslot;
                }
                else
                {

                    List<int> conflicts = new();

                    foreach (var assigned in assignedExams)
                    {
                        //Stop infinite loop bug
                        if (examID == assigned)
                            return;

                        //Do nothing if there is no conflicts
                        if (conflictMatrix[examID - 1, assigned - 1] != 0)
                            conflicts.Add(assigned);
                    }

                    switch (conflicts.Count)
                    {
                        case 0:
                            assignedExams.Add(examID);
                            timetable[currentTimeslot] = new(assignedExams);
                            break;

                        default:
                            TryAllTimeslots(timetable, conflictMatrix, ch, examID, visitedTimeslots);
                            break;
                    }
                }
            }
            else
                TryAllTimeslots(timetable, conflictMatrix, ch, examID, visitedTimeslots);
        }
        //Tries to place exam in all remaining timeslots
        private static void TryAllTimeslots(Dictionary<int, List<int>> timetable, int[,] conflictMatrix, Chromosome ch, int examID, List<int> visitedTimeslots)
        {
            int currentTimeslot = 1;

            while (currentTimeslot <= Settings.maxTimeslot)
            {
                while (visitedTimeslots.Count < Settings.maxTimeslot && visitedTimeslots.Contains(currentTimeslot))
                    currentTimeslot++;

                if (visitedTimeslots.Count >= Settings.maxTimeslot || currentTimeslot > Settings.maxTimeslot)
                    return;

                List<int> assignedExams = new(timetable[currentTimeslot]);
                List<int> conflicts = new();

                foreach (var assigned in assignedExams)
                {
                    //Stop infinite loop bug
                    if (examID == assigned)
                        return;

                    //Do nothing if there is no conflicts
                    if (conflictMatrix[examID - 1, assigned - 1] != 0)
                        conflicts.Add(assigned);
                }

                if (conflicts.Count == 0)
                {
                    timetable[currentTimeslot].Add(examID);
                    ch.Timeslots[ch.ExamIDs.IndexOf(examID)] = currentTimeslot;
                    return;
                }
                else
                    currentTimeslot++;
            }
        }

        private static void CheckSoftConstraintFitness(Chromosome solution)
        {
            HashSet<int> students = new();
            double fitness = 0;
            foreach (var item in examStudentList)
                students.UnionWith(item.Value);
            for (int i = 0; i < solution.Timeslots.Count; i++)
            {
                //change to int j = chromosome[i] + 1 for proper fitness
                //j is timeslots
                for (int j = solution.Timeslots[i]+1; j <= solution.Timeslots[i] + 5; j++)
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
                    allStudents.AddRange(new List<int>(examStudentList[i + 1]));
                    //students in exam in neighbouring timeslot
                    foreach (var exam in examsAtTimeSlotJ)
                        allStudents.AddRange(new List<int>(examStudentList[exam]));

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
