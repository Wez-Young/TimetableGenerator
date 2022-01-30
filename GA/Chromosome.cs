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
            int x = Timeslots.Max();
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
            //Go through each exam in the conflict list (already in order of most conflicts)
            foreach (var exam in conflictTracker)
            {
                //Get the index of the exam in the Exam Id list
                int index = ExamIDs.IndexOf(exam.Key);
                //Set timeslot value to 1 (first timeslot available)
                int currentTimeslot = 1;
                //While the value == 0 (null)
                while (timeslots[index] == 0 && currentTimeslot <= Settings.maxTimeslot)
                {
                    //Set flag to false, assume there is no conflict
                    bool conflicts = false;
                    //if the exam is the first
                    if (timeslots.All(x => x == 0))
                    {
                        //set timeslot value to the item at the index of the timeslot list
                        timeslots[index] = currentTimeslot;
                        break;
                    }
                    //find all exam scheduled at the current timeslot
                    List<int> scheduledExams = new();
                    FindScheduledExams(scheduledExams, timeslots, currentTimeslot);
                    //if there are already schduled exams in the timeslot
                    if (scheduledExams.Count > 0)
                        //go through each scheduled exam
                        foreach (var scheduled in scheduledExams)
                            //Check if there any conflicts between the current exam and the scheduled exam
                            if (conflictMatrix[exam.Key - 1, scheduled - 1] != 0)
                            {
                                //if there is conflict increase timeslot
                                currentTimeslot++;
                                //set flag to true
                                conflicts = true;
                                //continue the search
                                continue;
                            }
                    //if there is no conflict found set timeslot
                    if (!conflicts)
                        timeslots[index] = currentTimeslot;
                }
                //If the exam can't be assigned a timeslot, randomly give it one
                if (currentTimeslot > Settings.maxTimeslot)
                    timeslots[index] = Settings.rand.Next(1, Settings.maxTimeslot + 1);
            }
        }

        private void FindScheduledExams(List<int> scheduled, List<int> timeslots, int currentTimeslot)
        {
            List<int> indexList = new();
            //go through all timeslots in the timeslot list
            foreach(int timeslot in timeslots)
            {
                //if timeslot is equal to the current timeslot
                if(timeslot == currentTimeslot)
                {
                    //add the index of the timeslot
                    indexList.Add(timeslots.IndexOf(timeslot));
                }
            }

            //add exam id in ExamID list using the indexes
            foreach(int index in indexList)
            {
                scheduled.Add(ExamIDs[index]);
            }
        }
    }
}
