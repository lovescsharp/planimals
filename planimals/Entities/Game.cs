using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

public class Game
{
    public MainForm form;

    public string username;
    private int time;
    public Timer countDownTimer;
    private int imageIndex;
    public Timer readySteadyGoTimer;
    
    public Deck deck;
    private string strArr;
    public Hand playerHand;
    public Chain playerChain;
    public List<List<(Rectangle, bool)>> cells;
    public static Rectangle cell;
    public Rectangle liRectangle;

    public int overallScore;
    public int totalPoints;
    public Game(string u, int t, string s, MainForm f)
    {
        username = u;
        time = t;
        form = f;

        strArr = s;

        countDownTimer = new System.Windows.Forms.Timer();
        countDownTimer.Interval = 1000;
        countDownTimer.Tick += new EventHandler(countDownTimer_Tick);

        imageIndex = 3;

        readySteadyGoTimer = new Timer();
        readySteadyGoTimer.Interval = 1000;
        readySteadyGoTimer.Tick += readySteadyGoTimer_Tick;

        deck = new Deck(this);
        playerHand = new Hand(this);

        playerChain = new Chain(this);
        cells = new List<List<(Rectangle, bool)>>();
        cell = new Rectangle(form.Width / 8, form.Height / 6, form.workingHeight / 8 + 3, form.workingWidth / 10 + 3);
    }
    // query creates an example of a saved game, so that you can test pulling and pushing \\
    /*
    delete from FoodChainCards where Username='player1'
    delete from Games where Username='player1'
    delete from Hand where Username='player1'
    insert into Games(Username, Time, Deck) values
    ('player1', 36, ',1,1,1,1,1,1,1,1')

    insert into Hand(Username, CardID) values
    ('player1', 'Omocestus viridulus'),
    ('player1', 'Omocestus viridulus'),
    ('player1', 'Omocestus viridulus');

    Insert into FoodChainCards(Username, CardID, RowNo, PositionNo) values
    ('player1', 'Poa pratensis', 0, 0),
    ('player1', 'Omocestus viridulus', 0, 1),
    ('player1', 'Turdus merula', 0, 2),
    ('player1', 'Pantherophis obsoletus', 0, 3),
    ('player1', 'Tyto alba', 0, 4),
    ('player1', 'Poa pratensis', 1, 0),
    ('player1', 'Microtus arvalis', 1, 1); 
    */
    public void dbTesting()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            SqlCommand test = new SqlCommand("delete from FoodChainCards where Username='player1'\r\ndelete from Games where Username='player1'\r\ndelete from Hand where Username='player1'\r\ninsert into Games(Username, Time, Deck) values\r\n('player1', 36, ',1,1,1,1,1,1,1,1')\r\n\r\ninsert into Hand(Username, CardID) values\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus');\r\n\r\nInsert into FoodChainCards(Username, CardID, RowNo, PositionNo) values\r\n('player1', 'Poa pratensis', 0, 0),\r\n('player1', 'Omocestus viridulus', 0, 1),\r\n('player1', 'Turdus merula', 0, 2),\r\n('player1', 'Pantherophis obsoletus', 0, 3),\r\n('player1', 'Tyto alba', 0, 4),\r\n('player1', 'Poa pratensis', 1, 0),\r\n('player1', 'Microtus arvalis', 1, 1);", sqlConnection);
            sqlConnection.Open(); test.ExecuteNonQuery(); sqlConnection.Close();
        }
    }
    public void CleanDb()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            SqlCommand clearGame = new SqlCommand($"DELETE FROM Games WHERE Username='{username}'", sqlConnection);
            SqlCommand clearHand = new SqlCommand($"DELETE FROM Hand WHERE Username='{username}'", sqlConnection);
            SqlCommand clearChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{username}'", sqlConnection);
            sqlConnection.Open();
            clearGame.ExecuteNonQuery();
            clearHand.ExecuteNonQuery();
            clearChain.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    public void countDownTimer_Tick(object sender, EventArgs e)
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            SqlCommand updateTimer = new SqlCommand($"UPDATE Games SET Time='{time}' WHERE Username='{username}'", sqlConnection);
            sqlConnection.Open();
            if (time > -1)
            {
                updateTimer.ExecuteNonQuery();
                form.labelTimer.Text = time.ToString();
            }
            else
            {
                countDownTimer.Stop();
                form.Hide();
                if (username != "")
                {
                    form.UpdateStatsLabel();
                    CleanDb();
                };

                form.label.Location = new Point(form.workingWidth / 2 - form.label.Width, 100);
                form.label.Font = form.largeFont;
                form.label.Text = "Score: " + overallScore.ToString();
                foreach (Control control in form.endControls)
                {
                    control.Enabled = true;
                    control.Show();
                }
                foreach (Control control in form.gameControls) control.Enabled = false;
                foreach (Card c in playerHand) c.Enabled = false;
                foreach (List<Card> subchain in playerChain.chain)
                {
                    foreach (Card card in subchain) card.Enabled = false;
                }
            }
            UpdateTime();
            sqlConnection.Close();
        }
    }
    private void readySteadyGoTimer_Tick(object sender, EventArgs e)
    {
        imageIndex--;
        if (imageIndex > 0)
        {
            form.readySteadyGo.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", imageIndex.ToString() + ".png"));
        }
        else
        {
            Console.WriteLine("\nthe game started");
            readySteadyGoTimer.Stop();
            form.readySteadyGo.Hide();
            foreach (Control control in form.gameControls)
            {
                control.Enabled = true;
                control.Show();
            }
            if (deck.Count == 0)
            {
                form.drawCardButton.Enabled = false;
                form.drawCardButton.Hide();
            }
            foreach (Control control in form.endControls) { control.Enabled = false; control.Hide(); }
            foreach (List<Card> chain in playerChain.chain)
            {
                foreach (Card c in chain) c.Show();
            }
            foreach (Card c in playerHand) c.Show();
            countDownTimer.Start();
            form.Invalidate();
        }
    }
    public void Start()
    {

        foreach (Card c in playerHand)
        {
            c.Hide();
            c.Image.Dispose();
            c.Dispose();
        }
        playerHand.Clear();
        foreach (List<Card> subchain in playerChain.chain)
        {
            foreach (Card c in subchain)
            {
                c.Hide();
                c.Image.Dispose();
                c.Dispose();
            }
            subchain.Clear();
        }
        playerChain.chain.Clear();
        cells.Clear();
        UpdateCells();
        deck.GenerateDeck();
        imageIndex = 3;
        time = 180;
        form.labelTimer.Show();
        form.labelTimer.Text = "";
        overallScore = 0;
        form.label.Show();
        form.label.Text = "";
        form.label.Location = new Point(form.workingWidth / 10, form.workingHeight / 20);

        if (form.loggedIn)
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
            {
                CleanDb();
                SqlCommand createGame = new SqlCommand($"INSERT INTO Games(Username, Time, Deck) VALUES ('{username}', 40, '{deck.sb.ToString()}')", sqlConnection);
                sqlConnection.Open();
                createGame.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }

        if (imageIndex == 3)
        {
            form.readySteadyGo.Show();
            form.readySteadyGo.Enabled = true;
            try
            {
                int indx = imageIndex;
                form.readySteadyGo.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory,
                                                                       "assets",
                                                                       "photos",
                                                                       indx.ToString() + ".png"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load image: " + ex.Message);
            }
            readySteadyGoTimer.Start();
        }
        else
        {
            Console.WriteLine("the game started");
            readySteadyGoTimer.Start();
        }
    }
    public void Load()
    {
        cells = new List<List<(Rectangle, bool)>>();
        playerChain = new Chain(this);

        foreach (Control control in form.menuControls) control.Hide();
        imageIndex = 3;
        form.labelTimer.Show();
        form.labelTimer.Text = "";
        form.label.Show();
        form.label.Text = "";
        form.label.Location = new Point(form.workingWidth / 10, form.workingHeight / 20);
        form.readySteadyGo.Show();
        form.readySteadyGo.Enabled = true;
        try
        {
            int indx = imageIndex;
            form.readySteadyGo.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", indx.ToString() + ".png"));
        }
        catch (Exception ex) { MessageBox.Show("Failed to load image: " + ex.Message); }
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            SqlCommand getSizes = new SqlCommand(
                $"SELECT RowNo, MAX(PositionNo) AS MaxPositionNo " +
                $"FROM FoodChainCards " +
                $"WHERE Username='{username}' " +
                $"GROUP BY RowNo " +
                $"ORDER BY RowNo; ", sqlConnection);
            sqlConnection.Open();
            foreach (char c in strArr)
            {
                if (c == ',') continue;
                deck.Push(int.Parse(c.ToString()));
            }
            SqlCommand pullHand = new SqlCommand(
                $"SELECT Hand.CardID, Organisms.Common_name, Organisms.Habitat, Organisms.Hierarchy, Organisms.Description " +
                $"FROM Hand " +
                $"JOIN Organisms ON Hand.CardID = Organisms.Scientific_name " +
                $"WHERE Username='{username}'", sqlConnection);
            using (SqlDataReader reader = pullHand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Card c = new Card(
                        this,
                        reader["CardID"].ToString(),
                        reader["Common_name"].ToString(),
                        reader["Description"].ToString(),
                        Path.Combine(Environment.CurrentDirectory, "assets", "photos", reader["CardID"].ToString() + ".jpg"),
                        (int)reader["Hierarchy"],
                        reader["Habitat"].ToString(),
                        new Point(Card.cardWidth * playerHand.Count, form.Height - form.workingWidth / 10),
                        false
                        );
                    playerHand.Add(c);
                    form.Controls.Add(c);
                }
            }
            LoadCells();
            playerChain.LoadChain();

            sqlConnection.Close();
            foreach (List<Card> chain in playerChain.chain)
            {
                foreach (Card c in chain) c.Hide();
            }
            foreach (Card c in playerHand) c.Hide();
            readySteadyGoTimer.Start();
        }
    }
    public void UpdateTime() => time -= 1;
    public void UpdateCells()
    {
        cells.Clear();
        Rectangle rect;
        for (int i = 0; i <= playerChain.chain.Count; i++)
        {
            cells.Add(new List<(Rectangle, bool)>());
            if (i < playerChain.chain.Count)
            {
                for (int j = 0; j <= playerChain.chain[i].Count; j++)
                {
                    if (j == playerChain.chain[i].Count)
                    {
                        rect = new Rectangle(
                            j * (cell.Width) + cell.X, 
                            i * cell.Height + cell.Y, 
                            cell.Width, 
                            cell.Height);
                        cells[i].Add((rect, false));
                    }
                    else
                    {
                        rect = new Rectangle(
                            j * (cell.Width) + cell.X, 
                            i * cell.Height + cell.Y, 
                            cell.Width, 
                            cell.Height);
                        cells[i].Add((rect, true));
                    }
                }
            }
            else
            {
                rect = new Rectangle(
                    cell.X, 
                    i * cell.Height + cell.Y, 
                    cell.Width, 
                    cell.Height);
                cells[i].Add((rect, false));
            }
        }
    }
    public void LoadCells()
    {
        cells.Clear();
        int rows = 0, columns = 0;
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            sqlConnection.Open();

            SqlCommand getSizes = new SqlCommand("SELECT RowNo, MAX(PositionNo) AS MaxPositionNo\r\nFROM FoodChainCards\r\nWHERE Username = @Username\r\nGROUP BY RowNo\r\n", sqlConnection);
            getSizes.Parameters.AddWithValue("@Username", username);
            using (SqlDataReader r = getSizes.ExecuteReader())
            {
                while (r.Read())
                {
                    rows = int.Parse(r["RowNo"].ToString());
                    columns = int.Parse(r["MaxPositionNo"].ToString());

                    cells.Add(new List<(Rectangle, bool)>());
                    Console.WriteLine($"Added a row {rows}");
                    for (int j = 0; j <= columns + 1; j++)
                    {
                        if (j == columns + 1)
                        {
                            Rectangle rect = new Rectangle(
                                j * (cell.Width) + cell.X, 
                                rows * cell.Height + cell.Y, 
                                cell.Width, 
                                cell.Height);
                            cells[rows].Add((rect, false));
                        }
                        else
                        {
                            Rectangle rect = new Rectangle(
                                j * (cell.Width) + cell.X,
                                rows * cell.Height + cell.Y, 
                                cell.Width, 
                                cell.Height);
                            cells[rows].Add((rect, true));
                        }
                        Console.WriteLine($"Added cell {j} to row {rows}");
                    }
                }
            }
        }
    }
}