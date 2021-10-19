using System;
using System.Collections.Generic;
using System.IO;
using TimetableGenerator.GA;
namespace TimetableGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            var exmas = ReadFile("");
            
            Population population = new Population(popSize: 100, chromSize: 24);

        }

        //Reads in and sorts the data from a file  
        private static Dictionary<int, List<int>> ReadFile(string filename)
        {
            //Need to fix to actually read in file
            List<string> fileData = new List<string>(File.ReadAllLines(@$"Toronto/{filename}"));
            Dictionary<int, List<int>> exams = new Dictionary<int, List<int>>();
            int studentID = 0;

            foreach(var item in fileData)
            {
                studentID++;

                foreach (var exam in item.Split(" "))
                {
                    if (exam.Equals(""))
                        break;

                    int examKey = Convert.ToInt32(exam);
                    List<int> studentList;

                    if (exams.ContainsKey(examKey))
                        studentList = exams[examKey];
                    else
                    {
                        studentList = new List<int>();
                        exams.Add(examKey, studentList);
                    }

                    studentList.Add(studentID);
                    exams[examKey] = studentList;
                }
            }

            return exams;
        }
    }
}
