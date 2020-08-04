using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace File_Parser
{
    public class Parser
    {
        // Structure to hold all the details of a line
        public struct LineDetails
        {
            public string lineType;
            public int lineNumber;
            public string line;
        }

        // Structure to hold all the details of a file
        public struct FileContents
        {
            public string directory;
            public List<LineDetails> lines;
            public List<string> dependencies;
            public List<string> availableNamespaces;
        }

        // Method to check if the directory is an excluded directory type
        private static bool IsExcludedDirectory(string directory)
        {
            List<string> excludedDirectories = new List<string>() { "bin", "obj", "Properties" };
            return excludedDirectories.Any(dir => new DirectoryInfo(directory).Name.Equals(dir));
        }

        // Method to find all C# files in the directory - recursively checks all subdirectories excluding bin, obj, and properties
        public List<string> FindFilesInDirectory(string directory)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.GetFiles(directory, "*.cs"))
            {
                files.Add(file);
            }
            foreach (string dir in Directory.GetDirectories(directory).Where(dir => !IsExcludedDirectory(dir)))
            {
                files = files.Concat<string>(FindFilesInDirectory(dir)).ToList();
            }
            return files;
        }

        // Method to create a list of all lines in a given file
        public List<string> ReadFile(string file)
        {
            string line;
            List<string> lines = new List<string>();
            StreamReader fileStream = new StreamReader(file);
            while ((line = fileStream.ReadLine()) != null)
            {
                lines.Add(line);
            }
            fileStream.Close();
            return lines;
        }

        // Method to classify each line and return the FileContents struct
        public string ClassifyLine(string line)
        {
            List<string> comments = new List<string>() { "//", "/*", "*", "*/" };
            List<string> classes = new List<string>() { "class", "interface", "struct" };
            List<string> controls = new List<string>() { "if", "for", "foreach", "while", "catch" };
            if (comments.Any(semi => line.StartsWith(semi)))
            {
                if (line.Contains("@info"))
                {
                    return "infoComment";
                }
                if (line.Contains("@warning"))
                {
                    return "warningComment";
                }
                if (line.Contains("*/"))
                {
                    return "commentEnd";
                }
                return "comment";
            }
            if (line.StartsWith("namespace"))
            {
                return "namespace";
            }
            if (Array.Exists(line.Split(' '), word => classes.Any(semi => semi.Equals(word))))
            {
                return "class";
            }
            if (controls.Any(semi => line.StartsWith(semi)))
            {
                return "control";
            }
            if (line.StartsWith("using"))
            {
                return "using";
            }
            if (line.StartsWith("{"))
            {
                return "startScope";
            }
            if (line.StartsWith("}"))
            {
                return "endScope";
            }
            return "line";
        }

        // Method to create package containing file contents (FileContents struct)
        public FileContents CreateFileContents (string file)
        {
            FileContents fileContents = new FileContents
            {
                directory = file,
                lines = new List<LineDetails>(),
                dependencies = new List<string>(),
                availableNamespaces = new List<string>()
            };
            List<string> fileLines = ReadFile(file);
            foreach (var line in fileLines.Select((value, index) => new { index, value }))
            {
                string lineType = ClassifyLine(line.value.Trim());
                if (lineType == "dependency")
                {
                    fileContents.dependencies.Add(line.value.Trim().Replace("using", "").Replace(";",""));
                }
                if(lineType == "namespace")
                {
                    fileContents.availableNamespaces.Add(line.value.Trim().Replace("namespace",""));
                }
                fileContents.lines.Add(new LineDetails
                {
                    line = line.value,
                    lineNumber = line.index,
                    lineType = lineType
                });
            }
            return fileContents;
        }

#if(TEST_PARSER)
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            List<string> files = parser.FindFilesInDirectory("../../");
            foreach (string file in files)
            {
                FileContents fileContents = parser.CreateFileContents(file);
            }
        }
#endif
    }
}
