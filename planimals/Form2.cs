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

        private TextBox username;
        private TextBox password;

        public Form2()
        {
            InitializeComponent();

            workingHeight = Form1.workingHeight / 5;
            workingWidth = Form1.workingWidth / 8;
            
            username = new TextBox();
            username.Location = new Point(workingWidth / 20, workingHeight / 4 + 5);
            
            password = new TextBox();
            password.Location = new Point(workingWidth / 20, workingHeight / 4 + 15);
            //password.PasswordChar = '*';

            loginButton = new Button();
            loginButton.Size = new Size(20, 5);
            loginButton.Location = new Point(workingWidth - 2*loginButton.Width - 2, workingHeight - 2);
            loginButton.Text = "log in";
            Controls.Add(loginButton);
            loginButton.Click += Login;

            cancelButton = new Button();
            cancelButton.Size = new Size(20, 5);
            cancelButton.Location = new Point(workingWidth - cancelButton.Width - 2, workingHeight -2);
            cancelButton.Text = "cancel";
            Controls.Add(cancelButton);
            cancelButton.Click += Cancel;

            signUpButton = new Button();
            signUpButton.Size = new Size(30, 5);
            signUpButton.Location = new Point(5, workingHeight - signUpButton.Height - 5);
            signUpButton.Text = "create account";
            Controls.Add(signUpButton);
            signUpButton.Click += SignUp;

            BackColor = Color.Black;
        }

        private void Login(object sender, EventArgs e)
        {
            //login logic
            //pass username to form1 at the end



        }
        private void Cancel(object sender, EventArgs e) 
        { 
            this.Close();
        }

        private void SignUp(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
