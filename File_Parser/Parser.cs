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
        // Method to find all files in the directory - recursively checks all subdirectories as well
        public List<string> FindFilesInDirectory(string directory)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.GetFiles(directory, "*.cs"))
            {
                files.Add(file);
            }

            foreach (string dir in Directory.GetDirectories(directory))
            {
                files = files.Concat<string>(FindFilesInDirectory(dir)).ToList();
            }
            return files;
        }
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            List<string> files = parser.FindFilesInDirectory("C:/Users/james/Dropbox/Masters Work/Coursework/CSE 681 - Software Modeling and Analysis/C# Code Samples/LinqExamples");
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }
        }
    }
}
