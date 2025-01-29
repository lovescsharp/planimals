using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace planimals.Forms
{
    //drap and drop effect
    //https://www.youtube.com/watch?v=XWB7gbSQom4
    public partial class Editor : Form
    {
        List<string> existingConsumes;
        List<string> existingConsumedBy;
        
        string imagePathAddTab = "";
        string imagePathEditTab = "";

        public Editor()
        {
            InitializeComponent();

            existingConsumes = new List<string>();
            existingConsumedBy = new List<string>();

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
        void pictureInput_DragDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string[] filename = data as string[];
                if (filename.Length > 0) pictureInput.Image = Image.FromFile(imagePathAddTab = filename[0]);
            }
        }
        void pictureInput_DragEnter(object sender, DragEventArgs e) => e.Effect = DragDropEffects.Copy;

        void addButton_Click(object sender, EventArgs e)
        {
            if (scientificNameInput.Text.Trim() == string.Empty)
            {
                label.Text = "Please enter a valid binomial name";
                return;
            }
            if (pictureInput.Image == null) 
            {
                label.Text = "Please drop a photo of an organism into the yellow field bellow";
                return;
            }
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                string sci_name = normalizeScientificName(scientificNameInput.Text);
                File.Copy(imagePathAddTab, Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{sci_name}.png"));
                sqlConnection.Open();
                SqlCommand exists = new SqlCommand($"SELECT COUNT(*) FROM Organisms WHERE Scientific_name='{sci_name}';", sqlConnection);
                int b = (int)exists.ExecuteScalar();
                if (b == 1)
                {
                    label.Text = "organism with this binomial name already exists";
                    return;
                }
                SqlCommand insertOrganism = new SqlCommand($"INSERT INTO Organisms(Scientific_name, Common_name, Habitat, Hierarchy, Description) VALUES ('{sci_name}', '{commonNameInput.Text}', '{habitatInput.Text}', '{hierarchyInput.Text}', '{descriptionInput.Text}');", sqlConnection);
                SqlCommand insertRelations = new SqlCommand("",sqlConnection);
                foreach (object item in consumesInput.CheckedItems)
                    insertRelations.CommandText += $"INSERT INTO Relations(Consumer, Consumed) values ('{sci_name}', '{item.ToString()}');\n";
                foreach (object item in consumedByInput.CheckedItems)
                    insertRelations.CommandText += $"INSERT INTO Relations(Consumer, Consumed) values ('{item.ToString()}', '{sci_name}');\n";
                
                insertOrganism.ExecuteNonQuery();
                if (insertRelations.CommandText != string.Empty) insertRelations.ExecuteNonQuery();

                label.Text = $"Successfully added {sci_name}";

                sqlConnection.Close();
            }
        }
        void getOrganismsAdd() //adding organisms to check box list on Add Tab
        {
            consumesInput.Items.Clear();
            consumedByInput.Items.Clear();
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
                        consumesInput.Items.Add(r["Scientific_name"].ToString());
                        consumedByInput.Items.Add(r["Scientific_name"].ToString());
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

        void editButton_Click(object sender, System.EventArgs e)
        {
            if (scientificNameEditInput.Text.Trim() == string.Empty)
            {
                label.Text = "please enter a valid binomial name";
            }
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                string sci_name = normalizeScientificName(scientificNameEditInput.Text);
                File.Copy(imagePathEditTab, Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{sci_name}.png"));
                sqlConnection.Open();
                SqlCommand exists = new SqlCommand($"SELECT COUNT(*) FROM Organisms WHERE Scientific_name='{sci_name}';", sqlConnection);
                int b = (int)exists.ExecuteScalar();
                if (b == 0)
                {
                    label.Text = "No organism found";
                    return;
                }

                SqlCommand updateOrganism = new SqlCommand($"update Organisms\r\nset Scientific_name='{sci_name}', Common_name='{commonNameEditInput.Text}', Habitat='{habitatEditInput.Text}', Hierarchy='{hierarchyEditInput.Text}', Description='{descriptionEditInput.Text}'\r\nwhere Scientific_name='{sci_name}'", sqlConnection);

                SqlCommand insertRelations = new SqlCommand("", sqlConnection);
                foreach (object item in consumesEditInput.CheckedItems) 
                {
                    if (existingConsumes.Contains(item.ToString()) || item.ToString() == sci_name) continue;
                    insertRelations.CommandText += $"INSERT INTO Relations(Consumer, Consumed) values ('{sci_name}', '{item.ToString()}');\n";
                    existingConsumes.Add(item.ToString());
                }

                foreach (object item in consumedByEditInput.CheckedItems)
                { 
                    if (existingConsumedBy.Contains(item.ToString()) || item.ToString() == sci_name) continue;
                    insertRelations.CommandText += $"INSERT INTO Relations(Consumer, Consumed) values ('{item.ToString()}', '{sci_name}');\n";
                    existingConsumedBy.Add(item.ToString());
                }

                updateOrganism.ExecuteNonQuery();
                if (insertRelations.CommandText != string    .    Empty) insertRelations.ExecuteNonQuery();

                label.Text = $"Successfully edited {sci_name}";
                sqlConnection.Close();
            }
        }
        void searchEditButton_Click(object sender, System.EventArgs e)
        {
            string sci_name = normalizeScientificName(scientificNameEditInput.Text);
            scientificNameEditInput.Text = sci_name;
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                sqlConnection.Open();
                SqlCommand exists = new SqlCommand($"SELECT COUNT(*) FROM Organisms WHERE Scientific_name='{sci_name}';", sqlConnection);
                int b = (int) exists.ExecuteScalar();
                if (b == 0) 
                {
                    label.Text = "No organism found";
                    return;
                }
                getOrganismsEdit();
                existingConsumes.Clear();
                existingConsumedBy.Clear();
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
                    "(SELECT STRING_AGG(Organisms.Scientific_name, ',') " +
                        "FROM Relations " +
                        "JOIN Organisms ON Relations.Consumed = Organisms.Scientific_name " +
                        $"WHERE Relations.Consumer = '{sci_name}') AS Consumes, " +
                    "(SELECT STRING_AGG(Organisms.Scientific_name, ',') " +
                        "FROM Relations " +
                        "JOIN Organisms ON Relations.Consumer = Organisms.Scientific_name " +
                        $"WHERE Relations.Consumed = '{sci_name}') AS ConsumedBy;", sqlConnection);
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
                consumes += ',';
                consumedBy += ',';
                string organismCommonNameBuff = "";
                for (int i = 0; i < consumes.Length; i++)
                {
                    if (consumes[i] == ',')
                    {
                        try
                        {
                            consumesEditInput.SetItemChecked(consumesEditInput.Items.IndexOf(organismCommonNameBuff), true); // i find organism which name is $organismCommonNameBuff and check it
                        }
                        catch 
                        {
                            break;
                        }
                        existingConsumes.Add(organismCommonNameBuff);
                        organismCommonNameBuff = "";
                    }
                    else organismCommonNameBuff += consumes[i];
                }
                organismCommonNameBuff = "";
                for (int i = 0; i < consumedBy.Length; i++)
                {
                    if (consumedBy[i] == ',')
                    {
                        try
                        {
                            consumedByEditInput.SetItemChecked(consumesEditInput.Items.IndexOf(organismCommonNameBuff), true);
                        }
                        catch 
                        {
                            break;
                        }
                        existingConsumedBy.Add(organismCommonNameBuff);
                        organismCommonNameBuff = "";
                    }
                    else organismCommonNameBuff += consumedBy[i];
                }
                try
                {
                    pictureEditInput.Image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{sci_name}.png"));
                }
                catch { }
                    
                sqlConnection.Close();
            }
        }
        string normalizeScientificName(string s)
        {
            if (s.Length == 0) return "";
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
        void getOrganismsEdit()
        {
            consumesEditInput.Items.Clear();
            consumedByEditInput.Items.Clear();
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
                        consumesEditInput.Items.Add(r["Scientific_name"].ToString());
                        consumedByEditInput.Items.Add(r["Scientific_name"].ToString());
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

        private void pictureEditInput_DragDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string[] filename = data as string[];
                if (filename.Length > 0) pictureEditInput.Image = Image.FromFile(imagePathEditTab = filename[0]);
            }
        }

        private void pictureEditInput_DragEnter(object sender, DragEventArgs e) => e.Effect = DragDropEffects.Copy;
    }
}
