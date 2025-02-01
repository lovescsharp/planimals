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
    List<string> organisms;

    public Deck(Game g) : base() // starter
    {
        rnd = new Random();
        deckStr = "";
        game = g;
        size = 40;
        organisms = new List<string> ();
        GetOrganisms();
        Console.WriteLine($"# of organisms = {organisms.Count}");
        GenerateDeck();
    }
    public Deck(Game g, string d) : base() // loader
    {
        rnd = new Random();
        deckStr = d;
        game = g;
        Load();
        size = Count;
        organisms = new List<string>();
        GetOrganisms();
    }
    void GetOrganisms() 
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING)) 
        {
            SqlCommand get = new SqlCommand("SELECT Scientific_name FROM Organisms ORDER BY Scientific_name", sqlConnection);
            sqlConnection.Open();
            using (SqlDataReader r = get.ExecuteReader()) 
            {
                while (r.Read()) organisms.Add(r["Scientific_name"].ToString());
            }
            sqlConnection.Close();
        }
    }
    public void GenerateDeck()
    {
        int randIdx;
        int upperBound = organisms.Count;
        for (int i = 0; i < size; i++)
        {
            randIdx = rnd.Next(1, upperBound);
            Console.WriteLine($"random index of a card in deck is : {randIdx}");
            Push(randIdx);
            deckStr += randIdx + ",";
        }
    }
    public string GetScientificNameFromDeck()
    {
        int i = Pop();
        deckStr = deckStr.Remove(deckStr.Length - 1);
        for (int j = 0; j < (int)Math.Floor(Math.Log10(i)) + 1; j++) deckStr = deckStr.Remove(deckStr.Length - 1);
        
        return organisms[i];
    }
    public void DrawCard(object sender, EventArgs e)
    {
        //Console.WriteLine($"hand.count = {game.playerHand.Count}\nchain.count = {game.playerChain.CountAll()}");
        if (game.playerHand.Count <= 20 || !game.playerHand.IsHot())
        //if (Card.cardWidth * game.playerHand.Count < game.form.ClientRectangle.Width || !game.playerHand.IsHot())
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
                        if (game.username != string.Empty) //just save deck state and continue the dealing cards
                        {
                            SqlCommand removeCard = new SqlCommand($"UPDATE Games SET Deck='{deckStr}' WHERE Username='{game.username}'", sqlConnection);
                            sqlConnection.Open();
                            removeCard.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                    else //there is no more cards in the deck so we must terminate for loop that deals cards
                    {
                        if (game.username != string.Empty)
                        {
                            SqlCommand removeCard = new SqlCommand($"UPDATE Games SET Deck='{deckStr}' WHERE Username='{game.username}'", sqlConnection);
                            sqlConnection.Open();
                            removeCard.ExecuteNonQuery();
                            sqlConnection.Close();
                        }

                        if (!game.playerHand.IsHot()) game.Stop();
                        game.form.Display("the deck is empty");
                        game.form.drawCardButton.Enabled = false;
                        game.form.drawCardButton.Hide();

                        if (Card.cardWidth * game.playerHand.Count > game.form.ClientRectangle.Width) game.playerHand.putCardsOnTopOfEachOther();
                        return;
                    }
                }
            }
            if (Card.cardWidth * game.playerHand.Count > game.form.ClientRectangle.Width) game.playerHand.putCardsOnTopOfEachOther();
        }
        else
        {
            game.form.Display("i think you can build a chain out of cards on your hand");
        }
    }
    public void Load()
    {
        if (deckStr == string.Empty) return;
        string[] nums = deckStr.Trim(',').Split(',');
        for (int i = 0; i < nums.Length; i++) Push(int.Parse(nums[i]));
    }
}