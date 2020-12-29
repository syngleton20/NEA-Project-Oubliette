using System.Collections.Generic;
using NEA_Project_Oubliette.Maps;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System;

namespace NEA_Project_Oubliette.Database
{
    internal static class MapBrowser
    {
        private static void ShowMapProfile(MapProfile profile)
        {
            Display.Clear();
            GUI.Title(profile.Name + " by " + profile.Author);

            if(!GUI.YesOrNo("Are you sure you want to download this map?"))
                return;

            Download(profile.ID);
        }

        private static void Download(int mapID)
        {
            DataTable mapQuery = DatabaseManager.QuerySQLIntoTable("SELECT * FROM Map WHERE MapID = @MapID", mapID);

            string mapData = mapQuery.Rows[0][3].ToString();
            mapData = mapData.Replace("\r", "");

            MySqlDataReader authorQuery = DatabaseManager.QuerySQL("SELECT U.Username FROM Map M, User U INNER JOIN Author A ON A.UserID = U.UserID WHERE A.AuthorID = M.AuthorID AND M.MapID = @MapID", mapID);
            authorQuery.Read();

            string authorName = authorQuery.GetString("Username");

            authorQuery.Close();
            authorQuery.Dispose();

            bool fileAlreadyExists = FileHandler.FileExists($"maps/downloads/{mapQuery.Rows[0][2].ToString()}-{authorName}.map");

            try
            {
                Map map = MapFormatter.Deserialize(mapData);
                FileHandler.WriteToFile($"maps/downloads/{map.Name}-{authorName}.map", MapFormatter.Serialize(map));
                DatabaseManager.ExecuteDDL("UPDATE Map SET Downloads = Downloads + 1 WHERE MapID = @MapID", mapID);

                Display.Clear();
                GUI.Title("Download - Complete");
                Display.WriteAtCentre($"Successfully {(fileAlreadyExists ? "updated" : "downloaded")} '{map.Name}'.");

                Console.WriteLine();
                GUI.Confirm();
            }
            catch(Exception exception)
            {
                Debug.Error(exception);
                Display.Clear();
                GUI.Title("Download - Failed");

                Display.WriteAtCentre("An error occurred while downloading this map.");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        private static void Upload(Map map)
        {
            if(AccountManager.Account.GetType() != typeof(AuthorAccount)) return;

            int alreadyUploadedMapID = -1;
            bool alreadyUploaded = false;
            DataTable uploadedMaps = DatabaseManager.QuerySQLIntoTable("SELECT * FROM Map WHERE AuthorID = @AuthorID", (AccountManager.Account as AuthorAccount).AuthorID);

            for (int i = 0; i < uploadedMaps.Rows.Count; i++)
            {
                if(uploadedMaps.Rows[i][2].ToString() == map.Name)
                {
                    alreadyUploaded = true;
                    alreadyUploadedMapID = (int)uploadedMaps.Rows[i][0];
                    break;
                }
            }

            try
            {
                Display.Clear();
                GUI.Title("Upload Map");

                if(GUI.YesOrNo($"Are you sure you want to {(alreadyUploaded ? "update" : "upload")} {map.Name}?"))
                {
                    int authorID = (AccountManager.Account as AuthorAccount).AuthorID;
                    string mapData = MapFormatter.Serialize(map);

                    if(!alreadyUploaded) DatabaseManager.ExecuteDDL("INSERT INTO Map(AuthorID, Name, Data) VALUES (@AuthorID, @Name, @Data)", authorID, map.Name, mapData);
                    else if(alreadyUploadedMapID > -1) DatabaseManager.ExecuteDDL("UPDATE Map SET Data = @Data WHERE MapID = @MapID", mapData, alreadyUploadedMapID);

                    Display.Clear();
                    GUI.Title("Upload Map - Complete");

                    Display.WriteAtCentre($"Successfully {(alreadyUploaded ? "updated" : "uploaded")} '{map.Name}'.");
                    Console.WriteLine();
                    GUI.Confirm();
                }
            }
            catch
            {
                Display.Clear();
                GUI.Title("Upload Map - Error");

                Display.WriteAtCentre("The map you are trying to upload seems to be corrupted.");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        public static void ShowBrowserMenu()
        {
            Display.Clear();
            GUI.Title("Map Browser");

            DataTable mapTable = DatabaseManager.QuerySQLIntoTable("SELECT M.MapID, M.Name, U.Username FROM Map M, User U INNER JOIN Author A ON A.UserID = U.UserID WHERE A.AuthorID = M.AuthorID");
            MapProfile[] mapProfiles = new MapProfile[mapTable.Rows.Count];

            for (int i = 0; i < mapProfiles.Length; i++)
                mapProfiles[i] = new MapProfile((int)mapTable.Rows[i][0], mapTable.Rows[i][1].ToString(), mapTable.Rows[i][2].ToString());

            List<string> maps = new List<string>(Array.ConvertAll<MapProfile, string>(mapProfiles, map => Display.SplitStringOverBufferWidth(map.Name, map.Author)));

            if(maps.Count > 0)
            {
                maps.Insert(0, "Back");
                int chosenMapIndex = GUI.VerticalMenu(maps.ToArray()) - 1;

                if(chosenMapIndex <= -1)
                {
                    Display.Clear();
                    return;
                }

                ShowMapProfile(mapProfiles[chosenMapIndex]);
            }
            else
            {
                Display.WriteAtCentre("No maps found!");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        public static void ShowDownloadsMenu()
        {
            Display.Clear();
            GUI.Title("Downloads");

            FileInfo[] downloads = FileHandler.GetFilesInDirectory("maps/downloads");
            List<string> maps = new List<string>(Array.ConvertAll<FileInfo, string>(downloads, file => Display.SplitStringOverBufferWidth(file.Name.Split('.')[0].Split('-')[0], file.Name.Split('.')[0].Split('-')[1])));

            if(maps.Count > 0)
            {
                maps.Insert(0, "Back");
                int chosenMapIndex = GUI.VerticalMenu(maps.ToArray()) - 1;

                if(chosenMapIndex <= -1)
                {
                    Display.Clear();
                    return;
                }

                try
                {
                    Game.Current?.Stop();
                    Game.Current = new Game(GameType.Game, $"downloads/{downloads[chosenMapIndex].Name}");
                    Game.Current.Start();
                }
                catch
                {
                    Display.Clear();
                    GUI.Title("Play Map - Error");

                    Display.WriteAtCentre("The map you are trying to play seems to be corrupted.");
                    Console.WriteLine();
                    GUI.Confirm();
                }

            }
            else
            {
                Display.WriteAtCentre("No maps found!");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        public static void ShowUploadMenu()
        {
            Display.Clear();
            GUI.Title("Upload Map");

            FileInfo[] customMaps = FileHandler.GetFilesInDirectory("maps/custom");
            List<string> maps = new List<string>(Array.ConvertAll<FileInfo, string>(customMaps, file => file.Name.Split('.')[0]));

            if(maps.Count > 0)
            {
                maps.Insert(0, "Back");
                int chosenMapIndex = GUI.VerticalMenu(maps.ToArray()) - 1;

                if(chosenMapIndex <= -1)
                {
                    Display.Clear();
                    return;
                }

                Map map = MapFormatter.Deserialize(FileHandler.ReadFile($"maps/custom/{customMaps[chosenMapIndex].Name}"));
                Upload(map);
            }
            else
            {
                Display.WriteAtCentre("No maps found!");
                Console.WriteLine();
                GUI.Confirm();
            }
        }
    }
}