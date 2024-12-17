using planimals;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;

public class Card : PictureBox
{
    private Game game;

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

    public static int cardWidth;
    public static int cardHeight;

    public Card(Game g, string scientific_name, string common_name, string description, string path, int hierarchy, string habitat, Point position, bool inchain)
    {
        game = g;
        DoubleBuffered = true;

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
            Image = Image.FromFile(path);
        }
        catch
        {
            MessageBox.Show($"cant load this : {path}\n probably missing file");
        }

        SizeMode = PictureBoxSizeMode.Zoom;
        Size = new Size(cardWidth, cardHeight);
        Location = position;
        prevLocation = new Point(cardWidth * game.playerHand.Count, game.form.workingHeight - cardHeight);
        BackColor = System.Drawing.Color.Gray;
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
        using (Font myFont = new Font("Mono", 10)) e.Graphics.DrawString(CommonName, myFont, System.Drawing.Brushes.Black, new Point(Width / 10, Height / 20));
    }
    private void card_MouseDown(object sender, MouseEventArgs e)
    {
        for (int i = 0; i < game.playerHand.Count; i++)
        {
            if (game.playerHand[i].Picked)
            {
                Drop(game.playerHand[i]);
                game.playerHand[i].Location = game.playerHand[i].prevLocation;
            }
        }
        Pick(this);
        //Console.WriteLine($"picked {CommonName}");
    }
    private void card_Mouseup(object sender, MouseEventArgs e)
    {
        if (!inChain)
        {
            Console.WriteLine("moving from hand");
            for (int i = 0; i < game.cells.Count; i++)
            {
                for (int j = 0; j < game.cells[i].Count; j++)
                {
                    if (game.cells[i][j].Item1.Contains(FindForm().PointToClient(MousePosition)))
                    {
                        if (!game.cells[i][j].Item2) //cell is empty
                        {
                            if (game.cells[i].Count == 1) game.playerChain.chain.Add(new List<Card>());
                            Drop(this);
                            Location = game.cells[i][j].Item1.Location;
                            (Rectangle, bool) tuple = (game.cells[i][j].Item1, true);
                            game.cells[i][j] = tuple;
                            inChain = true;
                            game.playerHand.Remove(this);
                            prevLocation = new Point(cardWidth * game.playerHand.Count, game.form.workingHeight - cardHeight);
                            game.playerChain.chain[i].Add(this);
                            if (game.username != "")
                            {
                                RemoveFromHand();
                                PushToChain(i, j);
                            }
                            game.UpdateCells();
                            game.playerHand.ShiftCards();
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
            Console.WriteLine("moving from chain");
            for (int i = 0; i < game.cells.Count; i++)
            {
                for (int j = 0; j < game.cells[i].Count; j++)
                {
                    if (game.cells[i][j].Item1.Location == rectLocation)
                    {
                        (Rectangle, bool) tuple = (game.cells[i][j].Item1, false);
                        game.cells[i][j] = tuple;
                        //Console.WriteLine($"before removing {CommonName}: {playerChain[i].Count}");
                        game.playerChain.chain[i].Remove(this);
                        //Console.WriteLine($"afte r : {playerChain[i].Count}");
                        if (game.playerChain.chain[i].Count == 0)
                        {
                            Console.WriteLine($"removing chain[{i}]");
                            game.playerChain.chain.RemoveAt(i);
                        }
                        game.playerHand.Add(this);
                        if (game.username != "")
                        {
                            RemoveFromChain(i, j);
                            PushToHand();
                        }
                        game.UpdateCells();
                        game.playerHand.ShiftCards();
                        ShiftCards();
                        Drop(this);
                        Location = prevLocation = new Point(cardWidth * game.playerHand.Count, game.form.workingHeight - cardHeight);
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
        for (int i = 0; i < game.playerChain.chain.Count; i++)
            for (int j = 0; j < game.playerChain.chain[i].Count; j++)
                game.playerChain.chain[i][j].Location = game.playerChain.chain[i][j].rectLocation =
                game.cells[i][j].Item1.Location;
    }
    public void PushToHand()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            sqlConnection.Open();
            SqlCommand insert = new SqlCommand($"INSERT INTO Hand(Username, CardID) VALUES ('{game.username}', '{ScientificName}')", sqlConnection);
            insert.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    public void RemoveFromHand()
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            SqlCommand removeCard = new SqlCommand($"DELETE FROM Hand Where Username='{game.username}' AND CardID='{ScientificName}'", sqlConnection);
            sqlConnection.Open();
            removeCard.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    public void PushToChain(int RowNo, int ColNo)
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            SqlCommand pushCardToChain = new SqlCommand($"INSERT INTO FoodChainCards(Username, CardID, RowNo, PositionNo) VALUES ('{game.username}', '{ScientificName}', {RowNo}, {ColNo})", sqlConnection);
            sqlConnection.Open();
            pushCardToChain.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    public void RemoveFromChain(int RowNo, int ColNo)
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
        {
            SqlCommand deleteCardFromChain = new SqlCommand($"DELETE FROM FoodChainCards WHERE Username='{game.username}' AND CardID='{ScientificName}' AND RowNo={RowNo} AND PositionNo={ColNo}", sqlConnection);
            sqlConnection.Open();
            deleteCardFromChain.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
    private void Drop(Card c)
    {
        c.Picked = false;
        c.BackColor = System.Drawing.Color.Gray;
        FindForm().Invalidate();
    }
    private void Pick(Card c)
    {
        c.Picked = true;
        c.BackColor = System.Drawing.Color.White;
        FindForm().Invalidate();
        BringToFront();
    }
    public void card_RightClick(object sender, EventArgs e)
    {
        game.countDownTimer.Stop();
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
}