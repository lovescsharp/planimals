using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.IO;
using System.Windows.Forms;

public class Hand : List<Card>
{
    private Game game;
    public Hand(Game g) : base() => game = g;
    public void ShiftCards()
    {
        for (int i = 0; i < Count; i++)
        this[i].Location =
        this[i].prevLocation = 
        new Point(Card.cardWidth * i, game.form.workingHeight - Card.cardHeight);
    }
    public bool IsHot() 
    {
        if (Count <= 1)
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand checkRelation;
                for (int i = 0; i < Count; i++)
                {
                    for (int j = 1; j < Count - 1; j++)
                    {
                        if (i == j) continue;
                        checkRelation = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer = '{this[i]}' AND Consumed = '{this[j]}'", sqlConnection);
                        sqlConnection.Open();
                        int b = (int)checkRelation.ExecuteScalar();
                        if (b == 0)
                        {
                            sqlConnection.Close();
                            return true;
                        }
                    }
                }
                sqlConnection.Close();
                return false;
            }
        }
        else return false;
    }
    public void LoadHand(SqlConnection sqlConnection)
    {
        SqlCommand pullHand = new SqlCommand(
                $"SELECT Hand.CardID, Organisms.Common_name, Organisms.Habitat, Organisms.Hierarchy, Organisms.Description " +
        $"FROM Hand " +
                $"JOIN Organisms ON Hand.CardID = Organisms.Scientific_name " +
                $"WHERE Username='{game.username}'", sqlConnection);
        using (SqlDataReader reader = pullHand.ExecuteReader())
        {
            while (reader.Read())
            {
                Card c = new Card(
                    game,
                    reader["CardID"].ToString(),
                    reader["Common_name"].ToString(),
                    reader["Description"].ToString(),
                    Path.Combine(Environment.CurrentDirectory, "assets", "photos", reader["CardID"].ToString() + ".jpg"),
                    (int)reader["Hierarchy"],
                    reader["Habitat"].ToString(),
                    new Point(Card.cardWidth * Count, game.form.Height - game.form.workingWidth / 10),
                    false
                    );
                Add(c);
                game.form.Controls.Add(c);
            }
        }
    }
}
