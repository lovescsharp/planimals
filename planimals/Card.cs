using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media;

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
                Image = System.Drawing.Image.FromFile(path);
            }
            catch
            {
                MessageBox.Show(path);
            }

            SizeMode = PictureBoxSizeMode.Zoom;
            Size = new Size(pictureBoxWidth, pictureBoxHeight);
            Location = new Point(position.X, position.Y);
            prevLocation = new Point(Card.pictureBoxWidth * MainForm.playerHand.Count, MainForm.workingHeight - Card.pictureBoxHeight);
            BackColor = System.Drawing.Color.Gray;
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
            using (Font myFont = new Font("Arial", 10)) e.Graphics.DrawString(common_name, myFont, System.Drawing.Brushes.Yellow, new Point(Width/10, Height/20));
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
            putCard(this);
        }
        //fix
        private void putCard(Card card)
        {
            bool cardPlaced = false; // Track whether the card was placed successfully

            // Iterate through location indicators
            for (int i = 0; i < MainForm.locationIndicators.Count; i++)
            {
                for (int j = 0; j < MainForm.locationIndicators[i].Count; j++)
                {
                    // Check if the cursor position is within the current rectangle
                    if (MainForm.locationIndicators[i][j].Contains(this.FindForm().PointToClient(Cursor.Position)))
                    {
                        // Add a new rectangle if necessary
                        if (MainForm.locationIndicators.Count == 1 && MainForm.locationIndicators[i].Count == 1)
                        {
                            MainForm.locationIndicators.Add(new List<Rectangle>());
                        }

                        // Add a new rectangle for the card
                        MainForm.locationIndicators[i].Add(new Rectangle(
                            MainForm.locationIndicators[i][j].X + MainForm.locationIndicators[i][j].Width + 5,
                            MainForm.locationIndicators[i][j].Y,
                            MainForm.workingHeight / 8 + 10,
                            MainForm.workingWidth / 10 + 10
                        ));

                        // Ensure playerChain is initialized correctly
                        if (MainForm.playerChain.Count <= i)
                        {
                            MainForm.playerChain.Add(new List<Card>());
                        }

                        // If the card is already in the chain, update its position
                        if (MainForm.playerChain[i].Contains(this))
                        {
                            MainForm.playerChain[i][MainForm.playerChain[i].IndexOf(this)] = this; // Just refresh its reference
                        }
                        else
                        {
                            MainForm.playerChain[i].Add(this); // Add the card to the chain
                        }

                        // Push to chain if username is available
                        if (!string.IsNullOrEmpty(MainForm.username))
                        {
                            MainForm.PushToChain(this.scientific_name, i, j);
                        }

                        // Remove the card from the hand
                        MainForm.playerHand.Remove(this);
                        Location = MainForm.locationIndicators[i][j].Location; // Set the location of the card
                        inChain = true; // Indicate that the card is now in the chain
                        Drop(this);
                        Invalidate(); // Redraw the UI if necessary
                        cardPlaced = true; // Mark that the card has been placed
                        break; // Exit the inner loop
                    }
                }
                if (cardPlaced) break; // Exit the outer loop if the card was placed
            }

            // Handle the case when the card was not placed
            if (!cardPlaced)
            {
                if (inChain)
                {
                    if (!string.IsNullOrEmpty(MainForm.username))
                    {
                        MainForm.PushToHand(new List<string>() { this.scientific_name });
                    }
                    MainForm.playerHand.Remove(this);
                }
                Location = prevLocation; // Reset to previous location if not placed
                Drop(this);
            }
        }


        private void putToChain(Card c, int row, int col)
        {

        }
        private void Drop(Card c) {
            c.Picked = false;
            c.BackColor = System.Drawing.Color.Gray;
            this.FindForm().Invalidate();
        }
        private void Pick(Card c) {
            c.Picked = true;
            c.BackColor = System.Drawing.Color.White;
            this.FindForm().Invalidate();
            BringToFront();
        }
        private void card_LeftClick() => Location = prevLocation;
        public void card_RightClick(object sender, EventArgs e)
        {
            MainForm.countDownTimer.Stop();
            switch (hierarchy)
            {
                case (1):
                    MessageBox.Show($"Binomial name: {scientific_name}\nCommon name: {common_name}\n{description}\nRole in chain: Carnivore\nPrimarily lives in {habitat}");
                    break;
                case (2):
                    MessageBox.Show($"Binomial name: {scientific_name}\nCommon name: {common_name}\n{description}\nRole in chain: Carnivore/Omnivore\nPrimarily lives in {habitat}");
                    break;
                case (3):
                    MessageBox.Show($"Binomial name: {scientific_name}\nCommon name: {common_name}\n{description}\nRole in chain: Herbivore\nPrimarily lives in {habitat}");
                    break;
                case (4):
                    MessageBox.Show($"Binomial name: {scientific_name}\nCommon name: {common_name}\n{description}\nRole in chain: Producer\nPrimarily lives in {habitat}");
                    break;
            }
        }
        public static bool InRectangle(Point p) => p.X < MainForm.fieldRectangle.Right && p.X > MainForm.fieldRectangle.Left - pictureBoxWidth / 2 && p.Y > MainForm.fieldRectangle.Top - pictureBoxHeight / 2 && p.Y < MainForm.fieldRectangle.Bottom;
    }
}
