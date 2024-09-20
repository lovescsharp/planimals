using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;

namespace planimals
{
    public partial class Form1 : Form
    {
        ///logic
        //push and pull changes to Cards.mdf
        private Button playButton;
        private Button loginButton;
        private Button exitButton;

        private bool loggedIn;
        public string username;

        public static List<(Card, Point, Point, long, long)> MoveList;
        private static Random rnd;
        private System.Windows.Forms.Timer timer1;
        private Stopwatch sw1;

        private System.Windows.Forms.Timer countDownTimer;
        private int timeLeft;
        private System.Windows.Forms.Label labelTimer;

        private System.Windows.Forms.Timer readySteadyGoTimer;
        private static PictureBox readySteadyGo;
        private int imageIndex = 3;

        public static string currentDir = Environment.CurrentDirectory;
        private static string dbPath = currentDir + "\\cards.mdf";
        public static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" + $"AttachDbFilename={dbPath}" + ";Integrated Security=True;Connect Timeout=30";
        private static readonly SqlConnection sqlConnection = new SqlConnection(connectionString);

        public static int workingHeight;
        public static int workingWidth;

        public static List<Card> playerHand;
        public static List<List<Card>> playerChain;
        public static Rectangle fieldRectangle;
        public static Rectangle cardRectangle;

        private PictureBox retryButton;
        private PictureBox goToMenuButton;
        private PictureBox goToMenuInGameButton;

        private Button yesButton;
        private Button noButton;

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

        private System.Windows.Forms.Label label;

        private Font largeFont;
        private Font smallFont; 

        private int overallScore;

        public Form1()
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

            playButton = new Button();
            playButton.Text = "play";
            playButton.BackColor = Color.White;
            playButton.Size = new Size(50, 30);
            playButton.Location = new Point(workingWidth / 2 - playButton.Width / 2, workingHeight / 2 - playButton.Height / 2 - 30);
            Controls.Add(playButton);
            playButton.Click += PlayButton_Click;

            loginButton = new Button();
            loginButton.Text = "login";
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
            retryButton.Size = new Size(workingWidth  / 10, workingWidth / 10);
            retryButton.Location = new Point(workingWidth / 2 - retryButton.Width / 2, workingHeight / 2 - retryButton.Height/2);
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
            yesButton.Location = new Point(workingWidth / 2 + yesButton.Width / 2, workingHeight/2 - yesButton.Height);
            yesButton.Click += yesButton_Click;
            Controls.Add(yesButton);

            noButton = new Button();
            noButton.Text = "no";
            noButton.BackColor = Color.White;
            noButton.Size = new Size(50, 30);
            noButton.Location = new Point(workingWidth / 2  - noButton.Width - noButton.Width / 2, workingHeight/2 - noButton.Height);
            noButton.Click += noButton_Click;
            Controls.Add(noButton);

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

            labelTimer = new System.Windows.Forms.Label();
            labelTimer.ForeColor = Color.White;
            labelTimer.Font = new Font(label.Font.FontFamily, 25);
            labelTimer.Location = new Point((int)((double)workingWidth * 0.9), (int)((double)workingHeight / 10));
            labelTimer.Size = new Size(100, 100);
            Controls.Add(labelTimer);

            foreach (Control control in Controls)
            {
                if (control.GetType() == typeof(System.Windows.Forms.Label))
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

            readySteadyGoTimer = new System.Windows.Forms.Timer();
            readySteadyGoTimer.Interval = 1000;
            readySteadyGoTimer.Tick += readySteadyGoTimer_Tick;

            #endregion

            loggedIn = false;
            username = String.Empty;

            MoveList = new List<(Card, Point, Point, long, long)>();
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(MoveCards);
            timer1.Interval = 10;
            sw1 = new Stopwatch();

            countDownTimer = new System.Windows.Forms.Timer();
            countDownTimer.Interval = 1000;
            countDownTimer.Tick += new EventHandler(countDownTimer_Tick);

            rnd = new Random();
            playerHand = new List<Card>();
            playerChain = new List<List<Card>>() { new List<Card>() { } };

            MouseClick += new MouseEventHandler(MouseLeftClick);
            MouseMove += DrawCardButton_MouseMove;
            Paint += new PaintEventHandler(DrawFieldBorders);
            Resize += new EventHandler(OnResize);

            gameControls = new List<Control>() { drawCardButton, chainButton, goToMenuInGameButton };
            menuControls = new List<Control>() { loginButton, playButton, exitButton };
            endControls = new List<Control>() { retryButton, goToMenuButton};
            youSureWannaQuitControls = new List<Control>() { yesButton, noButton };

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
            imageIndex = 3;
            timeLeft = 40;
            labelTimer.Show();
            labelTimer.Text = "";
            overallScore = 0;
            label.Show();
            label.Text = "";
            label.Location = new Point(workingWidth / 10, workingHeight / 20);

            readySteadyGo.Show();
            readySteadyGo.Enabled = true; try
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
        private void readySteadyGoTimer_Tick(object sender, EventArgs e)
        {
            imageIndex--;

            if (imageIndex > 0) readySteadyGo.Image = Image.FromFile(currentDir + "\\assets\\photos\\" + imageIndex.ToString() + ".png");
            else
            {
                readySteadyGoTimer.Stop();
                Controls.Remove(readySteadyGo);
                foreach (Control control in gameControls) { control.Enabled = true; control.Show(); }
                foreach (Control control in endControls) { control.Enabled= false; control.Hide(); }
                countDownTimer.Start();
            }
        }
        private void countDownTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > -1)
            {
                labelTimer.Text = timeLeft.ToString();
            }
            else
            {
                countDownTimer.Stop();
                labelTimer.Hide();

                label.Location = new Point(workingWidth / 2 - label.Width, 100);
                label.Font = largeFont;
                label.Text = "Score: " + overallScore.ToString();
                foreach (Control control in endControls)
                {
                    control.Enabled = true; 
                    control.Show();
                }
                foreach(Control control in gameControls)
                {
                    control.Enabled= false;
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
            foreach(Control control in gameControls)
            {
                control.Enabled= false;
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
            foreach (Control control in youSureWannaQuitControls){ control.Hide(); control.Enabled = false; }
        }
        private void goToMenuInGameButton_Click(object sender, EventArgs e) 
        {
            countDownTimer.Stop();
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
            Height = (int)(Width*0.5625);
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
                playerHand[i].Location = playerHand[i].prevLocation = new Point(Card.pictureBoxWidth*i, workingHeight-Card.pictureBoxHeight);
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
            label.Location = new Point(workingWidth/10, workingHeight/8);
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
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }
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
        private int CalcScore(int noOfCards) {
            int score = 0;
            for (int i = 0; i < noOfCards; i++)
            {
                //gonna make a better calculation of the score some day
                score += i + 1;
            }
            return score;
        }
        public void chainButton_Click(object sender, EventArgs e) { foreach (List<Card> chain in playerChain) Chain(chain); } 
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
        private void Chain(List<Card> chain)
        {
            bool valid = true;
            string s = "";
            foreach (Card c in chain) s += $"{c.common_name}\n";
            FixChainIndices(chain);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (chain.Count < 2) { Display("the chain must consist of at least two organisms."); return; }
                else
                {
                    sqlConnection.Open();
                    for (int i = 0; i < chain.Count - 1; i++)
                    {
                        SqlCommand sqlCommand = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer='{chain[i + 1].scientific_name}' AND Consumed='{chain[i].scientific_name}'", sqlConnection);
                        int b = (int)sqlCommand.ExecuteScalar(); //1 - relation exists. 0 - does not exist
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
                                playerHand.Add(chain[j]);
                            }
                            chain.Clear();
                            return;
                        }
                    }
                    sqlConnection.Close();
                    if (valid)
                    {
                        Display($"+{CalcScore(chain.Count)} points\n");
                        overallScore += CalcScore(chain.Count);
                        foreach (Card c in chain)
                        {
                            Controls.Remove(c);
                            c.Image.Dispose();
                        }
                        chain.Clear();

                        for (int j = 0; j < playerHand.Count; j++) { playerHand[j].Location = playerHand[j].prevLocation = new Point(Card.pictureBoxWidth * (j), Height - Card.pictureBoxHeight); }
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
        public string GetRandomScientificName()
        {
            int noOfOrganisms = GetNumberOfOrganisms();
            int randInx = rnd.Next(noOfOrganisms) + 1; //add one cause count starts from 1
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand($"WITH Numbered AS (SELECT Scientific_name, ROW_NUMBER() OVER(ORDER BY Scientific_name) as ROW_NUM FROM Organisms) SELECT Scientific_name FROM Numbered where ROW_NUM = {randInx}", sqlConnection);
                sqlConnection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read()) { return reader["Scientific_name"].ToString(); }
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
                string sciname = GetRandomScientificName();
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
                    sqlConnection.Close();
                }
            }
            else
            {
                Display("cannot hold more than 15 cards");
            }

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
            if (InRectangle(endPosition) && !InRectangle(c.Location)) //check whether user wants to move card onto the table or just messing with them          
            {
                Point offset = new Point(endPosition.X - c.Location.X - c.Width / 2, endPosition.Y - c.Location.Y - c.Height / 2);
                (Card, Point, Point, long, long) data = (c, c.Location, offset, length, sw1.ElapsedMilliseconds);
                MoveList.Add(data);
                c.Picked = false;
                c.BackColor = Color.Gray;
                chain.Add(c);
                playerHand.Remove(c);
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
                chain.Remove(c);
                return;
            }
            else
            {
                c.Picked = false;
                c.BackColor = Color.Gray;
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

            foreach (List<Card> chain in playerChain)
            {
                foreach (Card c in chain)
                {
                    if (c.Picked == true)
                    {
                        MoveList.Clear();
                        EaseInOut(c, e.Location, 400, playerChain[0]);
                        return;
                    }
                }
            }
        }
        #endregion
    }
}
