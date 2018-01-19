using System;
using System.IO;

namespace CheckFileStatus
{
    public static class Watcher
    {
        public static DateTime? LastFileRead;
        private static string _cachedContent;
        
        public static string Content
        {
            get
            {
                var path = @"../../test.txt";
                var lastFileChange = File.GetLastWriteTime(path);

                if (!LastFileRead.HasValue || lastFileChange > LastFileRead)
                {
                    LastFileRead = lastFileChange;
                    _cachedContent = File.ReadAllText(path);
                }

                return _cachedContent;
            }
        }
    }
}