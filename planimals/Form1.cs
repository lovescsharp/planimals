using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;

namespace planimals
{
    public partial class Form1 : Form
    {
        //instantiatig a card is good, now have to make a hand of em
        //store hand into the Hand table
        //done that thing also^^^^
        //now i have to draw a field where cards are going to be placed
        //if the card is not placed in the field put it to the initial location
        //if it is then start the q of cards
        //after that i need a button that will start the process of validating the chain
        //if the chain is correct add 10 points to the score and draw a new card

        private List<(Card, Point, Point, long, long)> MoveList;
        private static Random rnd;
        private Timer timer1;
        private Stopwatch sw1;


        private static string currentDir = Environment.CurrentDirectory;
        private static string dbPath = currentDir + "\\cards.mdf";
        private static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" + $"AttachDbFilename={dbPath}" + ";Integrated Security=True;Connect Timeout=30";
        private static readonly SqlConnection sqlConnection = new SqlConnection(connectionString);


        public static int workingHeight;
        public static int workingWidth;


        public static List<Card> playerHand;
        public static List<List<Card>> playerChain;
        public static Rectangle fieldRectangle;


        private PictureBox drawCardButton;
        private Image drawCardButtonBack;
        private Rectangle cardRectangle;

        private PictureBox chainButton;
        private Image chainButtonBack;
        private Rectangle chainButtonRectangle;


        private Label label = new Label();

        public Form1()
        {

            InitializeComponent();
            
            MoveList = new List<(Card, Point, Point, long, long)>();
            timer1 = new Timer();
            timer1.Tick += new EventHandler(MoveCards);
            timer1.Interval = 10;
            sw1 = new Stopwatch();

            timer1.Start();
            sw1.Start();

            #region UI
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MinimizeBox = false;
            Text = "Planimals";
            StartPosition = FormStartPosition.CenterScreen;

            Height = Screen.PrimaryScreen.WorkingArea.Height;
            Width = Screen.PrimaryScreen.WorkingArea.Width;
            workingHeight = ClientRectangle.Height;
            workingWidth = ClientRectangle.Width;

            fieldRectangle = new Rectangle(workingWidth / 100 * 20, workingHeight / 4, workingWidth / 10 * 6, workingHeight / 2);

            BackColor = Color.Black;

            drawCardButton = new PictureBox();
            drawCardButtonBack = Image.FromFile(currentDir + "\\assets\\photos\\back.png");
            drawCardButton.SizeMode = PictureBoxSizeMode.StretchImage;
            drawCardButton.Width = workingHeight / 8;
            drawCardButton.Height = workingWidth / 10;
            drawCardButton.Location = new Point(drawCardButton.Width - workingHeight / 100 * 5, workingHeight / 2 - drawCardButton.Height / 2);
            cardRectangle = new Rectangle(drawCardButton.Width - workingHeight / 100 * 5, workingHeight / 2 - drawCardButton.Height / 2, workingHeight / 8, workingWidth / 10);
            drawCardButton.Image = drawCardButtonBack;
            Controls.Add(drawCardButton);
            drawCardButton.Click += new EventHandler(drawCardButton_Click);
            drawCardButton.MouseMove += DrawCardButton_MouseMove;

            chainButton = new PictureBox();
            chainButtonBack = Image.FromFile(currentDir + "\\assets\\photos\\chain.png");
            chainButton.SizeMode = PictureBoxSizeMode.StretchImage;
            chainButton.Width = workingWidth / 10;
            chainButton.Height = workingHeight / 10;
            chainButton.Location = new Point(workingWidth - drawCardButton.Width - workingHeight / 10, workingHeight / 2 - drawCardButton.Height / 2);
            chainButtonRectangle = new Rectangle(workingWidth - drawCardButton.Width - workingHeight / 10, workingHeight / 2 - drawCardButton.Height / 2, workingWidth / 10, workingHeight / 10);
            chainButton.Image = chainButtonBack;
            Controls.Add(chainButton);
            chainButton.Click += new EventHandler(chainButton_Click);
            chainButton.MouseMove += chainButton_MouseMove;

            label.Location = new Point(workingWidth / 10, workingHeight / 20);
            label.ForeColor = Color.White;
            label.AutoSize = true;
            Controls.Add(label);

            #endregion

            rnd = new Random();
            playerHand = new List<Card>();

            playerChain = new List<List<Card>>() { new List<Card>() { } };

            MouseClick += new MouseEventHandler(MouseLeftClick);
            Paint += new PaintEventHandler(DrawFieldBorders);
            MouseMove += DrawCardButton_MouseMove;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 1; i++)
            {
                DrawCard(playerHand);
            }
            foreach (Card card in playerHand)
            {
                Controls.Add(card);
            }
        }
        public void DrawFieldBorders(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.White, 10.0f))
            {
                e.Graphics.DrawRectangle(pen, fieldRectangle);
            }
        }
        public void drawCardButton_Click(object sender, EventArgs e)
        {
            DrawCard(playerHand);
            Controls.Add(playerHand[playerHand.Count - 1]);
            Card.lastMouseButtonUp = MouseButtons.None;
        }
        private void DrawCardButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (MousePosition.X < cardRectangle.Right && MousePosition.X > cardRectangle.Left && MousePosition.Y < cardRectangle.Bottom && MousePosition.Y > cardRectangle.Top)
            {
                drawCardButton.Width = workingHeight / 8 + 5;
                drawCardButton.Height = workingWidth / 10 + 5;
            } 
            else
            {
                drawCardButton.Width = workingHeight / 8;
                drawCardButton.Height = workingWidth / 10;
            }
        }
        private void Chain(List<List<Card>> chain) {
            bool valid = true;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                if (chain[0].Count < 2) { label.Text = "The chain must consist of at least two organisms."; }
                else
                {
                    sqlConnection.Open();
                    for (int i = 0; i < chain[0].Count; i++)
                    {
                        if (i == chain.Count)
                        {
                            break;
                        }
                        SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) from Relations where Consumer='{chain[0][i+1].scientific_name}' AND Consumed='{chain[0][i].scientific_name}'", sqlConnection);
                        int b = (int) cmd.ExecuteScalar(); //1 - then relation exists. 0 - does not exist
                        if (b == 0) { 
                            label.Text = "Food chain is invalid";
                            valid = false;
                            //put cards back to hand
                            //PutToHand(chain[0]);
                            break;
                        }
                    }

                    sqlConnection.Close();
                    if (valid)
                    {
                        MessageBox.Show($"+{CalcScore(chain[0].Count)} points");
                        //delete cards from table
                        //TidyUp();
                    }
                }
            }
        }
        private void FixChainIndices(List<Card> chain) {
            for (int i = 0; i < chain.Count; i++) {
                if (i == chain.Count) {
                    break;
                }

                if (chain[i].Location.X > chain[i + 1].Location.X) {
                    Card temp = chain[i];
                    chain[i] = chain[i + 1];
                    chain[i + 1] = temp;
                }
            }
        }

        //private void TidyUp(List<Card> chain) {
        //      foreach (Card c in chain) {
        //          playerHand[].remove or something
        //      }
        //}

        //make function PutToHand() to put cards from the table back to the players hand
        

        private int CalcScore(int noOfCards) {
            int score = 0;
            for (int i = 0; i < noOfCards; i++)
            {
                score += 5 + 5 * i;
            }
            return score;
        }

        public void chainButton_Click(object sender, EventArgs e)
        {
            Chain(playerChain);
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

        public int GetNumberOfOrganisms()
        {
            int count = 0;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) AS num FROM Organisms", sqlConnection);
                sqlConnection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count = int.Parse(reader["num"].ToString());
                    }
                }
            }
            return count;
        }
        public string GetRandomScientificName()
        {
            int noOfOrganisms = GetNumberOfOrganisms();
            int randInx = rnd.Next(noOfOrganisms);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand($"WITH Numbered AS (SELECT Scientific_name, ROW_NUMBER() OVER(ORDER BY Scientific_name) as ROW_NUM FROM Organisms) SELECT Scientific_name FROM Numbered where ROW_NUM = {randInx}", sqlConnection);
                sqlConnection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader["Scientific_name"].ToString();
                    }
                }
            }
            return null;
        }
        public void DrawCard(List<Card> playerHand)
        {
            if (playerHand.Count <= 6)
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

                            if (playerHand.Count == 0)
                            {
                                Card c = new Card(sciname, cname, desc, path, hierarchy, habitat, new Point(Width / 2 - Card.pictureBoxWidth / 2, Height - Card.pictureBoxHeight));
                                playerHand.Add(c);
                            }
                            else if (playerHand[playerHand.Count - 1].Location.Y != Height - Card.pictureBoxHeight)
                            {
                                Card c = new Card(sciname, cname, desc, path, hierarchy, habitat, new Point(Width / 2 - Card.pictureBoxWidth / 2, Height - Card.pictureBoxHeight));
                                playerHand.Add(c);
                            }
                            else
                            {
                                foreach (Card card in playerHand)
                                {
                                    if (card.Location.Y == Height - Card.pictureBoxHeight)
                                    {
                                        card.Location = new Point(int.Parse((card.Location.X - Math.Pow(playerHand.Count, 2)).ToString()), card.Location.Y);
                                    }
                                }
                                Card c = new Card(sciname, cname, desc, path, hierarchy, habitat, new Point(playerHand[(playerHand.Count) - 1].Location.X + 100, playerHand[(playerHand.Count) - 1].Location.Y));
                                playerHand.Add(c);
                            }
                        }
                    }
                    sqlConnection.Close();
                }
            }
            else
            {
                label.Text = "Cannot hold more than 6 cards.";
            }

        }
        #region fancy card moving
        private void EaseInOut(Card card, Point endPosition, long length)
        {
            if (endPosition.X < fieldRectangle.Right && endPosition.X > fieldRectangle.Left && endPosition.Y > fieldRectangle.Top && endPosition.Y < fieldRectangle.Bottom) //check whether user wants to move card onto the table or just messing with them
            {
                Point offset = new Point(endPosition.X - card.Location.X - card.Width / 2, endPosition.Y - card.Location.Y - card.Height / 2);
                (Card, Point, Point, long, long) data = (card, card.Location, offset, length, sw1.ElapsedMilliseconds);
                MoveList.Add(data);
                card.Picked = false;
                card.BackColor = Color.Gray;
                playerChain[0].Add(card);
            } else
            {
                card.Picked = false;
                card.BackColor = Color.Gray;
            }
        }
        private void MoveCards(object sender, EventArgs e)
        {
            long currentTime = sw1.ElapsedMilliseconds;
            List<int> purgeIndexes = new List<int>();
            int index = 0;
            foreach ((Card card, Point startPosition, Point offset, long length, long startTime) in MoveList)
            {
                long dt = currentTime - startTime;
                if (dt > length)
                {
                    card.Location = new Point(startPosition.X + offset.X, startPosition.Y + offset.Y);
                    purgeIndexes.Add(index);
                    continue;
                }
                double f = F((double)(dt) / length, -3f);
                card.Location = new Point((int)(offset.X * f) + startPosition.X, (int)(offset.Y * f) + startPosition.Y);
                index++;
            }

            int shift = 0;
            foreach (int i in purgeIndexes)
            {
                MoveList.RemoveAt(i - shift);
                shift++;
            }
        }
        private static double F(double timeThrough, double a)
        {
            if (timeThrough >= 1) { return 1; }
            double x = timeThrough;
            //double y = Math.Pow(1 - Math.Pow(x - 1, 2), 0.5f);
            double y = Math.Sin((Math.PI * x)/2);
            return y;
        }
        private void MouseLeftClick(object sender, MouseEventArgs e)
        {
            MoveList.Clear();
            int i = PickedCard();
            EaseInOut(playerHand[i], e.Location, 500);
            Card.lastMouseButtonUp = MouseButtons.None;
        }
        private int PickedCard()
        {
            foreach (Card c in playerHand)
            {
                if (c.Picked == true)
                {
                    return playerHand.IndexOf(c);
                }
            }
            return 0;
        }
        #endregion
    }
}
