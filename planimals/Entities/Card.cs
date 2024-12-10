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

    public static int pictureBoxWidth = MainForm.workingHeight / 8;
    public static int pictureBoxHeight = MainForm.workingWidth / 10;

    public Card(string scientific_name, string common_name, string description, string path, int hierarchy, string habitat, Point position, bool inchain)
    {
        DoubleBuffered = true;

        ScientificName = scientific_name;
        CommonName = common_name;
        Description = description;
        Hierarchy = hierarchy;
        Habitat = habitat;
        inChain = inchain;

        pictureBoxHeight = MainForm.workingWidth / 10;
        pictureBoxWidth = MainForm.workingHeight / 8;
        offset = new Point(pictureBoxWidth / 2, pictureBoxHeight / 2);
        try
        {
            Image = Image.FromFile(path);
        }
        catch
        {
            MessageBox.Show($"cant load this : {path}\n probably missing file");
        }

        SizeMode = PictureBoxSizeMode.Zoom;
        Size = new Size(pictureBoxWidth, pictureBoxHeight);
        Location = position;
        prevLocation = new Point(pictureBoxWidth * MainForm.playerHand.Count, MainForm.workingHeight - Height);
        BackColor = Color.Gray;
        Picked = false;

        ContextMenu cm = new ContextMenu();
        cm.MenuItems.Add("Show Info", new EventHandler(card_RightClick));
        ContextMenu = cm;

        MouseDown += card_MouseDown;
        MouseUp += card_Mouseup;
        MouseEnter += card_MouseEnter;
        MouseLeave += card_MouseLeave;
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
            Console.Write($"{CommonName} in chain at ");
            for (int i = 0; i < MainForm.cells.Count; i++)
            {
                for (int j = 0; j < MainForm.cells[i].Count; j++)
                {
                    if (MainForm.cells[i][j].Item1.Location == rectLocation)
                    {
                        Console.Write($"cells[{i}][{j}]");
                        (Rectangle, bool) tuple = (MainForm.cells[i][j].Item1, false);
                        MainForm.cells[i][j] = tuple;
                        MainForm.playerChain[i].Remove(this);
                        MainForm.playerHand.Add(this);
                        MainForm.UpdateCells();
                        if (MainForm.playerChain[i].Count != 0) ShiftCards(MainForm.playerChain[i], j);
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
    public void ShiftCards(List<Card> subchain, int startingIdx) // starting point
    {
        for (int i = startingIdx; i < subchain.Count; i++)
        {
            subchain[i].rectLocation = rectLocation;
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
    public void card_RightClick(object sender, EventArgs e)
    {
        MainForm.countDownTimer.Stop();
        MessageBox.Show($"{Description}\nprimarily lives in {Habitat} and is {Hierarchy} in the foodchain");
    }
    private void card_MouseEnter(object sender, EventArgs e) => Location = new Point(prevLocation.X, prevLocation.Y - 10);
    private void card_MouseLeave(object sender, EventArgs e) => Location = new Point(prevLocation.X, prevLocation.Y);
}