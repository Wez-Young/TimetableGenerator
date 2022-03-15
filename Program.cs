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
        static void Main()
        {
            bool run = true;
            while (run)
            {
                Console.WriteLine("\n[1] Start genetic algortihm\n[2] Start Hill Climber\n[0] Exit\nSelect one:");
            string action = Console.ReadLine();


                if (action.Equals("1"))
                {
                    SelectProblem();
                    Settings.examStudentList = IO.ReadProblemFile();
                    GeneticAlgorithm.StartGA();
                }
                else if (action.Contains("2"))
                {
                    SelectProblem();
                    IO.GetStudents(Settings.examStudentList);
                    for (int i = 0; i < Settings.testNum; i++)
                        HillClimber.StartHillClimber(IO.ReadSolutionFile());
                }
                else if (action.Equals('0'))
                    run = false;
            }
        }    
        
        private static void SelectProblem()
        {
            Console.WriteLine("\n[1] car-f-92\n[2] tre-s-92\n[3] yor-f-83\n[0] Exit\nSelect one problem to solve:");
            switch (Console.ReadLine())
            {
                case "1":
                    Settings.filename = "car-f-92";
                    Settings.maxTimeslot = 32;
                    break;
                case "2":
                    Settings.filename = "tre-s-92";
                    Settings.maxTimeslot = 23;
                    break;
                case "3":
                    Settings.filename = "yor-f-83";
                    Settings.maxTimeslot = 21;
                    break;
                default:
                    break;
            }
        }
    }
}
