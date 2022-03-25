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
<<<<<<< HEAD
        public static int maxTime;
        public static int fitnessEvalCount = 0;

        public const int elitismPercentage = 10;
        public static double mutationProbability = 0.25;
        public static double crossoverProbability = 0.75;
=======

        public const int elitismPercentage = 10;
        public const double mutationProbability = 0.5;
        public const double crossoverProbability = 0.75;
>>>>>>> 51da1bb5daf3dc866d56cf3439ddaceaad36650a

        public static Dictionary<int, List<int>> examStudentList = new();

        public static string filename;
        public const int testNum = 10;
<<<<<<< HEAD
        public static string testName = "";
        public static DirectoryInfo directory;
=======
        public const string testName = "PBR";
        public static DirectoryInfo directory = Directory.CreateDirectory(@$"{AppDomain.CurrentDomain.BaseDirectory}/Solutions/{filename}");
>>>>>>> 51da1bb5daf3dc866d56cf3439ddaceaad36650a
    }
}
