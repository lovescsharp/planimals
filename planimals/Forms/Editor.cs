using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.IO;

namespace planimals.Forms
{
    //drap and drop effect
    //https://www.youtube.com/watch?v=XWB7gbSQom4

    public partial class Editor : Form
    {
        List<string> existingConsumes;
        List<string> existingConsumedBy;
 
        string imagePathAddTab;
        string imagePathEditTab;

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

            hierarchyInput.DropDownStyle = ComboBoxStyle.DropDownList;
            hierarchyEditInput.DropDownStyle = ComboBoxStyle.DropDownList;

            habitatInput.DropDownStyle = ComboBoxStyle.DropDown;
            habitatEditInput.DropDownStyle = ComboBoxStyle.DropDown;

            getHabitatsAdd();
            getHabitatsEdit();
            getOrganismsAdd();
            getOrganismsEdit();
            
            BackColor = Color.DarkSeaGreen;
            foreach (Control control in Controls) if (control is Label) control.ForeColor = Color.BlueViolet;
        }
        void pictureInput_DragDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string[] filename = data as string[];
                if (filename.Length > 0)
                {
                    try 
                    {
                        pictureInput.Image = Image.FromFile(imagePathAddTab = filename[0]);
                    }
                    catch
                    {
                        label.Text = "please drop an image file (.png, .jpg, .jpeg)";
                        return;
                    }
                }
            }
        }
        void pictureInput_DragEnter(object sender, DragEventArgs e) => e.Effect = DragDropEffects.Copy;
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
        bool IsValid(string s)
        {
            if (s == string.Empty) return false;
            foreach (Char c in s)
            {
                if (c == ' ') continue;
                else if (!Char.IsLetter(c)) return false;
            }
            return true;
        }
        bool isValidDescription(string s)
        {
            if (s == string.Empty) return false;
            foreach (Char c in s)
            {
                if (c == '.' || c == ',' || c == '-' || c == ' ') continue;
                else if (!Char.IsLetter(c)) return false;
            }
            return true;
        }
        bool isValidHabitat(string s)
        {
            if (s == string.Empty) return false;
            foreach (Char c in s) if (!Char.IsLetter(c)) return false;
            return true;
        }
        void addButton_Click(object sender, EventArgs e)
        {
            foreach (Control c in tabControl.SelectedTab.Controls)
            {
                if (c == descriptionEditInput || c == descriptionInput)
                    if (!isValidDescription(c.Text))
                    {
                        label.Text = $"Please enter a valid description";
                        return;
                    }
                    else continue;
                if (c is TextBox)
                    if (!IsValid(c.Text))
                    {
                        label.Text = $"Please enter valid data in {c.Name} field";
                        return;
                    }
                if (c is ComboBox)
                {
                    if (c == habitatInput)
                    {
                        if (!isValidHabitat(c.Text))
                        {
                            label.Text = $"Please enter a valid habitat";
                            return;
                        }
                    }
                    if (c.Text == string.Empty)
                    {
                        label.Text = $"Please select organisms hierarchal order in a food chain";
                        return;
                    }
                }
                if (c is PictureBox) 
                    if (((PictureBox)c).Image is null)
                    {
                        label.Text = "Please drag and drop an organism picture in the field below";
                        return;
                    }
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
                    label.Text = "Organism with this binomial name already exists.";
                    return;
                }
                SqlCommand insertOrganism = new SqlCommand($"INSERT INTO Organisms(Scientific_name, Common_name, Habitat, Hierarchy, Description) VALUES ('{sci_name}', '{commonNameInput.Text}', '{habitatInput.Text}', {hierarchyInput.Text}, '{descriptionInput.Text}');", sqlConnection);
                SqlCommand insertRelations = new SqlCommand("",sqlConnection);
                foreach (object item in consumesInput.CheckedItems)
                {
                    SqlCommand checkHabitat = new SqlCommand($"SELECT Habitat, Hierarchy FROM Organisms WHERE Scientific_name='{item.ToString()}';", sqlConnection);
                    string habitat = "";
                    int hierarchyConsumed = 1;
                    using (SqlDataReader r = checkHabitat.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            habitat = r["Habitat"].ToString();
                            hierarchyConsumed = int.Parse(r["Hierarchy"].ToString());
                        }
                    }
                    if (hierarchyConsumed <= int.Parse(hierarchyInput.SelectedItem.ToString()))
                    {
                        label.Text = $"{sci_name} cannot eat {item.ToString()} as {item.ToString()} has a higher/equal hierarchal order to {sci_name}";
                        return;
                    }
                    if (habitat == habitatInput.SelectedItem.ToString()) insertRelations.CommandText += $"INSERT INTO Relations(Consumer, Consumed) values ('{sci_name}', '{item.ToString()}');\n";
                    else
                    {
                        label.Text = $"{sci_name} and {item.ToString()} live in different habitats";
                        return;
                    } 
                }
                foreach (object item in consumedByInput.CheckedItems)
                {
                    SqlCommand checkHabitat = new SqlCommand($"SELECT Habitat, Hierarchy FROM Organisms WHERE Scientific_name='{item.ToString()}';", sqlConnection);
                    string habitat = "";
                    int hierarchyConsumer = 5;
                    using (SqlDataReader r = checkHabitat.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            habitat = r["Habitat"].ToString();
                            hierarchyConsumer = int.Parse(r["Hierarchy"].ToString());
                        }
                    }
                    if (hierarchyConsumer >= int.Parse(hierarchyInput.SelectedItem.ToString()))
                    {
                        label.Text = $"{item.ToString()} cannot consume {sci_name} as {item.ToString()} has a lower/equal hierarchal order than/to {sci_name}'s";
                        return;
                    }
                    if (habitat == habitatInput.SelectedItem.ToString()) 
                        insertRelations.CommandText += $"INSERT INTO Relations(Consumer, Consumed) values ('{item.ToString()}', '{sci_name}');\n";
                    else
                    {
                        label.Text = $"{sci_name} and {item.ToString()} live in different habitats";
                        return;
                    }
                }
                /*
                MessageBox.Show("Everything is ok"); sqlConnection.Close();
                MessageBox.Show($"insertRelations.CommandText = {insertRelations.CommandText}");
                MessageBox.Show($"insertOrganism.CommandText = {insertOrganism.CommandText}");
                return;
                */
                insertOrganism.ExecuteNonQuery();
                if (insertRelations.CommandText != string.Empty) insertRelations.ExecuteNonQuery();

                label.Text = $"Successfully added {sci_name}";

                sqlConnection.Close();
            }
        }
        void editButton_Click(object sender, EventArgs e)
        {
            foreach (Control c in tabControl.SelectedTab.Controls)
            {
                if (c == descriptionEditInput || c == descriptionInput)
                    if (!isValidDescription(c.Text))
                    {
                        label.Text = $"Please enter a valid description";
                        return;
                    }
                    else continue;
                if (c is TextBox)
                    if (!IsValid(c.Text))
                    {
                        label.Text = $"Please enter valid data in {c.Name} field";
                        return;
                    }

                if (c is ComboBox)
                {
                    if (c == habitatEditInput)
                    {
                        if (!isValidHabitat(c.Text))
                        {
                            label.Text = $"Please enter a valid habitat";
                            return;
                        }
                    }
                    if (c.Text == string.Empty)
                    {
                        label.Text = $"Please select organisms hierarchal order in a food chain";
                        return;
                    }
                }

                if (c is PictureBox) 
                    if (((PictureBox)c).Image is null)
                    {
                        label.Text = "Please drag and drop an organism picture in the field below";
                        return;
                    }
            }
            using (SqlConnection sqlConnection = new SqlConnection(MainForm.CONNECTION_STRING))
            {
                string sci_name = normalizeScientificName(scientificNameEditInput.Text);
                sqlConnection.Open();
                SqlCommand exists = new SqlCommand($"SELECT COUNT(*) FROM Organisms WHERE Scientific_name='{sci_name}';", sqlConnection);
                int b = (int)exists.ExecuteScalar();
                if (b == 0)
                {
                    label.Text = "No organism found";
                    return;
                }
                File.Copy(imagePathEditTab, Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{sci_name}.png"));
                SqlCommand updateOrganism = new SqlCommand($"update Organisms\r\nset Scientific_name='{sci_name}', Common_name='{commonNameEditInput.Text}', Habitat='{habitatEditInput.Text}', Hierarchy={hierarchyEditInput.Text}, Description='{descriptionEditInput.Text}'\r\nwhere Scientific_name='{sci_name}'", sqlConnection);
                SqlCommand insertRelations = new SqlCommand("", sqlConnection);
                SqlCommand checkHabitat;
                foreach (object item in consumesEditInput.CheckedItems) 
                {
                    if (existingConsumes.Contains(item.ToString()) || item.ToString() == sci_name) continue;
                    checkHabitat = new SqlCommand($"SELECT Habitat, Hierarchy FROM Organisms WHERE Scientific_name='{item.ToString()}';", sqlConnection);
                    string habitat = "";
                    int hierarchy = 1;
                    using (SqlDataReader r = checkHabitat.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            habitat = r["Habitat"].ToString();
                            hierarchy = int.Parse(r["Hierarchy"].ToString());
                        }
                    }
                    if (hierarchy <= int.Parse(hierarchyEditInput.SelectedItem.ToString()))
                    {
                        label.Text = $"{sci_name} does not eat {item.ToString()} as {item.ToString()} has a higher or equal hierarchal order";
                        return;
                    }
                    if (habitat == habitatEditInput.SelectedItem.ToString())
                        insertRelations.CommandText += $"INSERT INTO Relations(Consumer, Consumed) values ('{sci_name}', '{item.ToString()}');\n";
                    else
                    {
                        label.Text = $"{sci_name} and {item.ToString()} live in different habitats";
                        return;
                    } 
                    existingConsumes.Add(item.ToString());
                }
                foreach (object item in consumedByEditInput.CheckedItems)
                {
                    if (existingConsumedBy.Contains(item.ToString()) || item.ToString() == sci_name) continue;
                    checkHabitat = new SqlCommand($"SELECT Habitat, Hierarchy FROM Organisms WHERE Scientific_name='{item.ToString()}';", sqlConnection);
                    string habitat = "";
                    int hierarchy = 5;
                    using (SqlDataReader r = checkHabitat.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            habitat = r["Habitat"].ToString();
                            hierarchy = int.Parse(r["Hierarchy"].ToString());
                        }
                    }
                    if (hierarchy >= int.Parse(hierarchyEditInput.SelectedItem.ToString()))
                    {
                        label.Text = $"{item.ToString()} does not eat {sci_name} as {item.ToString()} has a lower/equal hierarchal order than/to {sci_name}'s";
                        return;
                    }
                    if (habitat == habitatEditInput.SelectedItem.ToString()) 
                        insertRelations.CommandText += $"INSERT INTO Relations(Consumer, Consumed) values ('{item.ToString()}', '{sci_name}');\n";
                    else
                    {
                        label.Text = $"{item.ToString()} does not eat {sci_name} as they live in different habitats";
                        return;
                    } 
                    existingConsumedBy.Add(item.ToString());
                }
                /*
                MessageBox.Show("Everything is ok"); sqlConnection.Close();
                MessageBox.Show($"insertRelations.CommandText = {insertRelations.CommandText}");
                MessageBox.Show($"updateOrganism.CommandText = {updateOrganism.CommandText}");
                return;
                */
                updateOrganism.ExecuteNonQuery();
                if (insertRelations.CommandText != string    .    Empty) insertRelations.ExecuteNonQuery();

                label.Text = $"Successfully edited {sci_name}";
                sqlConnection.Close();
            }
        }
        void searchEditButton_Click(object sender,EventArgs e)
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
                            consumesEditInput.SetItemChecked(consumesEditInput.Items.IndexOf(organismCommonNameBuff), true); //find organism which name is $organismCommonNameBuff and check it
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
                catch 
                {
                    label.Text = $"File {Path.Combine(Environment.CurrentDirectory, "assets", "photos", $"{sci_name}.png")} does not exist";
                }
                    
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