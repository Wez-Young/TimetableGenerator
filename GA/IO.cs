﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TimetableGenerator.GA
{
    public class IO
    {
        //Reads in and sorts the data from a file  
        public static Dictionary<int, List<int>> ReadProblemFile()
        {
            //Splits each line of the file into seperate items
            Dictionary<int, List<int>> exams = new();
            GetStudents(exams);
            return exams;
        }

        public static void GetStudents(Dictionary<int, List<int>> exams)
        {
            List<string> fileData = new(File.ReadAllLines(@$"{AppDomain.CurrentDomain.BaseDirectory}/Data/Toronto/{Settings.filename + ".stu"}"));

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
        }
<<<<<<< HEAD

        public static Chromosome ReadSolutionFile()
        {
            Chromosome solution = new();
            List<string> solutionTimeslots = new(File.ReadAllLines(@$"{Settings.directory}/{Settings.filename}_{Settings.testName}.csv"));

            if(solutionTimeslots[1].Contains(","))
                solutionTimeslots[1] = solutionTimeslots[0].Replace(',', ' ');

            solutionTimeslots = new(solutionTimeslots[1].Split(' '));
=======
        public static Chromosome ReadSolutionFile()
        {
            Chromosome solution = new();
            List<string> solutionTimeslots = new(File.ReadAllLines(@$"{AppDomain.CurrentDomain.BaseDirectory}/Solutions/{Settings.filename}/{Settings.filename}_{Settings.testName}.csv"));

            if(solutionTimeslots[0].Contains(","))
                solutionTimeslots[0] = solutionTimeslots[0].Replace(',', ' ');

            solutionTimeslots = new(solutionTimeslots[0].Split(' '));
>>>>>>> 51da1bb5daf3dc866d56cf3439ddaceaad36650a
            solutionTimeslots.ForEach(x => { if (!x.Equals("")) solution.Timeslots.Add(Convert.ToInt32(x)); });

            return solution;
        }

<<<<<<< HEAD
        public static void PrintInfo(Population population, int gen, Stopwatch timer, Dictionary<int, List<int>> examStudentList, int fitnessEvalCount)
        {
            Console.WriteLine($"Timer elapsed: {timer.Elapsed} Generation: {gen}, Total Fitness Evals: {fitnessEvalCount}");
=======
        public static void PrintInfo(Population population, int gen, Stopwatch timer, Dictionary<int, List<int>> examStudentList)
        {
            Console.WriteLine($"Timer elapsed: {timer.Elapsed} Generation: {gen}");
>>>>>>> 51da1bb5daf3dc866d56cf3439ddaceaad36650a
            Console.WriteLine($"Best Fitness: {population.BestFitness().HardConstraintFitness} No. unplaced exams: {Math.Round(population.BestFitness().HardConstraintFitness * examStudentList.Count)}");

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

        public static void WriteData(DirectoryInfo directory, Chromosome solution, Stopwatch timer)
        {
            StreamWriter w;
            string filename = $"{Settings.filename}.csv";
            if (!File.Exists($"{directory}/{filename}"))
            {
                w = new StreamWriter($"{directory}/{filename}");
                w.WriteLine($"{Settings.testName},Original Fitness Score:,Fitness After Hill Climber:,Total Time:");
            }
            else
                w = new StreamWriter($"{directory}/{filename}", true);

            w.WriteLine($",{solution.OriginalSoftConstraintFitness},{solution.SoftConstraintFitness},{timer.Elapsed}");

            w.Flush();
            w.Close();
        }

<<<<<<< HEAD
        public static void WriteSolution(DirectoryInfo directory, Chromosome solution, Stopwatch timer, int gen, int fitnessEvalCount)
=======
        public static void WriteSolution(DirectoryInfo directory, Chromosome solution, Stopwatch timer)
>>>>>>> 51da1bb5daf3dc866d56cf3439ddaceaad36650a
        {
            StreamWriter w;
            string filename = $"{Settings.filename}_{Settings.testName}.csv";

            w = new StreamWriter($"{directory}/{filename}");
<<<<<<< HEAD
            w.WriteLine($"Overall time: {timer.Elapsed.TotalMinutes}, total generations: {gen}, total fitness evals: {fitnessEvalCount}, Hard constraint fitness: {solution.HardConstraintFitness}");
=======
            w.WriteLine($"Overall time: {timer.Elapsed.TotalMinutes}");
>>>>>>> 51da1bb5daf3dc866d56cf3439ddaceaad36650a
            foreach (var examID in solution.ExamIDs)
            {
                bool result = false;
                foreach (var item in solution.Timetable)
                    if (item.Value.Contains(examID))
                    {
                        w.Write($"{item.Key} ");
                        result = true;
                    }
                if (!result)
<<<<<<< HEAD
                    w.Write(Settings.rand.Next(Settings.maxTimeslot));
=======
                    w.Write("0 ");
>>>>>>> 51da1bb5daf3dc866d56cf3439ddaceaad36650a
            }
            w.Flush();
            w.Close();
        }
    }
}
