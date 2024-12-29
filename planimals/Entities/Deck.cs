﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms.VisualStyles;

public partial class Deck : Stack<int>
{
    Game game;
    Random rnd;
    public StringBuilder sb;
    private int size;

    public Deck(Game g) : base()
    {
        rnd = new Random();
        sb = new StringBuilder();
        game = g;
        size = 10;
    }
    private int GetNumberOfOrganisms()
    {
        int count = 0;
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) AS num FROM Organisms", sqlConnection);
            sqlConnection.Open();
            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read()) count = int.Parse(reader["num"].ToString());
            }
        }
        return count;
    }
    public void GenerateDeck()
    {
        int randIdx;
        sb.Append(',');
        for (int i = 0; i < size; i++)
        {
            randIdx = rnd.Next(1, GetNumberOfOrganisms() + 1);
            Push(randIdx);
            if (i != 14) sb.Append(randIdx + ",");
            else sb.Append(randIdx);
        }
    }
    public string GetScientificNameFromDeck()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            if (Count != 0)
            {
                SqlCommand cmd = new SqlCommand($"WITH NumberedRows AS ( SELECT Scientific_name, Common_name, ROW_NUMBER() OVER(ORDER BY Scientific_name) AS RowNum FROM Organisms ) SELECT Scientific_name, Common_name FROM NumberedRows WHERE RowNum = {game.deck.Pop()};", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader["Scientific_name"].ToString();
                    }
                }
                sqlConnection.Close();
            }
        }
        return null;
    }
    public void DrawCard(object sender, EventArgs e)
    {
        if (game.playerHand.Count + game.playerChain.CountAll() < 15)
        {
            string sciname;
            for (int i = 0; i < 3; i++)
            {
                sciname = GetScientificNameFromDeck();
                using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
                {
                    SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM Organisms WHERE Scientific_name='{sciname}'", sqlConnection);
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Card c = new Card(
                                game,
                                sciname,
                                reader["Common_name"].ToString(),
                                reader["Description"].ToString(),
                                Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{sciname}.jpg"),
                                (int)reader["Hierarchy"],
                                reader["Habitat"].ToString(),
                                new Point(Card.cardWidth * game.playerHand.Count, game.form.workingHeight - game.form.ClientRectangle.Width / 10),
                                false
                                );
                            game.playerHand.Add(c);
                            if (game.username != "") c.PushToHand();
                            game.form.Controls.Add(c);
                        }
                    }
                    if (game.deck.Count > 0)
                    {
                        if (game.username != string.Empty)
                        {
                            SqlCommand removeCard = new SqlCommand($"UPDATE Games SET Deck=LEFT(Deck, LEN(DECK) - 2) WHERE Username='{game.username}'", sqlConnection);
                            removeCard.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        game.form.Display("the deck is empty");
                        game.form.drawCardButton.Enabled = false;
                        game.form.drawCardButton.Hide();
                        if (!game.playerHand.IsHot()) game.form.Display("Hooray");

                    }
                    sqlConnection.Close();

                    //TODO! update prevLocations of cards in Chain and Hand
                }
            }
        }
        else
        {
            if (game.playerHand.IsHot()) game.form.Display("i think you can build a chain out of cards on your hand");
        }
    }
}