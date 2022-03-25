using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimetableGenerator.GA.Genetic_Operators;

namespace TimetableGenerator.GA
{
    public class GeneticAlgorithm
    {
       public static void StartGA()
        {
<<<<<<< HEAD
            List<int> popSizes = new() { 50, 100, 150 };
            List<double> mutationRates = new() { 0.25, 0.5, 0.75 };
            List<double> crossoverRates = new() { 0.25, 0.5, 0.75 };
            int testCount = 0;

            foreach (var popSize in popSizes)
            {
                foreach (var cRate in crossoverRates)
                {
                    Settings.crossoverProbability = cRate;
                    foreach (var mRate in mutationRates)
                    {
                        Settings.directory = Directory.CreateDirectory(@$"{AppDomain.CurrentDomain.BaseDirectory}/Solutions/P{popSize}/M{mRate}_C{cRate}");
                        Settings.mutationProbability = mRate;
                        testCount++;
                        int gen = 0;
                        Stopwatch timer = new();
                        Dictionary<int, int> conflictTracker = new();

                        int[,] conflictMatrix = new int[Settings.examStudentList.Count, Settings.examStudentList.Count];
                        CreateConflictMatrix(conflictMatrix, conflictTracker);

                        Population population = new(popSize, Settings.examStudentList, conflictMatrix, conflictTracker);
                        population.Chromosomes.ForEach(ch => { ch.HardConstraintFitness = CheckHardConstraintFitness(conflictMatrix, ch); Settings.fitnessEvalCount += 1; });

                        bool run = true;
                        timer.Start();
                        while (run)
                        {
                            gen++;
                            Console.WriteLine($" Generation: {gen} Best Fitness: {population.BestFitness().HardConstraintFitness} No. unplaced exams: {Math.Round(population.BestFitness().HardConstraintFitness * Settings.examStudentList.Count)}\n Time Elapsed: {timer.Elapsed}");
                            //Console.WriteLine($" Generation: {gen} Worst Fitness: {population.WorstFitness().Fitness} No. unplaced exams: {Math.Round(population.WorstFitness().Fitness * examStudentList.Count)}");
                            CheckForDuplicates(population, population.Chromosomes);

                            List<Chromosome> children = new();
                            //Create the rest of the population through children of the surviving population
                            for (int i = population.Chromosomes.Count; population.Chromosomes.Count + children.Count <= popSize; i++)
                            {
                                //Create child based on two randomly selected chromosomes using a Crossover method
                                CrossoverOperators.PartiallyMappedCrossover(children, SelectionOperators.RouletteWheelSelection(population, "parent"), SelectionOperators.RouletteWheelSelection(population, "parent")); Settings.testName.Concat("P");
                                //CrossoverOperators.OrderedCrossover(children, new Chromosome(RouletteWheelSelection(population, "parent")), new Chromosome(RouletteWheelSelection(population, "parent"))); Settings.testName.Concat("O");

                                //CrossoverOperators.PartiallyMappedCrossover(children, SelectionOperators.TournamentSelection(population, "parent"), SelectionOperators.TournamentSelection(population, "parent")); Settings.testName.Concat("P");
                                //CrossoverOperators.OrderedCrossover(children, TournamentSelection(population), TournamentSelection(population)); Settings.testName.Concat("O");

                                children.ForEach(child => MutationOperators.BlindMutate(child)); Settings.testName.Concat("B");
                                //children.ForEach(child => MutationOperators.ReverseMutate(child));Settings.testName.Concat("R");
                                //children.ForEach(child => MutationOperators.ScrambleMutate(child)); Settings.testName.Concat("S");
                                children.ForEach(child => { child.HardConstraintFitness = CheckHardConstraintFitness(conflictMatrix, child); ; Settings.fitnessEvalCount += 1; });

                                CheckForDuplicates(population, children);
                            }
                            //Add newly created children to the population
                            population.Chromosomes.AddRange(children);

                            while (population.Chromosomes.Count > popSize)
                            {
                                //SelectionOperators.TournamentSelection(population, "selection");Settings.testName.Concat("T");
                                SelectionOperators.RouletteWheelSelection(population, "selection"); Settings.testName.Concat("R");
                                //RandomSelection(population);
                            }

                            if (population.BestFitness().HardConstraintFitness == 0 || timer.Elapsed.TotalMinutes >= Settings.maxTime)
                            {
                                run = false;
                                timer.Stop();
                            }
                            SelectionOperators.ElitismSelection(population, popSize); Settings.testName.Concat("E");
                        }
                        population.BestFitness().ExamIDs = population.BestFitness().ExamIDs.OrderBy(x => x).ToList();
                        IO.PrintInfo(population, gen, timer, Settings.examStudentList, Settings.fitnessEvalCount);
                        IO.WriteSolution(Settings.directory, population.BestFitness(), timer, gen, Settings.fitnessEvalCount);
                    }
                }
            }
=======
            int popSize = 50, count = 0, gen = 0;
            Stopwatch timer = new();
            Dictionary<int, int> conflictTracker = new();

            int[,] conflictMatrix = new int[Settings.examStudentList.Count, Settings.examStudentList.Count];
            CreateConflictMatrix(conflictMatrix, conflictTracker);

            Population population = new(popSize, Settings.examStudentList, conflictMatrix, conflictTracker);
            population.Chromosomes.ForEach(ch => ch.HardConstraintFitness = CheckHardConstraintFitness(conflictMatrix, ch));

            bool run = true;
            timer.Start();
            while (run)
            {
                gen++;
                Console.WriteLine($" Generation: {gen} Best Fitness: {population.BestFitness().HardConstraintFitness} No. unplaced exams: {Math.Round(population.BestFitness().HardConstraintFitness * Settings.examStudentList.Count)}\n Time Elapsed: {timer.Elapsed}");
                //Console.WriteLine($" Generation: {gen} Worst Fitness: {population.WorstFitness().Fitness} No. unplaced exams: {Math.Round(population.WorstFitness().Fitness * examStudentList.Count)}");
                CheckForDuplicates(population, population.Chromosomes);
                SelectionOperators.ElitismSelection(population, popSize);

                List<Chromosome> children = new();
                //Create the rest of the population through children of the surviving population
                for (int i = population.Chromosomes.Count; population.Chromosomes.Count + children.Count <= popSize; i++)
                {
                    //Create child based on two randomly selected chromosomes using a Crossover method
                    CrossoverOperators.PartiallyMappedCrossover(children, SelectionOperators.RouletteWheelSelection(population, "parent"), SelectionOperators.RouletteWheelSelection(population, "parent"));
                    //CrossoverOperators.OrderedCrossover(children, new Chromosome(RouletteWheelSelection(population, "parent")), new Chromosome(RouletteWheelSelection(population, "parent")));

                    //CrossoverOperators.PartiallyMappedCrossover(children, SelectionOperators.TournamentSelection(population), SelectionOperators.TournamentSelection(population));
                    //CrossoverOperators.OrderedCrossover(children, TournamentSelection(population), TournamentSelection(population));

                    children.ForEach(child => MutationOperators.BlindMutate(child));
                    //children.ForEach(child => MutationOperators.ReverseMutate(child));
                    //children.ForEach(child => MutationOperators.ScrambleMutate(child));
                    children.ForEach(child => { child.HardConstraintFitness = CheckHardConstraintFitness(conflictMatrix, child); });

                    CheckForDuplicates(population, children);
                }
                //Add newly created children to the population
                population.Chromosomes.AddRange(children);

                while (population.Chromosomes.Count > popSize)
                    //SelectionOperators.TournamentSelection(population);
                    SelectionOperators.RouletteWheelSelection(population, "selection");
                //RandomSelection(population);

                if (population.BestFitness().HardConstraintFitness == 0 )//|| timer.Elapsed.Minutes == 5)
                {
                    run = false;
                    timer.Stop();
                }
            }
            population.BestFitness().ExamIDs = population.BestFitness().ExamIDs.OrderBy(x => x).ToList();
            IO.PrintInfo(population, gen, timer, Settings.examStudentList);
            IO.WriteSolution(Settings.directory, population.BestFitness(), timer);
>>>>>>> 51da1bb5daf3dc866d56cf3439ddaceaad36650a
        }

        //Initialises the exam conflicts matrix
        private static void CreateConflictMatrix(int[,] matrix, Dictionary<int, int> conflictTracker)
        {
            foreach (var firstExam in Settings.examStudentList)//Start with one exam
            {
                //Add exam to tracker with initial conflicts of 0
                conflictTracker.Add(firstExam.Key, 0);
                foreach (var nextExam in Settings.examStudentList)//Check against every other exam
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

        private static void CheckForDuplicates(Population population, List<Chromosome> chromosomeList)
        {
            List<Chromosome> removeDupesList = new();
            chromosomeList.ForEach(ch =>
            {

                var check = population.CheckPermutationExists(chromosomeList, ch);
                if (check != null)
                    removeDupesList.Add(ch);

            });
            if (removeDupesList.Count > 0)
                chromosomeList.RemoveAll(child => removeDupesList.Contains(child));
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
    }
}
