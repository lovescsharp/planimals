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
        private Point rectLocation;
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
            using (Font myFont = new Font("Arial", 10)) e.Graphics.DrawString(CommonName, myFont, Brushes.Yellow, new Point(Width / 10, Height / 20));
        }
        private void card_MouseMove(object sender, MouseEventArgs e)
        {
            if (Picked)
            {
                Point newPosition = FindForm().PointToClient(Cursor.Position);
                newPosition.Offset(-offset.X, -offset.Y);
                Location = newPosition;
            }
        }
        private void card_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < MainForm.playerHand.Count; i++)
            {
                if (MainForm.playerHand[i].Picked)
                {
                    Drop(MainForm.playerHand[i]);
                    MainForm.playerHand[i].Location = MainForm.playerHand[i].prevLocation;
                }
            }
            Pick(this);
            Console.WriteLine($"picked {CommonName}");
        }
        private void card_Mouseup(object sender, MouseEventArgs e)
        {
            if (!inChain)
            {
                Console.WriteLine($"Searching for reactangle with coords : {PointToClient(MousePosition)}");
                for (int i = 0; i < MainForm.cells.Count; i++)
                {
                    for (int j = 0; j < MainForm.cells[i].Count; j++)
                    {
                        Console.WriteLine(MainForm.cells[i][j].Item1.Location);
                        Console.WriteLine(MainForm.cells[i][j].Item1.Contains(FindForm().PointToClient(MousePosition)));
                        if (MainForm.cells[i][j].Item1.Contains(FindForm().PointToClient(MousePosition)))
                        {
                            if (!MainForm.cells[i][j].Item2)
                            {
                                Console.WriteLine("found an empty cell");
                                Drop(this);
                                Location = MainForm.cells[i][j].Item1.Location;
                                (Rectangle, bool) tuple = (MainForm.cells[i][j].Item1, true);
                                MainForm.cells[i][j] = tuple;
                                inChain = true;
                                MainForm.playerHand.Remove(this);
                                MainForm.playerChain[i].Add(this);
                                MainForm.UpdateCells();
                                FindForm().Invalidate();
                                rectLocation = Location;
                                return;
                            }
                            else
                            {
                                Console.WriteLine("cells not empty");
                                Drop(this);
                                Location = prevLocation;
                                return;
                            }
                        }
                    }
                }
                Drop(this);
                Location = prevLocation;
                return;
            }
            else if (inChain)
            {
                for (int i = 0; i < MainForm.cells.Count; i++)
                {
                    for (int j = 0; j < MainForm.cells[i].Count; j++)
                    {
                        if (MainForm.cells[i][j].Item1.Location == rectLocation)
                        {
                            (Rectangle, bool) tuple = (MainForm.cells[i][j].Item1, false);
                            MainForm.cells[i][j] = tuple;
                            MainForm.playerChain[i].Remove(this);
                            MainForm.playerHand.Add(this);
                            MainForm.UpdateCells();
                            Drop(this);
                            Location = prevLocation;
                            rectLocation = new Point(0, 0);
                            inChain = false;    
                            return;
                        }
                    }
                }
            }
        }
        private void Drop(Card c) {
            c.Picked = false;
            c.BackColor = Color.Gray;
            FindForm().Invalidate();
        }
        private void Pick(Card c) {
            c.Picked = true;
            c.BackColor = Color.White;
            FindForm().Invalidate();
            BringToFront();
        }
        private void PutToHand() 
        {
            
        }
        public void card_RightClick(object sender, EventArgs e)
        {
            MainForm.countDownTimer.Stop();
            MessageBox.Show($"{Description}\nprimarily lives in {Habitat} and is {Hierarchy} in the foodchain");
        }
        public static bool InRectangle(Point p, Rectangle r) => p.X < r.Right && p.X > r.Left - pictureBoxWidth / 2 && p.Y > r.Top - pictureBoxHeight / 2 && p.Y < r.Bottom;
    }
}   