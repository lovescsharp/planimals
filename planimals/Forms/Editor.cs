using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace planimals.Forms
{
    //drap and drop effect
    //https://www.youtube.com/watch?v=XWB7gbSQom4
    public partial class Editor : Form
    {
        public Editor()
        {
            InitializeComponent();

            pictureInput.AllowDrop = true;
            getHabitatsAdd();
            getOrganismsAdd();
            
            BackColor = Color.DarkSeaGreen;
            foreach (Control control in Controls) if (control is Label) control.ForeColor = Color.BlueViolet;
            commonNameInput.TabIndex = 0;
        }
        private void getOrganismsAdd() 
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand getOrganisms = new SqlCommand("SELECT Scientific_name FROM Organisms", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader r = getOrganisms.ExecuteReader())
                {
                    while (r.Read())
                    {
                        consumesInput.Items.Add(r["Scientific_name"].ToString());
                        consumedByInput.Items.Add(r["Scientific_name"].ToString());
                    }
                }
                sqlConnection.Close();
            }
        }
        private void getHabitatsAdd() 
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand getOrganisms = new SqlCommand("SELECT DISTINCT habitat FROM Organisms", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader r = getOrganisms.ExecuteReader())
                {
                    while (r.Read())
                    {
                        habitatInput.Items.Add(r["Habitat"].ToString());
                    }
                }
                sqlConnection.Close();
            }
        }
        private void getOrganismsEdit()
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand getOrganisms = new SqlCommand("SELECT Scientific_name FROM Organisms", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader r = getOrganisms.ExecuteReader())
                {
                    while (r.Read())
                    {
                        consumesEditInput.Items.Add(r["Scientific_name"].ToString());
                        consumedByEditInput.Items.Add(r["Scientific_name"].ToString());
                    }
                }
                sqlConnection.Close();
            }
        }
        private void getHabitatsEdit()
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand getOrganisms = new SqlCommand("SELECT DISTINCT habitat FROM Organisms", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader r = getOrganisms.ExecuteReader())
                {
                    while (r.Read())
                    {
                        habitatEditInput.Items.Add(r["Habitat"].ToString());
                    }
                }
                sqlConnection.Close();
            }
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            if (scientificNameInput.Text == string.Empty) 
            {
                label.Text = "binomial name must not be empty";

            }
        }
        private void pictureInput_DragDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null) 
            {
                string[] filename = data as string[];
                if (filename.Length > 0) pictureInput.Image = Image.FromFile(filename[0]);
            }
        }
        private void pictureInput_DragEnter(object sender, DragEventArgs e) => e.Effect = DragDropEffects.Copy;

        private void editTab_Click(object sender, System.EventArgs e)
        {
            getHabitatsEdit();
            getOrganismsEdit();
            
        }

        private void editButton_Click(object sender, System.EventArgs e)
        {

        }

        private void searchEditButton_Click(object sender, System.EventArgs e)
        {
            string sci_name = normalizeScientificName(scientificNameEditInput.Text);
            if (consumedByEditInput.Items.Contains(sci_name))
            {
                label.Text = "Please type scientific name of an organism in the field below.";
                return;
            }
            scientificNameEditInput.Text = sci_name;
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand getData = new SqlCommand("", sqlConnection);
            }
        }
        private string normalizeScientificName(string s)
        {
            string nrms = s[0].ToString().Trim().ToUpper();

            for (int i = 1; i < s.Length; i++)
            {
                if (s[i] == ' ') 
                {
                    nrms += ' ';
                    continue;
                }
                nrms += s[i].ToString().Trim().ToLower();
            }

            return nrms;
        }
    }
}
