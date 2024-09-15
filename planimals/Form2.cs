using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace planimals
{
    public partial class Form2 : Form
    {
        private int workingWidth;
        private int workingHeight;

        private Button loginButton;
        private Button signUpButton;
        private Button cancelButton;

        private TextBox usernameInput;
        private TextBox passwordInput;

        public Form2()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            MinimizeBox = false;

            Width = 400;
            Height = 200;

            workingHeight = ClientRectangle.Height;
            workingWidth = ClientRectangle.Width;   
            
            usernameInput = new TextBox();
            usernameInput.Location = new Point(workingWidth / 20, workingHeight / 4 + 5);
            usernameInput.Size = new Size(workingWidth - 2*(workingWidth / 20), 25);
            Label l1 = new Label();
            l1.Location = new Point(usernameInput.Location.X, usernameInput.Location.Y - 15);
            l1.Text = "username:";
            l1.ForeColor = Color.White;
            Controls.Add(usernameInput);
            Controls.Add(l1);


            passwordInput = new TextBox();
            passwordInput.Location = new Point(workingWidth / 20, workingHeight / 4 + 50);
            //passwordInput.passwordChar = '*';
            passwordInput.Size = new Size(workingWidth - 2*(workingWidth / 20), 25);
            Label l2 = new Label();
            l2.Location = new Point(passwordInput.Location.X, passwordInput.Location.Y - 15);
            l2.Text = "password:";
            l2.ForeColor = Color.White;
            Controls.Add(passwordInput);
            Controls.Add(l2);

            loginButton = new Button();
            loginButton.Size = new Size(60, 25);
            loginButton.Location = new Point(workingWidth - loginButton.Width - 5, workingHeight - loginButton.Height - 5);
            loginButton.Text = "log in";
            loginButton.BackColor = Color.White;
            Controls.Add(loginButton);
            loginButton.Click += Login;

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
        }
        private void Login(object sender, EventArgs e)
        {
            
        }
        private void Cancel(object sender, EventArgs e) { Close(); }
        private void SignUp(object sender, EventArgs e)
        {
            this.Close();
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }
    }
}
