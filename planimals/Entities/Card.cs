using planimals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class Card : PictureBox
{
    public bool Picked;
    public Point prevLocation;
    public Point rectLocation;
    private Point offset;
    public bool inChain;

    public string ScientificName;
    public string CommonName;
    private string Description;
    private int Hierarchy;
    private string Habitat;

    public static int cardWidth = MainForm.workingHeight / 8;
    public static int cardHeight = MainForm.workingWidth / 10;

    public Card(string scientific_name, string common_name, string description, string path, int hierarchy, string habitat, Point position, bool inchain)
    {
        DoubleBuffered = true;

        ScientificName = scientific_name;
        CommonName = common_name;
        Description = description;
        Hierarchy = hierarchy;
        Habitat = habitat;
        inChain = inchain;

 
        offset = new Point(cardWidth / 2, cardHeight / 2);
        try
        {
            Image = Image.FromFile(path);
        }
        catch
        {
            MessageBox.Show($"cant load this : {path}\n probably missing file");
        }

        SizeMode = PictureBoxSizeMode.Zoom;
        Size = new Size(cardWidth, cardHeight);
        Location = position;
        prevLocation = new Point(cardWidth * MainForm.playerHand.Count, MainForm.workingHeight - cardHeight);
        BackColor = Color.Gray;
        Picked = false;

        ContextMenu cm = new ContextMenu();
        cm.MenuItems.Add("Show Info", new EventHandler(card_RightClick));
        ContextMenu = cm;

        MouseDown += card_MouseDown;
        MouseUp += card_Mouseup;
        MouseMove += card_MouseMove;
        //MouseEnter += card_MouseEnter;
        //MouseLeave += card_MouseLeave;
        Paint += new PaintEventHandler(OnPaint);
    }
    protected void OnPaint(object sender, PaintEventArgs e)
    {
        using (Font myFont = new Font("Mono", 10)) e.Graphics.DrawString(CommonName, myFont, Brushes.Black, new Point(Width / 10, Height / 20));
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
        //Console.WriteLine($"picked {CommonName}");
    }
    private void card_Mouseup(object sender, MouseEventArgs e)
    {
        if (!inChain)
        {
            for (int i = 0; i < MainForm.cells.Count; i++)
            {
                for (int j = 0; j < MainForm.cells[i].Count; j++)
                {
                    if (MainForm.cells[i][j].Item1.Contains(FindForm().PointToClient(MousePosition)))
                    {
                        if (!MainForm.cells[i][j].Item2) //cell is empty
                        {
                            if (MainForm.cells[i].Count == 1) MainForm.playerChain.Add(new List<Card>()); 
                            Drop(this);
                            Location = MainForm.cells[i][j].Item1.Location;
                            (Rectangle, bool) tuple = (MainForm.cells[i][j].Item1, true);
                            MainForm.cells[i][j] = tuple;
                            inChain = true;
                            MainForm.playerHand.Remove(this);
                            prevLocation = new Point(cardWidth * MainForm.playerHand.Count, MainForm.workingHeight - cardHeight);
                            MainForm.playerChain[i].Add(this);
                            if (MainForm.game.username != "") ((MainForm) FindForm()).PushToChain(this, i, j);
                            MainForm.UpdateCells();
                            UpdateLocations();
                            ShiftCards();
                            FindForm().Invalidate();
                            rectLocation = Location;
                            return;
                        }
                        else
                        {
                            Drop(this);
                            Location = prevLocation;
                            return;
                        }
                    }
                }
            }
            Drop(this);
            Location = prevLocation;
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
                        //Console.WriteLine($"before removing {CommonName}: {MainForm.playerChain[i].Count}");
                        MainForm.playerChain[i].Remove(this);
                        //Console.WriteLine($"after : {MainForm.playerChain[i].Count}");
                        if (MainForm.playerChain[i].Count == 0 && i != 0)
                        {
                            //Console.WriteLine($"removing chain[{i}]");
                            MainForm.playerChain.RemoveAt(i);
                            MainForm.UpdateCells();
                        }
                        else if (i == 0 && MainForm.playerChain[0].Count == 0)
                        {
                            MainForm.playerChain.Clear();
                        }
                        if (MainForm.game.username != "") ((MainForm)FindForm()).RemoveFromChain(this, i, j);
                        MainForm.playerHand.Add(this);
                        MainForm.PushToHand(new List<string>() { ScientificName });
                        MainForm.UpdateCells();
                        UpdateLocations();
                        ShiftCards();
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
    public void ShiftCards() // when a card is removed in the middle of the chain, shift all cards to the left
    {
        for (int i = 0; i < MainForm.playerChain.Count; i++)
        for (int j = 0; j < MainForm.playerChain[i].Count; j++) 
        MainForm.playerChain[i][j].Location = MainForm.playerChain[i][j].rectLocation = 
        MainForm.cells[i][j].Item1.Location;
    }   
    private void Drop(Card c) 
    {
        c.Picked = false;
        c.BackColor = Color.Gray;
        FindForm().Invalidate();
    }
    private void Pick(Card c)
    {
        c.Picked = true;
        c.BackColor = Color.White;
        FindForm().Invalidate();
        BringToFront();
    }
    public void card_RightClick(object sender, EventArgs e)
    {
        MainForm.countDownTimer.Stop();
        MessageBox.Show($"{Description}\nprimarily lives in {Habitat} and is {Hierarchy} in the foodchain");
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
    private void card_MouseEnter(object sender, EventArgs e) => Location = new Point(prevLocation.X, prevLocation.Y - 10);
    private void card_MouseLeave(object sender, EventArgs e) => Location = new Point(prevLocation.X, prevLocation.Y);
    private void UpdateLocations() 
    {
        for (int i = 0; i < MainForm.playerHand.Count; i++)
        {
            MainForm.playerHand[i].prevLocation = new Point(cardWidth * i, MainForm.workingHeight - cardHeight);
            Location = prevLocation;
        }
    }
}