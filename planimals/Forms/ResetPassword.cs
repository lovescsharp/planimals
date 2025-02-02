using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

public partial class ResetPassword : Form
{
    private int workingWidth;
    private int workingHeight;

    private TextBox usernameInput;
    private TextBox emailInput;

    private Label label;
    private Button seePasswd;

    public ResetPassword() 
    {

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
        };
        Controls.Add(usernameInput);
        Controls.Add(l1);


        emailInput = new TextBox
        {
            Location = new Point(workingWidth / 20, workingHeight / 4 + 50),
            Size = new Size(workingWidth - 2 * (workingWidth / 20), 25),
        };
        Label l2 = new Label
        {
            Location = new Point(emailInput.Location.X, emailInput.Location.Y - 15),
            Text = "email:",
        };
        Controls.Add(emailInput);
        Controls.Add(l2);

        seePasswd = new Button();
        seePasswd.Size = new Size(60, 25);
        seePasswd.Location = new Point(workingWidth - seePasswd.Width - 5, workingHeight - seePasswd.Height - 5);
        seePasswd.Text = "log in";
        Controls.Add(seePasswd);
        seePasswd.Click += seePasswdClick;

        BackColor = Color.Black;

        label = new Label();
        label.Text = "";
        label.AutoSize = true;
        label.Location = new Point(10, 10);
        Controls.Add(label);

        BackColor = Color.DarkSeaGreen;
        foreach (Control control in Controls) if (control is Label) control.ForeColor = Color.BlueViolet;
    }

    private void seePasswdClick(object sender, EventArgs e)
    {
        using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
        {
            SqlCommand exists = new SqlCommand($"SELECT Count(*) FROM Players WHERE Username=@username AND Email=@email", sqlConnection);

            SqlParameter paramUser = new SqlParameter();
            paramUser.ParameterName = "@username";
            paramUser.Value = usernameInput.Text.Trim();
            exists.Parameters.Add(paramUser);

            SqlParameter paramEmail = new SqlParameter();
            paramEmail.ParameterName = "@email";
            paramEmail.Value = emailInput.Text.Trim();
            exists.Parameters.Add(paramEmail);

            sqlConnection.Open();
            int b = (int)exists.ExecuteScalar();
            if (b == 1)
            {
                SqlCommand getPassword = new SqlCommand($"SELECT Password from Players WHERE Username='{usernameInput.Text.Trim()}' AND Email='{emailInput.Text.Trim()}'", sqlConnection);
                string passwd = getPassword.ExecuteScalar().ToString();
                MessageBox.Show($"your password is : {passwd}");
            }
            else label.Text = "no account was found";
            sqlConnection.Close();
        }
    }
}
