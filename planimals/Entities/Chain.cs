using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

public class Chain
{
    private Game game;
    public List<List<Card>> chain;
    private string lastOrganism;
    private int longestChainIndex;
    private int earned;

    public Chain(Game g)
    {
        game = g;

        lastOrganism = String.Empty;
        longestChainIndex = -1;
        chain = new List<List<Card>>();
    }
    public int CountAll()
    {
        int count = 0;
        for (int i = 0; i < chain.Count; i++)
        {
            for (int j = 0; j < chain[i].Count; j++)
            { 
                count++;
            }
        }
        return count;
    }
    public void lastLink()
    {
        int longestChainCount = -1;
        for (int i = 0; i < chain.Count; i++)
        {
            if (chain[i].Count > longestChainCount)
            {
                longestChainCount = chain[i].Count;
                longestChainIndex = i;
            }
        }

        lastOrganism = chain[longestChainIndex][chain[longestChainIndex].Count - 1].ScientificName;
    }
    private void FixChainIndices(List<Card> chain)
    {
        for (int i = 0; i < chain.Count - 1; i++)
        {
            //MessageBox.Show($"{chain[i].common_name} at {chain[i].Location.X.ToString()} and {chain[i+1].common_name} at {chain[i+1].Location.X.ToString()}");
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
    public void CHAIN()
    {
        if (chain.Count == 0)
        {
            game.form.Display("the chain must consist of at least to organisms");
            return;
        }
        else
        {
            /*
            string str = "";
            foreach (List<Card> subchain in chain)
            {
                str += $"   chain {chain.IndexOf(subchain).ToString()}\n";
                foreach (Card c in subchain) str += "        " + $"{c.CommonName}" + '\n';
            }
            Console.WriteLine("current chain:\n" + str);
            */
            lastLink();
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
            {
                for (int i = 0; i < chain.Count; i++)
                {
                    if (i != game.playerChain.longestChainIndex && chain[i].Count != 0)
                    {
                        SqlCommand checkRelation = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer = '{lastOrganism}' AND Consumed = '{chain[i][chain[i].Count - 1].ScientificName}'", sqlConnection);
                        //Console.WriteLine(sqlCommand.CommandText);
                        sqlConnection.Open();
                        int b = (int)checkRelation.ExecuteScalar();
                        if (b == 0)
                        {
                            game.form.Display("food chain is invalid");
                            Console.WriteLine($"playerChain.chain[{i}] is invalid as {lastOrganism} doesn't eat {chain[i][chain[i].Count - 1].CommonName}");
                            Console.WriteLine($"Moving cards back to hand");
                            for (int k = 0; k < chain.Count; k++)
                            {
                                Console.WriteLine($"row[{k}]");
                                for (int j = 0; j < chain[k].Count; j++)
                                {
                                    Console.WriteLine($"card[{k}][{j}]");
                                    chain[k][j].prevLocation = new Point(chain[k][j].Width * game.playerHand.Count, game.form.workingHeight - chain[k][j].Height);
                                    Console.WriteLine($"{i} {j}");
                                    chain[k][j].Location = chain[k][j].prevLocation;
                                    chain[k][j].Picked = false;
                                    chain[k][j].inChain = false;
                                    chain[k][j].BackColor = Color.Gray;
                                    game.playerHand.Add(chain[k][j]);
                                    if (game.username != "") chain[k][j].PushToHand();
                                    (Rectangle, bool) tuple = (game.cells[k][j].Item1, false);
                                    game.cells[k][j] = tuple;
                                }
                            }
                            earned = 0;
                            chain.Clear();
                            if (game.form.loggedIn)
                            {
                                SqlCommand disposeChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}'", sqlConnection);
                                disposeChain.ExecuteNonQuery();
                            }
                            game.UpdateCells();
                            game.form.Invalidate();
                            sqlConnection.Close();
                            return;
                        }
                    }
                }//checking the common predator
                for (int index = 0; index < chain.Count; index++) //iterating through the chain
                {
                    if (chain[index].Count < 2) //the subchain consists of one organism
                    {
                        game.form.Display("the chain must consist of at least two organisms.");
                        return;
                    }
                    else
                    {
                        FixChainIndices(chain[index]);
                        sqlConnection.Open();
                        for (int i = 0; i < chain[index].Count - 1; i++) // inefficient as i know that chains share a predator so TODO!
                        {
                            SqlCommand checkRelation = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer='{chain[index][i + 1].ScientificName}' AND Consumed='{chain[index][i].ScientificName}'", sqlConnection);
                            int b = (int)checkRelation.ExecuteScalar();
                            if (b == 0)
                            {
                                game.form.Display("food chain is invalid");
                                Console.WriteLine($"playerChain.chain[{chain.IndexOf(chain[index])}] is invalid as {chain[index][i + 1].CommonName} doesn't eat {chain[index][i].CommonName}");
                                for (int k = 0; k < chain.Count; k++)
                                {
                                    for (int j = 0; j < chain[k].Count; j++)
                                    {
                                        Console.WriteLine($"card[{k}][{j}]");
                                        chain[k][j].Location = chain[k][j].prevLocation;
                                        chain[k][j].Picked = false;
                                        chain[k][j].inChain = false;
                                        chain[k][j].BackColor = Color.Gray;
                                        game.playerHand.Add(chain[k][j]);
                                        if (game.username != "") chain[k][j].PushToHand();
                                        chain[k][j].Dispose();
                                        (Rectangle, bool) tuple = (game.cells[k][j].Item1, false);
                                        game.cells[k][j] = tuple;
                                    }
                                }
                                earned = 0;
                                
                                if (game.form.loggedIn)
                                {
                                    SqlCommand disposeChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}'", sqlConnection);
                                    disposeChain.ExecuteNonQuery();
                                }
                                game.UpdateCells();
                                game.form.Invalidate();
                                return;
                            }
                        }//if you get here, there is a common predator, now check all the other relations
                        game.overallScore += CalcScore(chain[index].Count); //lessgooo everything is okay
                        earned += CalcScore(chain[index].Count);
                    }
                }
                if (game.form.loggedIn)
                {
                    game.totalPoints += game.overallScore;
                    SqlCommand updatePoints = new SqlCommand($"UPDATE Players SET Points={game.totalPoints} WHERE Username='{game.username}'", sqlConnection);
                    updatePoints.ExecuteNonQuery();
                    SqlCommand disposeChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}'", sqlConnection);
                    disposeChain.ExecuteNonQuery();
                }
                for (int j = 0; j < game.playerHand.Count; j++)
                {
                    game.playerHand[j].Location = game.playerHand[j].prevLocation = new Point(
                        Card.cardWidth * j,
                        game.form.workingHeight - Card.cardHeight
                    );
                }//updating cards locations in chain
                foreach (List<Card> subchain in chain) for (int i = 0; i < subchain.Count; i++) subchain[i].Dispose();
                chain.Clear();
                game.form.Display($"+{earned} points");
                earned = 0;
                game.UpdateCells();
                game.form.Invalidate();
            }
        }
    }
    public void LoadChain(SqlConnection sqlConnection) //update prevLocations
    {
        SqlCommand loadChain = new SqlCommand(
            "SELECT Organisms.Scientific_name, Organisms.Common_name, Organisms.Habitat, Organisms.Hierarchy, Organisms.Description, FoodChainCards.RowNo, FoodChainCards.PositionNo " +
            "FROM FoodChainCards " +
            "JOIN Organisms ON FoodChainCards.CardID = Organisms.Scientific_name " +
            "WHERE Username = @Username " +
            "ORDER BY FoodChainCards.RowNo, FoodChainCards.PositionNo;", sqlConnection);

        loadChain.Parameters.AddWithValue("@Username", game.username);

        using (SqlDataReader reader = loadChain.ExecuteReader())
        {
            while (reader.Read())
            {
                string scientificName = reader.GetString(0);
                string commonName = reader.GetString(1);
                string habitat = reader.GetString(2);
                int hierarchy = reader.GetInt32(3);
                string description = reader.GetString(4);
                int rowNo = reader.GetInt32(5);
                int positionNo = reader.GetInt32(6);

                Console.WriteLine($"Adding {commonName} to cells[{rowNo}][{positionNo}]");

                Card c = new Card(
                    game,
                    scientificName,
                    commonName,
                    description,
                    Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{scientificName}.jpg"),
                    hierarchy,
                    habitat,
                    game.cells[rowNo][positionNo].Item1.Location,
                    true
                    );
                //c.rectLocation = game.cells[rowNo][positionNo].Item1.Location;
                //card.prevLocation = new Point(card.Width * game.playerHand.Count, workingHeight - card.Height); //do something about it

                while (chain.Count <= rowNo + 1) chain.Add(new List<Card>());
                chain[rowNo].Add(c);
                game.form.AddCardControl(c);
            }
        }
    }
}