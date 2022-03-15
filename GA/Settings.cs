using System;
using System.Collections.Generic;
using System.IO;

namespace TimetableGenerator.GA
{
    public class Settings
    {
        public static Random rand = new ();
        public const int tournamentSize = 2;
        public static int maxTimeslot;

        public const int elitismPercentage = 10;
        public const double mutationProbability = 0.5;
        public const double crossoverProbability = 0.75;

        public static Dictionary<int, List<int>> examStudentList = new();

        public static string filename;
        public const int testNum = 10;
        public const string testName = "PBR";
        public static DirectoryInfo directory = Directory.CreateDirectory(@$"{AppDomain.CurrentDomain.BaseDirectory}/Solutions/{filename}");
    }
}
