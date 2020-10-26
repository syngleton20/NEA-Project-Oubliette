using System.Text;
using System.IO;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Allows files to be easily read from and written to, from within the local bin/Debug/netcoreapp3.1/ directory</summary>
    internal static class FileHandler
    {
        ///<summary>Creates a directory inside the bin/Debug/netcoreapp3.1/ directory</summary>
        private static void CreateDirectory(string directoryName)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + directoryName;
            Directory.CreateDirectory(directoryPath);
        }

        ///<summary>Checks if a directory exists inside the bin/Debug/netcoreapp3.1/ directory</summary>
        private static bool DirectoryExists(string directoryName)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + directoryName;
            return Directory.Exists(directoryPath);
        }

        ///<summary>Adds the necessary project directories if they do not already exist</summary>
        public static void Setup()
        {
            if(!DirectoryExists("maps")) CreateDirectory("maps");
            if(!DirectoryExists("saves")) CreateDirectory("saves");
            if(!DirectoryExists("data")) CreateDirectory("data");

            if(!FileExists("data/default.set")) WriteToFile("data/default.set", "#,  ,0\n.,░░,1\n^,▒▒,0");
            if(!FileExists("maps/start.map")) WriteToFile("maps/start.map", "start\n20#\\#18^#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\20#\ndefault\nP 0 2,3 20");
        }

        ///<summary>Writes a file into the bin/Debug/netcoreapp3.1/ directory</summary>
        public static void WriteToFile(string fileName, string fileContent, bool appendFile = false)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;

            using (StreamWriter writer = new StreamWriter(filePath, appendFile))
                writer.Write(fileContent);
        }

        ///<summary>Returns the number of files in a directory</summary>
        public static int GetNumberOfFilesInDirectory(string directoryName)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + directoryName;
            return new DirectoryInfo(directoryPath).GetFiles().Length;
        }

        ///<summary>Checks if a file exists inside the bin/Debug/netcoreapp3.1/ directory</summary>
        public static bool FileExists(string fileName)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            return File.Exists(filePath);
        }

        ///<summary>Reads a file from the bin/Debug/netcoreapp3.1/ directory</summary>
        public static string ReadFile(string fileName)
        {
            StringBuilder fileContent = new StringBuilder();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;

            using (StreamReader reader = new StreamReader(filePath))
            {
                do
                {
                    if(fileContent.ToString() != "") fileContent.Append('\n'); // This is to avoid adding a newline character to the end of the file
                    fileContent.Append(reader.ReadLine());
                }
                while (!reader.EndOfStream);
            }

            return fileContent.ToString();
        }

        ///<summary>Returns an array of files from a directory</summary>
        public static FileInfo[] GetFilesInDirectory(string directoryName)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory + directoryName;
            return new DirectoryInfo(directoryPath).GetFiles();
        }
    }
}