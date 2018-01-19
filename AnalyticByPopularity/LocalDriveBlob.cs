using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnalyticByPopularity
{
    public class LocalDriveBlob
    {
        public LocalDriveBlob()
        {
            PossiblePairs = new List<FolderPair>();
            FolderInfos = new List<FolderInfo>();

            foreach (var file in DataFolder.EnumerateFiles())
            {
                var pairs = GetFolderPairFrom(file.FullName);
                PossiblePairs.AddRange(pairs);
            }

            Console.WriteLine("FINISH get folder pairs");

            foreach (var file in FullPathFolder.EnumerateFiles())
            {
                var infos = GetFolderInfosFrom(file.FullName);
                FolderInfos.AddRange(infos);
            }

            Console.WriteLine("FINISH get folder infos");

            //InitializeOutputFolder();

            if (!TemplateFile.Exists)
                throw new Exception("Missing template file...");
        }

        public DirectoryInfo DataFolder => new DirectoryInfo(@"../../data");
        public DirectoryInfo FullPathFolder => new DirectoryInfo(@"../../fullpath");
        public DirectoryInfo OutputFolder => new DirectoryInfo(@"../../output");
        public FileInfo TemplateFile => new FileInfo(@"../../template/index.html");
        public string[] LocalFiles => DataFolder.EnumerateFiles().Select(f => f.Name).ToArray();

        public List<FolderPair> PossiblePairs { get; set; }
        public List<FolderInfo> FolderInfos { get; set; }

        private void InitializeOutputFolder()
        {
            if (!OutputFolder.Exists)
                OutputFolder.Create();

            var files = OutputFolder.EnumerateFiles();
            foreach (var file in files)
                file.Delete();
        }

        private IEnumerable<FolderPair> GetFolderPairFrom(string file)
        {
            var fileName = Path.GetFileName(file);

            Console.WriteLine("GET folder pair from " + fileName);

            var lines = File.ReadAllLines(file);
            var folderNames = lines
                .Select(line => line.Split(',')[1])
                .ToArray();

            var query = from n1 in folderNames
                        where !string.IsNullOrEmpty(n1.Trim())
                        from n2 in folderNames
                        where !string.IsNullOrEmpty(n2.Trim())
                        select new FolderPair
                        {
                            Parent = n1,
                            Child = n2,
                            RegexDirectChild = $@"{n1} \\{n2}".Replace('(', ' ').Replace(')', ' '),
                            RegexNonDirectChild = $@"{n1} \\.+\\{n2}".Replace('(', ' ').Replace(')', ' '),
                            FileName = fileName
                        };

            return query;
        }

        private IEnumerable<FolderInfo> GetFolderInfosFrom(string file)
        {
            var fileName = Path.GetFileName(file);

            Console.WriteLine("Get folder info from " + fileName);

            var lines = File.ReadAllLines(file);

            var folders = lines.Select(line =>
                                       {
                                           var parts = line.Split(',');

                                           if (parts.Length != 3)
                                           {
                                               return null;
                                           }

                                           return new FolderInfo
                                           {
                                               DataRoomId = parts[0],
                                               FolderId = parts[1],
                                               FullPath = parts[2],
                                               FileName = fileName
                                           };
                                       });

            return folders.Where(f => f != null);
        }
    }
}