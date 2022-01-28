using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TimetableGenerator.GA
{
    public class Chromosome
    {
        //Properties
        public List<int> ExamIDs { get; set; }
        public List<int> Timeslots { get; set; }
        public List<int> ReserveTimeslots { get; set; }
        public double Fitness { get; set; }
        public Dictionary<int, List<int>> Timetable { get; set; }

        //Constructors
        public Chromosome() { }

        public Chromosome (int size)
        {
            ExamIDs = new(new int[size]);
        }

        public Chromosome(Chromosome ch)//Make a copy of the chromosome
        {
            ExamIDs = new(ch.ExamIDs);
            Timeslots = new(ch.Timeslots);
            ReserveTimeslots = new(ch.ReserveTimeslots);
            Fitness = ch.Fitness;
            Timetable = ch.Timetable;
        }

        public Chromosome(int maxTimeslot, int examCount)
        {
            ExamIDs = new();
            Timeslots = new();
            ReserveTimeslots = new();
            Timetable = new();

            GenerateChromosome(examCount);
            GenerateTimeslot(Timeslots, maxTimeslot);
            GenerateTimeslot(ReserveTimeslots, maxTimeslot);
        }

        public Chromosome(int maxTimeslot, int examCount, int[,] conflictMatrix,  Dictionary<int, int> conflictTracker)
        {
            ExamIDs = new();
            Timeslots = new(new int[conflictTracker.Count]);
            ReserveTimeslots = new();
            Timetable = new();

            conflictTracker = conflictTracker.OrderBy(x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
            GenerateChromosome(examCount);
            GenerateHeuristicTimeslot(Timeslots, maxTimeslot, conflictMatrix, conflictTracker);
            GenerateTimeslot(ReserveTimeslots, maxTimeslot);
        }

        //Methods
        private void GenerateTimeslot(List<int> timeslots, int maxTimeslot)
        {
            while (timeslots.Count < ExamIDs.Count)
            {
                int timeslot = Settings.rand.Next(1, maxTimeslot + 1);
                timeslots.Add(timeslot);
            }
        }

        private void GenerateChromosome(int examCount)
        {
            //Generate list of numbers based on the number of exams
            List<int> nums = Enumerable.Range(1, examCount).ToList();
            //Run as many times as there is exams
            for (int i = 0; i < examCount; i++)
            {
                //Randomly choose an exam id
                int examID = Settings.rand.Next(1, examCount);

                //Check that the exam hasn't already been placed
                while (!nums.Contains(examID) && nums.Count > 1)
                    //Get new exam id
                    examID = Settings.rand.Next(1, examCount);

                //Set exam id to the only exam left
                if (nums.Count == 1)
                    examID = nums[0];

                //Add the exam id to the permutation of exam ids
                ExamIDs.Add(examID);
                //Remove the exam id to stop duplicates being placed
                nums.Remove(examID);
            }
        }

        private void GenerateHeuristicTimeslot(List<int> timeslots, int maxTimeslot, int[,] conflictMatrix, Dictionary<int, int> conflictTracker)
        {
            int counter = 0;
            foreach(var exam in conflictTracker)
            {
                int timeslot = 1;
                int index = ExamIDs.IndexOf(exam.Key);
                while (timeslots[index] == 0)
                {
                    if (timeslot <= maxTimeslot)
                    {
                        if (timeslots.All(x => x == 0))
                        {
                            timeslots[index] = timeslot;
                            break;
                        }

                        var examsInTimeslot = ExamsInTimeslot(timeslot, timeslots);
                        bool noConflicts = false;
                        if (examsInTimeslot.Count != 0)
                        {
                            foreach (var assigned in examsInTimeslot)
                            {
                                if (conflictMatrix[exam.Key - 1, assigned - 1] == 0)
                                {
                                    noConflicts = true;
                                    continue;
                                }
                                else
                                {
                                    noConflicts = false;
                                    timeslot++;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            noConflicts = true;
                        }

                        if(noConflicts)
                            timeslots[index] = timeslot;
                    }
                    else
                    {
                        counter++;
                        timeslots[index] = Settings.rand.Next(1, maxTimeslot + 1);
                    }
                }
            }

        }

        private List<int> ExamsInTimeslot(int timeslot, List<int> timeslots)
        {
            List<int> indexList = new();

            foreach (var item in timeslots)
            {
                if (item == timeslot)
                    indexList.Add(timeslots.IndexOf(item));
            }

            List<int> examsInTimeslot = new();

            for (int i = 0; i < indexList.Count; i++)
            {
                examsInTimeslot.Add(ExamIDs[indexList[i]]);
            }

            return examsInTimeslot;
        }
    }
}
