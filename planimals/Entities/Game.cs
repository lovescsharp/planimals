using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Game
{
    public MainForm form;

    public string username;
    private int time;
    public Timer countDownTimer;
    private int imageIndex;
    public Timer readySteadyGoTimer;
    
    public Deck deck;
    public Hand playerHand;
    public Chain playerChain;
    public List<List<(Rectangle, bool)>> cells;
    public static Rectangle cell;
    public Rectangle liRectangle;

    public int overallScore; //points earned in this round
    public int totalPoints; //total points the player has on their account

    public Game(MainForm f, string u) //starter
    {
        form = f;
        username = u;
        
        countDownTimer = new Timer();
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
        
        List<Control> controls = new List<Control>(); 
        for (int i = 0; i < form.Controls.Count; i++) if (form.Controls[i] is Card) controls.Add(form.Controls[i]);
        foreach (Control c in controls) form.Controls.Remove(c);
        playerHand.Clear();
        playerChain.Clear();
        cells.Clear();
        UpdateCells();
        deck.GenerateDeck();
        imageIndex = 3;
        time = 120;
        form.labelTimer.Show();
        form.labelTimer.Text = "";
        overallScore = 0;
        form.label.Show();
        form.label.Text = "";
        form.label.Location = new Point(form.workingWidth / 10, form.workingHeight / 20);
        if (username != string.Empty)
        {
            CleanDb();
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand createGame = new SqlCommand($"INSERT INTO Games(Username, Time, Deck) VALUES ('{username}', {time}, '{deck.deckStr}')", sqlConnection);
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
        else readySteadyGoTimer.Start();

    }
    public Game(MainForm f, string u, int t, string d) //loader
    {
        form = f;
        username = u;
        time = t;

        deck = new Deck(this, d);

        playerHand = new Hand(this);
        playerHand.LoadHand();
        
        playerChain = new Chain(this);
        playerChain.LoadChain();

        cells = new List<List<(Rectangle, bool)>>();
        cell = new Rectangle(form.Width / 8, form.Height / 6, form.workingHeight / 8 + 3, form.workingWidth / 10 + 3);

        for (int i = 0; i < d.Length; i++)
        {
            if (d[i] == ',') continue;
            deck.Push(d[i]);
        }

        countDownTimer = new Timer();
        countDownTimer.Interval = 1000;
        countDownTimer.Tick += new EventHandler(countDownTimer_Tick);

        imageIndex = 3;

        readySteadyGoTimer = new Timer();
        readySteadyGoTimer.Interval = 1000;
        readySteadyGoTimer.Tick += readySteadyGoTimer_Tick;
        
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
            form.readySteadyGo.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", imageIndex.ToString() + ".png"));
        }
        catch (Exception ex) 
        {
            MessageBox.Show("Failed to load image: " + ex.Message); 
        }

        playerHand.LoadHand();
        playerChain.LoadChain();
        UpdateCells();
        for (int i = 0; i < playerChain.Count; i++)
            for (int j = 0; j < playerChain[i].Count; j++)
            {
                Console.WriteLine($"card.rectLocation[{i}][{j}] = {cells[i][j].Item1.Location}");
                playerChain[i][j].rectLocation = cells[i][j].Item1.Location;
            }
        foreach (List<Card> chain in playerChain) foreach (Card c in chain) c.Hide();
        foreach (Card c in playerHand) c.Hide();
        readySteadyGoTimer.Start();
    }
    public void CleanDb()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand clearGame = new SqlCommand($"DELETE FROM Games WHERE Username='{username}';" + $"DELETE FROM Hand WHERE Username='{username}';" + $"DELETE FROM FoodChainCards WHERE Username='{username}';", sqlConnection);
            sqlConnection.Open();
            clearGame.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    public void countDownTimer_Tick(object sender, EventArgs e)
    {
        if (username != string.Empty) //we need to update time if the user is logged in
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand updateTimer = new SqlCommand($"UPDATE Games SET Time='{time}' WHERE Username='{username}'", sqlConnection);
                sqlConnection.Open();
                if (time >= 0) updateTimer.ExecuteNonQuery();
                else Stop();
                sqlConnection.Close();
            }
        }
        else //otherwise just wait 'til timer is 0
        {
            if (time <= 0) Stop(); 
        }
        form.labelTimer.Text = time.ToString();
        time -= 1;
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
            //Console.WriteLine("\nthe game started");
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
            foreach (List<Card> chain in playerChain) foreach (Card c in chain) c.Show();
            foreach (Card c in playerHand) c.Show();
            foreach (Card c in playerHand) c.MoveCard();
            foreach (List<Card> ch in playerChain) foreach (Card c in ch) c.MoveCard();
            countDownTimer.Start();
            form.Invalidate();
        }
    }
    public void Stop()
    {
        countDownTimer.Stop();
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
        foreach (List<Card> subchain in playerChain)
            foreach (Card card in subchain) card.Enabled = false;
    }
    public void UpdateCells()
    {
        cells.Clear();
        Rectangle rect;
        for (int i = 0; i <= playerChain.Count; i++)
        {
            cells.Add(new List<(Rectangle, bool)>());
            if (i < playerChain.Count)
            {
                for (int j = 0; j <= playerChain[i].Count; j++)
                {
                    if (j == playerChain[i].Count)
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
    void fancyDealership()
    {
        foreach (Card c in playerHand) c.MoveCard();
        foreach (List<Card> ch in playerChain) foreach (Card c in ch) c.MoveCard();
    }
}