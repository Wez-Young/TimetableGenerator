using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

using TimetableGenerator.GA;
using TimetableGenerator.GA.Genetic_Operators;

namespace TimetableGenerator
{
    class Program
    {
        private static Dictionary<int, List<int>> students = new();
        private static Stopwatch timer = new();

        static void Main()
        {
            int popSize = 50, gen = 0;
            students = new(new SortedDictionary<int, List<int>>(ReadFile("ear-f-83.stu")));

            int[,] conflictMatrix = new int[students.Count, students.Count];
            CreateConflictMatrix(conflictMatrix);

            Population population = new(popSize, Settings.maxTimeslot, students);
            population.Chromosomes.ForEach(ch => ch.Fitness = CheckFitness(conflictMatrix, ch));
            bool run = true;
            timer.Start();
            while (run)
            {
                gen++;
                Console.WriteLine($" Generation: {gen} Best Fitness: {population.BestFitness().Fitness} No. unplaced exams: {Math.Round(population.BestFitness().Fitness * students.Count)}");

                Population elites = new();
                //Get the x top chromsomes based on the fitness
                elites.Chromosomes = population.Chromosomes.OrderBy(x => x.Fitness).Take(Settings.elitismPercentage).ToList();
                //Remove the 'elite' chromosomes from the population to avoid duplication
                population.Chromosomes.RemoveAll(x => elites.Chromosomes.Contains(x));
                //Select another x num of chromosomes to go into the next generation
                population = RouletteWheelSelection(population);
                //Add the elites back into the population
                population.Chromosomes.AddRange(elites.Chromosomes);

                List<Chromosome> children = new();
                //Create the rest of the population through children of the surviving x*2
                for (int i = population.Chromosomes.Count; i < popSize; i++)
                {
                    //Create child based on two randomly selected chromosomes using the PartiallyMapped Crossover method
                    Chromosome child = CrossoverOperators.PartiallyMapped(SelectParent(population), SelectParent(population));
                    child = MutationOperators.SwapMutate(child);
                    //child = MutationOperators.ScrambleMutate(child);
                    //child = MutationOperators.SwapMutate(child); Adding a second swap mutate slows down the evolution of the timetables
                    //if(Settings.rand.NextDouble() > Settings.mutationProbability)
                    //child = MutationOperators.ReverseMutate(child);

                    child.Fitness = CheckFitness(conflictMatrix, child);
                    children.Add(child);
                }
                //Add newly created children to the population
                population.Chromosomes.AddRange(children);
                //Implement genetic operators
                if (population.BestFitness().Fitness == 0 || gen == 10000)
                {
                    run = false;
                    timer.Stop();
                }
            }

            Console.WriteLine($"Timer elapsed: {timer.Elapsed} Generation: {gen}");
            Console.WriteLine($"Best Fitness: {population.BestFitness().Fitness} No. unplaced exams: {Math.Round(population.BestFitness().Fitness * students.Count)}");


            population.BestFitness().ExamIDs = population.BestFitness().ExamIDs.OrderBy(x => x).ToList();
            population.BestFitness().ExamIDs.ForEach(id =>
            {
                bool result = false;
                foreach (var item in population.BestFitness().Timetable)
                {
                    if (item.Value.Contains(id))
                    {
                        Console.Write($"{item.Key} ");
                        result = true;
                    }
                }

                if (!result)
                    Console.Write("0 ");

            });
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
        private static void CreateConflictMatrix(int[,] matrix)
        {
            foreach (var firstExam in students)//Start with one exam
                foreach (var nextExam in students)//Check against every other exam
                    matrix[firstExam.Key - 1, nextExam.Key - 1] = FindConflicts(firstExam, nextExam);//Add the number of conflicts to the matrix
        }

        //Check the number of conflicts between two exams
        private static int FindConflicts(KeyValuePair<int, List<int>> firstExam, KeyValuePair<int, List<int>> nextExam)
        {
            int conflicts = 0;

            List<int> allStudents = new(firstExam.Value);
            //Add students from both exams into one list
            allStudents.AddRange(nextExam.Value);
            //Count the number of duplicate student IDs within the list
            conflicts = allStudents.Count - allStudents.Distinct().Count();

            return conflicts;
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
                            KeyValuePair<int, int> biggestExam = new(exam, students[exam].Count);

                            if (biggestExam.Value < students[conflicts[0]].Count)
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
                        KeyValuePair<int, int> currentExam = new(examID, students[examID].Count);

                        if (currentExam.Value > students[conflicts[0]].Count)
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

        private static bool CheckDuplicate(Population pop, Chromosome ch)
        {
            bool result = false;
            Population popCopy = new(pop);
            pop.Chromosomes.ForEach(chromosome =>
            {
                if (chromosome.ExamIDs.SequenceEqual(ch.ExamIDs) && chromosome.Timeslots.SequenceEqual(ch.Timeslots) && chromosome.ReserveTimeslots.SequenceEqual(ch.ReserveTimeslots))
                {
                    popCopy = RandomSelection(popCopy, ch);
                    result = true;
                }
            });
            pop = new(popCopy);

            return result;
        }//Possibly not needed

        //---Survival Selection Methods---

        //basic survial selection method using randomness
        private static Population RandomSelection(Population pop, Chromosome child)
        {
            //Choose random chromosome
            Chromosome competitor = pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)];
            //Compare the fitness, if child has lower remove competitor and add child
            if (competitor.Fitness > child.Fitness)
            {
                pop.Chromosomes.Remove(competitor);
                pop.Chromosomes.Add(child);
            }

            return pop;
        }

        private static Population RouletteWheelSelection(Population pop)
        {
            //Get cumlative fitness
            List<double> cumalativeFitnesses = new(new double[pop.Chromosomes.Count]);
            cumalativeFitnesses[0] = pop.Chromosomes[0].Fitness;

            //Add fitness of the next xhromsome on top of the previous one and add it to list
            for (int i = 1; i < pop.Chromosomes.Count; i++)
                cumalativeFitnesses[i] = cumalativeFitnesses[i - 1] + pop.Chromosomes[i].Fitness;

            //Create empty list with a size of the number of x surviors 
            List<Chromosome> selection = new(new Chromosome[(pop.Chromosomes.Count + Settings.elitismPercentage) / Settings.elitismPercentage]);

            for (int i = 0; i < selection.Count; i++)
            {
                //Generate a random fitness value
                double rndFitness = Settings.rand.NextDouble() * cumalativeFitnesses[cumalativeFitnesses.Count - 1];
                //Find the index of the culmative value that is equal to the random fitness value
                int index = cumalativeFitnesses.BinarySearch(rndFitness);

                //Change index to a positive number
                if (index < 0)
                    index = Math.Abs(index + 1);
                //Add the selected candidate to the selection list
                selection[i] = (pop.Chromosomes[index]);
            }
            //Assign the selection list to the population
            pop.Chromosomes = selection;
            return pop;
        }

    }
}
