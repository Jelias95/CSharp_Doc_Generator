using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Parser
{
    class Parser
    {
        // Method to find all C# files in the directory - recursively checks all subdirectories as well
        public List<string> FindFilesInDirectory(string directory)
        {
            List<string> files = new List<string>();

            // Find each file in a directory that ends with .cs
            foreach (string file in Directory.GetFiles(directory, "*.cs"))
            {
                files.Add(file);
            }

            // Recursively check each subdirectory and add the returned result to the existing list of files
            foreach (string dir in Directory.GetDirectories(directory))
            {
                files = files.Concat<string>(FindFilesInDirectory(dir)).ToList();
            }
            return files;
        }

        // Method to find all lines in a given file
        public List<string> ReadFile(string file)
        {
            List<string> lines = new List<string>();
            string line;

            // Read the file, write it to the list line by line, and close the file
            StreamReader fileStream = new StreamReader(file);
            while ((line = fileStream.ReadLine()) != null)
            {
                lines.Add(line);
            }
            fileStream.Close();
            return lines;
        }
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            List<string> files = parser.FindFilesInDirectory("C:/Users/james/Dropbox/Masters Work/Coursework/CSE 681 - Software Modeling and Analysis/C# Code Samples/LinqExamples");
            foreach (string file in files)
            {
                foreach (string line in parser.ReadFile(file))
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
