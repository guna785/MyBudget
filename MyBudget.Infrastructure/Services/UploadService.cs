﻿using MyBudget.Application.Extensions;
using MyBudget.Application.Interfaces.Services;
using MyBudget.Application.Requests;

namespace MyBudget.Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        public string UploadAsync(UploadRequest request)
        {
            if (request.Data == null)
            {
                return string.Empty;
            }

            MemoryStream streamData = new(request.Data);
            if (streamData.Length > 0)
            {
                string folder = request.UploadType.ToDescriptionString();
                string folderName = Path.Combine("Files", folder);
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                bool exists = System.IO.Directory.Exists(pathToSave);
                if (!exists)
                {
                    _ = System.IO.Directory.CreateDirectory(pathToSave);
                }

                string fileName = request.FileName.Trim('"');
                string fullPath = Path.Combine(pathToSave, fileName);
                string dbPath = Path.Combine(folderName, fileName);
                if (File.Exists(dbPath))
                {
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = NextAvailableFilename(fullPath);
                }
                using (FileStream stream = new(fullPath, FileMode.Create))
                {
                    streamData.CopyTo(stream);
                }
                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private static readonly string numberPattern = " ({0})";

        public static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
            {
                return path;
            }

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
            {
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));
            }

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            //if (tmp == pattern)
            //throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
            {
                return tmp; // short-circuit if no matches
            }

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                {
                    min = pivot;
                }
                else
                {
                    max = pivot;
                }
            }

            return string.Format(pattern, max);
        }
    }
}
