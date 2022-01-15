using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TimetableGenerator.GA;
using TimetableGenerator.GA.Genetic_Operators;

namespace TimetableGenerator
{
    class Program
    {
        private static Dictionary<int, List<int>> students = new();

        static void Main()
        {
            int popSize = 100, gen = 0;
            students = new(new SortedDictionary<int, List<int>>(ReadFile("ear-f-83.stu")));

            int[,] conflictMatrix = new int[students.Count, students.Count];
            CreateConflictMatrix(conflictMatrix);

            Population population = new(popSize, Settings.maxTimeslot, students);
            population.Chromosomes.ForEach(ch => ch.Fitness = CheckFitness(conflictMatrix, ch)); 
            bool run = true;
            while (run)
            {
                gen++;
                Console.WriteLine($" Generation: {gen} Best Fitness: {population.BestFitness().Fitness} Worst Fitness: {population.WorstFitness().Fitness}");

                for (int i = 0; i < students.Count / 10; i++)
                {
                    Chromosome p1 = SelectParent(population);
                    Chromosome p2 = SelectParent(population);
                    Chromosome child = CrossoverOperators.PartiallyMapped(p1, p2);

                    population.Chromosomes.ForEach(x =>
                    {
                        if (x.Genes.Count < students.Count)
                            Console.ReadLine();
                    });

                    while (CheckDuplicate(population, child))
                        child = CrossoverOperators.PartiallyMapped(p1, p2);

                    child.Fitness = CheckFitness(conflictMatrix, child);
                    population = SurvivalSelection(population, child);
                }
                //Implement genetic operators
                if (gen == 10000)
                    run = false;
            }

            Console.WriteLine($"Best Fitness: {population.BestFitness().Fitness}");
            Console.WriteLine($"Total No. unplaced exams: {population.BestFitness().Fitness * students.Count}");
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
                    matrix[firstExam.Key - 1, nextExam.Key - 1] = CheckConflicts(firstExam, nextExam);//Add the number of conflicts to the matrix
        }

        //Check the number of conflicts between two exams
        private static int CheckConflicts(KeyValuePair<int, List<int>> firstExam, KeyValuePair<int, List<int>> nextExam)
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
            Dictionary<int, List<int>> Timetable = new();
            int index = 0;
            //Populate timetable with timeslot number (ID) and an empty assigned exam list
            for (int i = 1; i <= Settings.maxTimeslot; i++)
                Timetable.Add(i, new());
            //Go through the permutation of exams
            foreach (var exam in ch.Genes)
            {
                //Get the next timeslot in the permutation at the same index as the exam
                int currentTimeslot = ch.Timeslots[index];
                //Create a copy of the current assigned exams in the timeslot
                List<int> assignedExams = new(Timetable[currentTimeslot]);
                //Add exam to list in timeslot if there are no other assigned exams
                if (assignedExams.Count == 0)
                {
                    assignedExams.Add(exam.Event);
                    Timetable[currentTimeslot] = assignedExams;
                }
                else
                {
                    List<int> conflicts = new();
                    //Check the conflicts between the current exam and the assigned exams
                    foreach (var assigned in assignedExams)
                    {
                        //Do nothing if there is no conflict
                        if (conflictMatrix[exam.Event - 1, assigned - 1] == 0)
                            continue;
                        else
                            //Add the conflicting assigned exam to the conflicts list
                            conflicts.Add(assigned);
                    }

                    //Do an action based on the number of conflicts, 0, 1, 1<X
                    switch (conflicts.Count)
                    {
                        case 0://Assign the current exam as there is no conflicts
                            assignedExams.Add(exam.Event);
                            Timetable[currentTimeslot] = new(assignedExams);
                            break;

                        case 1://Check if the conflicting exam or the current one is larger based on the number of students
                            KeyValuePair<int, int> biggestExam = new(exam.Event, students[exam.Event].Count);

                            if (biggestExam.Value < students[conflicts[0]].Count)
                                //Attempt to assign the current exam to its backup timeslot
                                AssignBackup(Timetable, conflictMatrix, ch, biggestExam.Key);
                            else
                            {
                                //Remove the assigned exam - smaller exam
                                assignedExams.Remove(conflicts[0]);
                                assignedExams.Add(exam.Event);
                                Timetable[currentTimeslot] = new(assignedExams);
                                //Attempt to assign the assigned exam to its reserved timeslot
                                AssignBackup(Timetable, conflictMatrix, ch, conflicts[0]);
                            }
                            break;

                        default:
                            //If the number of conflicting exams is greater than one, directly attempt to assign exam to back up timeslot 
                            AssignBackup(Timetable, conflictMatrix, ch, exam.Event);
                            break;
                    }
                }

                index++;
            }
            //Calculate fitness based onthe number of unplaced exams divided by the total number of exams e.g., 33/190 = fitness
            fitness = Math.Round((double)(ch.Genes.Count - Timetable.Values.Sum(list => list.Count)) / ch.Genes.Count , 4);
            return fitness;
        }

        //Attempt to assign exam to its reserve timeslot
        private static void AssignBackup(Dictionary<int, List<int>> timetable, int[,] conflictMatrix, Chromosome ch, int examID)
        {
            int index = ch.Genes.FindIndex(gene => gene.Event == examID);

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

        private static Population SurvivalSelection(Population pop, Chromosome child)
        {

            Chromosome competitor = pop.WorstFitness();

            if(competitor.Fitness > child.Fitness)
            {
                pop.Chromosomes.Remove(competitor);
                pop.Chromosomes.Add(child);
            }

            return pop;
        }

        private static bool CheckDuplicate(Population pop, Chromosome ch)
        {
            bool result = false;

                pop.Chromosomes.ForEach(chromosome =>
                {
                    if (chromosome.Genes.SequenceEqual(ch.Genes))
                        result = true;
                });


            return result;
        }
    }
}
