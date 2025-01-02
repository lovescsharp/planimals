using planimals;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


//https://stackoverflow.com/questions/51567681/how-to-do-task-continewith-using-a-delay-before-the-continuewith-in-c
//allows to asynchronously change label.Text, looks cool 8) 

public partial class MainForm : Form
{
    public string username;
    public bool loggedIn;
    Game game;
    private static string dbPath = Path.Combine(Environment.CurrentDirectory, "cards.mdf");
    public static string CONNECTION_STRING = "Data Source=(LocalDB)\\MSSQLLocalDB;" + $"AttachDbFilename={dbPath}" + ";Integrated Security=True;Connect Timeout=30";


    public Button playButton;
    public Button loadButton;
    public Button loginButton;
    public Button exitButton;
    
    public Label title;
    public Label label;
    public Label stats;
    public Label labelTimer;
    
    public PictureBox readySteadyGo;
    
    public int workingHeight;
    public int workingWidth;
    
    public PictureBox retryButton;
    
    public PictureBox goToMenuButton;
    public PictureBox goToMenuInGameButton;
    
    public Button yesButton;
    public Button noButton;
    public Label youSureWannaQuitLabel;
    
    public PictureBox drawCardButton;
    public Rectangle drawCardRectangle;
    public PictureBox chainButton;
    public Rectangle chainButtonRectangle;

    public List<Control> gameControls;
    public List<Control> menuControls;
    public List<Control> endControls;
    public List<Control> youSureWannaQuitControls;

    public Font largeFont;
    public Font smallFont;

    public MainForm()
    {
        InitializeComponent();
        #region ux/ui
        MinimumSize = new Size(1120, 620);
        Text = "planimals";
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;

        largeFont = new Font("Arial", 28);
        largeFont = new Font("Arial", 28);
        smallFont = new Font("Arial", 14);

        //static size
        FormBorderStyle = FormBorderStyle.Fixed3D;
        MinimizeBox = false;
        //Size = new Size(1366, 768);
        Size = new Size(1600, 900);
        workingHeight = ClientRectangle.Height;
        workingWidth = ClientRectangle.Width;

        //BackgroundImage = new Bitmap(Image.FromFile(Environment.CurrentDirectory + "\\assets\\photos\\background.png"));
        //BackgroundImageLayout = ImageLayout.Stretch;
        BackColor = Color.DarkSeaGreen;
        stats = new Label();
        stats.Size = new Size(300, 100);
        stats.Location = new Point(workingWidth - 300, 10);
        stats.ForeColor = Color.White;
        Controls.Add(stats);

        playButton = new Button();
        playButton.Text = "play";
        playButton.BackColor = Color.White;
        playButton.Size = new Size(50, 30);
        playButton.Location = new Point(workingWidth / 2 - playButton.Width / 2, workingHeight / 2 - playButton.Height / 2 - 60);
        Controls.Add(playButton);
        playButton.Click += playButton_Click;

        loadButton = new Button();
        loadButton.Text = "load";
        loadButton.BackColor = Color.White;
        loadButton.Size = new Size(50, 30);
        loadButton.Location = new Point(workingWidth / 2 - playButton.Width / 2, workingHeight / 2 - playButton.Height / 2 - 30);
        Controls.Add(loadButton);
        loadButton.Click += continueButton_Click;

        loginButton = new Button();
        loginButton.Text = "log in";
        loginButton.BackColor = Color.White;
        loginButton.Size = new Size(50, 30);
        loginButton.Location = new Point(workingWidth / 2 - loginButton.Width / 2, workingHeight / 2 - loginButton.Height / 2);
        Controls.Add(loginButton);
        loginButton.Click += Login;

        exitButton = new Button();
        exitButton.Text = "exit";
        exitButton.BackColor = Color.White;
        exitButton.Size = new Size(50, 30);
        exitButton.Location = new Point(workingWidth / 2 - loginButton.Width / 2, workingHeight / 2 - exitButton.Height / 2 + 30);
        Controls.Add(exitButton);
        exitButton.Click += Exit;

        title = new Label();
        title.Text = "Planimals";
        title.AutoSize = true;
        title.Font = largeFont;
        title.ForeColor = Color.White;
        title.Location = new Point((workingWidth - title.Size.Width) / 2, workingHeight / 8);
        Controls.Add(title);

        retryButton = new PictureBox();
        retryButton.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "retry.png"));
        retryButton.SizeMode = PictureBoxSizeMode.StretchImage;
        retryButton.Size = new Size(workingWidth / 10, workingWidth / 10);
        retryButton.Location = new Point(workingWidth / 2 - retryButton.Width / 2, workingHeight / 2 - retryButton.Height / 2);
        retryButton.Click += retryButton_Click;
        Controls.Add(retryButton);

        goToMenuButton = new PictureBox();
        goToMenuButton.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "exit.png"));
        goToMenuButton.SizeMode = PictureBoxSizeMode.StretchImage;
        goToMenuButton.Size = new Size(workingWidth / 10, workingWidth / 8);
        goToMenuButton.Location = new Point(workingWidth / 2 + goToMenuButton.Width, workingHeight / 2 - goToMenuButton.Height / 2);
        goToMenuButton.Click += goToMenuButton_Click;
        Controls.Add(goToMenuButton);

        goToMenuInGameButton = new PictureBox();
        goToMenuInGameButton.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "exit.png"));
        goToMenuInGameButton.SizeMode = PictureBoxSizeMode.StretchImage;
        goToMenuInGameButton.Size = new Size(workingWidth / 20, workingWidth / 16);
        goToMenuInGameButton.Location = new Point(5, 5);
        goToMenuInGameButton.Click += goToMenuInGameButton_Click;
        Controls.Add(goToMenuInGameButton);

        yesButton = new Button();
        yesButton.Text = "yes";
        yesButton.BackColor = Color.White;
        yesButton.Size = new Size(50, 30);
        yesButton.Location = new Point(workingWidth / 2 + yesButton.Width / 2, workingHeight / 2 - yesButton.Height);
        yesButton.Click += yesButton_Click;
        Controls.Add(yesButton);

        noButton = new Button();
        noButton.Text = "no";
        noButton.BackColor = Color.White;
        noButton.Size = new Size(50, 30);
        noButton.Location = new Point(workingWidth / 2 - noButton.Width - noButton.Width / 2, workingHeight / 2 - noButton.Height);
        noButton.Click += noButton_Click;
        Controls.Add(noButton);

        youSureWannaQuitLabel = new Label();
        youSureWannaQuitLabel.AutoSize = true;
        youSureWannaQuitLabel.Location = new Point(workingWidth / 2 - youSureWannaQuitLabel.Width, workingHeight / 3);
        youSureWannaQuitLabel.Text = "Are you sure you want to quit?";
        youSureWannaQuitLabel.ForeColor = Color.White;
        Controls.Add(youSureWannaQuitLabel);

        drawCardButton = new PictureBox();
        drawCardButton.SizeMode = PictureBoxSizeMode.StretchImage;
        drawCardButton.Size = new Size(workingHeight / 8, workingWidth / 10);
        drawCardButton.Location = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);
        Point initPos = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);
        drawCardRectangle = new Rectangle(drawCardButton.Location.X, drawCardButton.Location.Y, drawCardButton.Width, drawCardButton.Height);
        drawCardButton.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "back.png"));
        Controls.Add(drawCardButton);
        drawCardButton.MouseEnter += drawCardButton_MouseEnter;
        drawCardButton.MouseLeave += drawCardButton_MouseLeave;

        chainButton = new PictureBox();
        chainButton.SizeMode = PictureBoxSizeMode.StretchImage;
        chainButton.Size = new Size(workingWidth / 10, workingHeight / 10);
        chainButton.Location = new Point(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2);
        chainButtonRectangle = new Rectangle(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2,
            workingWidth / 10,
            workingHeight / 10);
        chainButton.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "chain.png"));
        Controls.Add(chainButton);
        chainButton.Click += new EventHandler(chainButton_Click);
        chainButton.MouseEnter += chainButton_MouseEnter;
        chainButton.MouseLeave += chainButton_MouseLeave;

        label = new Label();
        label.Location = new Point(workingWidth / 10, workingHeight / 20);
        label.ForeColor = Color.White;
        label.AutoSize = true;
        Controls.Add(label);

        labelTimer = new Label();
        labelTimer.ForeColor = Color.White;
        labelTimer.Font = new Font(label.Font.FontFamily, 25);
        labelTimer.Location = new Point((int)((double)workingWidth * 0.9), (int)((double)workingHeight / 10));
        labelTimer.Size = new Size(100, 100);
        Controls.Add(labelTimer);

        foreach (Control control in Controls)
        {
            if (control is Label)
            {
                if (Width < 1920 || Height < 1080) control.Font = smallFont;
                else control.Font = largeFont;
            }
        }
        readySteadyGo = new PictureBox();
        readySteadyGo.Size = new Size(workingWidth / 10, workingWidth / 4);
        readySteadyGo.SizeMode = PictureBoxSizeMode.CenterImage;
        readySteadyGo.Location = new Point(workingWidth / 2 - readySteadyGo.Width / 2, workingHeight / 2 - readySteadyGo.Height / 2);
        Controls.Add(readySteadyGo);

        #endregion
        Paint += new PaintEventHandler(Draw);
        //Resize += new EventHandler(OnResize);
        gameControls = new List<Control>() { drawCardButton, chainButton, goToMenuInGameButton };
        menuControls = new List<Control>() { loginButton, playButton, loadButton, exitButton, stats, title };
        endControls = new List<Control>() { retryButton, goToMenuButton };
        youSureWannaQuitControls = new List<Control>() { yesButton, noButton, youSureWannaQuitLabel };

        foreach (Control control in gameControls) control.Hide();
        foreach (Control control in endControls) control.Hide();
        foreach (Control control in youSureWannaQuitControls) control.Hide();
        foreach (Control control in Controls) if (control is Label) control.ForeColor = Color.LightGreen;
    }
    private void playButton_Click(object sender, EventArgs e)
    {
        foreach (Control control in menuControls)
        {
            control.Enabled = false;
            control.Hide();
        }
        game = new Game(this, username);
    }
    private void continueButton_Click(object sender, EventArgs e)
    {
        if (username != string.Empty)
        {
            using (SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING))
            {
                /* testing load feature */
                /*
                delete from FoodChainCards where Username='player1'
                delete from Games where Username='player1'
                delete from Hand where Username='player1'
                insert into Games(Username, Time, Deck) values
                ('player1', 36, '1,1,1,1,1,1,1,1,')

                insert into Hand(Username, CardID) values
                ('player1', 'Omocestus viridulus'),
                ('player1', 'Omocestus viridulus'),
                ('player1', 'Omocestus viridulus');

                Insert into FoodChainCards(Username, CardID, RowNo, PositionNo) values
                ('player1', 'Poa pratensis', 0, 0),
                ('player1', 'Omocestus viridulus', 0, 1),
                ('player1', 'Turdus merula', 0, 2),
                ('player1', 'Pantherophis obsoletus', 0, 3),
                ('player1', 'Tyto alba', 0, 4),
                ('player1', 'Poa pratensis', 1, 0),
                ('player1', 'Microtus arvalis', 1, 1); 
                */

                ///
                SqlCommand test = new SqlCommand("delete from FoodChainCards where Username='player1'\r\ndelete from Games where Username='player1'\r\ndelete from Hand where Username='player1'\r\ninsert into Games(Username, Time, Deck) values\r\n('player1', 36, ',1,1,1,1,1,1,1,1')\r\n\r\ninsert into Hand(Username, CardID) values\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus');\r\n\r\nInsert into FoodChainCards(Username, CardID, RowNo, PositionNo) values\r\n('player1', 'Poa pratensis', 0, 0),\r\n('player1', 'Omocestus viridulus', 0, 1),\r\n('player1', 'Turdus merula', 0, 2),\r\n('player1', 'Pantherophis obsoletus', 0, 3),\r\n('player1', 'Tyto alba', 0, 4),\r\n('player1', 'Poa pratensis', 1, 0),\r\n('player1', 'Microtus arvalis', 1, 1);", sqlConnection);
                sqlConnection.Open(); 
                test.ExecuteNonQuery(); 
                ///

                SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM Games WHERE Username='{username}'", sqlConnection);
                int b = (int)cmd.ExecuteScalar();
                sqlConnection.Close();
                if (b == 1)
                {
                    SqlCommand pullTimeHand = new SqlCommand($"SELECT Time, Deck FROM Games WHERE Username='{username}'", sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader r = pullTimeHand.ExecuteReader(); 
                    while (r.Read())
                    {
                        Console.WriteLine(r["Time"]);
                        game = new Game(this, username, (int)r["Time"], r["Deck"].ToString());
                        drawCardButton.Click += new EventHandler(game.deck.DrawCard);
                        sqlConnection.Close();
                        return;
                    }
                    sqlConnection.Close();
                }
                else MessageBox.Show("there is no saved game");
            }
        }
        else Display("you are not logged in");
    }
    private void retryButton_Click(object sender, EventArgs e)
    {
        foreach (Control control in menuControls)
        {
            control.Enabled = false;
            control.Hide();
        }
        foreach (Control control in endControls)
        {
            control.Enabled = false;
            control.Hide();
        }
        foreach (Control control in gameControls)
        {
            control.Enabled = false;
            control.Hide();
        }
        game = new Game(this,username);
    }
    private void goToMenuButton_Click(object sender, EventArgs e)
    {
        label.Hide();
        labelTimer.Hide();
        foreach (Control control in gameControls) { control.Enabled = false; control.Hide(); }
        foreach (Control control in endControls) { control.Enabled = false; control.Hide(); }
        foreach (Control control in menuControls) { control.Enabled = true; control.Show(); }
        foreach (Card c in game.playerHand)c.Dispose();
        foreach (List<Card> subchain in game.playerChain) foreach (Card c in subchain) c.Dispose();
        //need to dispose the game!!!!!!!!!!!!!!!!!!
        foreach (Control control in youSureWannaQuitControls) { control.Hide(); control.Enabled = false; }
        Invalidate();
    }
    private void goToMenuInGameButton_Click(object sender, EventArgs e) => Quit();
    public void Quit() 
    {
        game.countDownTimer.Stop();
        UpdateStatsLabel();
        List<Control> controls = new List<Control>(); 
        for (int i = 0; i < Controls.Count; i++) if (Controls[i] is Card) controls.Add(Controls[i]);
        foreach (Control c in controls) Controls.Remove(c);
        game.playerHand.Clear();
        game.playerChain.Clear();
        game.cells.Clear();
        foreach (Control control in gameControls) control.Hide();
        
        foreach (Control control in youSureWannaQuitControls)
        {
            control.Show();
            control.Enabled = true;
        }
        Invalidate();
    }
    private void yesButton_Click(object sender, EventArgs e) => goToMenuButton_Click(sender, e);
    private void noButton_Click(object sender, EventArgs e)
    {
        foreach (Control control in youSureWannaQuitControls) control.Hide();
        foreach (Control control in gameControls) control.Show();
        foreach (Card c in game.playerHand) c.Show();
        foreach (List<Card> subchain in game.playerChain)
        {
            foreach (Card c in subchain) c.Show();
        }
        game.countDownTimer.Start();
    }
    private void Exit(object sender, EventArgs e) => Close();
    private void OnResize(object sender, EventArgs e)
    {
        Height = (int)(Width * 0.5625);
        workingHeight = ClientRectangle.Height;
        workingWidth = ClientRectangle.Width;

        drawCardButton.Width = workingHeight / 8;
        drawCardButton.Height = workingWidth / 10;

        drawCardButton.Location = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);

        chainButton.Width = workingWidth / 10;
        chainButton.Height = workingHeight / 10;
        chainButton.Location = new Point(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2);

        foreach (Control c in menuControls)
        {
            c.Size = new Size(50, 30);
            c.Location = new Point(workingWidth / 2 - loginButton.Width / 2, workingHeight / 2 - loginButton.Height / 2);
        }    

        Card.cardWidth = workingHeight / 8;
        Card.cardHeight = workingWidth / 10;
        for (int i = 0; i < game.playerHand.Count; i++)
        {
            game.playerHand[i].Width = workingHeight / 8;
            game.playerHand[i].Height = workingWidth / 10;
            game.playerHand[i].Location = game.playerHand[i].prevLocation = new Point(Card.cardWidth * i, workingHeight - Card.cardHeight);
        }
        foreach (List<Card> chain in game.playerChain)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                chain[i].Width = workingHeight / 8;
                chain[i].Height = workingWidth / 10;
                //this[i].Location = new Point((int)(this[i].Location.X * 0.5625), (int)(this[i].Location.Y * 0.5625));
                chain[i].prevLocation = new Point(game.playerHand.LastOrDefault().Location.X + Card.cardWidth * i, workingHeight - Card.cardHeight);
            }
        }
        for (int i = 0; i < game.playerHand.Count; i++)
        {
            game.playerHand[i].Width = workingHeight / 8;
            game.playerHand[i].Height = workingWidth / 10;
            game.playerHand[i].Location = new Point(Card.cardWidth * i, workingHeight - Card.cardHeight);
        }
        label.Location = new Point(workingWidth / 10, workingHeight / 8);
        Invalidate();
    }
    public void Draw(object sender, PaintEventArgs e)
    {
        if (game != null) 
            if (game.countDownTimer.Enabled)
                using (Pen pen = new Pen(Color.NavajoWhite, 6.0f))
                    foreach (List<(Rectangle, bool)> tuples in game.cells)
                        foreach ((Rectangle, bool) tuple in tuples) e.Graphics.DrawRectangle(pen, tuple.Item1);
    }
    public async void Display(string s) {
        label.Text = s;
        await System.Threading.Tasks.Task.Delay(7883).ContinueWith(
            _ => Invoke(new MethodInvoker(() => label.Text = ""))
        );
    }
    private void drawCardButton_MouseEnter(object sender, EventArgs e)
    {
        drawCardButton.Location = new Point(drawCardButton.Location.X - 3, drawCardButton.Location.Y - 3);
        drawCardButton.Size = new Size(drawCardButton.Width + 6, drawCardButton.Height + 6);
    }
    private void drawCardButton_MouseLeave(object sender, EventArgs e)
    {
        drawCardButton.Location = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);
        drawCardButton.Size = new Size(workingHeight / 8, workingWidth / 10);
    }
    private void chainButton_MouseEnter(object sender, EventArgs e)
    {
        chainButton.Location = new Point(chainButton.Location.X - 3, chainButton.Location.Y - 3);
        chainButton.Size = new Size(chainButton.Width + 6, chainButton.Height + 6);
    }
    private void chainButton_MouseLeave(object sender, EventArgs e)
    {
        chainButton.Location = new Point(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2);
        chainButton.Size = new Size(workingWidth / 10, workingHeight / 10);
    }
    private void Login(object sender, EventArgs e)
    {
        if (!loggedIn)
        {
            LoginForm loginForm = new LoginForm(this);
            loginForm.ShowDialog();
        }
        else
        {
            username = "";
            stats.Text = "";
            loggedIn = false;
            loginButton.Text = "log in";
        }
    }
    public void UpdateStatsLabel()
    {
        if (username != "") stats.Text = $"Hey, {username}!\ntotal points: {game.totalPoints}";
    }
    private void chainButton_Click(object sender, EventArgs e) => game.playerChain.CHAIN(); //   \(0o0)/
    private void chainButton_MouseMove(object sender, MouseEventArgs e)
    {
        if (MousePosition.X < chainButtonRectangle.Right && MousePosition.X > chainButtonRectangle.Left && MousePosition.Y < chainButtonRectangle.Bottom && MousePosition.Y > chainButtonRectangle.Top)
        {
            chainButton.Width = workingWidth / 10 + 5;
            chainButton.Height = workingHeight / 10 + 5;
        }
        else
        {
            chainButton.Width = workingWidth / 10;
            chainButton.Height = workingHeight / 10;
        }
    }
}