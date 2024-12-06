using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace planimals
{
    public partial class LoginForm : Form
    {
        private int workingWidth;
        private int workingHeight;

        private Button loginButton;
        private Button signUpButton;
        private Button cancelButton;

        private TextBox usernameInput;
        private TextBox passwordInput;

        private Label label;

        public LoginForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            MinimizeBox = false;

            Width = 400;
            Height = 200;

            workingHeight = ClientRectangle.Height;
            workingWidth = ClientRectangle.Width;

            usernameInput = new TextBox
            {
                Location = new Point(workingWidth / 20, workingHeight / 4 + 5),
                Size = new Size(workingWidth - 2 * (workingWidth / 20), 25)
            };
            Label l1 = new Label
            {
                Location = new Point(usernameInput.Location.X, usernameInput.Location.Y - 15),
                Text = "username:",
                ForeColor = Color.White
            };
            Controls.Add(usernameInput);
            Controls.Add(l1);


            passwordInput = new TextBox
            {
                Location = new Point(workingWidth / 20, workingHeight / 4 + 50),
                Size = new Size(workingWidth - 2 * (workingWidth / 20), 25),
                PasswordChar = '*'
            };
            Label l2 = new Label
            {
                Location = new Point(passwordInput.Location.X, passwordInput.Location.Y - 15),
                Text = "password:",
                ForeColor = Color.White
            };
            Controls.Add(passwordInput);
            Controls.Add(l2);

            loginButton = new Button();
            loginButton.Size = new Size(60, 25);
            loginButton.Location = new Point(workingWidth - loginButton.Width - 5, workingHeight - loginButton.Height - 5);
            loginButton.Text = "log in";
            loginButton.BackColor = Color.White;
            Controls.Add(loginButton);
            loginButton.Click += LoginClick;

            cancelButton = new Button();
            cancelButton.Size = new Size(60, 25);
            cancelButton.Location = new Point(5, workingHeight - cancelButton.Height - 5);
            cancelButton.Text = "cancel";
            cancelButton.BackColor = Color.White;
            Controls.Add(cancelButton);
            cancelButton.Click += Cancel;

            signUpButton = new Button();
            signUpButton.Size = new Size(90, 25);
            signUpButton.Location = new Point(workingWidth - 2 * signUpButton.Width - 5, workingHeight - signUpButton.Height - 5);
            signUpButton.Text = "create account";
            signUpButton.BackColor = Color.White;
            Controls.Add(signUpButton);
            signUpButton.Click += SignUp;

            BackColor = Color.Black;

            label = new Label();
            label.Text = "";
            label.ForeColor = Color.White;
            label.Location = new Point(10, 10);
            Controls.Add(label);
        }
        private void LoginClick(object sender, EventArgs e)
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM Players WHERE Username=@username AND Password=@password", sqlConnection);
                
                SqlParameter paramUser = new SqlParameter();
                paramUser.ParameterName = "@username";
                paramUser.Value = usernameInput.Text.Trim();
                cmd.Parameters.Add(paramUser);

                SqlParameter paramPasswd = new SqlParameter();
                paramPasswd.ParameterName = "@password";
                //paramPasswd.Value = Hash(passwordInput.Text);
                paramPasswd.Value = passwordInput.Text.Trim();
                cmd.Parameters.Add(paramPasswd);
                sqlConnection.Open();
                int b = (int) cmd.ExecuteScalar();
                if (b == 1)
                {
                    //log in, and assign username in form1
                    MainForm.game.username = usernameInput.Text;
                    label.Text = "Login successful";
                    Close();
                    MainForm.stats.Text = $"hey, {usernameInput.Text}!";
                    SqlCommand cmd1 = new SqlCommand($"SELECT Points from Players where Username='{usernameInput.Text}'", sqlConnection);
                    using (SqlDataReader reader = cmd1.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MainForm.stats.Text += $"\ntotal points: {reader["Points"].ToString()}";
                            MainForm.totalPoints = int.Parse(reader["Points"].ToString());
                        }
                    }
                    MainForm.loginButton.Text = "log out";
                }
                else
                {
                    label.Text = "Incorrect username or password";
                    passwordInput.Text = "";
                }
                sqlConnection.Close();
            }
        }
        private void Cancel(object sender, EventArgs e) => Close();
        private void SignUp(object sender, EventArgs e)
        {
            Close();
            CreateAccountForm createAccountForm = new CreateAccountForm();
            createAccountForm.Show();
            createAccountForm.BringToFront();
            
        }
        public static string Hash(string password)
        {
            //probably gonna use md5 for hashing
            StringBuilder sb = new StringBuilder();
            

            
            return sb.ToString();
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
