using System;
using System.IO;

namespace Sciendo.Common.IO
{
    public abstract class ContentWriterBase: IContentWriter
    {
        protected readonly IDirectory Directory;

        protected ContentWriterBase(IDirectory directory)
        {
            Directory = directory;
        }

        public abstract void Do(string fromPath, string toPath);

        protected string StripAllIllegalChars(string toPath)
        {
            var cleanToPath = StripIllegalChars(toPath, Path.GetInvalidPathChars());
            var cleanToPathFileName = StripIllegalChars(Path.GetFileName(cleanToPath), Path.GetInvalidFileNameChars());
            return string.IsNullOrEmpty(cleanToPath) ? string.Empty : Path.Combine((Path.GetDirectoryName(cleanToPath))??string.Empty, cleanToPathFileName);
        }


        protected string StripIllegalChars(string input, char[] illegalCharsSet, string replaceWith ="_")
        {
            string result = input;

            foreach (char achr in illegalCharsSet)
            {
                var charPos = input.IndexOf(achr);
                if (charPos > 0)
                {
                    result = result.Replace(achr.ToString(), replaceWith);
                }
            }
            return result;
        }


        protected void DoOperation(string fromPath, string toPath, Action<string, string> action)
        {
            var cleanToPath = StripAllIllegalChars(toPath);

            if (File.Exists(fromPath))
            {
                var toDirectoryPath = Path.GetDirectoryName(cleanToPath);
                if (!string.IsNullOrEmpty(toDirectoryPath) &&!System.IO.Directory.Exists(toDirectoryPath))
                    System.IO.Directory.CreateDirectory(toDirectoryPath);
                if (!File.Exists(cleanToPath))
                    action(fromPath, cleanToPath);
                return;
            }
            if (System.IO.Directory.Exists(fromPath))
            {
                if (!System.IO.Directory.Exists(cleanToPath))
                    System.IO.Directory.CreateDirectory(cleanToPath);
                var childDirectories = Directory.GetTopLevel(fromPath);
                foreach (var childDirectory in childDirectories)
                {
                    var childDirectoryParts = childDirectory.Split(Path.DirectorySeparatorChar);
                    Do(childDirectory, $"{cleanToPath}{Path.DirectorySeparatorChar}{childDirectoryParts[childDirectoryParts.Length-1]}");
                }
                var childFiles = Directory.GetFiles(fromPath, SearchOption.TopDirectoryOnly);
                foreach (var childFile in childFiles)
                {
                    var childFileOnly = Path.GetFileName(childFile);
                    if(!string.IsNullOrEmpty(childFileOnly))
                        Do(childFile, Path.Combine(cleanToPath, childFileOnly));
                }
            }
        }


    }
}
