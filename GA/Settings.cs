using System;
using System.Collections.Generic;

namespace TimetableGenerator.GA
{
    public class Settings
    {
        public static Random rand = new ();
        public const int tournamentSize = 2;
        public const int maxTimeslot = 21;

        public const int elitismPercentage = 10;
        public const double mutationProbability = 0.5;
        public const double crossoverProbability = 0.75;

        public static Dictionary<int, List<int>> examStudentList = new();

        public const string filename = "yor-f-83";
        public const int testNum = 10;
        public const string testName = "PBR";
    }
}
