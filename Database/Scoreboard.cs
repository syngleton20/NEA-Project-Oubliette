using System.Data;
using System;

namespace NEA_Project_Oubliette.Database
{
    internal static class Scoreboard
    {
        public static void SubmitScore(int score, int userID)
        {
            bool alreadyHasScore = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM Scoreboard WHERE UserID = @UserID", userID) > 0;

            if(!alreadyHasScore) DatabaseManager.ExecuteDDL("INSERT INTO Scoreboard (Score, UserID) VALUES (@Score, @UserID)", score, userID);
            else DatabaseManager.ExecuteDDL("UPDATE Scoreboard SET Score = @Score WHERE UserID = @UserID", score, userID);
        }

        public static void DisplayAll()
        {
            Display.Clear();
            GUI.Title("Scoreboard");

            bool hasScores = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM Scoreboard") > 0;

            if(!hasScores)
            {
                Display.WriteAtCentre("No scores found!");
                Console.WriteLine();
                GUI.Confirm();
                return;
            }

            DataTable scoreTable = DatabaseManager.QuerySQLIntoTable("SELECT U.Username, S.Score FROM Scoreboard S INNER JOIN User U ON U.UserID WHERE U.UserID = S.UserID ORDER BY S.Score DESC");
            string[] scores = new string[scoreTable.Rows.Count];

            for(int i = 0; i < scores.Length; i++)
                scores[i] = Display.SplitStringOverBufferWidth(scoreTable.Rows[i][0].ToString(), scoreTable.Rows[i][1].ToString());

            GUI.VerticalScrollView(scores);
            Console.WriteLine();
            GUI.Confirm("FINISH");
        }

        public static bool IsHighScore(int score, int userID)
        {
            bool hasScores = DatabaseManager.QueryRowScalarValue("SELECT COUNT(*) FROM Scoreboard") > 0;

            if(!hasScores)
                return false;

            DatabaseManager.QuerySQL("SELECT Score FROM Scoreboard WHERE UserID = @UserID", userID);
            DatabaseManager.Reader.Read();

            int queriedScore = DatabaseManager.Reader.GetInt32("Score");
            return score > queriedScore;
        }
    }
}