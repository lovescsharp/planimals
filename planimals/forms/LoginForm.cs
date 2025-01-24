using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace planimals
{
    public partial class LoginForm : Form
    {
        MainForm form;

        private int workingWidth;
        private int workingHeight;

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
            signUpButton.Location = new Point(loginButton.Location.X - signUpButton.Width - 5, workingHeight - signUpButton.Height - 5);
            signUpButton.Text = "create account";
            signUpButton.BackColor = Color.White;
            Controls.Add(signUpButton);
            signUpButton.Click += SignUp;

            resestPasswdButton = new Button();
            resestPasswdButton.Size = new Size (60, 25);
            resestPasswdButton.Location = new Point(signUpButton.Location.X - resestPasswdButton.Width - 5, workingHeight - resestPasswdButton.Height - 5);
            resestPasswdButton.Text = "reset password";
            resestPasswdButton.BackColor = Color.White;
            Controls.Add(resestPasswdButton);
            resestPasswdButton.Click += ResestPasswdButton_Click;

            BackColor = Color.Black;

            label = new Label();
            label.Text = "";
            label.AutoSize = true;
            label.ForeColor = Color.White;
            label.Location = new Point(10, 10);
            Controls.Add(label);
        }

        private void ResestPasswdButton_Click(object sender, EventArgs e)
        {
            ResetPassword rp = new ResetPassword();
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
                    SqlCommand getPoints = new SqlCommand($"SELECT Points FROM Players", sqlConnection);
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
            Close();
            CreateAccountForm createAccountForm = new CreateAccountForm(form);
            createAccountForm.Show();
            createAccountForm.BringToFront();
            
        }           
    }
}