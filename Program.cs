using System;
using System.Collections.Generic;
using System.IO;
using TimetableGenerator.GA;

namespace TimetableGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            int popSize = 40;
            var exams = ReadFile("ear-f-83.stu");

            Population population = new Population(popSize, exams.Count, 24, exams);

            bool run = true;
            while(run)
            {
                Chromosome p1 = SelectParent(population);
                Chromosome p2 = SelectParent(population);
            }
        }

        //Reads in and sorts the data from a file  
        private static Dictionary<int, List<int>> ReadFile(string filename)
        {
            //Splits each line of the file into seperate items
            List<string> fileData = new List<string>(File.ReadAllLines(@$"{AppDomain.CurrentDomain.BaseDirectory}/Data/Toronto/{filename}"));
            Dictionary<int, List<int>> exams = new Dictionary<int, List<int>>();
            int studentID = 0;

            //Goes through each item within the list
            foreach(var item in fileData)
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

        private static Chromosome SelectParent(Population pop)
        {
            Chromosome winner = new Chromosome(pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)]);

            for(int i = 0; i < Settings.tournamentSize; ++i)
            {
                Chromosome candidate = new Chromosome(pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)]);

                while(winner.Equals(candidate))
                    candidate = new Chromosome(pop.Chromosomes[Settings.rand.Next(pop.Chromosomes.Count)]);

                if (candidate.Fitness < winner.Fitness)
                    winner = candidate;
            }

            return winner;
        }
    }
}
