﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

using TimetableGenerator.GA;
using TimetableGenerator.GA.Genetic_Operators;
using System.Runtime.CompilerServices;

namespace TimetableGenerator
{
    class Program
    {
        private static Dictionary<int, List<int>> examStudentList = new();
        private static Stopwatch timer = new();
        public static int count = 0;


        static void Main()
        {
            int popSize = 100, gen = 0;
            examStudentList = new(new SortedDictionary<int, List<int>>(ReadFile("ear-f-83.stu")));
            Dictionary<int, int> conflictTracker = new();

            int[,] conflictMatrix = new int[examStudentList.Count, examStudentList.Count];
            CreateConflictMatrix(conflictMatrix, conflictTracker);

            Population population = new(popSize, Settings.maxTimeslot, examStudentList.Count, conflictMatrix, conflictTracker);
            population.Chromosomes.ForEach(ch => ch.Fitness = CheckFitness(conflictMatrix, ch));

            bool run = true;
            timer.Start();
            while (run)
            {
                gen++;
                Console.WriteLine($" Generation: {gen} Best Fitness: {population.BestFitness().Fitness} No. unplaced exams: {Math.Round(population.BestFitness().Fitness * examStudentList.Count)}");

                Population elites = new();
                //Get the x top chromsomes based on the fitness
                decimal percentage = popSize / 100m;
                elites.Chromosomes = population.Chromosomes.OrderBy(x => x.Fitness).Take(Convert.ToInt32(percentage * Settings.elitismPercentage)).ToList();
                //Remove the 'elite' chromosomes from the population to avoid duplication
                population.Chromosomes.RemoveAll(x => elites.Chromosomes.Contains(x));
                //Select another x num of chromosomes to go into the next generation
                RouletteWheelSelection(population, "selection");
                //Add the elites back into the population
                population.Chromosomes.AddRange(elites.Chromosomes);

                List<Chromosome> children = new();
                //Create the rest of the population through children of the surviving population
                for (int i = population.Chromosomes.Count; population.Chromosomes.Count + children.Count < popSize; i++)
                {
                    //Create child based on two randomly selected chromosomes using a Crossover method
                    CrossoverOperators.PartiallyMappedCrossover(children, new Chromosome(RouletteWheelSelection(population, "parent")), new Chromosome(RouletteWheelSelection(population, "parent")));
                    CrossoverOperators.OrderedCrossover(children, new Chromosome(RouletteWheelSelection(population, "parent")), new Chromosome(RouletteWheelSelection(population, "parent")));

                    CrossoverOperators.PartiallyMappedCrossover(children, SelectParent(population), SelectParent(population));
                    CrossoverOperators.OrderedCrossover(children, SelectParent(population), SelectParent(population));

                    children.ForEach(child => MutationOperators.SwapMutate(child));
                    //children.ForEach(child => MutationOperators.ScrambleMutate(child));
                    children.ForEach(child => {child.Fitness = CheckFitness(conflictMatrix, child); });
                }
                //Add newly created children to the population
                population.Chromosomes.AddRange(children);

                while (population.Chromosomes.Count > popSize)
                    RouletteWheelSelection(population, "selection");
       
                if (population.BestFitness().Fitness == 0 )//|| gen == 100000)
                {
                    run = false;
                    timer.Stop();
                }

            }

            PrintInfo(population, gen, conflictMatrix);
        }
        //Reads in and sorts the data from a file  
        private static Dictionary<int, List<int>> ReadFile(string filename)
        {
            //Splits each line of the file into seperate items
            List<string> fileData = new(File.ReadAllLines(@$"{AppDomain.CurrentDomain.BaseDirectory}/Data/Toronto/{filename}"));
            Dictionary<int, List<int>> exams = new();
            int studentID = 0;

            //Goes through each item within the list
            foreach (var item in fileData)
            {
                studentID++;
                //Splits the item into seperate exam IDs
                foreach (var exam in item.Split(" "))
                {
                    if (exam.Equals(""))
                        break;

                    int examKey = Convert.ToInt32(exam);
                    List<int> studentList;

                    //Gets the current strudent list associated to the exam ID
                    if (exams.ContainsKey(examKey))
                        studentList = exams[examKey];
                    else
                    {
                        //Create a new student list if exam ID does not exist in exams list
                        //and adds both new exam ID and new List to the Dictionary
                        studentList = new List<int>();
                        exams.Add(examKey, studentList);
                    }
                    //Adds the new student ID to the student list
                    studentList.Add(studentID);
                    //Updates the student list associated to the exam ID
                    exams[examKey] = studentList;
                }
            }

            return exams;
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

        //Select a parent for reproducing
        private static Chromosome SelectParent(Population pop)
        {
            //Create a copy of a random chromosome within the population
            Chromosome winner = new(pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)]);

            //Iterate until value equals touranament size
            for (int i = 1; i < Settings.tournamentSize; ++i)
            {
                //Create copy of chromosome from population
                Chromosome candidate = new(pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)]);

                //Check that the candidate is not the same individual as the winner
                while (winner.Equals(candidate))
                    candidate = new(pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)]);

                //If the fitness value of the candidate is lower then set winner to be the candidate
                if (candidate.Fitness < winner.Fitness)
                    winner = candidate;
            }
            //return the winner of the tournament
            return winner;
        }

        //Calculate the fitness of a solution within the population
        private static double CheckFitness(int[,] conflictMatrix, Chromosome ch)
        {
            count = 0;
            double fitness = 0;
            Dictionary<int, List<int>> timetable = new();
            int index = -1;
            //Populate timetable with timeslot number (ID) and an empty assigned exam list
            for (int i = 1; i <= Settings.maxTimeslot; i++)
                timetable.Add(i, new());
            //Go through the permutation of exams
            foreach (var exam in ch.ExamIDs)
            {
                index++;
                //Get the next timeslot in the permutation at the same index as the exam
                int currentTimeslot = ch.Timeslots[index];
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

                        case 1://Check if the conflicting exam or the current one is larger based on the number of students
                            KeyValuePair<int, int> biggestExam = new(exam, examStudentList[exam].Count);

                            if (biggestExam.Value < examStudentList[conflicts[0]].Count)
                                //Attempt to assign the current exam to its backup timeslot
                                AssignBackup(timetable, conflictMatrix, ch, biggestExam.Key);
                            else
                            {
                                //Remove the assigned exam - smaller exam
                                assignedExams.Remove(conflicts[0]);
                                assignedExams.Add(exam);
                                timetable[currentTimeslot] = new(assignedExams);
                                //Attempt to assign the assigned exam to its reserved timeslot
                                AssignBackup(timetable, conflictMatrix, ch, conflicts[0]);
                            }
                            break;

                        default:
                            //If the number of conflicting exams is greater than one, directly attempt to assign exam to back up timeslot 
                            AssignBackup(timetable, conflictMatrix, ch, exam);
                            break;
                    }
                }
            }

            ch.Timetable = new(timetable);
            //Calculate fitness based onthe number of unplaced exams divided by the total number of exams e.g., 33/190 = fitness
            fitness = Math.Round((double)(ch.ExamIDs.Count - timetable.Values.Sum(list => list.Count)) / ch.ExamIDs.Count, 4);
            return fitness;
        }

        //Attempt to assign exam to its reserve timeslot
        private static void AssignBackup(Dictionary<int, List<int>> timetable, int[,] conflictMatrix, Chromosome ch, int examID)
        {
            int index = ch.ExamIDs.FindIndex(id => id == examID);

            //Get the next timeslot in the permutation
            int currentTimeslot = ch.ReserveTimeslots[index];
            //Create a copy of the current assigned exams in the timeslot
            List<int> assignedExams = new(timetable[currentTimeslot]);
            //Add exam to list in timeslot if there are no other assigned exams
            if (timetable[currentTimeslot].Count == 0)
            {
                assignedExams.Add(examID);
                timetable[currentTimeslot] = assignedExams;
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
                    if (conflictMatrix[examID - 1, assigned - 1] == 0)
                        continue;
                    else
                        conflicts.Add(assigned);
                }

                switch (conflicts.Count)
                {
                    case 0:
                        assignedExams.Add(examID);
                        timetable[currentTimeslot] = new(assignedExams);
                        break;

                    case 1:
                        KeyValuePair<int, int> currentExam = new(examID, examStudentList[examID].Count);

                        if (currentExam.Value > examStudentList[conflicts[0]].Count)
                        {
                            assignedExams.Add(examID);
                            assignedExams.Remove(conflicts[0]);
                            timetable[currentTimeslot] = new(assignedExams);
                            AssignBackup(timetable, conflictMatrix, ch, conflicts[0]);
                        }
                        break;

                    default:
                        //Console.WriteLine($"{examID} cannot be placed");
                        break;
                }
            }
        }

        private static int CheckConflicts(int[,] conflictMatrix, Chromosome solution)
        {
            int conflicts = 0;

            foreach (var item in solution.Timetable)
            {
                var list = item.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    var currentExam = list[i];

                    foreach (var exam in list)
                    {
                        if (currentExam == exam)
                            continue;
                        conflicts += conflictMatrix[currentExam - 1, exam - 1];
                    }
                }
            }
            return conflicts;
        }

        private static void PrintInfo(Population population, int gen, int[,] conflictMatrix)
        {
            Console.WriteLine($"Timer elapsed: {timer.Elapsed} Generation: {gen}");
            Console.WriteLine($"Best Fitness: {population.BestFitness().Fitness} No. unplaced exams: {Math.Round(population.BestFitness().Fitness * examStudentList.Count)}");
            Console.WriteLine($"No. of Conflicts: {CheckConflicts(conflictMatrix, population.BestFitness())}");

            population.BestFitness().ExamIDs = population.BestFitness().ExamIDs.OrderBy(x => x).ToList();
            population.BestFitness().ExamIDs.ForEach(id =>
            {
                bool result = false;
                foreach (var item in population.BestFitness().Timetable)
                    if (item.Value.Contains(id))
                    {
                        Console.Write($"{item.Key} ");
                        result = true;
                    }

                if (!result)
                    Console.Write("0 ");

            });
        }

        //---Survival Selection Methods---

        //basic survial selection method using randomness
        private static Population RandomSelection(Population pop)
        {
            //Choose random chromosomes
            Chromosome winner = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];
            Chromosome competitor = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];

            if (winner.Equals(competitor))
                competitor = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];
            //Compare the fitness, if child has lower remove competitor and add child
            if (competitor.Fitness > winner.Fitness)
                pop.Chromosomes.Remove(competitor);
            else
                pop.Chromosomes.Remove(winner);
            return pop;
        }

        private static Chromosome RouletteWheelSelection(Population pop, string option)
        {
            //Get cumlative fitness
            List<double> cumalativeFitnesses = new(new double[pop.Chromosomes.Count]);
            cumalativeFitnesses[0] = pop.Chromosomes[0].Fitness;

            //Add fitness of the next chromsome on top of the previous one and add it to list
            for (int i = 1; i < pop.Chromosomes.Count; i++)
                cumalativeFitnesses[i] = cumalativeFitnesses[i - 1] + pop.Chromosomes[i].Fitness;

            switch (option)
            {
                case "survival":
                    //Create empty list with a size of the number of x surviors 
                    List<Chromosome> selection = new(new Chromosome[(pop.Chromosomes.Count + Settings.elitismPercentage) / Settings.elitismPercentage]);

                    for (int i = 0; i < selection.Count; i++)
                    {
                        //();
                        Chromosome individual = SelectIndividual(pop, cumalativeFitnesses);
                        selection[i] = individual;
                    }
                    //Assign the selection list to the population
                    pop.Chromosomes = selection;
                    break;
                case "parent":

                    Chromosome winner = new(SelectIndividual(pop, cumalativeFitnesses));
                    for (int i = 1; i < Settings.tournamentSize; i++)
                    {
                        Chromosome candidate = new(SelectIndividual(pop, cumalativeFitnesses));

                        while (winner.Equals(candidate))
                            candidate = new(SelectIndividual(pop, cumalativeFitnesses));

                        if (candidate.Fitness < winner.Fitness)
                            winner = candidate;
                    }
                    return winner;

                case "selection":
                    Chromosome firstIndividual = SelectIndividual(pop, cumalativeFitnesses);
                    for (int i = 1; i < Settings.tournamentSize; i++)
                    {
                        Chromosome candidate = SelectIndividual(pop, cumalativeFitnesses);

                        while (firstIndividual.Equals(candidate))
                            candidate = SelectIndividual(pop, cumalativeFitnesses);

                        if (candidate.Fitness >= firstIndividual.Fitness)
                            pop.Chromosomes.Remove(candidate);
                        else
                            pop.Chromosomes.Remove(firstIndividual);
                    }
                    break;
            }

            return null;
        }

        private static Chromosome SelectIndividual(Population pop, List<double> cumalativeFitnesses)
        {
            //Generate a random fitness value
            double rndFitness = Settings.rand.NextDouble() * cumalativeFitnesses[cumalativeFitnesses.Count - 1];
            //Find the index of the culmative value that is equal to the random fitness value
            int index = cumalativeFitnesses.BinarySearch(rndFitness);

            //Change index to a positive number
            if (index < 0)
                index = Math.Abs(index + 1);

            return pop.Chromosomes[index];
        }
    }
}
