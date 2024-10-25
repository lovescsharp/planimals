using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace planimals
{
    public class Card : PictureBox
    {
        public bool Picked;
        public Point prevLocation;
        private Point offset;
        private bool inChain;

        public string scientific_name;
        public string common_name;
        private string description;
        private int hierarchy;
        private string habitat;

        public static int pictureBoxWidth = MainForm.workingHeight / 8;
        public static int pictureBoxHeight = MainForm.workingWidth / 10;
        public Card(string sname, string cname, string desc, string path, int hier, string habt, Point position)
        {
            scientific_name = sname;
            common_name = cname;
            description = desc;
            hierarchy = hier;
            habitat = habt;
            Height = pictureBoxHeight;
            Width = pictureBoxWidth;
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
            prevLocation = new Point(pictureBoxWidth * MainForm.playerHand.Count, MainForm.workingHeight - pictureBoxHeight);
            BackColor = Color.Gray;
            Picked = false;
            inChain = false;

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
            using (Font myFont = new Font("Arial", 10)) e.Graphics.DrawString(common_name, myFont, System.Drawing.Brushes.Yellow, new Point(Width / 10, Height / 20));
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
            if (e.Button == MouseButtons.Left)
            {
                offset = new Point(e.X, e.Y);
                Pick(this);
            }
        }
        private void card_Mouseup(object sender, MouseEventArgs e)
        {
            List<List<Rectangle>> li = MainForm.locationIndicators;
            int row = 0;
            int col = 0;
            foreach (List<Card> subchain in MainForm.playerChain)
            {
                foreach (Card c in subchain)
                {
                    if (this == c)
                    {
                        for (int i = 0; i < li.Count; i++)
                        {
                            for (int j = 0; j < li[i].Count; j++)
                            {
                                if (li[i][j].Contains(FindForm().PointToClient(Cursor.Position))) //the card was just put somewhere else in the chain
                                {
                                    if (cellIsBusy(i, j))
                                    {
                                        foreach (Card card in MainForm.playerChain[i])
                                        {
                                            card.Location = new Point(c.Location.X + c.Width + 5, c.Location.Y);
                                            if (card == c) break;
                                        }
                                    }
                                    row = i;
                                    col = j;
                                    try
                                    {
                                        MainForm.playerChain[i][j] = this;
                                    }
                                    catch
                                    {
                                        Drop(this);
                                        Location = li[i][j].Location;
                                        return;
                                    }
                                    MainForm.playerChain[MainForm.playerChain.IndexOf(subchain)].RemoveAt(subchain.IndexOf(c));
                                    if (MainForm.username != "")
                                    {
                                        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
                                        {
                                            sqlConnection.Open();
                                            SqlCommand update = new SqlCommand(
                                                $"UPDATE FoodChainCards " +
                                                $"SET RowNo={i}, PositionNo={j} " +
                                                $"WHERE Username='{MainForm.username}' " +
                                                $"AND RowNo={MainForm.playerChain.IndexOf(subchain)} " +
                                                $"AND PositionNo={subchain.IndexOf(c)}"
                                                , sqlConnection);
                                            update.ExecuteNonQuery();
                                            sqlConnection.Close();
                                        }
                                    }
                                    li[i].Add(
                                        new Rectangle(
                                            li[i][j].X + MainForm.locationIndicators[i][j].Width + 5,
                                            MainForm.locationIndicators[i][j].Y,
                                            MainForm.workingHeight / 8 + 10,
                                            MainForm.workingWidth / 10 + 10
                                        )
                                    );
                                    if (li[0].Count == 1)
                                    {
                                        MainForm.locationIndicators.Add(new List<Rectangle>());
                                        MainForm.locationIndicators[1].Add(
                                            new Rectangle
                                            (
                                                10,
                                                MainForm.fieldRectangle.Top + 10 + MainForm.locationIndicators[0][0].Height + 10,
                                                MainForm.workingHeight / 8 + 10,
                                                MainForm.workingWidth / 10 + 10
                                                )                                   
                                            );
                                    }
                                    FindForm().Invalidate();
                                    Drop(this);
                                    Location = li[i][j].Location;
                                    return;
                                }
                            }
                        }
                        //the card was removed from the chain
                        putToHand(row, col);
                        return;
                    }
                }
            }
            for (int i = 0; i < li.Count; i++)
            {
                for (int j = 0; j < li[i].Count; j++)
                {
                    if (li[i][j].Contains(FindForm().PointToClient(Cursor.Position))) //the card was put into the chain
                    {
                        if (cellIsBusy(i, j))
                        {
                            foreach (Card c in MainForm.playerChain[i])
                            {
                                c.Location = new Point(c.Location.X + c.Width + 5, c.Location.Y);
                            }
                        }
                        row = i;
                        col = j;
                        putToChain(row, col);
                        li[i].Add(
                            new Rectangle(
                                li[i][j].X + MainForm.locationIndicators[i][j].Width + 5,
                                MainForm.locationIndicators[i][j].Y,
                                    MainForm.workingHeight / 8 + 10,
                                    MainForm.workingWidth / 10 + 10
                            )
                        );
                        Drop(this);
                        return;
                    }
                }
            }
            Drop(this);
            Location = prevLocation;
        }
        private void putToHand(int row, int col)
        {
            MainForm.playerChain[row].Remove(this);
            MainForm.playerHand.Add(this);
            if (MainForm.username != "")
            {
                using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand push = new SqlCommand($"INSERT INTO Hand values ('{MainForm.username}', '{scientific_name}')", sqlConnection);
                    SqlCommand delete = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{MainForm.username}' AND CardID='{scientific_name}' AND RowNo={row} AND PositionNo={col}", sqlConnection);
                    push.ExecuteNonQuery();
                    delete.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            if (MainForm.locationIndicators[row].Count > 1) MainForm.locationIndicators[row].Remove(MainForm.locationIndicators[row].Last());
            Location = prevLocation;
            Drop(this);
            UpdateLocations(row);
            if (MainForm.locationIndicators[0].Count == 0) MainForm.locationIndicators = new List<List<Rectangle>>() { new List<Rectangle>() { new Rectangle(
                                    MainForm.fieldRectangle.Left + 10,
                                    MainForm.fieldRectangle.Top + 10,
                                    MainForm.workingHeight / 8 + 10,
                                    MainForm.workingWidth / 10 + 10
                                    ) } };
        }
        private bool cellIsBusy(int row, int col)
        {
            try
            {
                foreach (Card c in MainForm.playerChain[row]) if (MainForm.locationIndicators[row][col].Location == c.Location) return true;
            }
            catch
            {
                MainForm.playerChain.Add(new List<Card>());
                foreach (Card c in MainForm.playerChain[row]) if (MainForm.locationIndicators[row][col].Location == c.Location) return true;
            }
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

            if (MainForm.username != "")
            {
                using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand push = new SqlCommand($"INSERT INTO FoodChainCards values ('{MainForm.username}', '{scientific_name}', {row}, {col})", sqlConnection);
                    SqlCommand delete = new SqlCommand($"DELETE FROM Hand WHERE Username='{MainForm.username}' AND CardID='{scientific_name}'", sqlConnection);
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
                MainForm.playerChain[row][i].Location = new Point(MainForm.fieldRectangle.Left + i*Width + 5, MainForm.playerChain[row][i].Location.Y);
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
            MessageBox.Show($"{description}");
        }
        public static bool InRectangle(Point p) => p.X < MainForm.fieldRectangle.Right && p.X > MainForm.fieldRectangle.Left - pictureBoxWidth / 2 && p.Y > MainForm.fieldRectangle.Top - pictureBoxHeight / 2 && p.Y < MainForm.fieldRectangle.Bottom;
    }
}