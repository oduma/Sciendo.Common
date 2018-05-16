﻿using System;
using System.IO;

namespace Sciendo.Common.IO
{
    public abstract class ContentWriterBase: IContentWriter
    {
        protected readonly IDirectoryEnumerator DirectoryEnumerator;
        protected readonly IFileEnumerator FileEnumerator;

        protected ContentWriterBase(IDirectoryEnumerator directoryEnumerator, IFileEnumerator fileEnumerator)
        {
            DirectoryEnumerator = directoryEnumerator;
            FileEnumerator = fileEnumerator;
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
                if (!string.IsNullOrEmpty(toDirectoryPath) &&!Directory.Exists(toDirectoryPath))
                    Directory.CreateDirectory(toDirectoryPath);
                if (!File.Exists(cleanToPath))
                    action(fromPath, cleanToPath);
                return;
            }
            if (Directory.Exists(fromPath))
            {
                if (!Directory.Exists(cleanToPath))
                    Directory.CreateDirectory(cleanToPath);
                var childDirectories = DirectoryEnumerator.GetTopLevel(fromPath);
                foreach (var childDirectory in childDirectories)
                {
                    var childDirectoryParts = childDirectory.Split(Path.DirectorySeparatorChar);
                    Do(childDirectory, $"{cleanToPath}{Path.DirectorySeparatorChar}{childDirectoryParts[childDirectoryParts.Length-1]}");
                }
                var childFiles = FileEnumerator.Get(fromPath, SearchOption.TopDirectoryOnly);
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
