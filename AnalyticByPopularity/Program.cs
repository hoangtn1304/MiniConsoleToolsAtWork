using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnalyticByPopularity
{
    public class Program
    {
        private static void Main(string[] args)
        {
            //var watch = new Stopwatch();
            //watch.Start();

            //var blob = new LocalDriveBlob();

            //foreach (var file in blob.LocalFiles)
            //{
            //    var pairs = blob.PossiblePairs.Where(t => t.FileName == file);
            //    var infos = blob.FolderInfos.Where(t => t.FileName == file);

            //    var textOutput = Path.Combine(blob.OutputFolder.FullName, Path.GetFileNameWithoutExtension(file) + ".txt");

            //    using (var writter = File.AppendText(textOutput))
            //    {
            //        foreach (var pair in pairs)
            //        {
            //            foreach (var info in infos)
            //            {
            //                if (!info.FileName.Equals(pair.FileName, StringComparison.InvariantCultureIgnoreCase))
            //                    continue;

            //                var m1 = Regex.Matches(info.FullPath, pair.RegexNonDirectChild);
            //                var m2 = Regex.Matches(info.FullPath, pair.RegexDirectChild);

            //                pair.Popularity += m1.Count + m2.Count;
            //            }

            //            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {pair.Parent} / {pair.Child} : {pair.Popularity}");
            //            if (pair.Popularity <= 10)
            //                continue;

            //            var line = $"{{source: \"{pair.Child}\", target: \"{pair.Parent}\", value: \"{pair.Popularity}\",  type: \"suit\"}},";
            //            writter.WriteLine(line);
            //        }
            //    }
            //}

            //Console.WriteLine("BEGIN PRODUCING HTML FILES");

            //var templateContent = File.ReadAllText(blob.TemplateFile.FullName);
            //var files = blob.OutputFolder.EnumerateFiles("*.txt");

            //foreach (var file in files)
            //{
            //    var htmlOutput = Path.Combine(blob.OutputFolder.FullName, Path.GetFileNameWithoutExtension(file.Name) + ".html");
            //    var fileContent = File.ReadAllText(file.FullName);
            //    var content = templateContent.Replace("{SOURCE-TO-UPDATE}", fileContent);

            //    using (var writter = File.AppendText(htmlOutput))
            //    {
            //        writter.WriteLine(content);
            //    }
            //}

            //watch.Stop();
            //Console.WriteLine("{0} FINISH in {1} seconds", DateTime.Now.ToShortTimeString(), watch.Elapsed.TotalSeconds);

            var files = Directory.EnumerateFiles(@"../../html", "*.txt").ToArray();
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                Console.WriteLine($"#{i} Processing for {Path.GetFileName(file)}");

                var output = Path.Combine(@"../../text", Path.GetFileName(file));

                var lines = File.ReadAllLines(file);

                foreach (var line in lines)
                {
                    using (var writter = File.AppendText(output))
                    {
                        var newLine = Transform(line);
                        writter.WriteLine(newLine);
                    }
                }
            }
        }

        public static string Transform(string line)
        {
            string result = line
                .Replace("{source: \" ", "|")
                .Replace("\", target: \"", "|")
                .Replace("\", value: \"", "|")
                .Replace("\",  type: \"suit\"}", "");

            var parts = result.Split('|', ',');
            return $"{parts[2]} / {parts[1]} : {parts[3]}";
        }

        public static Task CalculatePopularityAsync(FolderInfo folder, FolderPair pair)
        {
            return Task.FromResult<string>(null);
        }
    }
}