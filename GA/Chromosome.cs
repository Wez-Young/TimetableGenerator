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
        public double HardConstraintFitness { get; set; }
        public double SoftConstraintFitness { get; set; }
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
            HardConstraintFitness = ch.HardConstraintFitness;
            SoftConstraintFitness = ch.SoftConstraintFitness;
            Timetable = ch.Timetable;
        }

        public Chromosome(Dictionary<int, List<int>> exams)
        {
            ExamIDs = new();
            Timeslots = new();
            ReserveTimeslots = new();
            Timetable = new();

            GenerateChromosome(exams.Count);
            GenerateTimeslot(Timeslots);
            GenerateTimeslot(ReserveTimeslots);
        }

        public Chromosome(Dictionary<int, List<int>> exams, int[,] conflictMatrix, Dictionary<int, int> conflictTracker)
        {
            ExamIDs = new();
            Timeslots = new();
            ReserveTimeslots = new(new int[exams.Count]);
            Timetable = new();

            conflictTracker = conflictTracker.OrderBy(x => x.Value).Reverse().ToDictionary(x => x.Key, x => x.Value);
            exams = exams.OrderBy(x => x.Value.Count).Reverse().ToDictionary(x => x.Key, x => x.Value);
            GenerateChromosome(exams.Count);
            GenerateHeuristicTimeslot(ReserveTimeslots, conflictMatrix, exams.Keys.ToList());
            GenerateTimeslot(Timeslots);
            //GenerateHeuristicTimeslot(ReserveTimeslots, conflictMatrix, exams.Keys.ToList());
        }
            //Methods
        private void GenerateTimeslot(List<int> timeslots)
        {
            while (timeslots.Count < ExamIDs.Count)
            {
                int timeslot = Settings.rand.Next(1, Settings.maxTimeslot + 1);
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

        private void GenerateHeuristicTimeslot(List<int> timeslots, int[,] conflictMatrix, List<int> conflictTracker)
        {
            //Go through each exam in the conflict list (already in order of most conflicts)
            foreach (var exam in conflictTracker)
            {
                //Get the index of the exam in the Exam Id list
                int index = ExamIDs.IndexOf(exam);
                //Set timeslot value to 1 (first timeslot available)
                int currentTimeslot = 1;
                //if the exam is the first
                if (timeslots.All(x => x == 0))
                {
                    //set timeslot value to the item at the index of the timeslot list
                    timeslots[index] = currentTimeslot;
                    continue;
                }
                //While the value == 0 (null)
                while (timeslots[index] == 0 && currentTimeslot <= Settings.maxTimeslot)
                {
                    //Set flag to false, assume there is no conflict
                    bool conflicts = false;

                    //find all exam scheduled at the current timeslot
                    List<int> scheduledExams = new();
                    FindScheduledExams(scheduledExams, timeslots, currentTimeslot);
                    //if there are already schduled exams in the timeslot
                    if (scheduledExams.Count > 0)
                        //go through each scheduled exam
                        foreach (var scheduled in scheduledExams)
                        {
                            //Check if there any conflicts between the current exam and the scheduled exam
                            if (conflictMatrix[exam - 1, scheduled - 1] != 0)
                            {
                                //if there is conflict increase timeslot
                                currentTimeslot++;
                                //set flag to true
                                conflicts = true;
                                //continue the search
                                break;
                            }
                        }
                    //if there is no conflict found set timeslot
                    if (!conflicts)
                        timeslots[index] = currentTimeslot;
                }
                //If the exam can't be assigned a timeslot, randomly give it one
                if (currentTimeslot > Settings.maxTimeslot && timeslots[index] == 0)
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
