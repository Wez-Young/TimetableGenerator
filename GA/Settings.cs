using System;
namespace TimetableGenerator.GA
{
    public class Settings
    {
        public static Random rand = new Random();
        public static int tournamentSize = 2;
        public static int maxTimeslot = 23;

        public static int elitismPercentage = 10;
        public static double mutationProbability = 0.5;
        public static double crossoverProbability = 0.75;

        public static string filename = "tre-s-92";
        public static int testNum = 10;
        public static string testName = "PBR";
    }
}
