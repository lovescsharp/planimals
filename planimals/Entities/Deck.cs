using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

public partial class Deck : Stack<int>
{
    Game game;
    Random rnd;
    public string deckStr;
    int size;

    public Deck(Game g) : base() // starter
    {
        rnd = new Random();
        deckStr = "";
        game = g;
        size = 20;
        GenerateDeck();
    }
    public Deck(Game g, string d) : base() // loader
    {
        rnd = new Random();
        deckStr = d;
        game = g;
        LoadDeck();
        size = Count;
    }
    private int GetNumberOfOrganisms()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Organisms", sqlConnection);
            sqlConnection.Open();
            int b = (int)sqlCommand.ExecuteScalar();
            sqlConnection.Close();
            return b;
        }
    }
    public void GenerateDeck()
    {
        Console.WriteLine($"size = {size}");
        int randIdx;
        int upperBound = GetNumberOfOrganisms() + 1;
        for (int i = 0; i < size; i++)
        {
            randIdx = rnd.Next(1, upperBound);
            Console.WriteLine(randIdx);
            Push(randIdx);
            deckStr += randIdx + ",";
        }
    }
    public string GetScientificNameFromDeck()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            int i = Pop();
            Console.WriteLine(i);
            deckStr = deckStr.Remove(deckStr.Length - 1);
            Console.WriteLine((int)Math.Log10(i) + 1);
            for (int j = 0; j < (int)Math.Floor(Math.Log10(i)) + 1; j++) deckStr = deckStr.Remove(deckStr.Length - 1);
            Console.WriteLine(deckStr);
            Console.WriteLine("--");
            SqlCommand cmd = new SqlCommand($"WITH NumberedRows AS(SELECT Scientific_name, ROW_NUMBER() OVER(ORDER BY Scientific_name) AS RowNum FROM Organisms) SELECT Scientific_name FROM NumberedRows WHERE RowNum = {i};", sqlConnection);
            sqlConnection.Open();
            string name = (string) cmd.ExecuteScalar();
            sqlConnection.Close();
            return name;
        }
    }
    public void DrawCard(object sender, EventArgs e)
    {
        if (game.playerHand.Count + game.playerChain.CountAll() < 10)
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
                                Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{sciname}.png"),
                                (int)reader["Hierarchy"],
                                reader["Habitat"].ToString(),
                                new Point(Card.cardWidth * game.playerHand.Count, game.form.workingHeight - game.form.ClientRectangle.Width / 10),
                                false
                                );
                            game.playerHand.Add(c);
                            if (game.username != string.Empty) c.PushToHand();
                            game.form.Controls.Add(c);
                        }
                    }
                    sqlConnection.Close();
                    if (Count > 0)                    
                    {
                        if (game.username != string.Empty)
                        {
                            //delete comma AND number of digits, not just 2
                            SqlCommand removeCard = new SqlCommand($"UPDATE Games SET Deck='{deckStr}' WHERE Username='{game.username}'", sqlConnection);
                            sqlConnection.Open();
                            removeCard.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                    else
                    {
                        if (game.username != string.Empty)
                        {
                            //delete comma AND number of digits, not just 2
                            SqlCommand removeCard = new SqlCommand($"UPDATE Games SET Deck='{deckStr}' WHERE Username='{game.username}'", sqlConnection);
                            sqlConnection.Open();
                            removeCard.ExecuteNonQuery();
                            sqlConnection.Close();
                        }

                        game.form.Display("the deck is empty");
                        game.form.drawCardButton.Enabled = false;
                        game.form.drawCardButton.Hide();
                        return;
                    }
                }
            }
        }
        else
        {
            if (game.playerHand.IsHot()) game.form.Display("i think you can build a chain out of cards on your hand");
        }
    }
    public void LoadDeck()
    {
        if (deckStr == string.Empty) return;
        string[] nums = deckStr.Trim(',').Split(',');
        for (int i = 0; i < nums.Length; i++) Push(int.Parse(nums[i]));
    }
}