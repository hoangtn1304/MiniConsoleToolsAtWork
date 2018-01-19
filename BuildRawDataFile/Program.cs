using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildRawDataFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var setA = GetSetA();

            Console.WriteLine($"LOAD set A successfully : {setA.Length} items");

            var setB = GetSetB();

            Console.WriteLine($"LOAD set B successfully : {setB.Length} items");

            const string output = @"../../output.txt";
            if (File.Exists(output))
            {
                File.Delete(output);
            }

            using (var writter = File.AppendText(output))
            {
                foreach (var a in setA)
                {
                    foreach (var b in setB)
                    {
                        if (string.IsNullOrWhiteSpace(a.FolderName) && string.IsNullOrWhiteSpace(b.FolderName))
                        {
                            continue;
                        }

                        if (!a.FolderName.Equals(b.FolderName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        var entity = new Entity
                        {
                            FolderName = b.FolderName,
                            ViewCount = b.ViewCount,
                            Role = b.Role,
                            DataRoomCount = a.DataRoomCount
                        };

                        Console.WriteLine(entity);
                        writter.WriteLine(entity);
                    }
                }
            }

            Console.WriteLine("Finish");
        }

        private static RoleFolderView[] GetSetB()
        {
            var lines = File.ReadAllLines(@"../../Raw/rolefolder-by-view-count.csv");
            var query = lines.Select(line =>
                                     {
                                         var parts = line.Split('|');
                                         string folderName = parts[1];
                                         string viewCount = parts[2];

                                         if (parts.Length > 3)
                                         {
                                             folderName = "";

                                             int length = parts.Length;
                                             for (int i = 1; i < length - 1; i++)
                                             {
                                                 folderName += parts[i];
                                             }

                                             viewCount = parts[length - 1];
                                         }

                                         return new RoleFolderView
                                         {
                                             Role = parts[0],
                                             FolderName = folderName,
                                             ViewCount = Convert.ToInt32(viewCount)
                                         };
                                     });
            return query.ToArray();
        }

        private static FolderDataRoom[] GetSetA()
        {
            var lines = File.ReadAllLines(@"../../Raw/foders-by-dataroom-count.csv");
            var query = lines.Select(line =>
                                     {
                                         var parts = line.Split('|');
                                         string folderName = parts[0];
                                         string dataRoomCount = parts[1];

                                         if (parts.Length > 2)
                                         {
                                             folderName = "";
                                             int length = parts.Length;
                                             for (int i = 0; i < length - 1; i++)
                                             {
                                                 folderName += parts[i];
                                             }
                                             dataRoomCount = parts[length - 1];
                                         }

                                         return new FolderDataRoom
                                         {
                                             FolderName = folderName,
                                             DataRoomCount = Convert.ToInt32(dataRoomCount)
                                         };
                                     });
            return query.ToArray();
        }
    }

    public class FolderDataRoom
    {
        public string FolderName { get; set; }
        public int DataRoomCount { get; set; }
    }

    public class RoleFolderView
    {
        public string Role { get; set; }
        public string FolderName { get; set; }
        public int ViewCount { get; set; }
    }

    public class Entity
    {
        public string Role { get; set; }
        public string FolderName { get; set; }
        public int DataRoomCount { get; set; }
        public int ViewCount { get; set; }
        public override string ToString()
        {
            return $"{Role}|{FolderName}|{DataRoomCount}|{ViewCount}";
        }
    }
}
