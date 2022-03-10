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
            string action = string.Empty;
            Console.WriteLine("Would you like to generate a solution or improve one? (type ga or hill climber)");

            switch()
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

                Population population = new(popSize, examStudentList, conflictMatrix, conflictTracker);
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

                    if (population.BestFitness().HardConstraintFitness == 0)
                    {
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
