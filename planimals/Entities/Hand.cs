using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;

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
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
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
}
