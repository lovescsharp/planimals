﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

public class Chain : List<List<Card>>
{
    private Game game;
    private string lastOrganism;
    private int longestChainIndex;
    private int earned;

    public Chain(Game g)
    {
        game = g;

        lastOrganism = String.Empty;
        longestChainIndex = -1;
    }
    public override string ToString()
    {
        string s = "[\n";
        for (int i = 0; i < Count; i++) 
        {
            if (i == Count - 1)
            {
                s += "    ";
                s += subchainToString(this[i]);
                s += "\n";
                continue;
            }
            s += "    ";
            s += subchainToString(this[i]);
            s += ",\n";
        }
        s += "]";
        return s;
    }
    private string subchainToString(List<Card> ch) 
    {
        string s = "[ ";
        for (int i = 0; i < ch.Count; i++)
        {

            if (i == ch.Count - 1)
            {
                s += ch[i].CommonName;
                continue;
            }
            s += ch[i].CommonName;
            s += ", ";
        }
        s += " ]";
        return s;
    }
    public int CountAll()
    {
        int count = 0;
        for (int i = 0; i < Count; i++)
            for (int j = 0; j < this[i].Count; j++) count++;
        return count;
    }
    public void lastLink()
    {
        int longestChainCount = -1;
        for (int i = 0; i < Count; i++)
        {
            if (this[i].Count > longestChainCount)
            {
                longestChainCount = this[i].Count;
                longestChainIndex = i;
            }
        }

        lastOrganism = this[longestChainIndex][this[longestChainIndex].Count - 1].ScientificName;
    }
    private void FixChainIndices(List<Card> chain)
    {
        for (int i = 0; i < chain.Count - 1; i++)
        {
            if (chain[i].Location.X > chain[i + 1].Location.X)
            {
                Card temp = chain[i];
                chain[i] = chain[i + 1];
                chain[i + 1] = temp;
            }
        }
    }
    private int CalcScore(int noOfCards)
    {
        int score = 0;
        for (int i = 0; i < noOfCards; i++) score += i + 1;
        return score;
    }
    public bool CHAIN()
    {
        if (Count == 0)
        {
            game.form.Display("the chain must consist of at least to organisms");
            return false;
        }
        else
        {
            lastLink();
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand disposeChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}'", sqlConnection);
                sqlConnection.Open();
                for (int i = 0; i < Count; i++)
                {
                    if (i != game.playerChain.longestChainIndex && this[i].Count != 0)
                    {
                        SqlCommand checkRelation = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer = '{lastOrganism}' AND Consumed = '{this[i][this[i].Count - 1].ScientificName}'", sqlConnection);
                        int b = (int)checkRelation.ExecuteScalar();
                        if (b == 0)
                        {
                            game.form.Display("food chain is invalid");
                            for (int k = 0; k < Count; k++)
                            {
                                for (int j = 0; j < this[k].Count; j++)
                                {
                                    this[k][j].prevLocation = new Point(this[k][j].Width * game.playerHand.Count, game.form.workingHeight - this[k][j].Height);
                                    this[k][j].MoveCard(this[k][j].prevLocation);
                                    this[k][j].Picked = false;
                                    this[k][j].inChain = false;
                                    this[k][j].BackColor = Color.Gray;
                                    game.playerHand.Add(this[k][j]);
                                    if (game.username != "") this[k][j].PushToHand();
                                    (Rectangle, bool) tuple = (game.cells[k][j].Item1, false);
                                    game.cells[k][j] = tuple;
                                }
                            }
                            earned = 0;
                            Clear();
                            if (game.username != string.Empty) disposeChain.ExecuteNonQuery();
                            game.UpdateCells();
                            game.form.Invalidate();
                            sqlConnection.Close(); ;
                            Console.WriteLine("terminating as no common predator");
                            return false;
                        }
                    }
                }//checking the common predator
                for (int index = 0; index < Count; index++)
                {
                    if (this[index].Count < 2) //the subchain consists of one organism
                    {
                        game.form.Display("the chain must consist of at least two organisms.");
                        sqlConnection.Close();
                        return false;
                    }
                    else
                    {
                        FixChainIndices(this[index]);
                        for (int i = 0; i < this[index].Count - 1; i++) // inefficient as i know that chains share a predator so TODO!
                        {
                            SqlCommand checkRelation = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer='{this[index][i + 1].ScientificName}' AND Consumed='{this[index][i].ScientificName}'", sqlConnection);
                            Console.WriteLine(checkRelation.CommandText);
                            int b = (int)checkRelation.ExecuteScalar();
                            if (b == 0)
                            {
                                game.form.Display("food chain is invalid");
                                Console.WriteLine($"playerChain[{IndexOf(this[index])}] is invalid as {this[index][i + 1].CommonName} doesn't eat {this[index][i].CommonName}");
                                for (int k = 0; k < Count; k++)
                                {
                                    for (int j = 0; j < this[k].Count; j++)
                                    {
                                        this[k][j].prevLocation = new Point(this[k][j].Width * game.playerHand.Count, game.form.workingHeight - this[k][j].Height);
                                        this[k][j].MoveCard(this[k][j].prevLocation);
                                        this[k][j].Picked = false;
                                        this[k][j].inChain = false;
                                        this[k][j].BackColor = Color.Gray;
                                        game.playerHand.Add(this[k][j]);
                                        if (game.username != "") this[k][j].PushToHand();
                                        (Rectangle, bool) tuple = (game.cells[k][j].Item1, false);
                                        game.cells[k][j] = tuple;
                                    }
                                }
                                earned = 0;
                                if (game.username != string.Empty) disposeChain.ExecuteNonQuery();
                                sqlConnection.Close();
                                Clear();
                                game.UpdateCells();
                                game.form.Invalidate();
                                return false;
                            }
                        }
                        game.overallScore += CalcScore(this[index].Count); //lessgooo everything is okay
                        earned += CalcScore(this[index].Count);
                    }
                }//iterating through the chain
                if (game.username != string.Empty)
                {
                    game.form.totalPoints += game.overallScore;
                    SqlCommand updatePoints = new SqlCommand($"UPDATE Players SET Points={game.form.totalPoints} WHERE Username='{game.username}'", sqlConnection);
                    updatePoints.ExecuteNonQuery();
                    disposeChain.ExecuteNonQuery();
                }
                for (int j = 0; j < game.playerHand.Count; j++)
                {
                    game.playerHand[j].MoveCard(
                        game.playerHand[j].prevLocation =
                            new Point(
                                Card.cardWidth * j,
                                game.form.workingHeight - Card.cardHeight
                        )
                    );
                }//updating cards locations in chain
                foreach (List<Card> subchain in this) 
                    for (int i = 0; i < subchain.Count; i++) 
                        game.form.Controls.Remove(subchain[i]);
                sqlConnection.Close(); 
                Clear();
                game.form.Display($"+{earned} points");
                earned = 0;
                game.UpdateCells();
                game.form.Invalidate();
                if (!game.playerHand.IsHot() && game.deck.Count == 0) game.Stop();
                return true;
            }
        }
    }
    public void ShiftCards() // when a card is removed in the middle of the chain, shift all cards to the left
    {
        for (int i = 0; i < game.playerChain.Count; i++)
            for (int j = 0; j < game.playerChain[i].Count; j++)
                game.playerChain[i][j].MoveCard(game.playerChain[i][j].rectLocation =
                game.cells[i][j].Item1.Location);
    }
    public void Load()
    {
        int count = game.playerHand.Count;
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand loadChain = new SqlCommand(
                "SELECT Organisms.Scientific_name, Organisms.Common_name, Organisms.Habitat, Organisms.Hierarchy, Organisms.Description, FoodChainCards.RowNo, FoodChainCards.PositionNo " +
                "FROM FoodChainCards " +
                "JOIN Organisms ON FoodChainCards.CardID = Organisms.Scientific_name " +
                "WHERE Username = @Username " +
                "ORDER BY FoodChainCards.RowNo, FoodChainCards.PositionNo;", sqlConnection);  loadChain.Parameters.AddWithValue("@Username", game.username);

            sqlConnection.Open();
            SqlDataReader reader = loadChain.ExecuteReader();
            while (reader.Read())
            {
                string scientificName = reader.GetString(0);
                string commonName = reader.GetString(1);
                string habitat = reader.GetString(2);
                int hierarchy = reader.GetInt32(3);
                string description = reader.GetString(4);
                int rowNo = reader.GetInt32(5);
                int positionNo = reader.GetInt32(6);

                Card c = new Card(
                    game,
                    scientificName,
                    commonName,
                    description,
                    Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{scientificName}.png"),
                    hierarchy,
                    habitat,
                    new Point(game.form.workingWidth, game.form.workingHeight),
                    true
                    );
                c.prevLocation = new Point(Card.cardWidth * count, game.form.workingHeight - Card.cardHeight);
                count++;
                while (Count <= rowNo) Add(new List<Card>());
                this[rowNo].Add(c);
                game.form.Controls.Add(c);
            }
            sqlConnection.Close();
        }
    }
}