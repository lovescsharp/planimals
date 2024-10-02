using System;
using System.Collections.Generic;
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
        public Point offset;

        public string scientific_name;
        public string common_name;
        private string description;
        public int hierarchy;
        private string habitat;

        public static int pictureBoxWidth = MainForm.workingHeight / 8;
        public static int pictureBoxHeight = MainForm.workingWidth / 10;

        public static MouseButtons lastMouseButtonUp = MouseButtons.None;

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
            foreach(List<Rectangle> row in MainForm.locationIndicators) 
            {
                foreach(Rectangle r in row)
                {
                    //FIXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    if (r.Contains(this.FindForm().PointToClient(Cursor.Position)))
                    {
                        if (MainForm.locationIndicators.Count == 1 && row.Count == 1)
                        {
                            MainForm.locationIndicators.Add(new List<Rectangle>());
                            MainForm.locationIndicators[MainForm.locationIndicators.Count - 1].Add(
                                new Rectangle(
                                r.X,
                                r.Y + r.Height + 5,
                                MainForm.workingHeight / 8 + 10,
                                MainForm.workingWidth / 10 + 10
                                )
                                );
                        }
                        row.Add(
                            new Rectangle(
                                r.X + r.Width + 5,
                                r.Y,
                                MainForm.workingHeight / 8 + 10,
                                MainForm.workingWidth / 10 + 10
                            )
                        );
                        //Add new row to locationIndicators
                        try
                        {
                            MainForm.playerChain[MainForm.locationIndicators.IndexOf(row)].Add(this);
                        }
                        catch
                        {
                            MainForm.playerChain.Add(new List<Card>());
                            MainForm.playerChain[MainForm.locationIndicators.IndexOf(row)].Add(this);
                        }
                        MainForm.playerHand.Remove(this);
                        Location = r.Location;
                        Drop(this);
                        return;
                    }
                }
            }
            Location = prevLocation;
            Drop(this);
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
