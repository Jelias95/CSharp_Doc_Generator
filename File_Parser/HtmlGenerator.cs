using File_Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Generator
{
    class HtmlGenerator
    {
        // Function to create header for HTML file
        public string CreateHeader()
        {
            return "<!DOCTYPE html>"
                + "<html>"
                + "<head>"
                + "<meta name='viewport' content='width=device-width, initial-scale=1'>"
                + "<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css'>"
                + "<script src='https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js'></script>"
                + "<script src='https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js'></script>"
                + "<style>"
                + ".infobox {"
                + "background: #BDF7BC;"
                + "border: 1px solid #4B6B4A;"
                + "-moz-border-radius: 15px;"
                + "border-radius: 15px;"
                + "}"
                + ".dangerbox {"
                + "background:#FC827C;"
                + "border: 1px solid #4C0F0C;"
                + "-moz-border-radius: 15px;"
                + "border-radius: 15px;"
                + "}"
                + ".headerbox {"
                + "background:#C2EDF7;"
                + "border: 1px solid #0A5A6D;"
                + "-moz-border-radius: 15px;"
                + "border-radius: 15px;"
                + "}"
                + "</style>"
                + "</head>";
        }

        // Function to check if the line should be an expand/collapse button
        public bool ShouldExpand(string thisLineType, string nextLineType)
        {
            List<string> expandingLineTypes = new List<string>() { "class", "control", "namespace" };
            if (thisLineType.Any(semi => thisLineType.Equals(semi)))
            {
                return true;
            }
            if (nextLineType == "startScope")
            {
                return true;
            }
            return false;
        }

        // Function to create HTML file
        public void CreateFile (string fileName, List<Parser.LineDetails> lines)
        {
            DirectoryInfo dir = Directory.CreateDirectory("../../../htmlFiles/");
            string di = dir.FullName;
            StreamWriter file = new StreamWriter(di + fileName + @".html", false);
            file.WriteLine(CreateHeader());
            file.WriteLine("<body>");
            foreach (Parser.LineDetails line in lines)
            {
                if ((line.lineNumber + 1) != lines.Count)
                {
                    if (ShouldExpand(line.lineType, lines[line.lineNumber + 1].lineType))
                    {
                        file.WriteLine("<div> <button type='button' class='btn btn-link' data-toggle='collapse' data-target='#scope"
                            + line.lineNumber
                            + "'>"
                            + line.line
                            + "</button> </div>"
                        );
                    }
                    else if (line.lineType == "startScope")
                    {
                        file.WriteLine("<div id='scope"
                            + (line.lineNumber - 1)
                            + "' class='collapse'> <pre>"
                            + line.line
                        );
                    }
                    else if (line.lineType == "endScope")
                    {
                        file.WriteLine(line.line + "</pre> </div>");
                    }
                    else
                    {
                        file.WriteLine(line.line);
                    }
                }
            }
            file.Close();
        }

        static void Main (string[] args)
        {
            Parser parser = new Parser();
            HtmlGenerator htmlGenerator = new HtmlGenerator();
            List<string> files = parser.FindFilesInDirectory("../../");
            List<Parser.FileContents> fileDetails = new List<Parser.FileContents>();
            foreach (string file in files)
            {
                Parser.FileContents test = parser.CreateFileContents(file);
                htmlGenerator.CreateFile(Path.GetFileNameWithoutExtension(test.directory), test.lines);
            }

        }
    }
}
