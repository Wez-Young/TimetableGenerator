using System;
using System.Collections.Generic;
using System.Linq;

namespace TimetableGenerator.GA
{
    public class Chromosome
    {
        //Properties
        public List<Gene> Genes { get; }
        public Dictionary<int, List<int>> TimeSlots { get; }

        //Constructors
        public Chromosome() {}
        
        public Chromosome(int length, int maxTimeslot, Dictionary<int, List<int>> exams)
        {
            Genes = new List<Gene>();
            TimeSlots = new Dictionary<int, List<int>>();

            for(int i = 1; i <= maxTimeslot; i++)
            {
                TimeSlots.Add(i, new List<int>());
            }

            GenerateChromosome(length, maxTimeslot, exams);
        }
        
        //Methods
        //Creates a chromosome filled with genes
        private void GenerateChromosome(int length, int maxTimeSlot, Dictionary<int, List<int>> exams)
        {
            var examIDs = new List<int>(exams.Keys);
            //Adds x amount of genes based on specified length
            for(int i = 0; i < length; i++)
            {
                //Gets random index
                int index = Settings.rand.Next(examIDs.Count);
                //Assigns the value associated to the index to the gene
                Gene gene = new Gene(examIDs[index]);
                Genes.Add(gene);
                //Removes the item from list to avoid duplicates
                examIDs.Remove(gene.Event);
            }

            CreateTimeSlots(maxTimeSlot, exams);
        }

        private void CreateTimeSlots(int maxTimeSlot, Dictionary<int, List<int>> exams)
        {
            //check for biggest exam
            for(int i = 0; i < exams.Count; i++)
            {
                int timeslot = 1;
                var assignedExams = new List<int>(TimeSlots[timeslot]);

                //Assign first exam in gene into first timeslot
                if (assignedExams.Count == 0)
                {
                    assignedExams.Add(Genes[i].Event);
                    TimeSlots[timeslot] = assignedExams;
                    continue;
                }

                var currentExamStudents = new List<int>(exams[Genes[i].Event]);
                //Loop through current assigned exams and check for conflicts with unassigned exam
                int max = assignedExams.Count();
                for(int j = 0; j < max; j++)
                {
                    int conflicts = 0;
                    //Make a copy of the current student list
                    var currentExamStudentsCopy = new List<int>(currentExamStudents);
                    var assignedExamStudents = new List<int>(exams[assignedExams[j]]);

                    //Add all assigned exam students to current exam student list
                    currentExamStudentsCopy.AddRange(assignedExamStudents);
                    //Get the number of duplicates and subtract the value from the number of students in list to get number of conflicts
                    conflicts += currentExamStudentsCopy.Count() - currentExamStudentsCopy.Distinct().Count();

                    if(conflicts > 0)
                    {
                        //Move smaller exam to next available timeslot
                        //Need to work on this section to work properly
                        //Check time slot being movedto for conflicts
                        //Make recursive function
                        //If current exam is smaller then need to move that exam instead of the assigned one
                        if(currentExamStudents.Count() < assignedExamStudents.Count())
                        {
                            timeslot += 1;
                            assignedExams = TimeSlots[timeslot];
                            assignedExams.Add(Genes[i].Event);
                            TimeSlots[timeslot] = assignedExams;
                        }

                        continue;
                    }

                    assignedExams.Add(Genes[i].Event);
                    TimeSlots[timeslot] = assignedExams;

                }
            }
        }
    }
}
