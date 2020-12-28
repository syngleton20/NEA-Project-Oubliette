using System.Collections.Generic;
using NEA_Project_Oubliette.Maps;
using System.Data;
using System;

namespace NEA_Project_Oubliette.Database
{
    internal static class MapBrowser
    {
        public static void Start()
        {
            Display.Clear();
            GUI.Title("Map Browser");

            DataTable mapTable = DatabaseManager.QuerySQLIntoTable("SELECT M.MapID, M.Name, U.Username FROM Map M, User U INNER JOIN Author A ON A.UserID = U.UserID");
            MapProfile[] mapProfiles = new MapProfile[mapTable.Rows.Count];

            for (int i = 0; i < mapProfiles.Length; i++)
                mapProfiles[i] = new MapProfile((int)mapTable.Rows[i][0], mapTable.Rows[i][1].ToString(), mapTable.Rows[i][2].ToString());

            List<string> maps = new List<string>(Array.ConvertAll<MapProfile, string>(mapProfiles, m => Display.SplitStringOverBufferWidth(m.Name, m.Author)));
            maps.Insert(0, "Back");

            int chosenMapIndex = GUI.VerticalMenu(maps.ToArray()) - 1;

            if(chosenMapIndex <= -1)
            {
                Display.Clear();
                return;
            }

            ShowMapProfile(mapProfiles[chosenMapIndex]);
        }

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

            try
            {
                Map map = MapFormatter.Deserialize(mapData);
                FileHandler.WriteToFile($"maps/custom/{map.Name}.map", MapFormatter.Serialize(map));

                Display.Clear();
                GUI.Title("Download - Complete");

                Display.WriteAtCentre($"Successfully downloaded {map.Name}.");
                Console.WriteLine();
                GUI.Confirm();
            }
            catch
            {
                Display.Clear();
                GUI.Title("Download - Failed");

                Display.WriteAtCentre("An error occurred while downloading this map.");
                Console.WriteLine();
                GUI.Confirm();
            }
        }

        public static void Upload(Map map)
        {
            if(AccountManager.Account.GetType() != typeof(AuthorAccount)) return;

            int authorID = (AccountManager.Account as AuthorAccount).AuthorID;
            string mapData = MapFormatter.Serialize(map);

            DatabaseManager.ExecuteDDL("INSERT INTO Map(AuthorID, Name, Data) VALUES (@AuthorID, @Name, @Data)", authorID, map.Name, mapData);
        }
    }
}