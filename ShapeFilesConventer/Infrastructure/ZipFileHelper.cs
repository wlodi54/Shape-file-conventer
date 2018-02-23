using System;
using System.IO;
using System.IO.Compression;
using System.Linq;


namespace ShapeFilesConventer.Infrastructure
{
    public static class ZipFileHelper
    {
        private const string ProjExt = ".prj";
        private const string ShpExt = ".shp";
        private const string ShxExt = ".shx";
        private const string DbfExt = ".dbf";
        private const string PathToZipDoesntExtists = "Path to zip file doesn't exists.";
        private const string RequeredFilesDoesntExtists = ".shp || .dbf doesn't exists.";
        private const string DestinationDoesntExtists = "Destination path doesn't exists.";

        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private static DirectoryInfo CreateDirectoryForFiles(string pathToZipFile)
        {
            var fileNameNoExt = pathToZipFile.Remove(pathToZipFile.LastIndexOf('.'));
            var pathToDirectory = Path.Combine(pathToZipFile, fileNameNoExt);
            DirectoryInfo directoryInfo =
                Directory.CreateDirectory(pathToDirectory);
            File.SetAttributes(pathToDirectory, FileAttributes.Normal);
            return directoryInfo;
        }
        /// <summary>
        /// This method extract file to new directory called like name of zipFile in zipFile directory.
        /// </summary>
        /// <param name="pathToZipFile">Path to zipFile</param>
        /// <returns>Full path to .shp file</returns>
        public static string ExtractZipFile(string pathToZipFile)
        {
            if (!File.Exists(pathToZipFile))
            {
                throw new Exception(PathToZipDoesntExtists);
            }
            DirectoryInfo directoryInfo = CreateDirectoryForFiles(pathToZipFile);

            Extract(pathToZipFile, directoryInfo.FullName, out string nameShpFile);

            return Path.Combine(directoryInfo.FullName, nameShpFile);
        }

        private static void Extract(string pathToZipFile, string destinationPath, out string nameShpFile)
        {
            nameShpFile = "";
            using (ZipArchive archive = ZipFile.OpenRead(pathToZipFile))
            {

                var countOfDbfFiles = archive.Entries.Count(x =>
                    x.FullName.EndsWith(DbfExt, StringComparison.OrdinalIgnoreCase));
                var countOfShpFiles = archive.Entries.Count(x =>
                    x.FullName.EndsWith(ShpExt, StringComparison.OrdinalIgnoreCase));
                if (countOfShpFiles == 0 && countOfDbfFiles == 0)
                {
                    throw new Exception(RequeredFilesDoesntExtists);

                }
                var shapeFile = archive.Entries.Where(x =>
                    x.FullName.EndsWith(DbfExt, StringComparison.OrdinalIgnoreCase) ||
                    x.FullName.EndsWith(ShpExt, StringComparison.OrdinalIgnoreCase) ||
                    x.FullName.EndsWith(ShxExt, StringComparison.OrdinalIgnoreCase) ||
                    x.FullName.EndsWith(ProjExt, StringComparison.OrdinalIgnoreCase));
                foreach (var zipArchiveEntry in shapeFile)
                {
                    zipArchiveEntry.ExtractToFile(Path.Combine(destinationPath, zipArchiveEntry.FullName));
                    File.SetAttributes(Path.Combine(destinationPath, zipArchiveEntry.FullName), FileAttributes.Normal);
                    if (zipArchiveEntry.FullName.EndsWith(ShpExt, StringComparison.OrdinalIgnoreCase))
                    {
                        nameShpFile = zipArchiveEntry.FullName;
                    }
                }
            }

        }
        /// <summary>
        /// This Method extract zip file to current destination
        /// </summary>
        /// <param name="pathToZipFile">Path to zipFile</param>
        /// <param name="destinationPath">Path to destination directory (must exists)</param>
        /// <returns>Full path to .shp file</returns>
        public static string ExtractZipFile(string pathToZipFile, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
            {
                throw new Exception(DestinationDoesntExtists);
            }
            if (!File.Exists(pathToZipFile))
            {
                throw new Exception(PathToZipDoesntExtists);
            }
            if (!IsDirectoryEmpty(destinationPath))
            {
                DeleteShapeFiles(destinationPath);
            }
            Extract(pathToZipFile, destinationPath, out string nameShpFile);

            return Path.Combine(destinationPath, nameShpFile);
        }

        private static void DeleteShapeFiles(string pathToSaveFile)
        {
            System.GC.Collect();
            GC.WaitForPendingFinalizers();
            try
            {
                Array.ForEach(Directory.GetFiles(pathToSaveFile), System.IO.File.Delete);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}