using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TimetableGenerator.GA
{
    public class IO
    {
        //Reads in and sorts the data from a file  
        public static Dictionary<int, List<int>> ReadFile()
        {
            //Splits each line of the file into seperate items
            List<string> fileData = new(File.ReadAllLines(@$"{AppDomain.CurrentDomain.BaseDirectory}/Data/Toronto/{Settings.filename + ".stu"}"));
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

        public static void PrintInfo(Population population, int gen, Stopwatch timer, Dictionary<int, List<int>> examStudentList)
        {
            Console.WriteLine($"Timer elapsed: {timer.Elapsed} Generation: {gen}");
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

        public static void WriteSolution(Chromosome solution, Stopwatch timer, int gen, int testNum)
        {
            var directory = Directory.CreateDirectory(@$"{AppDomain.CurrentDomain.BaseDirectory}/Solutions/{Settings.filename}");
            if (directory != null)
            {
                string filename = $"{Settings.filename}_test{testNum}.sol";
                using StreamWriter w = new StreamWriter($"{directory}/{filename}");


                w.WriteLine($"Fitness Score: {solution.HardConstraintFitness} Generation: {gen} Total Time: {timer.Elapsed}");

                foreach (var examID in solution.ExamIDs)
                {
                    bool result = false;
                    foreach (var item in solution.Timetable)
                        if (item.Value.Contains(examID))
                        {
                            w.WriteLine($"{examID}  {item.Key}");
                            result = true;
                        }
                    if (!result)
                        w.WriteLine("0");
                }
            }
        }

    }
}
