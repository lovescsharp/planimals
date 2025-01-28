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
            pictureEditInput.AllowDrop = true;

            consumedByInput.ColumnWidth = 120;
            consumesInput.ColumnWidth = 100;
            consumesEditInput.ColumnWidth = 100;
            consumedByEditInput.ColumnWidth = 100;

            getHabitatsAdd();
            getHabitatsEdit();
            getOrganismsAdd();
            getOrganismsEdit();
            
            BackColor = Color.DarkSeaGreen;
            foreach (Control control in Controls) if (control is Label) control.ForeColor = Color.BlueViolet;
            commonNameInput.TabIndex = 0;
        }
        void getOrganismsAdd() //adding organisms to check box list on Add Tab
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand getOrganisms = new SqlCommand("SELECT Scientific_name, Common_name FROM Organisms", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader r = getOrganisms.ExecuteReader())
                {
                    while (r.Read())
                    {
                        /*
                        consumesInput.Items.Add(r["Common_name"].ToString() + ' ' + '(' + r["Scientific_name"].ToString() + ')');
                        consumedByInput.Items.Add(r["Common_name"].ToString() + ' ' + '(' + r["Scientific_name"].ToString() + ')');
                        */
                        consumesInput.Items.Add(r["Common_name"].ToString());
                        consumedByInput.Items.Add(r["Common_name"].ToString());
                    }
                }
                sqlConnection.Close();
            }
        }
        void getHabitatsAdd() 
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand getOrganisms = new SqlCommand("SELECT DISTINCT habitat FROM Organisms", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader r = getOrganisms.ExecuteReader())
                {
                    while (r.Read()) habitatInput.Items.Add(r["Habitat"].ToString());
                }
                sqlConnection.Close();
            }
        }
        void getOrganismsEdit()
        {
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand getOrganisms = new SqlCommand("SELECT Scientific_name, Common_name FROM Organisms", sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader r = getOrganisms.ExecuteReader())
                {
                    while (r.Read())
                    {
                        /*
                        consumesEditInput.Items.Add(r["Common_name"].ToString() + ' ' + '(' + r["Scientific_name"].ToString() + ')');
                        consumedByEditInput.Items.Add(r["Common_name"].ToString() + ' ' + '(' + r["Scientific_name"].ToString() + ')');
                        */
                        consumesEditInput.Items.Add(r["Common_name"].ToString());
                        consumedByEditInput.Items.Add(r["Common_name"].ToString());
                    }
                }
                sqlConnection.Close();
            }
        }
        void getHabitatsEdit()
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
        void pictureInput_DragDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null) 
            {
                string[] filename = data as string[];
                if (filename.Length > 0) pictureInput.Image = Image.FromFile(filename[0]);
            }
        }
        void pictureInput_DragEnter(object sender, DragEventArgs e) => e.Effect = DragDropEffects.Copy;
        void editButton_Click(object sender, System.EventArgs e)
        {

        }
        void searchEditButton_Click(object sender, System.EventArgs e)
        {
            string sci_name = normalizeScientificName(scientificNameEditInput.Text);
            if (consumedByEditInput.Items.Contains(sci_name))
            {
                label.Text = "Please type scientific name of an organism";
                return;
            }
            scientificNameEditInput.Text = sci_name;
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                sqlConnection.Open();
                SqlCommand getOrganismData = new SqlCommand($"SELECT Common_name, Habitat, Hierarchy, Description FROM Organisms WHERE Scientific_name='{sci_name}';", sqlConnection);
                using (SqlDataReader r = getOrganismData.ExecuteReader()) 
                {
                    while (r.Read())
                    {
                        commonNameEditInput.Text = r["Common_name"].ToString();
                        habitatEditInput.SelectedIndex = habitatEditInput.FindStringExact(r["Habitat"].ToString());
                        hierarchyEditInput.SelectedIndex = hierarchyEditInput.FindStringExact(r["Hierarchy"].ToString());
                        descriptionEditInput.Text = r["Description"].ToString();
                    }
                }
                SqlCommand getRelations = new SqlCommand($"SELECT " +
                    "(SELECT STRING_AGG(Organisms.Common_name, ',') " +
                        "FROM Relations " +
                        "JOIN Organisms ON Relations.Consumed = Organisms.Scientific_name " +
                        "WHERE Relations.Consumer = 'Turdus merula') AS Consumes, " +
                    "(SELECT STRING_AGG(Organisms.Common_name, ',') " +
                        "FROM Relations " +
                        "JOIN Organisms ON Relations.Consumer = Organisms.Scientific_name " +
                        "WHERE Relations.Consumed = 'Turdus merula') AS ConsumedBy;", sqlConnection);
                string consumes = "";
                string consumedBy = "";
                using (SqlDataReader r = getRelations.ExecuteReader())
                {
                    while (r.Read())
                    {
                        consumes = r["Consumes"].ToString();
                        consumedBy = r["ConsumedBy"].ToString();
                    }
                }
                //now we parse our strings
                string organismCommonNameBuff = "";
                for (int i = 0; i < consumes.Length; i++)
                {
                    if (consumes[i] == ',')
                    {
                        consumesEditInput.SetItemChecked(consumesEditInput.Items.IndexOf(organismCommonNameBuff), true); // i find organism with common name @organismCommonNameBuff and check it
                        organismCommonNameBuff = "";
                    }
                    else organismCommonNameBuff += consumes[i];
                }
                organismCommonNameBuff = "";
                for (int i = 0; i < consumedBy.Length; i++)
                {
                    if (consumes[i] == ',')
                    {
                        consumesEditInput.SetItemChecked(consumesEditInput.Items.IndexOf(organismCommonNameBuff), true);
                        organismCommonNameBuff = "";
                    }
                    else organismCommonNameBuff += consumes[i];
                }
            }
        }
        string normalizeScientificName(string s)
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
            } return nrms;
        }
        void addButton_Click(object sender, System.EventArgs e)
        {
            if (scientificNameInput.Text.Trim() == string.Empty) 
            {
                label.Text = "Please enter a valid binomial name";
                return;
            }
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                SqlCommand insert = new SqlCommand(
                    ""
                    ,sqlConnection);
            }
        }
    }
}
