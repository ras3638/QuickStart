namespace QuickStart
{
	partial class frmQuickStart
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
			this.components = new System.ComponentModel.Container();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.btnSetup = new System.Windows.Forms.Button();
			this.cmbGUI = new System.Windows.Forms.ComboBox();
			this.cmbProcess = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabControl2 = new System.Windows.Forms.TabControl();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.cmbIDInsert = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.btnGenerate_Insert = new System.Windows.Forms.Button();
			this.txtTableName_Insert = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.chkUseIncrements = new System.Windows.Forms.CheckBox();
			this.cmbSchema = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.txtFKName = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.cmbCascadeOption = new System.Windows.Forms.ComboBox();
			this.btnGenerate_CascadeDelete = new System.Windows.Forms.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.tabPage6 = new System.Windows.Forms.TabPage();
			this.cmbLoadDB = new System.Windows.Forms.ComboBox();
			this.label15 = new System.Windows.Forms.Label();
			this.btnSubmitSchema = new System.Windows.Forms.Button();
			this.btnGenerateSchemaScript = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.cmbLoadEnv = new System.Windows.Forms.ComboBox();
			this.rttInput = new System.Windows.Forms.RichTextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.pbWait = new System.Windows.Forms.PictureBox();
			this.btnSearch = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.txtSearchString = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cmbFilePattern = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cmbVersion = new System.Windows.Forms.ComboBox();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
			this.errorProvider3 = new System.Windows.Forms.ErrorProvider(this.components);
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.TSlblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.TSlblTime = new System.Windows.Forms.ToolStripStatusLabel();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabControl2.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage6.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbWait)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider3)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(1, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(657, 493);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage1.Controls.Add(this.btnSetup);
			this.tabPage1.Controls.Add(this.cmbGUI);
			this.tabPage1.Controls.Add(this.cmbProcess);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(649, 464);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Devl";
			// 
			// btnSetup
			// 
			this.btnSetup.Location = new System.Drawing.Point(275, 130);
			this.btnSetup.Name = "btnSetup";
			this.btnSetup.Size = new System.Drawing.Size(90, 27);
			this.btnSetup.TabIndex = 6;
			this.btnSetup.Text = "Setup";
			this.btnSetup.UseVisualStyleBackColor = true;
			this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
			// 
			// cmbGUI
			// 
			this.cmbGUI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbGUI.FormattingEnabled = true;
			this.cmbGUI.Location = new System.Drawing.Point(210, 57);
			this.cmbGUI.Name = "cmbGUI";
			this.cmbGUI.Size = new System.Drawing.Size(225, 24);
			this.cmbGUI.TabIndex = 5;
			// 
			// cmbProcess
			// 
			this.cmbProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbProcess.FormattingEnabled = true;
			this.cmbProcess.Items.AddRange(new object[] {
            "5.0",
            "8.0"});
			this.cmbProcess.Location = new System.Drawing.Point(210, 22);
			this.cmbProcess.Name = "cmbProcess";
			this.cmbProcess.Size = new System.Drawing.Size(225, 24);
			this.cmbProcess.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label4.Location = new System.Drawing.Point(34, 168);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(552, 2);
			this.label4.TabIndex = 3;
			this.label4.Text = "label4";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(30, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 24);
			this.label2.TabIndex = 1;
			this.label2.Text = "GUI";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(30, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "Process";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage2.Controls.Add(this.tabControl2);
			this.tabPage2.Controls.Add(this.rttInput);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(649, 464);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Script Gen";
			// 
			// tabControl2
			// 
			this.tabControl2.Controls.Add(this.tabPage4);
			this.tabControl2.Controls.Add(this.tabPage5);
			this.tabControl2.Controls.Add(this.tabPage6);
			this.tabControl2.Location = new System.Drawing.Point(7, 6);
			this.tabControl2.Name = "tabControl2";
			this.tabControl2.SelectedIndex = 0;
			this.tabControl2.Size = new System.Drawing.Size(624, 223);
			this.tabControl2.TabIndex = 17;
			// 
			// tabPage4
			// 
			this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage4.Controls.Add(this.cmbIDInsert);
			this.tabPage4.Controls.Add(this.label10);
			this.tabPage4.Controls.Add(this.btnGenerate_Insert);
			this.tabPage4.Controls.Add(this.txtTableName_Insert);
			this.tabPage4.Controls.Add(this.label6);
			this.tabPage4.Location = new System.Drawing.Point(4, 25);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(616, 194);
			this.tabPage4.TabIndex = 0;
			this.tabPage4.Text = "Insert Gen";
			// 
			// cmbIDInsert
			// 
			this.cmbIDInsert.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbIDInsert.FormattingEnabled = true;
			this.cmbIDInsert.Items.AddRange(new object[] {
            "",
            "ON"});
			this.cmbIDInsert.Location = new System.Drawing.Point(212, 54);
			this.cmbIDInsert.Name = "cmbIDInsert";
			this.cmbIDInsert.Size = new System.Drawing.Size(225, 24);
			this.cmbIDInsert.TabIndex = 21;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(32, 52);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(118, 24);
			this.label10.TabIndex = 20;
			this.label10.Text = "Identity Insert";
			// 
			// btnGenerate_Insert
			// 
			this.btnGenerate_Insert.Location = new System.Drawing.Point(275, 158);
			this.btnGenerate_Insert.Name = "btnGenerate_Insert";
			this.btnGenerate_Insert.Size = new System.Drawing.Size(96, 27);
			this.btnGenerate_Insert.TabIndex = 19;
			this.btnGenerate_Insert.Text = "Generate";
			this.btnGenerate_Insert.UseVisualStyleBackColor = true;
			this.btnGenerate_Insert.Click += new System.EventHandler(this.btnGenerate_Insert_Click);
			// 
			// txtTableName_Insert
			// 
			this.txtTableName_Insert.Location = new System.Drawing.Point(212, 21);
			this.txtTableName_Insert.Name = "txtTableName_Insert";
			this.txtTableName_Insert.Size = new System.Drawing.Size(225, 22);
			this.txtTableName_Insert.TabIndex = 18;
			this.txtTableName_Insert.TextChanged += new System.EventHandler(this.txtTableName_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(32, 19);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(114, 24);
			this.label6.TabIndex = 17;
			this.label6.Text = "Table Name";
			// 
			// tabPage5
			// 
			this.tabPage5.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage5.Controls.Add(this.chkUseIncrements);
			this.tabPage5.Controls.Add(this.cmbSchema);
			this.tabPage5.Controls.Add(this.label13);
			this.tabPage5.Controls.Add(this.txtFKName);
			this.tabPage5.Controls.Add(this.label12);
			this.tabPage5.Controls.Add(this.cmbCascadeOption);
			this.tabPage5.Controls.Add(this.btnGenerate_CascadeDelete);
			this.tabPage5.Controls.Add(this.label11);
			this.tabPage5.Location = new System.Drawing.Point(4, 25);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(616, 194);
			this.tabPage5.TabIndex = 1;
			this.tabPage5.Text = "Cascade Delete Gen";
			// 
			// chkUseIncrements
			// 
			this.chkUseIncrements.AutoSize = true;
			this.chkUseIncrements.Location = new System.Drawing.Point(36, 124);
			this.chkUseIncrements.Name = "chkUseIncrements";
			this.chkUseIncrements.Size = new System.Drawing.Size(167, 21);
			this.chkUseIncrements.TabIndex = 28;
			this.chkUseIncrements.Text = "Delete in Increments?";
			this.chkUseIncrements.UseVisualStyleBackColor = true;
			// 
			// cmbSchema
			// 
			this.cmbSchema.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbSchema.FormattingEnabled = true;
			this.cmbSchema.Items.AddRange(new object[] {
            "Single",
            "Full"});
			this.cmbSchema.Location = new System.Drawing.Point(212, 89);
			this.cmbSchema.Name = "cmbSchema";
			this.cmbSchema.Size = new System.Drawing.Size(225, 24);
			this.cmbSchema.TabIndex = 27;
			this.cmbSchema.SelectedIndexChanged += new System.EventHandler(this.cmbSchema_SelectedIndexChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label13.Location = new System.Drawing.Point(32, 87);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(80, 24);
			this.label13.TabIndex = 26;
			this.label13.Text = "Schema";
			// 
			// txtFKName
			// 
			this.txtFKName.Location = new System.Drawing.Point(212, 55);
			this.txtFKName.Name = "txtFKName";
			this.txtFKName.Size = new System.Drawing.Size(225, 22);
			this.txtFKName.TabIndex = 25;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(32, 53);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(90, 24);
			this.label12.TabIndex = 24;
			this.label12.Text = "FK Name";
			// 
			// cmbCascadeOption
			// 
			this.cmbCascadeOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbCascadeOption.FormattingEnabled = true;
			this.cmbCascadeOption.Items.AddRange(new object[] {
            "Single",
            "Full",
            "All"});
			this.cmbCascadeOption.Location = new System.Drawing.Point(212, 21);
			this.cmbCascadeOption.Name = "cmbCascadeOption";
			this.cmbCascadeOption.Size = new System.Drawing.Size(225, 24);
			this.cmbCascadeOption.TabIndex = 23;
			this.cmbCascadeOption.SelectedIndexChanged += new System.EventHandler(this.cmbCascadeOption_SelectedIndexChanged);
			// 
			// btnGenerate_CascadeDelete
			// 
			this.btnGenerate_CascadeDelete.Location = new System.Drawing.Point(275, 158);
			this.btnGenerate_CascadeDelete.Name = "btnGenerate_CascadeDelete";
			this.btnGenerate_CascadeDelete.Size = new System.Drawing.Size(96, 27);
			this.btnGenerate_CascadeDelete.TabIndex = 22;
			this.btnGenerate_CascadeDelete.Text = "Generate";
			this.btnGenerate_CascadeDelete.UseVisualStyleBackColor = true;
			this.btnGenerate_CascadeDelete.Click += new System.EventHandler(this.btnGenerate_CascadeDelete_Click);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(32, 19);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(145, 24);
			this.label11.TabIndex = 20;
			this.label11.Text = "Cascade Option";
			// 
			// tabPage6
			// 
			this.tabPage6.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage6.Controls.Add(this.cmbLoadDB);
			this.tabPage6.Controls.Add(this.label15);
			this.tabPage6.Controls.Add(this.btnSubmitSchema);
			this.tabPage6.Controls.Add(this.btnGenerateSchemaScript);
			this.tabPage6.Controls.Add(this.label14);
			this.tabPage6.Controls.Add(this.cmbLoadEnv);
			this.tabPage6.Location = new System.Drawing.Point(4, 25);
			this.tabPage6.Name = "tabPage6";
			this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage6.Size = new System.Drawing.Size(616, 194);
			this.tabPage6.TabIndex = 2;
			this.tabPage6.Text = "Load Schema";
			// 
			// cmbLoadDB
			// 
			this.cmbLoadDB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLoadDB.FormattingEnabled = true;
			this.cmbLoadDB.Items.AddRange(new object[] {
            "QRA",
            "QCA",
            "QCFS",
            "ESUITE",
            "QFC",
            "ESUITE QFC"});
			this.cmbLoadDB.Location = new System.Drawing.Point(212, 55);
			this.cmbLoadDB.Name = "cmbLoadDB";
			this.cmbLoadDB.Size = new System.Drawing.Size(225, 24);
			this.cmbLoadDB.TabIndex = 26;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(32, 53);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(88, 24);
			this.label15.TabIndex = 25;
			this.label15.Text = "Database";
			// 
			// btnSubmitSchema
			// 
			this.btnSubmitSchema.Location = new System.Drawing.Point(329, 147);
			this.btnSubmitSchema.Name = "btnSubmitSchema";
			this.btnSubmitSchema.Size = new System.Drawing.Size(96, 27);
			this.btnSubmitSchema.TabIndex = 24;
			this.btnSubmitSchema.Text = "Submit";
			this.btnSubmitSchema.UseVisualStyleBackColor = true;
			this.btnSubmitSchema.Click += new System.EventHandler(this.btnSubmitSchema_Click);
			// 
			// btnGenerateSchemaScript
			// 
			this.btnGenerateSchemaScript.Location = new System.Drawing.Point(212, 147);
			this.btnGenerateSchemaScript.Name = "btnGenerateSchemaScript";
			this.btnGenerateSchemaScript.Size = new System.Drawing.Size(96, 27);
			this.btnGenerateSchemaScript.TabIndex = 23;
			this.btnGenerateSchemaScript.Text = "Generate";
			this.btnGenerateSchemaScript.UseVisualStyleBackColor = true;
			this.btnGenerateSchemaScript.Click += new System.EventHandler(this.btnGenerateSchemaScript_Click);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(32, 19);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(117, 24);
			this.label14.TabIndex = 19;
			this.label14.Text = "Environment";
			// 
			// cmbLoadEnv
			// 
			this.cmbLoadEnv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLoadEnv.FormattingEnabled = true;
			this.cmbLoadEnv.Items.AddRange(new object[] {
            "5.0",
            "8.0"});
			this.cmbLoadEnv.Location = new System.Drawing.Point(212, 19);
			this.cmbLoadEnv.Name = "cmbLoadEnv";
			this.cmbLoadEnv.Size = new System.Drawing.Size(225, 24);
			this.cmbLoadEnv.TabIndex = 5;
			// 
			// rttInput
			// 
			this.rttInput.Location = new System.Drawing.Point(11, 235);
			this.rttInput.Name = "rttInput";
			this.rttInput.Size = new System.Drawing.Size(616, 201);
			this.rttInput.TabIndex = 14;
			this.rttInput.Text = "";
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage3.Controls.Add(this.pbWait);
			this.tabPage3.Controls.Add(this.btnSearch);
			this.tabPage3.Controls.Add(this.label9);
			this.tabPage3.Controls.Add(this.txtSearchString);
			this.tabPage3.Controls.Add(this.label8);
			this.tabPage3.Controls.Add(this.label5);
			this.tabPage3.Controls.Add(this.cmbFilePattern);
			this.tabPage3.Controls.Add(this.label3);
			this.tabPage3.Controls.Add(this.cmbVersion);
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(649, 464);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "QFC";
			// 
			// pbWait
			// 
			this.pbWait.Image = global::QuickStart.Properties.Resources.ajax_loader_1_;
			this.pbWait.Location = new System.Drawing.Point(298, 193);
			this.pbWait.Name = "pbWait";
			this.pbWait.Size = new System.Drawing.Size(100, 50);
			this.pbWait.TabIndex = 21;
			this.pbWait.TabStop = false;
			this.pbWait.Visible = false;
			// 
			// btnSearch
			// 
			this.btnSearch.Location = new System.Drawing.Point(275, 130);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(90, 27);
			this.btnSearch.TabIndex = 19;
			this.btnSearch.Text = "Search";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// label9
			// 
			this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label9.Location = new System.Drawing.Point(28, 174);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(552, 2);
			this.label9.TabIndex = 18;
			this.label9.Text = "label9";
			// 
			// txtSearchString
			// 
			this.txtSearchString.Location = new System.Drawing.Point(210, 87);
			this.txtSearchString.Name = "txtSearchString";
			this.txtSearchString.Size = new System.Drawing.Size(225, 22);
			this.txtSearchString.TabIndex = 17;
			this.txtSearchString.TextChanged += new System.EventHandler(this.txtSearchString_TextChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(30, 85);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(123, 24);
			this.label8.TabIndex = 16;
			this.label8.Text = "Search String";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(30, 54);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(109, 24);
			this.label5.TabIndex = 15;
			this.label5.Text = "File Pattern ";
			// 
			// cmbFilePattern
			// 
			this.cmbFilePattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbFilePattern.FormattingEnabled = true;
			this.cmbFilePattern.Items.AddRange(new object[] {
            "*",
            "*.cs",
            "*.cpp",
            "*.sql",
            "*.xml"});
			this.cmbFilePattern.Location = new System.Drawing.Point(210, 54);
			this.cmbFilePattern.Name = "cmbFilePattern";
			this.cmbFilePattern.Size = new System.Drawing.Size(225, 24);
			this.cmbFilePattern.TabIndex = 14;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(30, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 24);
			this.label3.TabIndex = 13;
			this.label3.Text = "Version";
			// 
			// cmbVersion
			// 
			this.cmbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbVersion.FormattingEnabled = true;
			this.cmbVersion.Items.AddRange(new object[] {
            "5.0",
            "8.0",
            "16.0"});
			this.cmbVersion.Location = new System.Drawing.Point(210, 22);
			this.cmbVersion.Name = "cmbVersion";
			this.cmbVersion.Size = new System.Drawing.Size(225, 24);
			this.cmbVersion.TabIndex = 12;
			// 
			// errorProvider1
			// 
			this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.AlwaysBlink;
			this.errorProvider1.ContainerControl = this;
			// 
			// errorProvider2
			// 
			this.errorProvider2.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.AlwaysBlink;
			this.errorProvider2.ContainerControl = this;
			// 
			// errorProvider3
			// 
			this.errorProvider3.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.AlwaysBlink;
			this.errorProvider3.ContainerControl = this;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSlblStatus,
            this.toolStripStatusLabel2,
            this.TSlblTime});
			this.statusStrip1.Location = new System.Drawing.Point(0, 487);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(656, 25);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// TSlblStatus
			// 
			this.TSlblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
			this.TSlblStatus.Name = "TSlblStatus";
			this.TSlblStatus.Size = new System.Drawing.Size(52, 20);
			this.TSlblStatus.Text = "Status:";
			// 
			// toolStripStatusLabel2
			// 
			this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			this.toolStripStatusLabel2.Size = new System.Drawing.Size(526, 20);
			this.toolStripStatusLabel2.Spring = true;
			// 
			// TSlblTime
			// 
			this.TSlblTime.Name = "TSlblTime";
			this.TSlblTime.Size = new System.Drawing.Size(63, 20);
			this.TSlblTime.Text = "00:00:00";
			// 
			// frmQuickStart
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(656, 512);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "frmQuickStart";
			this.Text = "QuickStart";
			this.Load += new System.EventHandler(this.frmQuickStart_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabControl2.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.tabPage6.ResumeLayout(false);
			this.tabPage6.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbWait)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider3)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnSetup;
		private System.Windows.Forms.ComboBox cmbGUI;
		private System.Windows.Forms.ComboBox cmbProcess;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtSearchString;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cmbFilePattern;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cmbVersion;
		private System.Windows.Forms.PictureBox pbWait;
		private System.Windows.Forms.RichTextBox rttInput;
		private System.Windows.Forms.TabControl tabControl2;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.ComboBox cmbIDInsert;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button btnGenerate_Insert;
		private System.Windows.Forms.TextBox txtTableName_Insert;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.ErrorProvider errorProvider2;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button btnGenerate_CascadeDelete;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtFKName;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox cmbCascadeOption;
		private System.Windows.Forms.TabPage tabPage6;
		private System.Windows.Forms.Button btnGenerateSchemaScript;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.ComboBox cmbLoadEnv;
		private System.Windows.Forms.Button btnSubmitSchema;
		private System.Windows.Forms.ComboBox cmbLoadDB;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.ComboBox cmbSchema;
		private System.Windows.Forms.ErrorProvider errorProvider3;
		private System.Windows.Forms.CheckBox chkUseIncrements;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel TSlblStatus;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
		private System.Windows.Forms.ToolStripStatusLabel TSlblTime;

	}
}

