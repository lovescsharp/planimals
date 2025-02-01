namespace planimals.Forms
{
    partial class Editor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addButton = new System.Windows.Forms.Button();
            this.scientificNameInput = new System.Windows.Forms.TextBox();
            this.scientificNameLabel = new System.Windows.Forms.Label();
            this.commonNameLabel = new System.Windows.Forms.Label();
            this.commonNameInput = new System.Windows.Forms.TextBox();
            this.hierarchyLabel = new System.Windows.Forms.Label();
            this.hierarchyInput = new System.Windows.Forms.ComboBox();
            this.habitatInput = new System.Windows.Forms.ComboBox();
            this.habitatLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.descriptionInput = new System.Windows.Forms.TextBox();
            this.pictureInput = new System.Windows.Forms.PictureBox();
            this.consumesLabel = new System.Windows.Forms.Label();
            this.consumedByLabel = new System.Windows.Forms.Label();
            this.consumedByInput = new System.Windows.Forms.CheckedListBox();
            this.consumesInput = new System.Windows.Forms.CheckedListBox();
            this.label = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.addTab = new System.Windows.Forms.TabPage();
            this.pictureLabel = new System.Windows.Forms.Label();
            this.editTab = new System.Windows.Forms.TabPage();
            this.searchEditButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.consumesEditInput = new System.Windows.Forms.CheckedListBox();
            this.consumedByEditInput = new System.Windows.Forms.CheckedListBox();
            this.consumedByEditLabel = new System.Windows.Forms.Label();
            this.consumesEditLabel = new System.Windows.Forms.Label();
            this.hierarchyEditLabel = new System.Windows.Forms.Label();
            this.descriptionEditInput = new System.Windows.Forms.TextBox();
            this.hierarchyEditInput = new System.Windows.Forms.ComboBox();
            this.descriptionEditLabel = new System.Windows.Forms.Label();
            this.habitatEditInput = new System.Windows.Forms.ComboBox();
            this.habitatEditLabel = new System.Windows.Forms.Label();
            this.scientificNameEditInput = new System.Windows.Forms.TextBox();
            this.commonNameEditLabel = new System.Windows.Forms.Label();
            this.commonNameEditInput = new System.Windows.Forms.TextBox();
            this.scientificNameEditLabel = new System.Windows.Forms.Label();
            this.pictureEditInput = new System.Windows.Forms.PictureBox();
            this.pictureEditLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureInput)).BeginInit();
            this.tabControl.SuspendLayout();
            this.addTab.SuspendLayout();
            this.editTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEditInput)).BeginInit();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(676, 838);
            this.addButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(89, 34);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // scientificNameInput
            // 
            this.scientificNameInput.Location = new System.Drawing.Point(9, 283);
            this.scientificNameInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scientificNameInput.Name = "scientificNameInput";
            this.scientificNameInput.Size = new System.Drawing.Size(756, 22);
            this.scientificNameInput.TabIndex = 1;
            // 
            // scientificNameLabel
            // 
            this.scientificNameLabel.AutoSize = true;
            this.scientificNameLabel.Location = new System.Drawing.Point(5, 263);
            this.scientificNameLabel.Name = "scientificNameLabel";
            this.scientificNameLabel.Size = new System.Drawing.Size(242, 16);
            this.scientificNameLabel.TabIndex = 2;
            this.scientificNameLabel.Text = "Binomial name (starts with capital letter):";
            // 
            // commonNameLabel
            // 
            this.commonNameLabel.AutoSize = true;
            this.commonNameLabel.Location = new System.Drawing.Point(5, 308);
            this.commonNameLabel.Name = "commonNameLabel";
            this.commonNameLabel.Size = new System.Drawing.Size(98, 16);
            this.commonNameLabel.TabIndex = 3;
            this.commonNameLabel.Text = "Common name";
            // 
            // commonNameInput
            // 
            this.commonNameInput.Location = new System.Drawing.Point(9, 326);
            this.commonNameInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.commonNameInput.Name = "commonNameInput";
            this.commonNameInput.Size = new System.Drawing.Size(758, 22);
            this.commonNameInput.TabIndex = 4;
            // 
            // hierarchyLabel
            // 
            this.hierarchyLabel.AutoSize = true;
            this.hierarchyLabel.Location = new System.Drawing.Point(5, 350);
            this.hierarchyLabel.Name = "hierarchyLabel";
            this.hierarchyLabel.Size = new System.Drawing.Size(65, 16);
            this.hierarchyLabel.TabIndex = 5;
            this.hierarchyLabel.Text = "Hierarchy";
            // 
            // hierarchyInput
            // 
            this.hierarchyInput.FormattingEnabled = true;
            this.hierarchyInput.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.hierarchyInput.Location = new System.Drawing.Point(9, 370);
            this.hierarchyInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.hierarchyInput.Name = "hierarchyInput";
            this.hierarchyInput.Size = new System.Drawing.Size(121, 24);
            this.hierarchyInput.TabIndex = 6;
            // 
            // habitatInput
            // 
            this.habitatInput.FormattingEnabled = true;
            this.habitatInput.Location = new System.Drawing.Point(146, 370);
            this.habitatInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.habitatInput.Name = "habitatInput";
            this.habitatInput.Size = new System.Drawing.Size(121, 24);
            this.habitatInput.TabIndex = 7;
            // 
            // habitatLabel
            // 
            this.habitatLabel.AutoSize = true;
            this.habitatLabel.Location = new System.Drawing.Point(141, 350);
            this.habitatLabel.Name = "habitatLabel";
            this.habitatLabel.Size = new System.Drawing.Size(50, 16);
            this.habitatLabel.TabIndex = 8;
            this.habitatLabel.Text = "Habitat";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(5, 395);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(75, 16);
            this.descriptionLabel.TabIndex = 9;
            this.descriptionLabel.Text = "Description";
            // 
            // descriptionInput
            // 
            this.descriptionInput.Location = new System.Drawing.Point(9, 414);
            this.descriptionInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.descriptionInput.Multiline = true;
            this.descriptionInput.Name = "descriptionInput";
            this.descriptionInput.Size = new System.Drawing.Size(758, 59);
            this.descriptionInput.TabIndex = 10;
            // 
            // pictureInput
            // 
            this.pictureInput.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.pictureInput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureInput.Location = new System.Drawing.Point(5, 32);
            this.pictureInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureInput.Name = "pictureInput";
            this.pictureInput.Size = new System.Drawing.Size(760, 230);
            this.pictureInput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureInput.TabIndex = 11;
            this.pictureInput.TabStop = false;
            this.pictureInput.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureInput_DragDrop);
            this.pictureInput.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureInput_DragEnter);
            // 
            // consumesLabel
            // 
            this.consumesLabel.AutoSize = true;
            this.consumesLabel.Location = new System.Drawing.Point(7, 484);
            this.consumesLabel.Name = "consumesLabel";
            this.consumesLabel.Size = new System.Drawing.Size(74, 16);
            this.consumesLabel.TabIndex = 12;
            this.consumesLabel.Text = "Consumes:";
            // 
            // consumedByLabel
            // 
            this.consumedByLabel.AutoSize = true;
            this.consumedByLabel.Location = new System.Drawing.Point(7, 662);
            this.consumedByLabel.Name = "consumedByLabel";
            this.consumedByLabel.Size = new System.Drawing.Size(93, 16);
            this.consumedByLabel.TabIndex = 13;
            this.consumedByLabel.Text = "Consumed by:";
            // 
            // consumedByInput
            // 
            this.consumedByInput.FormattingEnabled = true;
            this.consumedByInput.Location = new System.Drawing.Point(9, 681);
            this.consumedByInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.consumedByInput.MultiColumn = true;
            this.consumedByInput.Name = "consumedByInput";
            this.consumedByInput.Size = new System.Drawing.Size(756, 140);
            this.consumedByInput.TabIndex = 15;
            // 
            // consumesInput
            // 
            this.consumesInput.CheckOnClick = true;
            this.consumesInput.FormattingEnabled = true;
            this.consumesInput.Location = new System.Drawing.Point(9, 503);
            this.consumesInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.consumesInput.MultiColumn = true;
            this.consumesInput.Name = "consumesInput";
            this.consumesInput.Size = new System.Drawing.Size(756, 140);
            this.consumesInput.TabIndex = 16;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(119, 7);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(0, 16);
            this.label.TabIndex = 17;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.addTab);
            this.tabControl.Controls.Add(this.editTab);
            this.tabControl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tabControl.Location = new System.Drawing.Point(11, 10);
            this.tabControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(781, 902);
            this.tabControl.TabIndex = 18;
            // 
            // addTab
            // 
            this.addTab.Controls.Add(this.pictureLabel);
            this.addTab.Controls.Add(this.addButton);
            this.addTab.Controls.Add(this.consumesInput);
            this.addTab.Controls.Add(this.scientificNameInput);
            this.addTab.Controls.Add(this.consumedByInput);
            this.addTab.Controls.Add(this.scientificNameLabel);
            this.addTab.Controls.Add(this.consumedByLabel);
            this.addTab.Controls.Add(this.commonNameLabel);
            this.addTab.Controls.Add(this.consumesLabel);
            this.addTab.Controls.Add(this.commonNameInput);
            this.addTab.Controls.Add(this.pictureInput);
            this.addTab.Controls.Add(this.hierarchyLabel);
            this.addTab.Controls.Add(this.descriptionInput);
            this.addTab.Controls.Add(this.hierarchyInput);
            this.addTab.Controls.Add(this.descriptionLabel);
            this.addTab.Controls.Add(this.habitatInput);
            this.addTab.Controls.Add(this.habitatLabel);
            this.addTab.Location = new System.Drawing.Point(4, 25);
            this.addTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.addTab.Name = "addTab";
            this.addTab.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.addTab.Size = new System.Drawing.Size(773, 873);
            this.addTab.TabIndex = 1;
            this.addTab.Text = "Add";
            this.addTab.UseVisualStyleBackColor = true;
            // 
            // pictureLabel
            // 
            this.pictureLabel.AutoSize = true;
            this.pictureLabel.Location = new System.Drawing.Point(7, 6);
            this.pictureLabel.Name = "pictureLabel";
            this.pictureLabel.Size = new System.Drawing.Size(440, 16);
            this.pictureLabel.TabIndex = 17;
            this.pictureLabel.Text = "Organism\'s photograph (drag and drop the photograph to yellow section):";
            // 
            // editTab
            // 
            this.editTab.Controls.Add(this.searchEditButton);
            this.editTab.Controls.Add(this.editButton);
            this.editTab.Controls.Add(this.consumesEditInput);
            this.editTab.Controls.Add(this.consumedByEditInput);
            this.editTab.Controls.Add(this.consumedByEditLabel);
            this.editTab.Controls.Add(this.consumesEditLabel);
            this.editTab.Controls.Add(this.hierarchyEditLabel);
            this.editTab.Controls.Add(this.descriptionEditInput);
            this.editTab.Controls.Add(this.hierarchyEditInput);
            this.editTab.Controls.Add(this.descriptionEditLabel);
            this.editTab.Controls.Add(this.habitatEditInput);
            this.editTab.Controls.Add(this.habitatEditLabel);
            this.editTab.Controls.Add(this.scientificNameEditInput);
            this.editTab.Controls.Add(this.commonNameEditLabel);
            this.editTab.Controls.Add(this.commonNameEditInput);
            this.editTab.Controls.Add(this.scientificNameEditLabel);
            this.editTab.Controls.Add(this.pictureEditInput);
            this.editTab.Controls.Add(this.pictureEditLabel);
            this.editTab.Location = new System.Drawing.Point(4, 25);
            this.editTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.editTab.Name = "editTab";
            this.editTab.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.editTab.Size = new System.Drawing.Size(773, 873);
            this.editTab.TabIndex = 0;
            this.editTab.Text = "Edit";
            this.editTab.UseVisualStyleBackColor = true;
            // 
            // searchEditButton
            // 
            this.searchEditButton.Location = new System.Drawing.Point(676, 277);
            this.searchEditButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.searchEditButton.Name = "searchEditButton";
            this.searchEditButton.Size = new System.Drawing.Size(89, 30);
            this.searchEditButton.TabIndex = 35;
            this.searchEditButton.Text = "Search";
            this.searchEditButton.UseVisualStyleBackColor = true;
            this.searchEditButton.Click += new System.EventHandler(this.searchEditButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(674, 839);
            this.editButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(89, 34);
            this.editButton.TabIndex = 24;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // consumesEditInput
            // 
            this.consumesEditInput.CheckOnClick = true;
            this.consumesEditInput.FormattingEnabled = true;
            this.consumesEditInput.Location = new System.Drawing.Point(9, 506);
            this.consumesEditInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.consumesEditInput.MultiColumn = true;
            this.consumesEditInput.Name = "consumesEditInput";
            this.consumesEditInput.Size = new System.Drawing.Size(760, 140);
            this.consumesEditInput.TabIndex = 34;
            // 
            // consumedByEditInput
            // 
            this.consumedByEditInput.FormattingEnabled = true;
            this.consumedByEditInput.Location = new System.Drawing.Point(9, 679);
            this.consumedByEditInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.consumedByEditInput.MultiColumn = true;
            this.consumedByEditInput.Name = "consumedByEditInput";
            this.consumedByEditInput.Size = new System.Drawing.Size(758, 140);
            this.consumedByEditInput.TabIndex = 33;
            // 
            // consumedByEditLabel
            // 
            this.consumedByEditLabel.AutoSize = true;
            this.consumedByEditLabel.Location = new System.Drawing.Point(9, 661);
            this.consumedByEditLabel.Name = "consumedByEditLabel";
            this.consumedByEditLabel.Size = new System.Drawing.Size(93, 16);
            this.consumedByEditLabel.TabIndex = 32;
            this.consumedByEditLabel.Text = "Consumed by:";
            // 
            // consumesEditLabel
            // 
            this.consumesEditLabel.AutoSize = true;
            this.consumesEditLabel.Location = new System.Drawing.Point(5, 486);
            this.consumesEditLabel.Name = "consumesEditLabel";
            this.consumesEditLabel.Size = new System.Drawing.Size(74, 16);
            this.consumesEditLabel.TabIndex = 31;
            this.consumesEditLabel.Text = "Consumes:";
            // 
            // hierarchyEditLabel
            // 
            this.hierarchyEditLabel.AutoSize = true;
            this.hierarchyEditLabel.Location = new System.Drawing.Point(5, 348);
            this.hierarchyEditLabel.Name = "hierarchyEditLabel";
            this.hierarchyEditLabel.Size = new System.Drawing.Size(65, 16);
            this.hierarchyEditLabel.TabIndex = 25;
            this.hierarchyEditLabel.Text = "Hierarchy";
            // 
            // descriptionEditInput
            // 
            this.descriptionEditInput.Location = new System.Drawing.Point(9, 418);
            this.descriptionEditInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.descriptionEditInput.Multiline = true;
            this.descriptionEditInput.Name = "descriptionEditInput";
            this.descriptionEditInput.Size = new System.Drawing.Size(760, 59);
            this.descriptionEditInput.TabIndex = 30;
            // 
            // hierarchyEditInput
            // 
            this.hierarchyEditInput.FormattingEnabled = true;
            this.hierarchyEditInput.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.hierarchyEditInput.Location = new System.Drawing.Point(9, 368);
            this.hierarchyEditInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.hierarchyEditInput.Name = "hierarchyEditInput";
            this.hierarchyEditInput.Size = new System.Drawing.Size(121, 24);
            this.hierarchyEditInput.TabIndex = 26;
            // 
            // descriptionEditLabel
            // 
            this.descriptionEditLabel.AutoSize = true;
            this.descriptionEditLabel.Location = new System.Drawing.Point(5, 398);
            this.descriptionEditLabel.Name = "descriptionEditLabel";
            this.descriptionEditLabel.Size = new System.Drawing.Size(75, 16);
            this.descriptionEditLabel.TabIndex = 29;
            this.descriptionEditLabel.Text = "Description";
            // 
            // habitatEditInput
            // 
            this.habitatEditInput.FormattingEnabled = true;
            this.habitatEditInput.Location = new System.Drawing.Point(142, 368);
            this.habitatEditInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.habitatEditInput.Name = "habitatEditInput";
            this.habitatEditInput.Size = new System.Drawing.Size(121, 24);
            this.habitatEditInput.TabIndex = 27;
            // 
            // habitatEditLabel
            // 
            this.habitatEditLabel.AutoSize = true;
            this.habitatEditLabel.Location = new System.Drawing.Point(139, 348);
            this.habitatEditLabel.Name = "habitatEditLabel";
            this.habitatEditLabel.Size = new System.Drawing.Size(50, 16);
            this.habitatEditLabel.TabIndex = 28;
            this.habitatEditLabel.Text = "Habitat";
            // 
            // scientificNameEditInput
            // 
            this.scientificNameEditInput.Location = new System.Drawing.Point(9, 282);
            this.scientificNameEditInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.scientificNameEditInput.Name = "scientificNameEditInput";
            this.scientificNameEditInput.Size = new System.Drawing.Size(660, 22);
            this.scientificNameEditInput.TabIndex = 21;
            // 
            // commonNameEditLabel
            // 
            this.commonNameEditLabel.AutoSize = true;
            this.commonNameEditLabel.Location = new System.Drawing.Point(9, 306);
            this.commonNameEditLabel.Name = "commonNameEditLabel";
            this.commonNameEditLabel.Size = new System.Drawing.Size(98, 16);
            this.commonNameEditLabel.TabIndex = 22;
            this.commonNameEditLabel.Text = "Common name";
            // 
            // commonNameEditInput
            // 
            this.commonNameEditInput.Location = new System.Drawing.Point(9, 325);
            this.commonNameEditInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.commonNameEditInput.Name = "commonNameEditInput";
            this.commonNameEditInput.Size = new System.Drawing.Size(754, 22);
            this.commonNameEditInput.TabIndex = 23;
            // 
            // scientificNameEditLabel
            // 
            this.scientificNameEditLabel.AutoSize = true;
            this.scientificNameEditLabel.Location = new System.Drawing.Point(9, 262);
            this.scientificNameEditLabel.Name = "scientificNameEditLabel";
            this.scientificNameEditLabel.Size = new System.Drawing.Size(99, 16);
            this.scientificNameEditLabel.TabIndex = 20;
            this.scientificNameEditLabel.Text = "Binomial name:";
            // 
            // pictureEditInput
            // 
            this.pictureEditInput.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.pictureEditInput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureEditInput.Location = new System.Drawing.Point(5, 22);
            this.pictureEditInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureEditInput.Name = "pictureEditInput";
            this.pictureEditInput.Size = new System.Drawing.Size(759, 238);
            this.pictureEditInput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureEditInput.TabIndex = 19;
            this.pictureEditInput.TabStop = false;
            this.pictureEditInput.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureEditInput_DragDrop);
            this.pictureEditInput.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureEditInput_DragEnter);
            // 
            // pictureEditLabel
            // 
            this.pictureEditLabel.AutoSize = true;
            this.pictureEditLabel.Location = new System.Drawing.Point(5, 2);
            this.pictureEditLabel.Name = "pictureEditLabel";
            this.pictureEditLabel.Size = new System.Drawing.Size(440, 16);
            this.pictureEditLabel.TabIndex = 18;
            this.pictureEditLabel.Text = "Organism\'s photograph (drag and drop the photograph to yellow section):";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(795, 916);
            this.Controls.Add(this.label);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Editor";
            this.Text = "Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pictureInput)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.addTab.ResumeLayout(false);
            this.addTab.PerformLayout();
            this.editTab.ResumeLayout(false);
            this.editTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEditInput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.TextBox scientificNameInput;
        private System.Windows.Forms.Label scientificNameLabel;
        private System.Windows.Forms.Label commonNameLabel;
        private System.Windows.Forms.TextBox commonNameInput;
        private System.Windows.Forms.Label hierarchyLabel;
        private System.Windows.Forms.ComboBox hierarchyInput;
        private System.Windows.Forms.ComboBox habitatInput;
        private System.Windows.Forms.Label habitatLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.TextBox descriptionInput;
        private System.Windows.Forms.PictureBox pictureInput;
        private System.Windows.Forms.Label consumesLabel;
        private System.Windows.Forms.Label consumedByLabel;
        private System.Windows.Forms.CheckedListBox consumedByInput;
        private System.Windows.Forms.CheckedListBox consumesInput;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage addTab;
        private System.Windows.Forms.TabPage editTab;
        private System.Windows.Forms.Label pictureLabel;
        private System.Windows.Forms.PictureBox pictureEditInput;
        private System.Windows.Forms.Label pictureEditLabel;
        private System.Windows.Forms.TextBox scientificNameEditInput;
        private System.Windows.Forms.Label commonNameEditLabel;
        private System.Windows.Forms.TextBox commonNameEditInput;
        private System.Windows.Forms.Label scientificNameEditLabel;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.CheckedListBox consumesEditInput;
        private System.Windows.Forms.CheckedListBox consumedByEditInput;
        private System.Windows.Forms.Label consumedByEditLabel;
        private System.Windows.Forms.Label consumesEditLabel;
        private System.Windows.Forms.Label hierarchyEditLabel;
        private System.Windows.Forms.TextBox descriptionEditInput;
        private System.Windows.Forms.ComboBox hierarchyEditInput;
        private System.Windows.Forms.Label descriptionEditLabel;
        private System.Windows.Forms.ComboBox habitatEditInput;
        private System.Windows.Forms.Label habitatEditLabel;
        private System.Windows.Forms.Button searchEditButton;
    }
}