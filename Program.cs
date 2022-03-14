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
                Console.WriteLine("[1] Start genetic algortihm\n[2] Start Hill Climber\n[0] Exit\nSelect one:");
            string action = Console.ReadLine();


                if (action.Equals("1"))
                {
                    Settings.examStudentList = IO.ReadProblemFile();
                    GeneticAlgorithm.StartGA();
                }
                else if (action.Contains("2"))
                {
                    Console.WriteLine("Enter solution filename (e.g. tre-s-91):");
                    string filename = Console.ReadLine();
                    IO.GetStudents(Settings.examStudentList, filename);
                    for (int i = 0; i < Settings.testNum; i++)
                        HillClimber.StartHillClimber(IO.ReadSolutionFile(filename));
                }
                else if (action.Equals('0'))
                    run = false;
            }
        }      
    }
}
