using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace planimals
{
    public class Card : PictureBox
    {
        public bool Picked;
        public Point prevLocation;
        private Point offset;
        private bool inChain;

        public string ScientificName;
        public string CommonName;
        private string Description;
        private int Hierarchy;
        private string Habitat;

        public static int pictureBoxWidth = MainForm.workingHeight / 8;
        public static int pictureBoxHeight = MainForm.workingWidth / 10;

        public Card(string scientific_name, string common_name, string description, string path, int hierarchy, string habitat, Point position, bool inchain)
        {
            ScientificName = scientific_name;
            CommonName = common_name;
            Description = description;
            Hierarchy = hierarchy;
            Habitat = habitat;
            inChain = inchain;

            pictureBoxHeight = MainForm.workingWidth / 10;
            pictureBoxWidth = MainForm.workingHeight / 8;
            offset = new Point(pictureBoxWidth/2, pictureBoxHeight/2);
            try
            {
                Image = Image.FromFile(path);
            }
            catch
            {
                MessageBox.Show(path);
            }

            SizeMode = PictureBoxSizeMode.Zoom;
            Size = new Size(pictureBoxWidth, pictureBoxHeight);
            Location = new Point(position.X, position.Y);
            prevLocation = new Point(pictureBoxWidth * MainForm.playerHand.Count, MainForm.workingHeight - Height);
            BackColor = Color.Gray;
            Picked = false;

            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Show Info", new EventHandler(card_RightClick));
            ContextMenu = cm;

            MouseDown += card_MouseDown;
            MouseUp += card_Mouseup;
            MouseMove += card_MouseMove;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Font myFont = new Font("Arial", 10)) e.Graphics.DrawString(CommonName, myFont, System.Drawing.Brushes.Yellow, new Point(Width / 10, Height / 20));
        }
        private void card_MouseMove(object sender, MouseEventArgs e)
        {
            if (Picked)
            {
                Point newPosition = this.FindForm().PointToClient(Cursor.Position);
                newPosition.Offset(-offset.X, -offset.Y);
                Location = newPosition;
            }
        }
        private void card_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < MainForm.playerHand.Count; i++)
            {
                
            }
            Pick(this);
        }
        private void card_Mouseup(object sender, MouseEventArgs e)
        {

        }
        private void putToHand(int row, int col)
        {

        }
        private bool cellIsBusy(int row, int col)
        {
            try
            {
                foreach (Card c in MainForm.playerChain[row])
                    if (MainForm.locationIndicators[row][col].Location == c.Location)
                    {
                        Console.WriteLine($"playerChain[{row}][{col}] is busy by {MainForm.playerChain[row][col].CommonName}");
                        return true;
                    }
            }
            catch
            {
                MainForm.playerChain.Add(new List<Card>());
                foreach (Card c in MainForm.playerChain[row]) 
                    if (MainForm.locationIndicators[row][col].Location == c.Location)
                    {
                        Console.WriteLine($"playerChain[{row}][{col}] is busy by {MainForm.playerChain[row][col].CommonName}");
                        return true;
                    }
            }
            Console.WriteLine($"playerChain[{row}][{col}] is empty");
            return false;
        }
        private void putToChain(int row, int col)
        {
            if (MainForm.playerChain[0].Count == 0 && row == 0)
            {
                MainForm.locationIndicators.Add(new List<Rectangle>());

                for (int i = 0; i < MainForm.locationIndicators[0].Count; i++)
                {
                    Rectangle newRect = new Rectangle(
                        MainForm.locationIndicators[0][i].X,
                        MainForm.locationIndicators[0][i].Y + pictureBoxHeight + 10,
                        pictureBoxWidth,
                        pictureBoxHeight
                    );
                    MainForm.locationIndicators[1].Add(newRect);
                }

            }
            if (row < MainForm.playerChain.Count)
            {
                if (col < MainForm.playerChain[row].Count) MainForm.playerChain[row][col] = this;
                else  MainForm.playerChain[row].Add(this);
            }
            MainForm.playerHand.Remove(this);

            if (MainForm.game.username != "")
            {
                using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand push = new SqlCommand($"INSERT INTO FoodChainCards values ('{MainForm.game.username}', '{ScientificName}', {row}, {col})", sqlConnection);
                    SqlCommand delete = new SqlCommand($"DELETE FROM Hand WHERE Username='{MainForm.game.username}' AND CardID='{ScientificName}'", sqlConnection);
                    push.ExecuteNonQuery();
                    delete.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }

            Drop(this);
            Location = MainForm.locationIndicators[row][col].Location;
        }
        private void UpdateLocations(int row) 
        { 
            for (int i = 0;  i < MainForm.playerChain[row].Count; i++)
            {
                MainForm.playerChain[row][i].Location = new Point(i*Width + 5, MainForm.playerChain[row][i].Location.Y);
            }
        }
        private void Drop(Card c) {
            c.Picked = false;
            c.BackColor = System.Drawing.Color.Gray;
            FindForm().Invalidate();
        }
        private void Pick(Card c) {
            c.Picked = true;
            c.BackColor = System.Drawing.Color.White;
            FindForm().Invalidate();
            BringToFront();
        }
        private void card_LeftClick() => Location = prevLocation;
        public void card_RightClick(object sender, EventArgs e)
        {
            MainForm.countDownTimer.Stop();
            MessageBox.Show($"{Description}\nprimarily lives in {Habitat} and is {Hierarchy} in the foodchain");
        }
        public static bool InRectangle(Point p, Rectangle r) => p.X < r.Right && p.X > r.Left - pictureBoxWidth / 2 && p.Y > r.Top - pictureBoxHeight / 2 && p.Y < r.Bottom;
    }
}   