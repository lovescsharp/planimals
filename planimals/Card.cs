using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace planimals
{
    public class Card : PictureBox
    {
        public bool Picked;
        public Point prevLocation;

        public string scientific_name;
        public string common_name;
        private string description;
        public int hierarchy;
        private string habitat;

        public static int pictureBoxWidth = Form1.workingHeight / 8;
        public static int pictureBoxHeight = Form1.workingWidth / 10;

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
                Image = Image.FromFile(path);
            }
            catch
            {
                MessageBox.Show(path);
            }

            SizeMode = PictureBoxSizeMode.Zoom;
            Size = new Size(pictureBoxWidth, pictureBoxHeight);
            Location = new Point(position.X, position.Y);
            prevLocation = Location;
            BackColor = Color.Gray;
            Picked = false;

            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Show Info", new EventHandler(cardRightClick));
            ContextMenu = cm;

            MouseClick += new MouseEventHandler(cardLeftClick);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.Black, ButtonBorderStyle.Solid);
            using (Font myFont = new Font("Arial", 8))
            {
                e.Graphics.DrawString(common_name, myFont, Brushes.Yellow, new Point(10, 14));
            }
        }

        private void Drop(Card c) {
            c.Picked = false;
            c.BackColor = Color.Gray;
        }
        private void Pick(Card c) {
            c.Picked = true;
            c.BackColor = Color.White;
            BringToFront();
        }

        public void cardLeftClick(object sender, EventArgs e)
        {
            if (!Picked)
            {
                foreach (Card cardCurrentlyHeld in Form1.playerHand)
                {
                    if (cardCurrentlyHeld.Picked)
                    {
                        Drop(cardCurrentlyHeld);
                        Pick(this);
                    }
                    else
                    {
                        Pick(this);
                    }
                }
                foreach (List<Card> ch in Form1.playerChain)
                {
                    foreach (Card cardCurrentlyHeld in ch)
                    {
                        if (cardCurrentlyHeld.Picked)
                        {
                            Drop(cardCurrentlyHeld);
                            Pick(this);
                        }
                        else
                        {
                            Pick(this);
                        }
                    }
                }
            }
            else
            {
                Drop(this);
            }

        }
        public void cardRightClick(object sender, EventArgs e)
        {
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

        public static bool InRectangle(Point p) {
            return p.X < Form1.fieldRectangle.Right && p.X > Form1.fieldRectangle.Left - pictureBoxWidth / 2 && p.Y > Form1.fieldRectangle.Top - pictureBoxHeight / 2 && p.Y < Form1.fieldRectangle.Bottom;
        }
    }

}
