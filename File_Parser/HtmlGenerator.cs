/*=INFO
 * For ease of use - anonymous controls will use the previous line as a trigger
 * Continuing comment
 */

using File_Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        // Function to encode special HTML characters
        public string EncodeHtml (string line)
        {
            return line.Replace("&", "&amp").Replace("<", "&lt").Replace(">", "&gt");
        }

        // Function to determine the comment type and style accordingly
        public string StyleComment (Parser.LineDetails comment)
        {
            if (comment.lineType == "infoComment")
            {
                return "<div class='infobox'>" + EncodeHtml(comment.line);
            }
            if (comment.lineType == "warningComment")
            {
                return "<div class='dangerbox'>" + EncodeHtml(comment.line);
            }
            if (comment.lineType == "commentEnd")
            {
                return EncodeHtml(comment.line) + "</div>";
            }
            return EncodeHtml(comment.line);
        }

        // Function to generate headerbox with links to project dependencies
        public string CreateHeaderBox (List<string> dependencies, string directory)
        {
            string headerbox = "<div class='headerbox'>\n";
            foreach (string dependency in dependencies)
            {
                headerbox += "<p><a href='"
                    + directory
                    + Path.GetFileNameWithoutExtension(dependency)
                    + ".html'>"
                    + Path.GetFileNameWithoutExtension(dependency)
                    + "</a></p>\n";
            }
            headerbox += "</div>\n";
            return headerbox;
        }

        // Function to create HTML file
        public void CreateHtmlFile (Parser.FileContents fileContents)
        {
            DirectoryInfo dir = Directory.CreateDirectory("../../../htmlFiles/");
            string di = dir.FullName;
            StreamWriter file = new StreamWriter(di + Path.GetFileNameWithoutExtension(fileContents.directory) + @".html", false);
            file.WriteLine(CreateHeader());
            file.WriteLine("<body>");
            file.WriteLine(CreateHeaderBox(fileContents.projectFileDependencies, di));
            foreach (Parser.LineDetails line in fileContents.lines)
            {
                if ((line.lineNumber + 1) != fileContents.lines.Count && ShouldExpand(line.lineType, fileContents.lines[line.lineNumber + 1].lineType))
                {
                    file.WriteLine("<div> <button type='button' class='btn btn-link' data-toggle='collapse' data-target='#scope"
                        + line.lineNumber
                        + "'>"
                        + EncodeHtml(line.line)
                        + "</button> </div>"
                    );
                }
                else if (line.lineType == "startScope")
                {
                    file.WriteLine("<div id='scope"
                        + (line.lineNumber - 1)
                        + "' class='collapse'> <pre>"
                        + EncodeHtml(line.line)
                    );
                }
                else if (line.lineType == "endScope")
                {
                    file.WriteLine(EncodeHtml(line.line) + "</pre> </div>");
                }
                else if (line.lineType.Contains("omment"))
                {
                    file.WriteLine(StyleComment(line));
                }
                else
                {
                    file.WriteLine(EncodeHtml(line.line));
                }
            }
            file.WriteLine("</body></html>");
            file.Close();
        }

        static void Main (string[] args)
        {
            Parser parser = new Parser();
            HtmlGenerator htmlGenerator = new HtmlGenerator();
            List<string> files = parser.FindFilesInDirectory("../../");
            List<Parser.FileContents> processedFiles = parser.ProcessFiles(files);
            foreach (Parser.FileContents file in processedFiles)
            {
                htmlGenerator.CreateHtmlFile(file);
            }

        }
    }
}
