﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

public class Card : PictureBox
{
    private Game game;

    private bool isAnimating;
    private Timer t;
    private Stopwatch s;

    public bool Picked;
    bool Hoovered;
    public Point prevLocation, rectLocation;
    private Point offset, p, v, pv;
    private double animationTime = 0.6;
    public bool inChain;

    public string ScientificName;
    public string CommonName;
    private string Description;
    private int Hierarchy;
    private string Habitat;

    public static int cardWidth;
    public static int cardHeight;

    public Card(Game g, string scientific_name, string common_name, string description, string path, int hierarchy, string habitat, Point position, bool inchain)
    {
        game = g;
        DoubleBuffered = true;

        t = new Timer();
        t.Interval = 6;
        t.Tick += Tick;

        s = new Stopwatch();

        ScientificName = scientific_name;
        CommonName = common_name;
        Description = description;
        Hierarchy = hierarchy;
        Habitat = habitat;
        inChain = inchain;

        cardWidth = game.form.ClientRectangle.Height / 8;
        cardHeight = game.form.ClientRectangle.Width / 10;

        offset = new Point(cardWidth / 2, cardHeight / 2);
        try
        {
            Image = Image.FromFile(path); //C:\Users\vbr2\Documents\Visual Studio 2022\projects\planimals\planimals\planimals\bin\Debug\assets\photos
        }
        catch
        {
            MessageBox.Show($"cant load this : {path}\n probably missing file");
        }

        SizeMode = PictureBoxSizeMode.Zoom;
        Size = new Size(cardWidth, cardHeight);
        Location = position;
        prevLocation = new Point(cardWidth * game.playerHand.Count, game.form.ClientRectangle.Height - cardHeight);
        if (game.form.currTheme == MainForm.Theme.Dark) BackColor = Color.DarkGray;
        else BackColor = Color.LightGray;
        Picked = false;
        Hoovered = false;

        ContextMenu cm = new ContextMenu();
        cm.MenuItems.Add("Show Info", new EventHandler(card_RightClick));
        ContextMenu = cm;

        MouseDown += card_MouseDown;
        MouseUp += card_MouseUp;
        MouseMove += card_MouseMove;
        MouseEnter += card_MouseEnter;
        MouseLeave += card_MouseLeave;
        Paint += new PaintEventHandler(OnPaint);
    }
    protected void OnPaint(object sender, PaintEventArgs e)
    {
        using (Font myFont = new Font("Mono", 10))
        {
            if (!Hoovered)
            {
                if (game.form.currTheme == MainForm.Theme.Dark) e.Graphics.DrawString(CommonName, myFont, Brushes.LightGray, new Point(Width / 10, Height / 20));
                else e.Graphics.DrawString(CommonName, myFont, Brushes.Black, new Point(Width / 10, Height / 20));
            }
            else 
            {
                if (game.form.currTheme == MainForm.Theme.Dark) e.Graphics.DrawString(CommonName, myFont, Brushes.White, new Point(Width / 10, Height / 20));
                else e.Graphics.DrawString(CommonName, myFont, Brushes.Black, new Point(Width / 10, Height / 20));
            }

        }
    }
    private void card_MouseDown(object sender, MouseEventArgs e)
    {
        for (int i = 0; i < game.playerHand.Count; i++)
        {
            if (game.playerHand[i].Picked)
            {
                game.playerHand[i].Drop();
                game.playerHand[i].Location = game.playerHand[i].prevLocation;
            }
        }
        Pick();    
    }
    private void card_MouseUp(object sender, MouseEventArgs e)
    {
        if (!inChain)
        {
            //Console.WriteLine("moving from hand");
            for (int i = 0; i < game.cells.Count; i++)
            {
                for (int j = 0; j < game.cells[i].Count; j++)
                {
                    if (game.cells[i][j].Item1.Contains(FindForm().PointToClient(MousePosition)))
                    {
                        if (!game.cells[i][j].Item2) //cell is empty
                        {
                            if (game.cells[i].Count == 1) game.playerChain.Add(new List<Card>());
                            Drop();
                            Location = game.cells[i][j].Item1.Location;
                            game.cells[i][j] = (game.cells[i][j].Item1, true);
                            inChain = true;
                            game.playerHand.Remove(this);
                            prevLocation = new Point(cardWidth * game.playerHand.Count, game.form.ClientRectangle.Height - cardHeight);
                            game.playerChain[i].Add(this);
                            if (game.username != string.Empty)
                            {
                                RemoveFromHand();
                                PushToChain(i, j);
                            }
                            game.UpdateCells();
                            if (cardWidth * game.playerHand.Count > game.form.ClientRectangle.Width) game.playerHand.putCardsOnTopOfEachOther();
                            else game.playerHand.ShiftCards();
                            game.form.Invalidate();
                            rectLocation = Location;
                            Console.WriteLine(game.playerHand.ToString());
                            Console.WriteLine(game.playerChain.ToString());
                            return;
                        }
                        else //replacing a card in the cell with the just dropped one
                        {
                            Drop();
                            game.playerHand.Remove(this);
                            if (game.username != string.Empty)
                            {
                                game.playerChain[i][j].RemoveFromChain(i, j);
                                game.playerChain[i][j].PushToHand();
                            }
                            game.playerChain[i][j].prevLocation = new Point(cardWidth * game.playerHand.Count, game.form.ClientRectangle.Height - cardHeight);
                            game.playerHand.Add(game.playerChain[i][j]);
                            game.playerChain[i][j].rectLocation = new Point(0, 0);
                            game.playerChain[i][j].inChain = false;
                            game.playerChain[i][j] = this;
                            inChain = true;
                            Location = rectLocation = game.cells[i][j].Item1.Location;
                            if (game.username != string.Empty)
                            {
                                RemoveFromHand();
                                PushToChain(i, j);
                            }
                            if (cardWidth * game.playerHand.Count > game.form.ClientRectangle.Width) game.playerHand.putCardsOnTopOfEachOther();
                            else game.playerHand.ShiftCards();
                            Console.WriteLine(game.playerHand.ToString());
                            Console.WriteLine(game.playerChain.ToString());
                            return;
                        }
                    }
                }
            }
            Drop();
            Location = prevLocation;
            Console.WriteLine(game.playerHand.ToString());
            Console.WriteLine(game.playerChain.ToString());
        }
        else if (inChain)
        {
            //Console.WriteLine("moving from chain");
            for (int i = 0; i < game.cells.Count; i++)
            {
                for (int j = 0; j < game.cells[i].Count; j++)
                {
                    if (game.cells[i][j].Item1.Location == rectLocation) //found a cell in which a card is in
                    {
                        for (int k = 0; k < game.cells.Count; k++)
                        {
                            for (int l = 0; l < game.cells[k].Count; l++)
                            {
                                if (game.cells[k][l].Item1.Contains(FindForm().PointToClient(MousePosition))) //found a cells in which we want to put a card
                                {
                                    if (k == game.cells.Count - 1) //starting a new subchain using a card from the chain 
                                    {
                                        Console.WriteLine($"starting a new subchain using {CommonName}");
                                        game.playerChain.Add(new List<Card>());
                                        game.playerChain[k].Add(this);
                                        game.playerChain[i].RemoveAt(j);
                                        if (game.playerChain[i].Count == 0) game.playerChain.RemoveAt(i);
                                        Drop();
                                        Location = rectLocation = new Point(game.cells[k][l].Item1.Location.X, game.cells[k][l].Item1.Location.Y);
                                        game.cells[k][l] = (game.cells[k][l].Item1, true);
                                        game.UpdateCells();
                                        game.playerChain.ShiftCards();

                                        if (game.username != string.Empty)
                                        {
                                            RemoveFromChain(i, j);
                                            PushToChain(k, l);
                                            game.playerChain.UpdateIndices();
                                        }
                                        Console.WriteLine(game.playerHand.ToString());
                                        Console.WriteLine(game.playerChain.ToString());
                                        return;
                                    }
                                    else if (l == game.cells[k].Count - 1) //moving this card to tail of subchain
                                    {
                                        Console.WriteLine($"moving {CommonName} card to tail of subchain");
                                        Drop();
                                        Location = rectLocation = new Point(game.cells[k][l].Item1.Location.X, game.cells[k][l].Item1.Location.Y);
                                        game.playerChain[k].Add(this);
                                        game.playerChain[i].RemoveAt(j);
                                        if (game.playerChain[i].Count == 0) game.playerChain.RemoveAt(i);
                                        game.UpdateCells();
                                        game.playerChain.ShiftCards();
                                        if (game.username != string.Empty)
                                        {
                                            RemoveFromChain(i, j);
                                            PushToChain(k, l);
                                            game.playerChain.UpdateIndices();
                                        }

                                        Console.WriteLine(game.playerHand.ToString());
                                        Console.WriteLine(game.playerChain.ToString());
                                        return;
                                    }

                                    if ((i, j) == (k, l))  //card was placed in the same cell it was or simply clicked
                                    {
                                        Drop();
                                        Location = rectLocation;

                                        Console.WriteLine(game.playerHand.ToString());
                                        Console.WriteLine(game.playerChain.ToString());
                                        return;
                                    }
                                    //if we are here we swap 2 cards in chain

                                    if (game.username != string.Empty)
                                    { 
                                        RemoveFromChain(i, j);
                                        game.playerChain[k][l].RemoveFromChain(k, l);
                                        PushToChain(k, l);
                                        game.playerChain[k][l].PushToChain(i, j);
                                    }

                                    Card temp = game.playerChain[k][l];
                                    Drop();
                                    Location = rectLocation = game.cells[k][l].Item1.Location;
                                    game.playerChain[k][l].MoveCard( game.playerChain[k][l].rectLocation = game.cells[i][j].Item1.Location);
                                    game.playerChain[k][l] = this;
                                    game.playerChain[i][j] = temp;

                                    Console.WriteLine(game.playerHand.ToString());
                                    Console.WriteLine(game.playerChain.ToString());
                                    return;
                                }
                            }
                        }
                        //if we are here then we couldnt find the cell in which the card was placed in so we just delete it from the chain and put it to hand.
                        (Rectangle, bool) tuple = (game.cells[i][j].Item1, false);
                        game.cells[i][j] = tuple;
                        game.playerChain[i].Remove(this);
                        if (game.playerChain[i].Count == 0) game.playerChain.RemoveAt(i); //no cards in the subchain so remove the subcahim
                        game.playerHand.Add(this);
                        if (game.username != string.Empty)
                        {
                            RemoveFromChain(i, j);
                            game.playerChain.UpdateIndices();
                            PushToHand();
                        }
                        game.UpdateCells();
                        if (cardWidth * game.playerHand.Count > game.form.ClientRectangle.Width) game.playerHand.putCardsOnTopOfEachOther();
                        else game.playerHand.ShiftCards();
                        game.playerChain.ShiftCards();
                        Drop();
                        rectLocation = new Point(0, 0);
                        inChain = false;
                        Console.WriteLine(game.playerHand.ToString());
                        Console.WriteLine(game.playerChain.ToString());
                        return;
                    }
                }
            }
        }
    }
    public void PushToHand()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            sqlConnection.Open();
            SqlCommand insert = new SqlCommand($"INSERT INTO Hand(Username, CardID) VALUES ('{game.username}', '{ScientificName}')", sqlConnection);
            insert.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    public void RemoveFromHand()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand removeCard = new SqlCommand($"DELETE FROM Hand Where Username='{game.username}' AND CardID='{ScientificName}'", sqlConnection);
            sqlConnection.Open();            
            removeCard.ExecuteNonQuery();
            sqlConnection.Close(); 
        }
    }
    public void PushToChain(int RowNo, int ColNo)
    {
        //Console.WriteLine($"pushing to chain {CommonName} to [{RowNo}][{ColNo}]");
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand pushCardToChain = new SqlCommand($"INSERT INTO FoodChainCards(Username, CardID, RowNo, PositionNo) VALUES ('{game.username}', '{ScientificName}', {RowNo}, {ColNo})", sqlConnection);
            sqlConnection.Open();            
            pushCardToChain.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    public void RemoveFromChain(int RowNo, int ColNo)
    {
        //Console.WriteLine($"removing from chain {CommonName} from [{RowNo}][{ColNo}]");
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand deleteCardFromChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}' AND CardID='{ScientificName}' AND RowNo={RowNo} AND PositionNo={ColNo}", sqlConnection);
            sqlConnection.Open(); 
            deleteCardFromChain.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    private void Drop() => Picked = false;
    private void Pick()
    {
        Picked = true;
        BringToFront();
    }
    public void card_RightClick(object sender, EventArgs e)
    {
        game.countDownTimer.Stop();
        MessageBox.Show($"{Description}\nprimarily lives in {Habitat} and is {Hierarchy} in the foodchain");
        game.countDownTimer.Start();
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
    private void card_MouseEnter(object sender, EventArgs e)
    {
        Hoovered = true;
        if (game.form.currTheme == MainForm.Theme.Dark) BackColor = Color.Gray;
        else BackColor = Color.DarkGray;
        BringToFront();
        Invalidate();
    }
    private void card_MouseLeave(object sender, EventArgs e)
    {
        Hoovered = false;
        if (game.form.currTheme == MainForm.Theme.Dark) BackColor = Color.DarkGray;
        else BackColor = Color.LightGray;    
        SendToBack();
        Invalidate();
    }
    public void MoveCard(Point u) 
    {
        if (isAnimating) return;

        v = u;                                                  //destination
        p = Location;                                           //starting point
        pv = new Point(v.X - p.X, v.Y - p.Y);                   //direction vector pv
        t.Start();
        s.Restart();
        isAnimating = true;
    }
    private double f(double t) => Math.Sin(3.1415 * t / 2);     //example of ease out function
    private void Tick(object sender, EventArgs e)
    {
        double elapsed = s.ElapsedMilliseconds / 1000.0;        //divding by a 1000 converts to seconds
        
        if (elapsed > animationTime)                            //the card moves o.6 seconds 
        {
            t.Stop();
            isAnimating = false;
            return;
        }

        double easingFactor = f(elapsed / animationTime);       //normalizing the value passed, so that it is in range [0, 1]

        Location = new Point(
            (int)(pv.X * easingFactor + p.X),                   //move in x direction of pv from the current x coordinate
            (int)(pv.Y * easingFactor + p.Y)                    //move in y directon of pv from the current y coordinate
        );
    }
}