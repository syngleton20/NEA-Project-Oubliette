using System.Collections.Generic;
using NEA_Project_Oubliette.Maps;
using System.Data;

namespace NEA_Project_Oubliette.Database
{
    internal static class MapBrowser
    {
        private static List<MapProfile> currentMapList = new List<MapProfile>();

        public static void Start()
        {
            Display.Clear();
            GUI.Title("Map Browser");

            DataTable mapTable = DatabaseManager.QuerySQLIntoTable("SELECT M.Name, U.Username FROM Map M, User U INNER JOIN Author A ON A.UserID = U.UserID");
            string[] maps = new string[mapTable.Rows.Count];

            for (int i = 0; i < maps.Length; i++)
                maps[i] = Display.SplitStringOverBufferWidth(mapTable.Rows[i][0].ToString(), mapTable.Rows[i][1].ToString());

            GUI.VerticalMenu(maps);
        }
    }
}