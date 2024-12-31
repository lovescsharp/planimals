﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

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
        if (Count > 1 && game.playerChain.Count != 0 && game.playerHand.Count != 0)
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand checkRelation;
                sqlConnection.Open();
                for (int i = 0; i < Count; i++)
                {
                    for (int j = 0; j < Count - 1; j++)
                    {
                        if (i == j) continue;
                        checkRelation = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer = '{this[i].ScientificName}' AND Consumed = '{this[j].ScientificName}'", sqlConnection);
                        int b = (int)checkRelation.ExecuteScalar();
                        if (b == 1)
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
        else if (game.playerChain.Count != 0) return true;
        else if (game.deck.Count != 0) return true;
        else return false;
    }
    public void LoadHand()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand pullHand = new SqlCommand(
                    $"SELECT Hand.CardID, Organisms.Common_name, Organisms.Habitat, Organisms.Hierarchy, Organisms.Description " +
            $"FROM Hand " +
                    $"JOIN Organisms ON Hand.CardID = Organisms.Scientific_name " +
                    $"WHERE Username='{game.username}'", sqlConnection);
            sqlConnection.Open();;
            SqlDataReader reader = pullHand.ExecuteReader();
            while (reader.Read())
            {
                Card c = new Card(
                    game,
                    reader["CardID"].ToString(),
                    reader["Common_name"].ToString(),
                    reader["Description"].ToString(),
                    Path.Combine(Environment.CurrentDirectory, "assets", "photos", reader["CardID"].ToString() + ".png"),
                    (int)reader["Hierarchy"],
                    reader["Habitat"].ToString(),
                    game.form.drawCardButton.Location,
                    false
                    );
                c.prevLocation = new Point(Card.cardWidth * Count, game.form.workingHeight - game.form.workingWidth / 10);
                Add(c);
                game.form.Controls.Add(c);
            }
            sqlConnection.Close(); 
        }
    }
}
