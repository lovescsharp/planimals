using System;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace planimals
{
    public class Card : PictureBox
    {
        public bool Picked;
        public Point previousLocation;

        public string scientific_name;
        public string common_name;
        public string description;
        private int hierarchy;
        public string habitat;

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

            Image = Image.FromFile(path);
            SizeMode = PictureBoxSizeMode.Zoom;
            Size = new Size(pictureBoxWidth, pictureBoxHeight);
            Location = new Point(position.X, position.Y);
            previousLocation = Location;
            BackColor = Color.Gray;
            Picked = false;

            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add("Show Info", new EventHandler(cardRightClick));
            ContextMenu = cm;

            MouseClick += new MouseEventHandler(cardLeftClick);
            MouseUp += new MouseEventHandler(LastMouseButtonUp);
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

        private void LastMouseButtonUp(object sender, MouseEventArgs e)
        {
            lastMouseButtonUp = e.Button;
        }

        public void cardLeftClick(object sender, EventArgs e)
        {
            if (lastMouseButtonUp == MouseButtons.Left && Picked == true) {
                if (Form1.playerChain.Count == 1) 
                {
                    Location = new Point(Form1.fieldRectangle.Left + Form1.fieldRectangle.Width / 10, Form1.fieldRectangle.Top + Form1.fieldRectangle.Height / 2);
                    Drop(this);
                }
                lastMouseButtonUp = MouseButtons.None;
            }
            else 
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
                }
                else
                {
                    Drop(this);
                }
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
    }

}
