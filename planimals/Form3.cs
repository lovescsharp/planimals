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
    public partial class Form3 : Form
    {
        private TextBox usernameInput;
        private TextBox emailInput;
        private TextBox passwordInput;

        private Button cancelButton;
        private Button createButton;

        private int workingWidth;
        private int workingHeight;

        public Form3()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            MinimizeBox = false;

            Width = 400;
            Height = 300;

            workingWidth = ClientRectangle.Width;
            workingHeight = ClientRectangle.Height;

            usernameInput = new TextBox();
            usernameInput.Location = new Point(workingWidth / 20, workingHeight / 4 + 5);
            usernameInput.Size = new Size(workingWidth - 2 * (workingWidth / 20), 25);
            Label l1 = new Label();
            l1.Location = new Point(usernameInput.Location.X, usernameInput.Location.Y - 15);
            l1.Text = "username:";
            l1.ForeColor = Color.White;
            Controls.Add(usernameInput);
            Controls.Add(l1);

            passwordInput = new TextBox();
            passwordInput.Location = new Point(workingWidth / 20, workingHeight / 4 + 50);
            //passwordInput.passwordChar = '*';
            passwordInput.Size = new Size(workingWidth - 2 * (workingWidth / 20), 25);
            Label l2 = new Label();
            l2.Location = new Point(passwordInput.Location.X, passwordInput.Location.Y - 15);
            l2.Text = "password:";
            l2.ForeColor = Color.White;
            Controls.Add(passwordInput);
            Controls.Add(l2);

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
        private void Cancel(object sender, EventArgs e) { Close(); }
        private void Create(object sender, EventArgs e) 
        {
            throw new NotImplementedException();
        }
    }
}
