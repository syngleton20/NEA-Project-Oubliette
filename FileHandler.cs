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
        }

        ///<summary>Writes a file into the bin/Debug/netcoreapp3.1/ directory</summary>
        public static void WriteToFile(string fileName, string fileContent, bool appendFile = false)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;

            using (StreamWriter writer = new StreamWriter(fileName, appendFile))
                writer.Write(fileContent);
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
    }
}