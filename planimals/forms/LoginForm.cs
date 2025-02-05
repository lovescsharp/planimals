using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace planimals
{
    public partial class LoginForm : Form
    {
        MainForm form;

        private Button loginButton;
        private Button signUpButton;
        private Button cancelButton;
        private Button resestPasswdButton;

        private TextBox usernameInput;
        private TextBox passwordInput;

        private Label label;
        public LoginForm(MainForm f)
        {
            InitializeComponent();

            form = f;
            Name = "log in";
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            MinimizeBox = false;

            Width = 400;
            Height = 200;

            usernameInput = new TextBox
            {
                Location = new Point(ClientRectangle.Width / 20, ClientRectangle.Height / 4 + 5),
                Size = new Size(ClientRectangle.Width - 2 * (ClientRectangle.Width / 20), 25)
            };
            Label l1 = new Label
            {
                Location = new Point(usernameInput.Location.X, usernameInput.Location.Y - 15),
                Text = "username:",
            };
            Controls.Add(usernameInput);
            Controls.Add(l1);


            passwordInput = new TextBox
            {
                Location = new Point(ClientRectangle.Width / 20, ClientRectangle.Height / 4 + 50),
                Size = new Size(ClientRectangle.Width - 2 * (ClientRectangle.Width / 20), 25),
                PasswordChar = '*'
            };
            Label l2 = new Label
            {
                Location = new Point(passwordInput.Location.X, passwordInput.Location.Y - 15),
                Text = "password:",
            };
            Controls.Add(passwordInput);
            Controls.Add(l2);

            loginButton = new Button();
            loginButton.Size = new Size(60, 25);
            loginButton.Location = new Point(ClientRectangle.Width - loginButton.Width - 5, ClientRectangle.Height - loginButton.Height - 5);
            loginButton.Text = "log in";
            Controls.Add(loginButton);
            loginButton.Click += LoginClick;

            cancelButton = new Button();
            cancelButton.Size = new Size(60, 25);
            cancelButton.Location = new Point(5, ClientRectangle.Height - cancelButton.Height - 5);
            cancelButton.Text = "cancel";
            Controls.Add(cancelButton);
            cancelButton.Click += Cancel;

            signUpButton = new Button();
            signUpButton.Size = new Size(90, 25);
            signUpButton.Location = new Point(loginButton.Location.X - signUpButton.Width - 5, ClientRectangle.Height - signUpButton.Height - 5);
            signUpButton.Text = "create account";
            Controls.Add(signUpButton);
            signUpButton.Click += SignUp;

            resestPasswdButton = new Button();
            resestPasswdButton.Size = new Size (60, 25);
            resestPasswdButton.Location = new Point(signUpButton.Location.X - resestPasswdButton.Width - 5, ClientRectangle.Height - resestPasswdButton.Height - 5);
            resestPasswdButton.Text = "forgot password";
            Controls.Add(resestPasswdButton);
            resestPasswdButton.Click += ResestPasswdButton_Click;


            BackColor = Color.DarkSeaGreen;
            foreach (Control control in Controls) if (control is Label) control.ForeColor = Color.BlueViolet;
            label = new Label();
            label.Text = "";
            label.AutoSize = true;
            label.Location = new Point(10, 10);
            Controls.Add(label);
        }
        private void ResestPasswdButton_Click(object sender, EventArgs e)
        {
            ForgotPassword rp = new ForgotPassword();
            rp.ShowDialog();
        }
        private void LoginClick(object sender, EventArgs e)
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand cmd = new SqlCommand($"SELECT Count(*) FROM Players WHERE Username=@username AND Password=@password", sqlConnection);
                SqlParameter paramUser = new SqlParameter();
                paramUser.ParameterName = "@username";
                paramUser.Value = usernameInput.Text.Trim();
                cmd.Parameters.Add(paramUser);

                SqlParameter paramPasswd = new SqlParameter();
                paramPasswd.ParameterName = "@password";
                paramPasswd.Value = passwordInput.Text.Trim();
                cmd.Parameters.Add(paramPasswd);
                sqlConnection.Open();
                int b = (int) cmd.ExecuteScalar();
                if (b == 1)
                {
                    //log in, and assign username in form1
                    SqlCommand getPoints = new SqlCommand($"SELECT Points FROM Players WHERE Username='{usernameInput.Text.Trim()}'", sqlConnection);
                    int p = int.Parse(getPoints.ExecuteScalar().ToString());
                    form.username = usernameInput.Text.Trim();
                    form.totalPoints = p;
                    label.Text = "Login successful";
                    Close();
                    form.stats.Text = $"hey, {usernameInput.Text}!\npoints: {p}";
                    form.loginButton.Text = "log out";
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
            CreateAccountForm createAccountForm = new CreateAccountForm(form);
            createAccountForm.ShowDialog();
        }           
    }
}