using planimals;
using planimals.Forms;
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
    public string username = "";
    public int totalPoints; //total points the player has on their account

    Game game;
    private static string dbPath = Path.Combine(Environment.CurrentDirectory, "cards.mdf");
    public static string CONNECTION_STRING = "Data Source=(LocalDB)\\MSSQLLocalDB;" + $"AttachDbFilename={dbPath}" + ";Integrated Security=True;Connect Timeout=30";


    public Button playButton;
    public Button loadButton;
    public Button loginButton;

    public Button openEditorButton;

    public Label title;
    public Label label;
    public Label stats;
    public Label labelTimer;
    public Label currentScore;

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
        MaximumSize = new Size(1400, 850);
        Text = "planimals";
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;

        largeFont = new Font("Arial", 28);
        largeFont = new Font("Arial", 28);
        smallFont = new Font("Arial", 14);

        //static size
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Size = new Size(1600, 900);
        workingHeight = ClientRectangle.Height;
        workingWidth = ClientRectangle.Width;

        BackgroundImage = new Bitmap(Image.FromFile(Environment.CurrentDirectory + "\\assets\\photos\\background.png"));
        BackgroundImageLayout = ImageLayout.Stretch;
        stats = new Label()
        {
            Location = new Point(workingWidth - 300, 10),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true
        };
        Controls.Add(stats);

        playButton = new Button() {
            Text = "play",
            BackColor = Color.White,
            Size = new Size(50, 30),
            Location = new Point(
                workingWidth / 2 - 50 / 2,
                workingHeight / 2 - 30 / 2 - 60)
        };
        Controls.Add(playButton);
        playButton.Click += playButton_Click;

        loadButton = new Button()
        {
            Text = "load",
            BackColor = Color.White,
            Size = new Size(50, 30),
            Location = new Point(workingWidth / 2 - playButton.Width / 2, workingHeight / 2 - playButton.Height / 2 - 30)
        };
        Controls.Add(loadButton);
        loadButton.Click += continueButton_Click;

        loginButton = new Button()
        {
            Text = "log in",
            BackColor = Color.White,
            Size = new Size(50, 30),
        };
        loginButton.Location = new Point(
                workingWidth / 2 - loginButton.Width / 2, workingHeight / 2 - loginButton.Height / 2);
        Controls.Add(loginButton);
        loginButton.Click += Login;

        openEditorButton = new Button()
        {
            Text = "editor",
            BackColor = Color.White,
            Size = new Size(50, 30),
        };
        openEditorButton.Location = new Point(workingWidth / 2 - openEditorButton.Width / 2, workingHeight / 2 - openEditorButton.Height / 2 + 30);
        Controls.Add(openEditorButton);
        openEditorButton.Click += openEditorButton_Click;

        title = new Label()
        { Text = "Planimals",
            AutoSize = true,
            Font = largeFont,
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };
        title.Location = new Point((workingWidth - title.Size.Width) / 2, workingHeight / 8);
        Controls.Add(title);

        retryButton = new PictureBox()
        {
            Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "retry.png")),
            SizeMode = PictureBoxSizeMode.StretchImage,
            Size = new Size(workingWidth / 10, workingWidth / 10),
        };
        retryButton.Location = new Point(
            workingWidth / 2 - retryButton.Width / 2,
            workingHeight / 2 - retryButton.Height / 2);
        Controls.Add(retryButton);
        retryButton.Click += retryButton_Click;

        goToMenuButton = new PictureBox()
        {
            Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "exit.png")),
            SizeMode = PictureBoxSizeMode.StretchImage,
            Size = new Size(workingWidth / 10, workingWidth / 8),
        };
        goToMenuButton.Location = new Point(
            workingWidth / 2 + goToMenuButton.Width,
            workingHeight / 2 - goToMenuButton.Height / 2);
        Controls.Add(goToMenuButton);
        goToMenuButton.Click += goToMenuButton_Click;

        goToMenuInGameButton = new PictureBox()
        {
            Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "exit.png")),
            SizeMode = PictureBoxSizeMode.StretchImage,
            Size = new Size(workingWidth / 20, workingWidth / 16),
            Location = new Point(5, 5)
        };
        Controls.Add(goToMenuInGameButton);
        goToMenuInGameButton.Click += goToMenuInGameButton_Click;

        yesButton = new Button()
        {
            Text = "yes",
            BackColor = Color.White,
            Size = new Size(50, 30),
        };
        yesButton.Location = new Point(
            workingWidth / 2 + yesButton.Width / 2,
            workingHeight / 2 - yesButton.Height);
        Controls.Add(yesButton);
        yesButton.Click += yesButton_Click;

        noButton = new Button()
        {
            Text = "no",
            BackColor = Color.White,
            Size = new Size(50, 30),
        };
        noButton.Location = new Point(
                workingWidth / 2 - noButton.Width - noButton.Width / 2,
                workingHeight / 2 - noButton.Height);
        Controls.Add(noButton);
        noButton.Click += noButton_Click;

        youSureWannaQuitLabel = new Label() {
            AutoSize = true,
            Text = "Are you sure you want to quit?",
            ForeColor = Color.White
        };
        youSureWannaQuitLabel.Location = new Point(
                workingWidth / 2 - youSureWannaQuitLabel.Width,
                workingHeight / 3);
        Controls.Add(youSureWannaQuitLabel);

        drawCardButton = new PictureBox()
        {
            SizeMode = PictureBoxSizeMode.StretchImage,
            Size = new Size(workingHeight / 8, workingWidth / 10),
            Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "back.png"))
        };
        drawCardButton.Location = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);

        Point initPos = new Point(
            drawCardButton.Width - workingHeight / 100 * 5,
            workingHeight / 2 - drawCardButton.Height / 2);
        drawCardRectangle = new Rectangle(drawCardButton.Location.X, drawCardButton.Location.Y, drawCardButton.Width, drawCardButton.Height);
        Controls.Add(drawCardButton);
        drawCardButton.MouseClick += drawCardButton_Click;
        drawCardButton.MouseEnter += drawCardButton_MouseEnter;
        drawCardButton.MouseLeave += drawCardButton_MouseLeave;

        chainButton = new PictureBox()
        {
            SizeMode = PictureBoxSizeMode.StretchImage,
            Size = new Size(workingWidth / 10, workingHeight / 10),
            Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", "chain.png"))
        };
        chainButton.Location = new Point(
                workingWidth - drawCardButton.Width - workingHeight / 10,
                workingHeight / 2 - drawCardButton.Height / 2);
        chainButtonRectangle = new Rectangle(
            workingWidth - drawCardButton.Width - workingHeight / 10,
            workingHeight / 2 - drawCardButton.Height / 2,
            workingWidth / 10,
            workingHeight / 10);
        Controls.Add(chainButton);
        chainButton.Click += new EventHandler(chainButton_Click);
        chainButton.MouseEnter += chainButton_MouseEnter;
        chainButton.MouseLeave += chainButton_MouseLeave;

        label = new Label()
        {
            Location = new Point(workingWidth / 10, workingHeight / 20),
            ForeColor = Color.White,
            AutoSize = true,
            BackColor = Color.Transparent
        };
        Controls.Add(label);

        labelTimer = new Label()
        {
            ForeColor = Color.White,
            Font = new Font(label.Font.FontFamily, 25),
            Location = new Point(
                (int)(workingWidth * 0.9),
                (int)((double)workingHeight / 10)),
            AutoSize = true
        };
        Controls.Add(labelTimer);

        currentScore = new Label()
        {
            ForeColor = Color.White,
            Font = new Font(label.Font.FontFamily, 25),
            AutoSize = true,
            Location = new Point(labelTimer.Location.X, labelTimer.Location.Y + 105)
        };
        Controls.Add(currentScore);

        foreach (Control control in Controls)
        {
            if (control is Label)
            {
                if (Width < 1920 || Height < 1080) control.Font = smallFont;
                else control.Font = largeFont;
            }
        }
        readySteadyGo = new PictureBox()
        {
            Size = new Size(workingWidth / 10, workingWidth / 4),
            SizeMode = PictureBoxSizeMode.CenterImage,
        };
        readySteadyGo.Location = new Point(
            workingWidth / 2 - readySteadyGo.Width / 2,
            workingHeight / 2 - readySteadyGo.Height / 2);
        Controls.Add(readySteadyGo);

        #endregion
        Paint += new PaintEventHandler(Draw);
        gameControls = new List<Control>()
        {
            label,
            drawCardButton,
            chainButton,
            goToMenuInGameButton,
            currentScore
        };
        menuControls = new List<Control>()
        {
            label,
            loginButton,
            playButton,
            loadButton,
            stats,
            title,
            openEditorButton
        };
        endControls = new List<Control>()
        {
            retryButton,
            goToMenuButton
        };
        youSureWannaQuitControls = new List<Control>()
        {
            yesButton,
            noButton,
            youSureWannaQuitLabel
        };

        foreach (Control c in Controls)
        {
            c.BackColor = Color.Transparent;
            if (gameControls.Contains(c) || endControls.Contains(c) || youSureWannaQuitControls.Contains(c)) 
            {
                if (c == label) continue;
                c.Hide();
                c.Enabled = false;
            }
        }
        foreach (Control control in Controls) if (control is Label) control.ForeColor = Color.BlueViolet;
    }
    private void openEditorButton_Click(object sender, EventArgs e)
    {
        if (username == "admin")
        {
            Editor editor = new Editor();
            editor.ShowDialog();
        }
        else Display("Only admin can access editor");
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
                SqlCommand test = new SqlCommand("delete from FoodChainCards where Username='player1'\r\ndelete from Games where Username='player1'\r\ndelete from Hand where Username='player1'\r\ninsert into Games(Username, Time, Deck, Score) values\r\n('player1', 36, '1,1,1,1,1,1,1,1,', 4)\r\n\r\ninsert into Hand(Username, CardID) values\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus'),\r\n('player1', 'Omocestus viridulus');\r\n\r\nInsert into FoodChainCards(Username, CardID, RowNo, PositionNo) values\r\n('player1', 'Poa pratensis', 0, 0),\r\n('player1', 'Omocestus viridulus', 0, 1),\r\n('player1', 'Turdus merula', 0, 2),\r\n('player1', 'Pantherophis obsoletus', 0, 3),\r\n('player1', 'Tyto alba', 0, 4),\r\n('player1', 'Poa pratensis', 1, 0),\r\n('player1', 'Microtus arvalis', 1, 1);", sqlConnection);

                sqlConnection.Open();
                test.ExecuteNonQuery();
                SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM Games WHERE Username='{username}'", sqlConnection);
                int b = (int)cmd.ExecuteScalar();
                sqlConnection.Close();
                if (b == 1)
                {
                    foreach (Control control in menuControls)
                    {
                        control.Enabled = false;
                        control.Hide();
                    }
                    SqlCommand pullTimeHand = new SqlCommand($"SELECT Time, Deck, Score FROM Games WHERE Username='{username}'", sqlConnection);
                    sqlConnection.Open();
                    SqlDataReader r = pullTimeHand.ExecuteReader();
                    while (r.Read())
                    {
                        game = new Game(this, username, (int)r["Time"], r["Deck"].ToString(), totalPoints, int.Parse(r["Score"].ToString()));
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
        foreach (Control c in Controls)
        {
            if (menuControls.Contains(c) || endControls.Contains(c) || gameControls.Contains(c)) 
            {
                c.Enabled = false;
                c.Hide();
            }
        }
        DisposeCards();
        game = new Game(this, username);
    }
    private void goToMenuButton_Click(object sender, EventArgs e)
    {
        label.Hide();
        labelTimer.Hide();
        foreach (Control c in Controls)
        {
            if (endControls.Contains(c) || gameControls.Contains(c) || youSureWannaQuitControls.Contains(c))
            {
                c.Enabled = false;
                c.Hide();
            }
            else if (menuControls.Contains(c)) 
            {
                c.Enabled = true; 
                c.Show();
            }
            label.Show();
        }
        DisposeCards();

        game = null;
        Invalidate();
    }
    private void goToMenuInGameButton_Click(object sender, EventArgs e)
    {
        game.countDownTimer.Stop();
        UpdateStatsLabel();
        foreach (Control c in Controls)
        {
            if (gameControls.Contains(c) || c is Card)
            {
                c.Enabled = false;
                c.Hide();
            }
            else if (youSureWannaQuitControls.Contains(c))
            {
                c.Show();
                c.Enabled = true;
            }
        }
        Invalidate();
    }
    private void yesButton_Click(object sender, EventArgs e) => goToMenuButton_Click(sender, e);
    private void noButton_Click(object sender, EventArgs e)
    {
        foreach (Control c in Controls)
        {
            if (gameControls.Contains(c) || c is Card)
            {
                c.Enabled = true;
                c.Show();
            }
            else if (youSureWannaQuitControls.Contains(c))
            {
                c.Hide();
                c.Enabled = false;
            }
        }
        game.countDownTimer.Start();
        Invalidate();
    }
    private void Draw(object sender, PaintEventArgs e)
    {
        if (game != null)
            if (game.countDownTimer.Enabled)
                using (Pen pen = new Pen(Color.NavajoWhite, 6.0f))
                    foreach (List<(Rectangle, bool)> tuples in game.cells)
                        foreach ((Rectangle, bool) tuple in tuples) e.Graphics.DrawRectangle(pen, tuple.Item1);
    }
    public async void Display(string s) {
        if (this == null) return;
        label.Text = s;
        await System.Threading.Tasks.Task.Delay(7883).ContinueWith(
            _ => Invoke(new MethodInvoker(() => label.Text = ""))
        );
    }
    private void drawCardButton_Click(object sender, EventArgs e) => game.deck.DrawCard(sender, e);
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
        if (username == string.Empty)
        {
            LoginForm loginForm = new LoginForm(this);
            loginForm.ShowDialog();
        }
        else
        {
            username = "";
            stats.Text = "";
            loginButton.Text = "log in";
        }
    }
    public void UpdateStatsLabel()
    {
        if (username != "") stats.Text = $"Hey, {username}!\ntotal points: {totalPoints}";
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
    public void DisposeCards() 
    {
        List<Control> controls = new List<Control>();
        for (int i = 0; i < Controls.Count; i++) if (Controls[i] is Card) controls.Add(Controls[i]);
        foreach (Control c in controls) Controls.Remove(c);
    }
}