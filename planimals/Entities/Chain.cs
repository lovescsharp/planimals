using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

public class Chain : List<List<Card>>
{
    private Game game;
    private int longestChainIndex;
    private int earned;

    public Chain(Game g)
    {
        game = g;
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
    public string lastLink()
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
        return this[longestChainIndex][this[longestChainIndex].Count - 1].ScientificName;
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
        for (int i = 1; i < noOfCards + 1; i++) score += i;
        return score;
    }
    public void ChainChain()
    {
        if (Count == 0)
        {
            game.form.Display("the chain must consist of at least to organisms");
            return;
        }
        else
        {
            string lastOrganism = lastLink();
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
                                    this[k][j].Picked = false;
                                    this[k][j].inChain = false;
                                    this[k][j].BackColor = Color.Gray;
                                    game.playerHand.Add(this[k][j]);
                                    if (game.username != "") this[k][j].PushToHand();
                                    (Rectangle, bool) tuple = (game.cells[k][j].Item1, false);
                                    game.cells[k][j] = tuple;
                                }
                            }
                            if (Card.cardWidth * game.playerHand.Count > game.form.ClientRectangle.Width) game.playerHand.putCardsOnTopOfEachOther();
                            else game.playerHand.ShiftCards();
                            earned = 0;
                            Clear();
                            if (game.username != string.Empty) disposeChain.ExecuteNonQuery();
                            game.UpdateCells();
                            game.form.Invalidate();
                            sqlConnection.Close(); ;
                            Console.WriteLine("terminating as no common predator");
                            return;
                        }
                    }
                }//checking the common predator
                for (int index = 0; index < Count; index++)
                {
                    if (this[index].Count < 2) //the subchain consists of one organism
                    {
                        game.form.Display("the chain must consist of at least two organisms.");
                        sqlConnection.Close();
                        return;
                    }
                    else
                    {
                        FixChainIndices(this[index]);
                        for (int i = 0; i < this[index].Count - 1; i++) 
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
                                        this[k][j].Picked = false;
                                        this[k][j].inChain = false;
                                        this[k][j].BackColor = Color.Gray;
                                        game.playerHand.Add(this[k][j]);
                                        if (game.username != "") this[k][j].PushToHand();
                                        (Rectangle, bool) tuple = (game.cells[k][j].Item1, false);
                                        game.cells[k][j] = tuple;
                                    }
                                }
                                if (Card.cardWidth * game.playerHand.Count > game.form.ClientRectangle.Width) game.playerHand.putCardsOnTopOfEachOther();
                                else game.playerHand.ShiftCards();
                                earned = 0;
                                if (game.username != string.Empty) disposeChain.ExecuteNonQuery();
                                sqlConnection.Close();
                                Clear();
                                game.UpdateCells();
                                game.form.Invalidate();
                                return;
                            }
                        }
                        game.overallScore += CalcScore(this[index].Count); //lessgooo everything is okay
                        game.form.currentScore.Text = $"points : {game.overallScore}";
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
                else if (game.playerHand.IsHot() && game.deck.Count == 0) game.form.Display("i see some cards on your hand that can form a chain");
                if (Card.cardWidth * game.playerHand.Count > game.form.ClientRectangle.Width) game.playerHand.putCardsOnTopOfEachOther();
                else game.playerHand.ShiftCards();
                return;
            }
        }
    }
    public void ShiftCards() // when a card is removed in the middle of the chain, shift all cards to the left
    {
        for (int i = 0; i < game.playerChain.Count; i++)
            for (int j = 0; j < game.playerChain[i].Count; j++) 
                game.playerChain[i][j].MoveCard(game.playerChain[i][j].rectLocation = game.cells[i][j].Item1.Location);
    }
    public void UpdateIndices()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {

            SqlCommand getNoOfRows = new SqlCommand($"SELECT COUNT(DISTINCT RowNo) FROM FoodChainCards WHERE Username='{game.username}';", sqlConnection);
            sqlConnection.Open();   
            int count = int.Parse(getNoOfRows.ExecuteScalar().ToString());
            if (count == 0) return;
            SqlCommand getRows = new SqlCommand($"SELECT RowNo, CardID, PositionNo FROM FoodChainCards WHERE Username='{game.username}' ORDER BY RowNo, PositionNo;", sqlConnection);
            List<List<(string, int, int)>> chain = new List<List<(string, int, int)>>();
            for (int i = 0; i < count; i++) chain.Add(new List<(string, int, int)>());

            //Console.WriteLine($"count = {chain.Count}");

            int c = 0;
            int currRow = int.Parse(getRows.ExecuteScalar().ToString());
            using (SqlDataReader r = getRows.ExecuteReader())
                while (r.Read())
                {
                    //Console.WriteLine($"(r[\"RowNo\"].ToString() = {int.Parse(r["RowNo"].ToString())}\n {currRow}");
                    if (int.Parse(r["RowNo"].ToString()) > currRow) 
                    {
                        c++;
                        currRow = int.Parse(r["RowNo"].ToString());
                    }
                    chain[c].Add((r["CardID"].ToString(), int.Parse(r["RowNo"].ToString()), int.Parse(r["PositionNo"].ToString())));
                }
            SqlCommand updateIndices = new SqlCommand("", sqlConnection);
            for (int i = 0; i < chain.Count; i++)
                for (int j = 0; j < chain[i].Count; j++)
                    updateIndices.CommandText += $"UPDATE FoodChainCards SET RowNo={i}, PositionNo={j} WHERE CardID='{chain[i][j].Item1}' AND RowNo={chain[i][j].Item2} AND PositionNo={chain[i][j].Item3};\n";

            if (updateIndices.CommandText != string.Empty) updateIndices.ExecuteNonQuery();
            sqlConnection.Close();
        }
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
                    new Point(game.form.ClientRectangle.Width, game.form.ClientRectangle.Height),
                    true
                    );
                c.prevLocation = new Point(Card.cardWidth * count, game.form.ClientRectangle.Height - Card.cardHeight);
                count++;
                while (Count <= rowNo) Add(new List<Card>());
                this[rowNo].Add(c);
                game.form.Controls.Add(c);
            }
            sqlConnection.Close();
        }
    }
}