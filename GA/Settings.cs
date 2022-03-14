using System;
using System.Collections.Generic;
using System.IO;

namespace TimetableGenerator.GA
{
    public class Settings
    {
        public static Random rand = new ();
        public const int tournamentSize = 2;
        public const int maxTimeslot = 23;

        public const int elitismPercentage = 10;
        public const double mutationProbability = 0.5;
        public const double crossoverProbability = 0.75;

        public static Dictionary<int, List<int>> examStudentList = new();

        public const string filename = "tre-s-92";
        public const int testNum = 10;
        public const string testName = "PBS";
        public static DirectoryInfo directory = Directory.CreateDirectory(@$"{AppDomain.CurrentDomain.BaseDirectory}/Solutions/{filename}");
    }
}
