using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace planimals
{
    public partial class CreateAccountForm : Form
    {
        private MainForm form;

        private TextBox usernameInput;
        private TextBox emailInput;
        private TextBox passwordInput;

        private Label label;

        private Button cancelButton;
        private Button createButton;

        private int workingWidth;
        private int workingHeight;

        public CreateAccountForm(MainForm f)
        {
            InitializeComponent();

            form = f;

            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            MinimizeBox = false;

            Width = 400;
            Height = 300;

            workingWidth = ClientRectangle.Width;
            workingHeight = ClientRectangle.Height;

            label = new Label
            {
                Location = new Point(10, 10),
                ForeColor = Color.White,
            };
            Controls.Add(label);

            usernameInput = new TextBox
            {
                Location = new Point(workingWidth / 20, workingHeight / 4 + 5),
                Size = new Size(workingWidth - 2 * (workingWidth / 20), 25)
            };
            Label l1 = new Label
            {
                Size = new Size(100, 25),
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

            emailInput = new TextBox
            {
                Location = new Point(workingWidth / 20, workingHeight / 4 + 100),
                Size = new Size(workingWidth - 2 * (workingWidth / 20), 25)
            };
            Label l3 = new Label
            {
                Location = new Point(emailInput.Location.X, emailInput.Location.Y - 15),
                Text = "email:",
                ForeColor = Color.White
            };
            Controls.Add(emailInput);
            Controls.Add(l3);

            createButton = new Button();
            createButton.Size = new Size(90, 25);
            createButton.Location = new Point(workingWidth - createButton.Width - 5, workingHeight - createButton.Height - 5);
            createButton.Text = "create account";
            createButton.BackColor = Color.White;
            Controls.Add(createButton);
            createButton.Click += Create;

            cancelButton = new Button();
            cancelButton.Size = new Size(60, 25);
            cancelButton.Location = new Point(5, workingHeight - cancelButton.Height - 5);
            cancelButton.Text = "cancel";
            cancelButton.BackColor = Color.White;
            Controls.Add(cancelButton);
            cancelButton.Click += Cancel;

            BackColor = Color.Black;
        }
        private void Cancel(object sender, EventArgs e) => Close();
        private void Create(object sender, EventArgs e) 
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand userExists = new SqlCommand("SELECT COUNT(*) FROM Players WHERE Username=@username", sqlConnection);
                SqlParameter username = new SqlParameter();
                username.ParameterName = "username";
                username.Value = usernameInput.Text.Trim();
                userExists.Parameters.Add(username);

                sqlConnection.Open();
                int b = (int) userExists.ExecuteScalar();
                userExists.Parameters.Remove(username);
                MessageBox.Show(b.ToString());
                if (b == 0)
                {
                    SqlCommand createUser = new SqlCommand("INSERT INTO Players(Username, GamesPlayed, Email, Password, Points) VALUES (@username, 0, @email, @password, 0)", sqlConnection);

                    createUser.Parameters.Add(username);

                    SqlParameter email = new SqlParameter();
                    email.ParameterName = "email";
                    email.Value = emailInput.Text;
                    createUser.Parameters.Add(email);

                    SqlParameter password = new SqlParameter();
                    password.ParameterName = "password";
                    password.Value = LoginForm.Hash(passwordInput.Text);
                    createUser.Parameters.Add(password);

                    form.username = usernameInput.Text.Trim();
                    Close();
                }
                else
                {
                    label.Text = "username already exists";
                    usernameInput.Text = "";
                    passwordInput.Text = "";
                }
                sqlConnection.Close();
            }
        }
    }
}