using System.Text;
using System.IO;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Allows files to be easily read from and written to, from within the local bin/Debug/netcoreapp3.1/ directory</summary>
    internal static class FileHandler
    {
        ///<summary>Creates a directory inside the bin/Debug/netcoreapp3.1/ directory</summary>
        private static void CreateDirectory(string directoryName) => Directory.CreateDirectory(GetPath(directoryName));

        ///<summary>Checks if a directory exists inside the bin/Debug/netcoreapp3.1/ directory</summary>
        private static bool DirectoryExists(string directoryName) => Directory.Exists(GetPath(directoryName));

        ///<summary>Returns a path to the AppData directory on Windows, and the Application Support directory on macOS</summary>
        private static string GetPath(string name)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "NEA-Project-Oubliette", name);

            return path;
        }

        ///<summary>Adds the necessary project directories if they do not already exist</summary>
        public static void Setup()
        {
            if(!DirectoryExists("maps")) CreateDirectory("maps");
            if(!DirectoryExists("maps/custom")) CreateDirectory("maps/custom");
            if(!DirectoryExists("maps/downloads")) CreateDirectory("maps/downloads");
            if(!DirectoryExists("maps/.hidden")) CreateDirectory("maps/.hidden");
            if(!DirectoryExists("saves")) CreateDirectory("saves");

            if(!FileExists("maps/start.map")) WriteToFile("maps/start.map", "start\n20#\\#12^#5^#\\#12.#5.#\\9#4.#5.#\\#7^#4.#5.#\\#7.^4.3#2.2#\\#12.3^2.^#\\#7.#10.#\\9#10.#\\#8^10.#\\#13.6#\\#5.2#6.#4^#\\#5.2^6.^4.#\\#18.#\\#13.#4.#\\#2.3#2.3#3.6#\\#2.#2^2.2^#3.5^#\\#2.#6.#8.#\\#2.#6.#4.2#2.#\\20#\nP 0 1,2 20");
            if(!FileExists("maps/.hidden/Untitled.map")) WriteToFile("maps/.hidden/Untitled.map", "Untitled\n20#\\#18^#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\#18.#\\20#");

            if(FileExists("maps/.hidden/.temp.map")) DeleteFile("maps/.hidden/.temp.map");
        }

        ///<summary>Writes a file into the bin/Debug/netcoreapp3.1/ directory</summary>
        public static void WriteToFile(string fileName, string fileContent, bool appendFile = false)
        {
            using (StreamWriter writer = new StreamWriter(GetPath(fileName), appendFile))
                writer.Write(fileContent);
        }

        ///<summary>Deletes a file in bin/Debug/netcoreapp3.1/ directory</summary>
        public static void DeleteFile(string fileName) => File.Delete(GetPath(fileName));

        ///<summary>Returns the number of files in a directory</summary>
        public static int GetNumberOfFilesInDirectory(string directoryName) => new DirectoryInfo(GetPath(directoryName)).GetFiles().Length;

        ///<summary>Checks if a file exists inside the bin/Debug/netcoreapp3.1/ directory</summary>
        public static bool FileExists(string fileName) => File.Exists(GetPath(fileName));

        ///<summary>Reads a file from the bin/Debug/netcoreapp3.1/ directory</summary>
        public static string ReadFile(string fileName)
        {
            StringBuilder fileContent = new StringBuilder();

            using (StreamReader reader = new StreamReader(GetPath(fileName)))
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
        public static FileInfo[] GetFilesInDirectory(string directoryName) => new DirectoryInfo(GetPath(directoryName)).GetFiles();
    }
}