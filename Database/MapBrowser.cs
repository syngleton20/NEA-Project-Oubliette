using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Maps;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System;

namespace NEA_Project_Oubliette.Database
{
    ///<summary>Provides a GUI interface for viewing, downloading and playing maps on the database</summary>
    internal static class MapBrowser
    {
        ///<summary>Shows some basic information about a map and allows the user to choose whether they want to download the map or not</summary>
        private static void ShowMapProfile(MapProfile profile)
        {
            Display.Clear();
            GUI.Title(profile.Name + " by " + profile.Author);

            if(!GUI.YesOrNo("Are you sure you want to download this map?"))
                return;

            Download(profile.ID);
        }

        ///<summary>Attempts to download a map or update a previously downloaded map in the game's downloads directory, using its mapID</summary>
        private static void Download(int mapID)
        {
            Map map = null;
            DataTable mapQuery = DatabaseManager.QuerySQLIntoTable("SELECT * FROM Map WHERE MapID = @MapID", mapID);

            string mapData = mapQuery.Rows[0][3].ToString();
            mapData = mapData.Replace("\r", ""); // Removes any line feed characters which may have been inadvertently placed in the map file when written on macOS

            DatabaseManager.QuerySQL("SELECT U.Username FROM Map M, User U INNER JOIN Author A ON A.UserID = U.UserID WHERE A.AuthorID = M.AuthorID AND M.MapID = @MapID", mapID); // Selects a collection of maps and the usernames of their authors

            DatabaseManager.Reader.Read();
            string authorName = DatabaseManager.Reader.GetString("Username");

            bool fileAlreadyExists = FileHandler.FileExists($"maps/downloads/{mapQuery.Rows[0][2].ToString()}-{authorName}.map");

            try
            {
                map = MapFormatter.Deserialize(mapData);
                FileHandler.WriteToFile($"maps/downloads/{map.Name}-{authorName}.map", MapFormatter.Serialize(map)); // Names the file with its map name and its author's name separate by a '-' symbol, to prevent maps with the same name overwriting each other
                DatabaseManager.ExecuteDDL("UPDATE Map SET Downloads = Downloads + 1 WHERE MapID = @MapID", mapID); // Increments the number of downloads the map has on the database

                Display.Clear();
                GUI.Title("Download - Complete");
                Display.WriteAtCentre($"Successfully {(fileAlreadyExists ? "updated" : "downloaded")} '{map.Name}'.");

                Console.WriteLine();
                GUI.Confirm();
            }
            catch(Exception exception)
            {
                // If the map could not be downloaded, either because it is corrupted or deprecated, an error screen is shown to the user
                Debug.Error(exception);
                Display.Clear();
                GUI.Title("Download - Failed");

                Display.WriteAtCentre("An error occurred while downloading this map.");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        ///<summary>Attempts to upload a map or update an existing map on the database</summary>
        private static void Upload(Map map)
        {
            // Notifies user account holders that they need to upgrade their account to an author account if they want to upload maps
            if(AccountManager.Account.GetType() != typeof(AuthorAccount))
            {
                Display.Clear();
                GUI.Title("Upload Map - Cancelled");

                Display.WriteAtCentre("You cannot upload maps unless you upgrade your account");
                Display.WriteAtCentre("to an author account. This can be done in");
                Display.WriteAtCentre("Account → Upgrade Account, where you will need");
                Display.WriteAtCentre("to enter your email address.");

                Console.WriteLine();
                GUI.Confirm();
                return;
            }

            // Cancels the upload if there is no player entity in the map, rendering it unplayable
            if(!map.Collection.ContainsEntityOfType<Player>())
            {
                Display.Clear();
                GUI.Title("Upload Map - Cancelled");

                Display.WriteAtCentre("The map you are trying to upload does not contain");
                Display.WriteAtCentre("a player entity. Maps must contain a player entity");
                Display.WriteAtCentre("for them to be playable. Please add one and try again.");

                Console.WriteLine();
                GUI.Confirm();
                return;
            }

            int alreadyUploadedMapID = -1;
            bool alreadyUploaded = false;
            DataTable uploadedMaps = DatabaseManager.QuerySQLIntoTable("SELECT * FROM Map WHERE AuthorID = @AuthorID", (AccountManager.Account as AuthorAccount).AuthorID);

            // Checks if this map has already been uploaded by the author
            for(int i = 0; i < uploadedMaps.Rows.Count; i++)
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

                    if(!alreadyUploaded) DatabaseManager.ExecuteDDL("INSERT INTO Map(AuthorID, Name, Data) VALUES (@AuthorID, @Name, @Data)", authorID, map.Name, mapData); // Adds the new map to the Map table on the database
                    else if(alreadyUploadedMapID > -1) DatabaseManager.ExecuteDDL("UPDATE Map SET Data = @Data WHERE MapID = @MapID", mapData, alreadyUploadedMapID); // Or updates the already existing map in the Map table on the database

                    Display.Clear();
                    GUI.Title("Upload Map - Complete");

                    Display.WriteAtCentre($"Successfully {(alreadyUploaded ? "updated" : "uploaded")} '{map.Name}'.");
                    Console.WriteLine();
                    GUI.Confirm();
                }
            }
            catch
            {
                // If the map is in the wrong format or is deprecated, the upload is cancelled and an error screen is shown
                Display.Clear();
                GUI.Title("Upload Map - Error");

                Display.WriteAtCentre("The map you are trying to upload seems to be corrupted.");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        ///<summary>Displays maps on the database as a vertical list</summary>
        public static void ShowBrowserMenu()
        {
            Display.Clear();
            GUI.Title("Map Browser");

            // Populates the mapTable with a map's ID, name and number of downloads, as well as the author's username
            DataTable mapTable = DatabaseManager.QuerySQLIntoTable("SELECT M.MapID, M.Name, U.Username, M.Downloads FROM Map M, User U INNER JOIN Author A ON A.UserID = U.UserID WHERE A.AuthorID = M.AuthorID");
            MapProfile[] mapProfiles = new MapProfile[mapTable.Rows.Count];

            for(int i = 0; i < mapProfiles.Length; i++) mapProfiles[i] = new MapProfile((int)mapTable.Rows[i][0], mapTable.Rows[i][1].ToString(), mapTable.Rows[i][2].ToString(), (int)mapTable.Rows[i][3]);
            List<string> maps = new List<string>(Array.ConvertAll<MapProfile, string>(mapProfiles, map => Display.SplitStringOverBufferWidth(map.Name + " by " + map.Author, map.Downloads + "↓"))); // Converts the mapProfile array into a list of strings for easy printing to the screen

            // Displays the maps as a vertical list only if there is more than one
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

        ///<summary>Displays the contents of the game's downloads directory as a vertical list so that users and authors can download maps</summary>
        public static void ShowDownloadsMenu()
        {
            Display.Clear();
            GUI.Title("Downloads");

            FileInfo[] downloads = FileHandler.GetFilesInDirectory("maps/downloads");
            List<string> maps = new List<string>(Array.ConvertAll<FileInfo, string>(downloads, file => Display.SplitStringOverBufferWidth(file.Name.Split('.')[0].Split('-')[0], file.Name.Split('.')[0].Split('-')[1])));

            // Displays the maps as a vertical list only if there is more than one
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
                    Game.Current?.Start();
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

        ///<summary>Displays the contents of the game's custom maps directory as a vertical list so that authors can upload maps</summary>
        public static void ShowUploadMenu()
        {
            Display.Clear();
            GUI.Title("Upload Map");

            FileInfo[] customMaps = FileHandler.GetFilesInDirectory("maps/custom");
            List<string> maps = new List<string>(Array.ConvertAll<FileInfo, string>(customMaps, file => file.Name.Split('.')[0])); // Converts the array of file information into a list of strings for easy printing

            // Displays the maps as a vertical list only if there is more than one
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