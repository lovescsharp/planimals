using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace planimals
{
    public partial class MainForm : Form
    {
        ///logic
        //push and pull changes to Cards.mdf
        // (*): Users points
        // ( ): Game state
        //      (*) : time left
        //      (*) : deck
        //      ( ) : chain(s)
        //      ( ) : hand

        #region attributes
        private Button playButton;
        public static Button continueButton;
        public static Button loginButton;
        private Button exitButton;

        public static string username;
        public static int totalPoints;
        public static Label stats;

        public static List<(Card, Point, Point, long, long)> MoveList;
        private static Random rnd; Timer timer1;
        private Stopwatch sw1;

        private Timer countDownTimer;
        private int timeLeft;
        private Label labelTimer;

        private Timer readySteadyGoTimer;
        private static PictureBox readySteadyGo;
        private int imageIndex = 3;

        public static string currentDir = Environment.CurrentDirectory;
        private static string dbPath = currentDir + "\\cards.mdf";
        public static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" + $"AttachDbFilename={dbPath}" + ";Integrated Security=True;Connect Timeout=30";
        private static readonly SqlConnection sqlConnection = new SqlConnection(connectionString);

        public static int workingHeight;
        public static int workingWidth;

        public static Stack<int> deck;
        private StringBuilder sb;
        public static List<Card> playerHand;
        public static List<List<Card>> playerChain;
        public static Rectangle fieldRectangle;
        public static Rectangle cardRectangle;

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

        private Label label;

        private Font largeFont;
        private Font smallFont;

        private int overallScore;
        #endregion
        public MainForm()
        {
            #region Init component
            /*
                    -=====-                             -=====-
                      _..._                               _..._
                    .~     `~.                         .~`     ~.
            ,_     /          }                       {          \     _,
            ,_\'--, \   _.'`~~/                        \~~`'._   / ,--'/_,
             \'--,_`{_,}    -(                          )-    {,_}`_,--'/
              '.`-.`\;--,___.'_                        _'.___,--;/`.-`.'
                '._`/    |_ _{@}                       {@}_ _|    \`_.'
                /     ` |-';/                        \;'-| `     \
               /   \    / */ InitializeComponent(); /*|  \    /   \
              /     '--;_                                _;--'     \
             _\          `\                            /`          /_
            / |`-.___.    /                           \    .___,-'| \
            `--`------'`--`^^^^^^^^^^^^^^^^^^^^^^^^^^^^`--`'------`--`
            */
            #endregion
            #region UI
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MinimumSize = new Size(1120, 620);
            Text = "planimals";
            StartPosition = FormStartPosition.CenterScreen;

            largeFont = new Font("Arial", 28);
            largeFont = new Font("Arial", 28);
            smallFont = new Font("Arial", 14);

            //static size
            MinimizeBox = false;
            //

            Height = Screen.PrimaryScreen.WorkingArea.Height;
            Width = Screen.PrimaryScreen.WorkingArea.Width;
            workingHeight = ClientRectangle.Height;
            workingWidth = ClientRectangle.Width;

            fieldRectangle = new Rectangle(
                workingWidth / 100 * 20,
                workingHeight / 4,
                workingWidth / 10 * 6,
                workingHeight / 2
            );

            BackColor = Color.Black;

            username = "";
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

            retryButton = new PictureBox();
            retryButton.Image = Image.FromFile(currentDir + "\\assets\\photos\\retry.png");
            retryButton.SizeMode = PictureBoxSizeMode.StretchImage;
            retryButton.Size = new Size(workingWidth / 10, workingWidth / 10);
            retryButton.Location = new Point(workingWidth / 2 - retryButton.Width / 2, workingHeight / 2 - retryButton.Height / 2);
            retryButton.Click += retryButton_Click;
            Controls.Add(retryButton);

            goToMenuButton = new PictureBox();
            goToMenuButton.Image = Image.FromFile(currentDir + "\\assets\\photos\\exit.png");
            goToMenuButton.SizeMode = PictureBoxSizeMode.StretchImage;
            goToMenuButton.Size = new Size(workingWidth / 10, workingWidth / 8);
            goToMenuButton.Location = new Point(workingWidth / 2 + goToMenuButton.Width, workingHeight / 2 - goToMenuButton.Height / 2);
            goToMenuButton.Click += goToMenuButton_Click;
            Controls.Add(goToMenuButton);

            goToMenuInGameButton = new PictureBox();
            goToMenuInGameButton.Image = Image.FromFile(currentDir + "\\assets\\photos\\exit.png");
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
            youSureWannaQuitLabel.Size = new Size(600, 50);
            youSureWannaQuitLabel.Location = new Point(workingWidth / 2 - 200, workingHeight / 3);
            youSureWannaQuitLabel.Text = "Are you sure you want to quit?";
            youSureWannaQuitLabel.ForeColor = Color.White;
            Controls.Add(youSureWannaQuitLabel);

            drawCardButton = new PictureBox();
            drawCardButtonBack = Image.FromFile(currentDir + "\\assets\\photos\\back.png");
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
            drawCardButton.MouseMove += DrawCardButton_MouseMove;

            chainButton = new PictureBox();
            chainButtonBack = Image.FromFile(currentDir + "\\assets\\photos\\chain.png");
            chainButton.SizeMode = PictureBoxSizeMode.StretchImage;
            chainButton.Width = workingWidth / 10;
            chainButton.Height = workingHeight / 10;
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
            chainButton.MouseMove += chainButton_MouseMove;

            label = new System.Windows.Forms.Label();
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

            #endregion
            #region backs
            MoveList = new List<(Card, Point, Point, long, long)>();
            timer1 = new Timer();
            timer1.Tick += new EventHandler(MoveCards);
            timer1.Interval = 10;
            sw1 = new Stopwatch();

            countDownTimer = new Timer();
            countDownTimer.Interval = 1000;
            countDownTimer.Tick += new EventHandler(countDownTimer_Tick);

            rnd = new Random();
            deck = new Stack<int>();
            sb = new StringBuilder();
            playerHand = new List<Card>();
            playerChain = new List<List<Card>>() { new List<Card>() { } };

            MouseClick += new MouseEventHandler(MouseLeftClick);
            MouseMove += DrawCardButton_MouseMove;
            Paint += new PaintEventHandler(DrawFieldBorders);
            Resize += new EventHandler(OnResize);

            gameControls = new List<Control>() { drawCardButton, chainButton, goToMenuInGameButton };
            menuControls = new List<Control>() { loginButton, playButton, continueButton, exitButton, stats };
            endControls = new List<Control>() { retryButton, goToMenuButton };
            youSureWannaQuitControls = new List<Control>() { yesButton, noButton, youSureWannaQuitLabel };

            foreach (Control control in gameControls)
            {
                control.Enabled = false;
                control.Hide();
            }
            foreach (Control control in endControls)
            {
                control.Enabled = false;
                control.Hide();
            }
            foreach (Control control in youSureWannaQuitControls)
            {
                control.Hide();
                control.Enabled = false;
            }
            #endregion
        }

        /*
delete from FoodChainCards where Username='player1'
delete from Games where Username='player1'
delete from Hand where Username='player1'
insert into Games(Username, Time, Deck) values
('player1', 36, ',1, 1, 1, 1, 1, 1, 1, 1')
insert into Hand(Username, CardID) values
('player1', 'Omocestus viridulus'),
('player1', 'Omocestus viridulus'),
('player1', 'Omocestus viridulus');
Insert into FoodChainCards(Username, CardID, RowNo, PositionNo) values
('player1', 'Poa pratensis', 0, 0),
('player1', 'Omocestus viridulus', 0, 1),
('player1', 'Turdus merula', 0, 2),
('player1', 'Black Rat Snake', 0, 3),
('player1', 'Tyto alba', 0, 4),
('player1', 'Poa pratensis', 1, 0),
('player1', 'Microtus arvalis', 1, 1);
         */

        private void dbTesting()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand test = new SqlCommand("delete from FoodChainCards where Username='player1'\r\ndelete from Games where Username='player1'\r\ndelete from Hand where Username='player1'\r\n\r\ninsert into Games(Username, Time, Deck) values\r\n('player1', 36, ',1, 1, 1, 1, 1, 1, 1, 1')\r\n\r\ninsert into Hand(Username, CardID) values\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus');\r\n\r\n\r\nInsert into FoodChainCards(Username, CardID, RowNo, PositionNo) values\r\n('player1', 'Poa pratensis', 0, 0),\r\n('player1', 'Omocestus viridulus', 0, 1),\r\n('player1', 'Turdus merula', 0, 2),\r\n('player1', 'Black Rat Snake', 0, 3),\r\n('player1', 'Tyto alba', 0, 4),\r\n('player1', 'Poa pratensis', 1, 0),\r\n('player1', 'Microtus arvalis', 1, 1);", sqlConnection);
                sqlConnection.Open(); test.ExecuteNonQuery(); sqlConnection.Close();
            }
        }

        #region flow
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
            if (username != "")
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM Games WHERE Username='{username}'", sqlConnection);
                    sqlConnection.Open();
                    int b = (int)cmd.ExecuteScalar();
                    if (b == 1)
                    {
                        Continue();
                    }
                    else
                    {
                        MessageBox.Show("there is no saved game");
                    }
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

            GenerateDeck();
            imageIndex = 3;
            timeLeft = 40;
            labelTimer.Show();
            labelTimer.Text = "";
            overallScore = 0;
            label.Show();
            label.Text = "";
            label.Location = new Point(workingWidth / 10, workingHeight / 20);

            if (username != "")
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    CleanDb();
                    SqlCommand createGame = new SqlCommand($"INSERT INTO Games(Username, Time, Deck) VALUES ('{username}', 40, '{sb.ToString()}')", sqlConnection);
                    sqlConnection.Open();
                    createGame.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }

            readySteadyGo.Show();
            readySteadyGo.Enabled = true; 
            try
            {
                readySteadyGo.Image = Image.FromFile(currentDir + "\\assets\\photos\\" + imageIndex.ToString() + ".png");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load image: " + ex.Message);
            }

            timer1.Start();
            sw1.Start();
            readySteadyGoTimer.Start();
        }
        private void Continue()
        {

            /*
             _           _   _
            | |_ ___ ___| |_(_)_ __   __ _
            | __/ _ / __| __| | '_ \ / _` |
            | ||  __\__ | |_| | | | | (_| |
             \__\___|___/\__|_|_| |_|\__, |
                                      |__/
             */

            dbTesting();

            /*
             _           _   _
            | |_ ___ ___| |_(_)_ __   __ _
            | __/ _ / __| __| | '_ \ / _` |
            | ||  __\__ | |_| | | | | (_| |
             \__\___|___/\__|_|_| |_|\__, |
                                      |__/
             */


            string strArr = "";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand pullGame = new SqlCommand($"SELECT * FROM Games WHERE Username='{username}'", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader reader = pullGame.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        timeLeft = int.Parse(reader["Time"].ToString());
                        strArr = reader["Deck"].ToString();
                    }
                }
                foreach (Char c in strArr)
                {
                    if (c == ',') continue;
                    deck.Push(int.Parse(c.ToString()));
                }
                SqlCommand pullHand = new SqlCommand($"SELECT * FROM Hand WHERE Username='{username}'", sqlConnection);
                List<string> cardsBuffer = new List<string>();
                using (SqlDataReader reader = pullHand.ExecuteReader())
                {
                    while (reader.Read()) cardsBuffer.Add(reader["CardID"].ToString());
                }
                foreach (string sciname in cardsBuffer)
                {
                    SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM Organisms WHERE Scientific_name='{sciname}'", sqlConnection);
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cname = reader["Common_name"].ToString();
                            var desc = reader["Description"].ToString();
                            var path = currentDir + "\\assets\\photos\\" + $"{sciname}.jpg";
                            int hierarchy = (int)reader["Hierarchy"];
                            var habitat = reader["Habitat"].ToString();
                            Card c = new Card(sciname, cname, desc, path, hierarchy, habitat, new Point(Card.pictureBoxWidth * playerHand.Count, Height - Card.pictureBoxHeight));
                            playerHand.Add(c);
                            Controls.Add(c);
                        }
                    }
                }
                SqlCommand countChains = new SqlCommand($"SELECT COUNT(*) FROM FoodChainCards WHERE ", sqlConnection);
                sqlConnection.Close();

                foreach (Control control in menuControls)
                {
                    control.Hide();
                    control.Enabled = false;
                }

                imageIndex = 3;
                labelTimer.Show();
                labelTimer.Text = "";
                label.Show();
                label.Text = "";
                label.Location = new Point(workingWidth / 10, workingHeight / 20);

                readySteadyGo.Show();
                readySteadyGo.Enabled = true;
                try
                {
                    readySteadyGo.Image = Image.FromFile(currentDir + "\\assets\\photos\\" + imageIndex.ToString() + ".png");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load image: " + ex.Message);
                }

                timer1.Start();
                sw1.Start();
                readySteadyGoTimer.Start();
            }
        }
        private void GenerateDeck()
        {
            int randIdx;
            deck = new Stack<int>();
            sb.Append(',');
            for (int i = 0; i < 15; i++)
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

            if (imageIndex > 0) readySteadyGo.Image = Image.FromFile(currentDir + "\\assets\\photos\\" + imageIndex.ToString() + ".png");
            else
            {
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
                countDownTimer.Start();
            }
        }
        private void countDownTimer_Tick(object sender, EventArgs e)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand updateTimer = new SqlCommand($"UPDATE Games SET Time='{timeLeft}' WHERE Username='{username}'", sqlConnection);
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
                    if (username != "")
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
                    foreach (Control control in gameControls)
                    {
                        control.Enabled = false;
                    }
                    foreach (Card c in playerHand)
                    {
                        c.Enabled = false;
                    }
                    foreach (List<Card> subchain in playerChain)
                    {
                        foreach (Card card in subchain)
                        {
                            card.Enabled = false;
                        }
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
        }
        private void goToMenuInGameButton_Click(object sender, EventArgs e)
        {
            countDownTimer.Stop();
            UpdateStatsLabel();
            foreach (Control control in youSureWannaQuitControls)
            {
                control.Show();
                control.Enabled = true;
            }
        }
        private void yesButton_Click(object sender, EventArgs e) { goToMenuButton_Click(sender, e); }
        private void noButton_Click(object sender, EventArgs e)
        {
            foreach (Control control in youSureWannaQuitControls)
            {
                control.Hide();
                control.Enabled = false;
            }
            countDownTimer.Start();
        }
        private void Exit(object sender, EventArgs e) { Close(); }
        #endregion
        #region ux/ui
        private void OnResize(object sender, EventArgs e)
        {
            Height = (int)(Width * 0.5625);
            workingHeight = ClientRectangle.Height;
            workingWidth = ClientRectangle.Width;

            fieldRectangle = new Rectangle(
                workingWidth / 100 * 20,
                workingHeight / 4,
                workingWidth / 10 * 6,
                workingHeight / 2);

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

            Card.pictureBoxWidth = workingHeight / 8;
            Card.pictureBoxHeight = workingWidth / 10;
            for (int i = 0; i < playerHand.Count; i++)
            {
                playerHand[i].Width = workingHeight / 8;
                playerHand[i].Height = workingWidth / 10;
                playerHand[i].Location = playerHand[i].prevLocation = new Point(Card.pictureBoxWidth * i, workingHeight - Card.pictureBoxHeight);
            }
            foreach (List<Card> chain in playerChain)
            {
                for (int i = 0; i < chain.Count; i++)
                {
                    chain[i].Width = workingHeight / 8;
                    chain[i].Height = workingWidth / 10;
                    //chain[i].Location = new Point((int)(chain[i].Location.X * 0.5625), (int)(chain[i].Location.Y * 0.5625));
                    chain[i].prevLocation = new Point(playerHand.LastOrDefault().Location.X + Card.pictureBoxWidth * i, workingHeight - Card.pictureBoxHeight);
                }
            }
            for (int i = 0; i < playerHand.Count; i++)
            {
                playerHand[i].Width = workingHeight / 8;
                playerHand[i].Height = workingWidth / 10;
                playerHand[i].Location = new Point(Card.pictureBoxWidth * i, workingHeight - Card.pictureBoxHeight);
            }
            label.Location = new Point(workingWidth / 10, workingHeight / 8);
            Invalidate();
        }
        public void DrawFieldBorders(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.White, 10.0f)) e.Graphics.DrawRectangle(pen, fieldRectangle);
        }
        private async void Display(string s) {
            label.Text = s;
            await System.Threading.Tasks.Task.Delay(5000).ContinueWith( ///ooh getting rusty
                _ => Invoke(new MethodInvoker(() => label.Text = ""))
            );
        }
        #endregion
        #region login and stuff
        private void Login(object sender, EventArgs e)
        {
            if (username == "")
            {
                LoginForm loginForm = new LoginForm();
                loginForm.ShowDialog();
            }
            else
            {
                username = "";
                stats.Text = "";
                loginButton.Text = "log in";
            }
        }
        private void CleanDb() 
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
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
        private void UpdateStatsLabel(){ if (username != "") stats.Text = $"Hey, {username}!\ntotal points: {totalPoints}"; }
            #endregion
        #region logic
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
            //gonna make a better calculation of the score some day
            return score;
        }
        public void chainButton_Click(object sender, EventArgs e)
        {   int i = 0;
            foreach (List<Card> chain in playerChain)
            {
                Chain(chain, i);
                i++;
            }
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
        private void Chain(List<Card> chain, int chainIndex)
        {
            bool valid = true;
            FixChainIndices(chain);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand disposeChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{username}' AND RowNo={chainIndex}", sqlConnection);
                if (chain.Count < 2) { Display("the chain must consist of at least two organisms."); return; }
                else
                {
                    sqlConnection.Open();
                    for (int i = 0; i < chain.Count - 1; i++)
                    {
                        SqlCommand sqlCommand = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer='{chain[i + 1].scientific_name}' AND Consumed='{chain[i].scientific_name}'", sqlConnection);
                        int b = (int)sqlCommand.ExecuteScalar();
                        if (b == 0)
                        {
                            Display("Food chain is invalid");
                            valid = false;
                            for (int j = 0; j < chain.Count; j++)
                            {
                                Point location = chain[j].prevLocation;
                                Point offset = new Point(location.X - chain[j].Location.X, location.Y - chain[j].Location.Y);
                                (Card, Point, Point, long, long) data = (chain[j], chain[j].Location, offset, 400, sw1.ElapsedMilliseconds);
                                MoveList.Add(data);
                                chain[j].Picked = false;
                                chain[j].BackColor = Color.Gray;
                                SqlCommand returnToHand = new SqlCommand($"INSERT INTO Hand(Username, CardID) VALUES ('{username}', '{chain[i].scientific_name}')", sqlConnection);
                                if (username != "") returnToHand.ExecuteNonQuery();
                                playerHand.Add(chain[j]);
                            }
                            chain.Clear();
                            return;
                        }
                    }
                    if (valid)
                    {
                        Display($"+{CalcScore(chain.Count)} points\n");
                        overallScore += CalcScore(chain.Count);
                        if (username != "")
                        {
                            totalPoints += CalcScore(chain.Count);
                            SqlCommand updatePoints = new SqlCommand($"UPDATE Players SET Points={totalPoints} WHERE Username='{username}'", sqlConnection);
                            updatePoints.ExecuteNonQuery();
                            disposeChain.ExecuteNonQuery();
                        }
                        foreach (Card c in chain)
                        {
                            Controls.Remove(c);
                            c.Image.Dispose();
                        }
                        sqlConnection.Close();
                        chain.Clear();
                        for (int j = 0; j < playerHand.Count; j++) playerHand[j].Location = playerHand[j].prevLocation = new Point(Card.pictureBoxWidth * (j), Height - Card.pictureBoxHeight);
                    }
                }
            }
        }
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
                    SqlCommand cmd = new SqlCommand($"WITH NumberedRows AS( SELECT Scientific_name, ROW_NUMBER() OVER(ORDER BY Scientific_name) AS RowNum FROM Organisms ) SELECT Scientific_name FROM NumberedRows WHERE RowNum = {deck.Pop()};", sqlConnection);
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
            foreach (List<Card> subchain in chain) {
                counter += subchain.Count;
            }
            return counter;
        }
        private void DrawCardButton_MouseMove(object sender, MouseEventArgs e)
        {
            Point initPos = new Point(drawCardButton.Width - workingHeight / 100 * 5,
                workingHeight / 2 - drawCardButton.Height / 2);
            if (MousePosition.X < drawCardRectangle.Right && MousePosition.X > drawCardRectangle.Left && MousePosition.Y < drawCardRectangle.Bottom && MousePosition.Y > drawCardRectangle.Top)
            {
                drawCardButton.Location = new Point(initPos.X - 5, initPos.Y - 5);
                drawCardButton.Width = workingHeight / 8 + 5;
                drawCardButton.Height = workingWidth / 10 + 5;
            }
            else
            {
                drawCardButton.Location = new Point(initPos.X + 5, initPos.Y + 5);
                drawCardButton.Width = workingHeight / 8;
                drawCardButton.Height = workingWidth / 10;
            }
        }
        public void DrawCard(object sender, EventArgs e)
        {
            if (playerHand.Count + Count(playerChain) < 15)
            {
                string sciname = GetScientificNameFromDeck();
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM Organisms WHERE Scientific_name='{sciname}'", sqlConnection);
                    sqlConnection.Open();
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cname = reader["Common_name"].ToString();
                            var desc = reader["Description"].ToString();
                            var path = currentDir + "\\assets\\photos\\" + $"{sciname}.jpg";
                            int hierarchy = (int)reader["Hierarchy"];
                            var habitat = reader["Habitat"].ToString();
                            Card c = new Card(sciname, cname, desc, path, hierarchy, habitat, new Point(Card.pictureBoxWidth * playerHand.Count, Height - Card.pictureBoxHeight));
                            playerHand.Add(c);
                            Controls.Add(c);
                        }
                    }
                    MessageBox.Show(deck.Count.ToString());
                    if (deck.Count > 0)
                    {
                        SqlCommand removeCard = new SqlCommand($"UPDATE Games SET Deck=LEFT(Deck, LEN(DECK) - 2) WHERE Username='{username}'", sqlConnection);
                        removeCard.ExecuteNonQuery();
                    }
                    else
                    {
                        Display("the deck is empty");
                        drawCardButton.Enabled = false;
                        drawCardButton.Hide();
                    }
                    SqlCommand addToHand = new SqlCommand($"INSERT INTO Hand(Username, CardID) VALUES ('{username}', '{sciname}')", sqlConnection);
                    addToHand.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            else Display("cannot hold more than 15 cards");
        }
        #endregion
        #region fancy card moving
        Func<Point, bool> InRectangle = p => p.X < fieldRectangle.Right && p.X > fieldRectangle.Left && p.Y > fieldRectangle.Top && p.Y < fieldRectangle.Bottom;
        private void MoveCards(object sender, EventArgs e)
        {
            long currentTime = sw1.ElapsedMilliseconds;
            List<int> purgeIndexes = new List<int>();
            int index = 0;
            foreach ((Card c, Point startPosition, Point offset, long length, long startTime) in MoveList)
            {
                long durationTime = currentTime - startTime;
                if (durationTime > length)
                {
                    c.Location = new Point(startPosition.X + offset.X, startPosition.Y + offset.Y);
                    purgeIndexes.Add(index);
                    continue;
                }
                double f = F((double)(durationTime) / length, -3f);
                c.Location = new Point((int)(offset.X * f) + startPosition.X, (int)(offset.Y * f) + startPosition.Y);
                index++;
            }

            for (int i = purgeIndexes.Count - 1; i >= 0; i--)
            {
                MoveList.RemoveAt(purgeIndexes[i]);
            }
        }
        private static double F(double timeThrough, double a)
        {
            if (timeThrough >= 1) return 1;
            double x = timeThrough;
            double y = Math.Sin((3 * x) / 2);
            return y;
        }
        public void EaseInOut(Card c, Point endPosition, long length, List<Card> chain)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand removeFromHand = new SqlCommand($"DELETE TOP(1) FROM Hand WHERE Username='{username}' and CardID='{c.scientific_name}'", sqlConnection);
                SqlCommand returnToHand = new SqlCommand($"INSERT INTO Hand(Username, CardID) VALUES ('{username}', '{c.scientific_name}')", sqlConnection);
                SqlCommand disposeChain = new SqlCommand($"DELETE TOP(1) FROM FoodChainCards WHERE Username='{username}' AND RowNo={playerChain.IndexOf(chain)} AND PositionNo='{chain.IndexOf(c)}'", sqlConnection);
                SqlCommand removeFromChain = new SqlCommand($"");
                if (InRectangle(endPosition) && !InRectangle(c.Location)) //check whether user wants to move card onto the table or just messing with them          
                {
                    Point offset = new Point(endPosition.X - c.Location.X - c.Width / 2, endPosition.Y - c.Location.Y - c.Height / 2);
                    (Card, Point, Point, long, long) data = (c, c.Location, offset, length, sw1.ElapsedMilliseconds);
                    MoveList.Add(data);
                    c.Picked = false;
                    c.BackColor = Color.Gray;
                    chain.Add(c);
                    playerHand.Remove(c);
                    removeFromHand.ExecuteNonQuery();
                    return;
                }
                else if (InRectangle(endPosition) && InRectangle(c.Location))
                {
                    Point offset = new Point(endPosition.X - c.Location.X - c.Width / 2, endPosition.Y - c.Location.Y - c.Height / 2);
                    (Card, Point, Point, long, long) data = (c, c.Location, offset, length, sw1.ElapsedMilliseconds);
                    MoveList.Add(data);
                    c.Picked = false;
                    c.BackColor = Color.Gray;
                    return;
                }
                else if (Card.InRectangle(c.Location) && !InRectangle(endPosition))
                {
                    Point location = c.prevLocation;
                    Point offset = new Point(location.X - c.Location.X, location.Y - c.Location.Y);
                    (Card, Point, Point, long, long) data = (c, c.Location, offset, length, sw1.ElapsedMilliseconds);
                    MoveList.Add(data);
                    c.Picked = false;
                    c.BackColor = Color.Gray;
                    playerHand.Add(c);
                    returnToHand.ExecuteNonQuery();
                    chain.Remove(c);

                    return;
                }
                else
                {
                    c.Picked = false;
                    c.BackColor = Color.Gray;
                }
                sqlConnection.Close();
            }
        }
        private void MouseLeftClick(object sender, MouseEventArgs e)
        {
            foreach (Card c in playerHand)
            {
                if (c.Picked == true)
                {
                    MoveList.Clear();
                    EaseInOut(c, e.Location, 400, playerChain[0]);
                    return;
                }
            }

            int i = 0;
            foreach (List<Card> chain in playerChain)
            {
                foreach (Card c in chain)
                {
                    if (c.Picked == true)
                    {
                        MoveList.Clear();
                        EaseInOut(c, e.Location, 400, playerChain[playerChain.IndexOf(chain)]);
                        return;
                    }
                }
                i++;
            }
        }
        #endregion
    }
}
