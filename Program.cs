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
            Console.WriteLine("Would you like to generate a solution or improve one? \n 'Start GA' or 'Start Hill Climber + {solution_name}.csv'");
            action = Console.ReadLine();
            switch (action)
            {

            }

        }
        
    }
}
