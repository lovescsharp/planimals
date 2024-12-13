﻿using planimals;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//absolutely fvcked up chaining system
//do somehthing about card locations

//https://stackoverflow.com/questions/51567681/how-to-do-task-continewith-using-a-delay-before-the-continuewith-in-c
//allows to asynchronously change label.Text, looks cool 8) 
public partial class MainForm : Form
{
    #region attributes
    Random rnd;

    private Button playButton;
    public static Button continueButton;
    public static Button loginButton;
    private Button exitButton;
    private Label title;

    public static Game game;
    public static int totalPoints;
    public static Label stats;

    public static Timer countDownTimer;
    private int timeLeft;
    private Label labelTimer;

    private Timer readySteadyGoTimer;
    private static PictureBox readySteadyGo;
    private int imageIndex = 3;

    private static string dbPath = Path.Combine(Environment.CurrentDirectory, "cards.mdf");
    public static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" + $"AttachDbFilename={dbPath}" + ";Integrated Security=True;Connect Timeout=30";
    private static readonly SqlConnection sqlConnection = new SqlConnection(connectionString);

    public static int workingHeight;
    public static int workingWidth;

    public static Stack<int> deck;
    public static int livingCards;
    private StringBuilder sb;
    public static List<Card> playerHand;
    public static List<List<Card>> playerChain;
    private string lastOrganism;
    private int longestChainIndex;
    public static List<List<(Rectangle, bool)>> cells;
    private static Rectangle cell;
    public static Rectangle liRectangle;

    private PictureBox retryButton;
    private PictureBox goToMenuButton;
    private PictureBox goToMenuInGameButton;

    private Button yesButton;
    private Button noButton;
    private Label youSureWannaQuitLabel;

    private PictureBox drawCardButton;
    private Image drawCardButtonBack;
    private Rectangle drawCardRectangle;

    private PictureBox chainButton;
    private Image chainButtonBack;
    private Rectangle chainButtonRectangle;

    private List<Control> gameControls;
    private List<Control> menuControls;
    private List<Control> endControls;
    private List<Control> youSureWannaQuitControls;

    private static Label label;

    private Font largeFont;
    private Font smallFont;

    private int overallScore;
    private int earned;
    #endregion
    public MainForm()
    {
        #region Init component
        /*
                  -=====-                            -=====-
                   _..._                               _..._
                 .~     `~.                         .~`     ~.
         ,_     /          }                       {          \     _,
        ,_\'--, \   _.'`~~/                        \~~`'._   / ,--'/_,
         \'--,_`{_,}    -(                          )-    {,_}`_,--'/
          '.`-.`\;--,___.'_                        _'.___,--;/`.-`.'
            '._`/    |_ _{@}                       {@}_ _|    \`_.'
               /     ` |-';/                        \;'-| `     \
              /   \    / */
        InitializeComponent(); /*|  \    /   \
             /     '--;_                                _;--'     \
            _\          `\                            /`          /_
        / |`-.___.    /                           \    .___,-'| \
        `--`------'`--`^^^^^^^^^^^^^^^^^^^^^^^^^^^^`--`'------`--`
        */
        #endregion
        Console.WriteLine("initializing game");
        #region ux/ui
        MinimumSize = new Size(1120, 620);
        Text = "planimals";
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;

        largeFont = new Font("Arial", 28);
        largeFont = new Font("Arial", 28);
        smallFont = new Font("Arial", 14);

        //static size
        FormBorderStyle = FormBorderStyle.Fixed3D;
        MinimizeBox = false;
        //Size = new Size(1366, 768);
        Size = new Size(1600, 900);
        workingHeight = ClientRectangle.Height;
        workingWidth = ClientRectangle.Width;

        liRectangle = new Rectangle(
            0, 0,
            workingWidth / 100 * 5,
            workingHeight / 5
        );

        //BackgroundImage = new Bitmap(Image.FromFile(currentDir + "\\assets\\photos\\background.png"));
        //BackgroundImageLayout = ImageLayout.Stretch;
        BackColor = Color.DarkSeaGreen;

        game = new Game("", 0, 0);
        stats = new Label();
        stats.Size = new Size(300, 100);
        stats.Location = new Point(workingWidth - 300, 10);
        stats.ForeColor = Color.White;
        Controls.Add(stats);

        playButton = new Button();
        playButton.Text = "play";
        playButton.BackColor = Color.White;
        playButton.Size = new Size(50, 30);
        playButton.Location = new Point(workingWidth / 2 - playButton.Width / 2, workingHeight / 2 - playButton.Height / 2 - 60);
        Controls.Add(playButton);
        playButton.Click += PlayButton_Click;

        continueButton = new Button();
        continueButton.Text = "load";
        continueButton.BackColor = Color.White;
        continueButton.Size = new Size(50, 30);
        continueButton.Location = new Point(workingWidth / 2 - playButton.Width / 2, workingHeight / 2 - playButton.Height / 2 - 30);
        Controls.Add(continueButton);
        continueButton.Click += continueButton_Click;

        loginButton = new Button();
        loginButton.Text = "log in";
        loginButton.BackColor = Color.White;
        loginButton.Size = new Size(50, 30);
        loginButton.Location = new Point(workingWidth / 2 - loginButton.Width / 2, workingHeight / 2 - loginButton.Height / 2);
        Controls.Add(loginButton);
        loginButton.Click += Login;

        exitButton = new Button();
        exitButton.Text = "exit";
        exitButton.BackColor = Color.White;
        exitButton.Size = new Size(50, 30);
        exitButton.Location = new Point(workingWidth / 2 - loginButton.Width / 2, workingHeight / 2 - exitButton.Height / 2 + 30);
        Controls.Add(exitButton);
        exitButton.Click += Exit;

        title = new Label();
        title.Text = "Planimals";
        title.AutoSize = true;
        title.Font = largeFont;
        title.ForeColor = Color.White;
        title.Location = new Point((workingWidth - title.Size.Width) / 2, workingHeight / 8);
        Controls.Add(title);

        retryButton = new PictureBox();
        retryButton.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "retry.png"));
        retryButton.SizeMode = PictureBoxSizeMode.StretchImage;
        retryButton.Size = new Size(workingWidth / 10, workingWidth / 10);
        retryButton.Location = new Point(workingWidth / 2 - retryButton.Width / 2, workingHeight / 2 - retryButton.Height / 2);
        retryButton.Click += retryButton_Click;
        Controls.Add(retryButton);

        goToMenuButton = new PictureBox();
        goToMenuButton.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "exit.png"));
        goToMenuButton.SizeMode = PictureBoxSizeMode.StretchImage;
        goToMenuButton.Size = new Size(workingWidth / 10, workingWidth / 8);
        goToMenuButton.Location = new Point(workingWidth / 2 + goToMenuButton.Width, workingHeight / 2 - goToMenuButton.Height / 2);
        goToMenuButton.Click += goToMenuButton_Click;
        Controls.Add(goToMenuButton);

        goToMenuInGameButton = new PictureBox();
        goToMenuInGameButton.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "exit.png"));
        goToMenuInGameButton.SizeMode = PictureBoxSizeMode.StretchImage;
        goToMenuInGameButton.Size = new Size(workingWidth / 20, workingWidth / 16);
        goToMenuInGameButton.Location = new Point(5, 5);
        goToMenuInGameButton.Click += goToMenuInGameButton_Click;
        Controls.Add(goToMenuInGameButton);

        yesButton = new Button();
        yesButton.Text = "yes";
        yesButton.BackColor = Color.White;
        yesButton.Size = new Size(50, 30);
        yesButton.Location = new Point(workingWidth / 2 + yesButton.Width / 2, workingHeight / 2 - yesButton.Height);
        yesButton.Click += yesButton_Click;
        Controls.Add(yesButton);

        noButton = new Button();
        noButton.Text = "no";
        noButton.BackColor = Color.White;
        noButton.Size = new Size(50, 30);
        noButton.Location = new Point(workingWidth / 2 - noButton.Width - noButton.Width / 2, workingHeight / 2 - noButton.Height);
        noButton.Click += noButton_Click;
        Controls.Add(noButton);

        youSureWannaQuitLabel = new Label();
        youSureWannaQuitLabel.AutoSize = true;
        youSureWannaQuitLabel.Location = new Point(workingWidth / 2 - youSureWannaQuitLabel.Width, workingHeight / 3);
        youSureWannaQuitLabel.Text = "Are you sure you want to quit?";
        youSureWannaQuitLabel.ForeColor = Color.White;
        Controls.Add(youSureWannaQuitLabel);

        drawCardButton = new PictureBox();
        drawCardButtonBack = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "back.png"));
        drawCardButton.SizeMode = PictureBoxSizeMode.StretchImage;
        drawCardButton.Size = new Size(workingHeight / 8, workingWidth / 10);
        drawCardButton.Location = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);
        Point initPos = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);
        drawCardRectangle = new Rectangle(drawCardButton.Location.X, drawCardButton.Location.Y, drawCardButton.Width, drawCardButton.Height);
        drawCardButton.Image = drawCardButtonBack;
        Controls.Add(drawCardButton);
        drawCardButton.Click += new EventHandler(DrawCard);
        drawCardButton.MouseEnter += drawCardButton_MouseEnter;
        drawCardButton.MouseLeave += drawCardButton_MouseLeave;

        chainButton = new PictureBox();
        chainButtonBack = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "chain.png"));
        chainButton.SizeMode = PictureBoxSizeMode.StretchImage;
        chainButton.Size = new Size(workingWidth / 10, workingHeight / 10);
        chainButton.Location = new Point(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2);
        chainButtonRectangle = new Rectangle(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2,
            workingWidth / 10,
            workingHeight / 10);
        chainButton.Image = chainButtonBack;
        Controls.Add(chainButton);
        chainButton.Click += new EventHandler(chainButton_Click);
        chainButton.MouseEnter += chainButton_MouseEnter;
        chainButton.MouseLeave += chainButton_MouseLeave;

        label = new Label();
        label.Location = new Point(workingWidth / 10, workingHeight / 20);
        label.ForeColor = Color.White;
        label.AutoSize = true;
        Controls.Add(label);

        labelTimer = new Label();
        labelTimer.ForeColor = Color.White;
        labelTimer.Font = new Font(label.Font.FontFamily, 25);
        labelTimer.Location = new Point((int)((double)workingWidth * 0.9), (int)((double)workingHeight / 10));
        labelTimer.Size = new Size(100, 100);
        Controls.Add(labelTimer);

        foreach (Control control in Controls)
        {
            if (control.GetType() == typeof(Label))
            {
                if (Width < 1920 || Height < 1080) control.Font = smallFont;
                else control.Font = largeFont;
            }
        }
        readySteadyGo = new PictureBox();
        readySteadyGo.Size = new Size(workingWidth / 10, workingWidth / 4);
        readySteadyGo.SizeMode = PictureBoxSizeMode.CenterImage;
        readySteadyGo.Location = new Point(workingWidth / 2 - readySteadyGo.Width / 2, workingHeight / 2 - readySteadyGo.Height / 2);
        Controls.Add(readySteadyGo);

        readySteadyGoTimer = new Timer();
        readySteadyGoTimer.Interval = 1000;
        readySteadyGoTimer.Tick += readySteadyGoTimer_Tick;

        cells = new List<List<(Rectangle, bool)>>();
        cell = new Rectangle(Width / 8, Height / 6, workingHeight / 8 + 3, workingWidth / 10 + 3);
        #endregion
        #region backs

        countDownTimer = new Timer();
        countDownTimer.Interval = 1000;
        countDownTimer.Tick += new EventHandler(countDownTimer_Tick);

        rnd = new Random();
        deck = new Stack<int>();
        sb = new StringBuilder();
        playerHand = new List<Card>();
        playerChain = new List<List<Card>>();
        lastOrganism = "";
        longestChainIndex = -1;
        //playerChain = new List<Chain>();
        Paint += new PaintEventHandler(Draw);
        //Resize += new EventHandler(OnResize);

        gameControls = new List<Control>() { drawCardButton, chainButton, goToMenuInGameButton };
        menuControls = new List<Control>() { loginButton, playButton, continueButton, exitButton, stats, title };
        endControls = new List<Control>() { retryButton, goToMenuButton };
        youSureWannaQuitControls = new List<Control>() { yesButton, noButton, youSureWannaQuitLabel };

        foreach (Control control in gameControls) control.Hide();
        foreach (Control control in endControls) control.Hide();
        foreach (Control control in youSureWannaQuitControls) control.Hide();
        foreach (Control control in Controls) if (control.GetType() == typeof(Label)) control.ForeColor = Color.LightGreen;
        #endregion
        Console.WriteLine("game was initialized");
        #region testing db
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
        dbTesting();
        #endregion
    }
    private void dbTesting()
    {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            SqlCommand test = new SqlCommand("delete from FoodChainCards where Username='player1'\r\ndelete from Games where Username='player1'\r\ndelete from Hand where Username='player1'\r\ninsert into Games(Username, Time, Deck) values\r\n('player1', 36, ',1,1,1,1,1,1,1,1')\r\n\r\ninsert into Hand(Username, CardID) values\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus');\r\n\r\nInsert into FoodChainCards(Username, CardID, RowNo, PositionNo) values\r\n('player1', 'Poa pratensis', 0, 0),\r\n('player1', 'Omocestus viridulus', 0, 1),\r\n('player1', 'Turdus merula', 0, 2),\r\n('player1', 'Pantherophis obsoletus', 0, 3),\r\n('player1', 'Tyto alba', 0, 4),\r\n('player1', 'Poa pratensis', 1, 0),\r\n('player1', 'Microtus arvalis', 1, 1);", sqlConnection);
            sqlConnection.Open(); test.ExecuteNonQuery(); sqlConnection.Close();
        }
    }
    #region start/continue
    private void PlayButton_Click(object sender, EventArgs e)
    {
        foreach (Control control in menuControls)
        {
            control.Enabled = false;
            control.Hide();
        }
        Start();
    }
    private void continueButton_Click(object sender, EventArgs e)
    {
        if (game.username != "")
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM Games WHERE Username='{game.username}'", sqlConnection);
                sqlConnection.Open();
                int b = (int)cmd.ExecuteScalar();
                if (b == 1) Continue();
                else MessageBox.Show("there is no saved game");
                sqlConnection.Close();
            }
        }
        else MessageBox.Show("you are not logged in.");
    }
    private void Start()
    {
        foreach (Card c in playerHand)
        {
            c.Hide();
            c.Image.Dispose();
            c.Dispose();
        }
        playerHand.Clear();

        foreach (List<Card> subchain in playerChain)
        {
            foreach (Card c in subchain)
            {
                c.Hide();
                c.Image.Dispose();
                c.Dispose();
            }
            subchain.Clear();
        }
        playerChain.Clear();
        cells.Clear();
        UpdateCells();

        GenerateDeck();
        //
        imageIndex = 3;
        //
        timeLeft = 180;
        labelTimer.Show();
        labelTimer.Text = "";
        overallScore = 0;
        label.Show();
        label.Text = "";
        label.Location = new Point(workingWidth / 10, workingHeight / 20);

        if (game.username != "")
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                CleanDb();
                SqlCommand createGame = new SqlCommand($"INSERT INTO Games(Username, Time, Deck) VALUES ('{game.username}', 40, '{sb.ToString()}')", sqlConnection);
                sqlConnection.Open();
                createGame.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }

        if (imageIndex == 3)
        {
            readySteadyGo.Show();
            readySteadyGo.Enabled = true;
            try
            {
                readySteadyGo.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", imageIndex.ToString() + ".png"));
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
    private void LoadPlayerChain() //update prevLocations
    {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            sqlConnection.Open();

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

                    Card card = new Card(
                        scientificName,
                        commonName,
                        description,
                        Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{scientificName}.jpg"),
                        hierarchy,
                        habitat,
                        cells[rowNo][positionNo].Item1.Location,
                        true 
                        );
                    card.rectLocation = cells[rowNo][positionNo].Item1.Location;
                    //card.prevLocation = new Point(card.Width * MainForm.playerHand.Count, workingHeight - card.Height); //do something about it

                    while (playerChain.Count <= rowNo + 1) playerChain.Add(new List<Card>());
                    playerChain[rowNo].Add(card);
                    Controls.Add(card);
                }
            }
        }
    }
    private void LoadCells()
    {
        cells.Clear();
        int rows = 0, columns = 0;
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            sqlConnection.Open();

            SqlCommand getSizes = new SqlCommand("SELECT RowNo, MAX(PositionNo) AS MaxPositionNo\r\nFROM FoodChainCards\r\nWHERE Username = @Username\r\nGROUP BY RowNo\r\n", sqlConnection);
            getSizes.Parameters.AddWithValue("@Username", game.username);
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
                            Rectangle rect = new Rectangle(j * (cell.Width) + cell.X, rows * cell.Height + cell.Y, cell.Width, cell.Height);
                            cells[rows].Add((rect, false));
                        }
                        else
                        {
                            Rectangle rect = new Rectangle(j * (cell.Width) + cell.X, rows * cell.Height + cell.Y, cell.Width, cell.Height);
                            cells[rows].Add((rect, true));
                        }
                        Console.WriteLine($"Added cell {j} to row {rows}");
                    }
                }
            }
        }
    }
    private void Continue()
    {
        string strArr = "";
        cells = new List<List<(Rectangle, bool)>>();
            
        foreach (Control control in menuControls) control.Hide();
        imageIndex = 3;
        labelTimer.Show();
        labelTimer.Text = "";
        label.Show();
        label.Text = "";
        label.Location = new Point(workingWidth / 10, workingHeight / 20);
        readySteadyGo.Show();
        readySteadyGo.Enabled = true;
        try { readySteadyGo.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", imageIndex.ToString() + ".png")); }
        catch (Exception ex) { MessageBox.Show("Failed to load image: " + ex.Message); }
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            SqlCommand getSizes = new SqlCommand(
                $"SELECT RowNo, MAX(PositionNo) AS MaxPositionNo " +
                $"FROM FoodChainCards " +
                $"WHERE Username='{game.username}' " +
                $"GROUP BY RowNo " +
                $"ORDER BY RowNo; ", sqlConnection);
            SqlCommand pullGame = new SqlCommand($"SELECT * FROM Games WHERE Username='{game.username}'", sqlConnection);
            sqlConnection.Open();
            using (SqlDataReader reader = pullGame.ExecuteReader())
            {
                while (reader.Read())
                {
                    timeLeft = int.Parse(reader["Time"].ToString());
                    strArr = reader["Deck"].ToString();
                }
            }
            foreach (char c in strArr)
            {
                if (c == ',') continue;
                deck.Push(int.Parse(c.ToString()));
            }
            SqlCommand pullHand = new SqlCommand(
                $"SELECT Hand.CardID, Organisms.Common_name, Organisms.Habitat, Organisms.Hierarchy, Organisms.Description " +
                $"FROM Hand " +
                $"JOIN Organisms ON Hand.CardID = Organisms.Scientific_name " +
                $"WHERE Username='{game.username}'", sqlConnection);
            using (SqlDataReader reader = pullHand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Card c = new Card(
                        reader["CardID"].ToString(),
                        reader["Common_name"].ToString(),
                        reader["Description"].ToString(),
                        Path.Combine(Environment.CurrentDirectory, "assets", "photos", reader["CardID"].ToString() + ".jpg"),
                        (int)reader["Hierarchy"],
                        reader["Habitat"].ToString(),
                        new Point(Card.cardWidth * playerHand.Count, Height - MainForm.workingWidth / 10),
                        false
                        );
                    playerHand.Add(c);
                    Controls.Add(c);
                }
            }
            LoadCells();
            LoadPlayerChain();
            sqlConnection.Close();
            foreach (List<Card> chain in playerChain) 
            {
                foreach (Card c in chain) c.Hide();        
            }
            foreach (Card c in playerHand) c.Hide();
            readySteadyGoTimer.Start();
        }
    }
    private void GenerateDeck()
    {
        Console.WriteLine("generating deck");
        int randIdx;
        deck = new Stack<int>();
        sb.Append(',');
        for (int i = 0; i < 40; i++)
        {
            randIdx = rnd.Next(1, GetNumberOfOrganisms() + 1);
            deck.Push(randIdx);
            if (i != 14) sb.Append(randIdx + ",");
            else sb.Append(randIdx);
        }
    }
    private void readySteadyGoTimer_Tick(object sender, EventArgs e)
    {
        imageIndex--;
        if (imageIndex > 0)
        {
            readySteadyGo.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", imageIndex.ToString() + ".png"));
        }
        else
        {
            Console.WriteLine("\nthe game started");
            readySteadyGoTimer.Stop();
            readySteadyGo.Hide();
            foreach (Control control in gameControls)
            {
                control.Enabled = true;
                control.Show();
            }
            if (deck.Count == 0)
            {
                drawCardButton.Enabled = false;
                drawCardButton.Hide();
            }
            foreach (Control control in endControls) { control.Enabled = false; control.Hide(); }
            foreach (List<Card> chain in playerChain)
            {
                foreach (Card c in chain) c.Show();
            }
            foreach (Card c in playerHand) c.Show();
            countDownTimer.Start();
            Invalidate();
        }
    }
    private void countDownTimer_Tick(object sender, EventArgs e)
    {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            SqlCommand updateTimer = new SqlCommand($"UPDATE Games SET Time='{timeLeft}' WHERE Username='{game.username}'", sqlConnection);
            sqlConnection.Open();
            if (timeLeft > -1)
            {
                updateTimer.ExecuteNonQuery();
                labelTimer.Text = timeLeft.ToString();
            }
            else
            {
                countDownTimer.Stop();
                labelTimer.Hide();
                if (game.username != "")
                {
                    UpdateStatsLabel();
                    CleanDb();        
                };

                label.Location = new Point(workingWidth / 2 - label.Width, 100);
                label.Font = largeFont;
                label.Text = "Score: " + overallScore.ToString();
                foreach (Control control in endControls)
                {
                    control.Enabled = true;
                    control.Show();
                }
                foreach (Control control in gameControls) control.Enabled = false;
                foreach (Card c in playerHand) c.Enabled = false;
                foreach (List<Card> subchain in playerChain)
                {
                    foreach (Card card in subchain) card.Enabled = false;
                }
            }
            timeLeft--;
            sqlConnection.Close();
        }
    }
    private void retryButton_Click(object sender, EventArgs e)
    {
        foreach (Control control in menuControls)
        {
            control.Enabled = false;
            control.Hide();
        }
        foreach (Control control in endControls)
        {
            control.Enabled = false;
            control.Hide();
        }
        foreach (Control control in gameControls)
        {
            control.Enabled = false;
            control.Hide();
        }
        Start();
    }
    private void goToMenuButton_Click(object sender, EventArgs e)
    {
        label.Hide();
        labelTimer.Hide();
        foreach (Control control in gameControls) { control.Enabled = false; control.Hide(); }
        foreach (Control control in endControls) { control.Enabled = false; control.Hide(); }
        foreach (Control control in menuControls) { control.Enabled = true; control.Show(); }
        foreach (Card c in playerHand)
        {
            c.Hide();
            c.Image.Dispose();
            c.Dispose();
        }
        playerHand.Clear();
        foreach (List<Card> subchain in playerChain)
        {
            foreach (Card c in subchain)
            {
                c.Hide();
                c.Image.Dispose();
                c.Dispose();
            }
            subchain.Clear();
        }
        foreach (Control control in youSureWannaQuitControls) { control.Hide(); control.Enabled = false; }
        Invalidate();
    }
    private void goToMenuInGameButton_Click(object sender, EventArgs e)
    {
        countDownTimer.Stop();
        UpdateStatsLabel();
        foreach (Control control in gameControls) control.Hide();
        foreach (Card c in playerHand) c.Hide();
        foreach (List<Card> subchain in playerChain)
        {
            foreach (Card c in subchain) c.Hide();
        }
        foreach (Control control in youSureWannaQuitControls)
        {
            control.Show();
            control.Enabled = true;
        }
        Invalidate();
    }
    private void yesButton_Click(object sender, EventArgs e) => goToMenuButton_Click(sender, e);
    private void noButton_Click(object sender, EventArgs e)
    {
        foreach (Control control in youSureWannaQuitControls) control.Hide();
        foreach (Control control in gameControls) control.Show();
        foreach (Card c in playerHand) c.Show();
        foreach (List<Card> subchain in playerChain)
        {
            foreach (Card c in subchain) c.Show();
        }
        countDownTimer.Start();
    }
    private void Exit(object sender, EventArgs e) => Close();
    #endregion
    #region ux/ui
    private void OnResize(object sender, EventArgs e)
    {
        Height = (int)(Width * 0.5625);
        workingHeight = ClientRectangle.Height;
        workingWidth = ClientRectangle.Width;

        drawCardButton.Width = workingHeight / 8;
        drawCardButton.Height = workingWidth / 10;

        drawCardButton.Location = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);

        chainButton.Width = workingWidth / 10;
        chainButton.Height = workingHeight / 10;
        chainButton.Location = new Point(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2);

        foreach (Control c in menuControls)
        {
            c.Size = new Size(50, 30);
            c.Location = new Point(workingWidth / 2 - loginButton.Width / 2, workingHeight / 2 - loginButton.Height / 2);
        }    

        Card.cardWidth = workingHeight / 8;
        Card.cardHeight = workingWidth / 10;
        for (int i = 0; i < playerHand.Count; i++)
        {
            playerHand[i].Width = workingHeight / 8;
            playerHand[i].Height = workingWidth / 10;
            playerHand[i].Location = playerHand[i].prevLocation = new Point(Card.cardWidth * i, workingHeight - Card.cardHeight);
        }
        foreach (List<Card> chain in playerChain)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                chain[i].Width = workingHeight / 8;
                chain[i].Height = workingWidth / 10;
                //chain[i].Location = new Point((int)(chain[i].Location.X * 0.5625), (int)(chain[i].Location.Y * 0.5625));
                chain[i].prevLocation = new Point(playerHand.LastOrDefault().Location.X + Card.cardWidth * i, workingHeight - Card.cardHeight);
            }
        }
        for (int i = 0; i < playerHand.Count; i++)
        {
            playerHand[i].Width = workingHeight / 8;
            playerHand[i].Height = workingWidth / 10;
            playerHand[i].Location = new Point(Card.cardWidth * i, workingHeight - Card.cardHeight);
        }
        label.Location = new Point(workingWidth / 10, workingHeight / 8);
        Invalidate();
    }
    public void Draw(object sender, PaintEventArgs e)
    {
        if (countDownTimer.Enabled)
        {
            using (Pen pen = new Pen(Color.NavajoWhite, 6.0f))
            {
                foreach (List<(Rectangle, bool)> tuples in cells)
                {
                    foreach ((Rectangle, bool) tuple in tuples) e.Graphics.DrawRectangle(pen, tuple.Item1);
                }
            }
        }
    }
    private async void Display(string s) {
        label.Text = s;
        await System.Threading.Tasks.Task.Delay(5000).ContinueWith(
            _ => Invoke(new MethodInvoker(() => label.Text = ""))
        );
    }
    public static void UpdateCells() 
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
                        rect = new Rectangle(j * (cell.Width) + cell.X, i * cell.Height + cell.Y, cell.Width, cell.Height);
                        cells[i].Add((rect, false));
                    }
                    else
                    {
                        rect = new Rectangle(j * (cell.Width) + cell.X, i * cell.Height + cell.Y, cell.Width, cell.Height);
                        cells[i].Add((rect, true));
                    }
                }
            }
            else
            {
                rect = new Rectangle(cell.X, i * cell.Height + cell.Y, cell.Width, cell.Height);
                cells[i].Add((rect, false));
            }
        }
    }
    private void drawCardButton_MouseEnter(object sender, EventArgs e)
    {
        drawCardButton.Location = new Point(drawCardButton.Location.X - 3, drawCardButton.Location.Y - 3);
        drawCardButton.Size = new Size(drawCardButton.Width + 6, drawCardButton.Height + 6);
    }
    private void drawCardButton_MouseLeave(object sender, EventArgs e)
    {
        drawCardButton.Location = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);
        drawCardButton.Size = new Size(workingHeight / 8, workingWidth / 10);
    }
    private void chainButton_MouseEnter(object sender, EventArgs e)
    {
        chainButton.Location = new Point(chainButton.Location.X - 3, chainButton.Location.Y - 3);
        chainButton.Size = new Size(chainButton.Width + 6, chainButton.Height + 6);
    }
    private void chainButton_MouseLeave(object sender, EventArgs e)
    {
        chainButton.Location = new Point(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2);
        chainButton.Size = new Size(workingWidth / 10, workingHeight / 10);
    }
    #endregion
    #region login and stuff
    private void Login(object sender, EventArgs e)
    {
        if (game.username == "")
        {
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
        }
        else
        {
            game.username = "";
            stats.Text = "";
            loginButton.Text = "log in";
        }
    }
    private void CleanDb()
    {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            SqlCommand clearGame = new SqlCommand($"DELETE FROM Games WHERE Username='{game.username}'", sqlConnection);
            SqlCommand clearHand = new SqlCommand($"DELETE FROM Hand WHERE Username='{game.username}'", sqlConnection);
            SqlCommand clearChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}'", sqlConnection);
            sqlConnection.Open();
            clearGame.ExecuteNonQuery();
            clearHand.ExecuteNonQuery();
            clearChain.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    private void UpdateStatsLabel() 
    { 
        if (game.username != "") stats.Text = $"Hey, {game.username}!\ntotal points: {totalPoints}";
    }
    #endregion
    #region chain
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
    private void chainButton_Click(object sender, EventArgs e) => Chain();
    private void Chain()
    {
        string str = "";
        foreach (List<Card> chain in playerChain)
        {
            str += $"   chain {playerChain.IndexOf(chain).ToString()}\n";
            foreach (Card c in chain) str += "        " + $"{c.CommonName}" + '\n';
        }
        Console.WriteLine("current chain:\n" + str);
        
        int chainIndex = 0;
        earned = 0;
        for (int index = 0; index < playerChain.Count - 1; index++)
        {
            bool valid = true;
            FixChainIndices(playerChain[index]);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand disposeChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}' AND RowNo={chainIndex}", sqlConnection);
                List<string> cards = new List<string>();
                if (playerChain[index].Count < 2 && index != playerChain.Count - 1)
                {
                    Display("the chain must consist of at least two organisms.");
                    chainIndex++;
                    return;
                }
                else
                {

                    lastLink();
                    Console.WriteLine(lastOrganism);
                    Console.WriteLine(longestChainIndex);
                    sqlConnection.Open();
                    for (int i = 0; i < playerChain.Count; i++)
                    {
                        if (i != longestChainIndex && playerChain[i].Count != 0)
                        {
                            SqlCommand sqlCommand = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer = '{lastOrganism}' AND Consumed = '{playerChain[i][playerChain[i].Count - 1].ScientificName}'", sqlConnection);
                            Console.WriteLine(sqlCommand.CommandText);
                            int b = (int)sqlCommand.ExecuteScalar();
                            if (b == 0)
                            {
                                Display("food chain is invalid");
                                Console.WriteLine($"playerChain[{playerChain.IndexOf(playerChain[index])}] is invalid as {lastOrganism} doesn't eat {playerChain[i][playerChain[i].Count - 1].CommonName}");
                                valid = false;
                                Console.WriteLine($"Moving cards back to hand");
                                for (int k = 0; k < playerChain.Count; k++)
                                {
                                    Console.WriteLine($"row[{k}]");
                                    for (int j = 0; j < playerChain[k].Count; j++)
                                    {
                                        Console.WriteLine($"card[{k}][{j}]");
                                        playerHand.Add(playerChain[k][j]);
                                        playerChain[k][j].prevLocation = new Point(playerChain[k][j].Width * playerHand.Count, workingHeight - playerChain[k][j].Height);
                                        Console.WriteLine($"{i} {j}");
                                        playerChain[k][j].Location = playerChain[k][j].prevLocation;
                                        playerChain[k][j].Picked = false;
                                        playerChain[k][j].inChain = false;
                                        playerChain[k][j].BackColor = Color.Gray;
                                    }
                                }
                                if (game.username != "") PushToHand(cards);
                                earned = 0;
                                chainIndex++;
                                playerChain.Clear();
                                UpdateCells();
                                Invalidate();
                                return;
                            }
                        }
                    }
                    for (int i = 0; i < playerChain[index].Count - 1; i++) // inefficient as i know that chains share a predator so TODO!
                    {
                        cards.Add(playerChain[index][i].ScientificName);
                        SqlCommand sqlCommand = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer='{playerChain[index][i + 1].ScientificName}' AND Consumed='{playerChain[index][i].ScientificName}'", sqlConnection);
                        int b = (int)sqlCommand.ExecuteScalar();
                        if (b == 0) 
                        {
                            Display("food chain is invalid");
                            Console.WriteLine($"playerChain[{playerChain.IndexOf(playerChain[index])}] is invalid as {playerChain[index][i + 1].CommonName} doesn't eat {playerChain[index][i].CommonName}");
                            valid = false;
                            Console.WriteLine($"Moving cards back to hand");
                            for (int k = 0; k < playerChain.Count; k++)
                            {
                                Console.WriteLine($"row[{k}]");
                                for (int j = 0; j < playerChain[k].Count; j++)
                                {
                                    Console.WriteLine($"card[{k}][{j}]");
                                    playerChain[k][j].Location = playerChain[k][j].prevLocation;
                                    playerChain[k][j].Picked = false;
                                    playerChain[k][j].inChain = false;
                                    playerChain[k][j].BackColor = Color.Gray;
                                    playerHand.Add(playerChain[k][j]);
                                }
                            }
                            if (game.username != "") PushToHand(cards);
                            earned = 0;
                            chainIndex++;
                            playerChain.Clear();
                            Console.WriteLine($"Chain size = {playerChain.Count}");
                            UpdateCells();
                            Invalidate();
                            return;
                        }
                    }
                    if (valid)
                    {
                        overallScore += CalcScore(playerChain[index].Count);
                        earned += CalcScore(playerChain[index].Count);
                        if (game.username != "")
                        {
                            totalPoints += CalcScore(playerChain[index].Count);
                            SqlCommand updatePoints = new SqlCommand($"UPDATE Players SET Points={totalPoints} WHERE Username='{game.username}'", sqlConnection);
                            updatePoints.ExecuteNonQuery();
                            disposeChain.ExecuteNonQuery();
                        }
                        foreach (Card c in playerChain[index])
                        {
                            Controls.Remove(c);
                            c.Image.Dispose();
                        }
                        sqlConnection.Close();
                        playerChain[index].Clear();
                        chainIndex++;
                        for (int j = 0; j < playerHand.Count; j++) playerHand[j].Location = playerHand[j].prevLocation = new Point(Card.cardWidth * (j), Height - Card.cardHeight);
                    }
                    UpdateCells();
                    Invalidate();
                }
            }
        }
        Display($"+{earned} points");
        earned = 0;
    }
    public void PushToChain(Card c, int RowNo, int ColNo) 
    {
        SqlCommand pushCardToChain = new SqlCommand($"INSERT INTO FoodChainCards VALUES ('{game.username}', '{c.ScientificName}', {RowNo}, {ColNo})", sqlConnection);
        sqlConnection.Open();
        pushCardToChain.ExecuteNonQuery();
        sqlConnection.Close();
    }
    public void RemoveFromChain(Card c, int RowNo, int ColNo) 
    {
        SqlCommand deleteCardFromChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}' AND CardID='{c.ScientificName}' AND RowNo={RowNo} AND PositionNo={ColNo}", sqlConnection);
        sqlConnection.Open();
        deleteCardFromChain.ExecuteNonQuery();
        sqlConnection.Close();
    }
    private void chainButton_MouseMove(object sender, MouseEventArgs e)
    {
        if (MousePosition.X < chainButtonRectangle.Right && MousePosition.X > chainButtonRectangle.Left && MousePosition.Y < chainButtonRectangle.Bottom && MousePosition.Y > chainButtonRectangle.Top)
        {
            chainButton.Width = workingWidth / 10 + 5;
            chainButton.Height = workingHeight / 10 + 5;
        }
        else
        {
            chainButton.Width = workingWidth / 10;
            chainButton.Height = workingHeight / 10;
        }
    }
    public static void PushToHand(List<string> cards)
    {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            SqlCommand insert;
            sqlConnection.Open();
            foreach (string name in cards) 
            {
                insert = new SqlCommand($"INSERT INTO Hand VALUES ('{game.username}', '{name}')", sqlConnection);
                insert.ExecuteNonQuery();
            }
            sqlConnection.Close();
        }
    }
    public void lastLink()
    {
        int longestChainCount = -1;
        for (int i = 0; i < playerChain.Count; i++)
        {
            if (playerChain[i].Count > longestChainCount)
            {
                longestChainCount = playerChain[i].Count;
                longestChainIndex = i;
            }
        }

        lastOrganism = playerChain[longestChainIndex][playerChain[longestChainIndex].Count - 1].ScientificName;
    }
    #endregion
    #region draw card
    private int GetNumberOfOrganisms()
    {
        int count = 0;
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
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
    public string GetScientificNameFromDeck()
    {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
            if (deck.Count != 0)
            {
                SqlCommand cmd = new SqlCommand($"WITH NumberedRows AS ( SELECT Scientific_name, Common_name, ROW_NUMBER() OVER(ORDER BY Scientific_name) AS RowNum FROM Organisms ) SELECT Scientific_name, Common_name FROM NumberedRows WHERE RowNum = {deck.Pop()};", sqlConnection);
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
    private int Count(List<List<Card>> chain) {
        int counter = 0;
        foreach (List<Card> subchain in chain) counter += subchain.Count;
        return counter;
    }
    public void DrawCard(object sender, EventArgs e)
    {
        if (playerHand.Count + Count(playerChain) < 15)
        {
            string sciname;
            for (int i = 0; i < 3; i++)
            {
                sciname = GetScientificNameFromDeck();
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM Organisms WHERE Scientific_name='{sciname}'", sqlConnection);
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Card c = new Card(
                                sciname,
                                reader["Common_name"].ToString(),
                                reader["Description"].ToString(),
                                Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{sciname}.jpg"),
                                (int)reader["Hierarchy"],
                                reader["Habitat"].ToString(),
                                new Point(Card.cardWidth * playerHand.Count, workingHeight - Card.cardHeight),
                                false
                                );
                            playerHand.Add(c);
                            Controls.Add(c);
                        }
                    }
                    if (deck.Count > 0)
                    {
                        SqlCommand removeCard = new SqlCommand($"UPDATE Games SET Deck=LEFT(Deck, LEN(DECK) - 2) WHERE Username='{game.username}'", sqlConnection);
                        removeCard.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.WriteLine($"the deck is empty, can't remove from Games.Deck in cards.mdf");
                        Display("the deck is empty");
                        drawCardButton.Enabled = false;
                        drawCardButton.Hide();
                    }
                    sqlConnection.Close();
                    PushToHand(new List<string>() { sciname });

                    //TODO! update prevLocations of cards in Chain and Hand
                }
            }
        }
        else Display("cannot hold more than 15 cards"); //instead call CheckHand();
    }
    #endregion
}