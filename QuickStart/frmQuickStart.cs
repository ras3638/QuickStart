using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace QuickStart
{


	public partial class frmQuickStart : Form
	{

		#region Class Variables

		Dictionary<string, string> ClientDictionary = new Dictionary<string, string>();
		List<string> Log = new List<string>();
		List<string> RTTLog = new List<string>();
		//static bool UseActualNulls = true;
		string sGlobalFile;
		bool bDirBreak;
		volatile bool bTimerThreadActive;
		volatile bool bTimerErrorIndicator;
		Thread TimerThread;
		Thread SearchStringThread;
		Thread GenInsertThread;
		Thread GenCascadeDeleteThread;
		
		#endregion

		#region Worker Thread Methods

		private void timerThreadActive(bool KeepActive)
		{
			bTimerThreadActive = KeepActive;

			//reset error indicator when restarting the timer
			if (KeepActive)
			{
				bTimerErrorIndicator = false;
			}
		}
		private void timerThreadErrorIndicator(bool ErrorInd)
		{
			bTimerErrorIndicator = ErrorInd;
		}
		private void TimerThreadProcSafe()
		{
			ThreadHelperUtility.SetText(this, statusStrip1, TSlblStatus, "Status: Executing...");
			ThreadHelperUtility.SetEnable(this, btnGenerate_Insert, false);
			ThreadHelperUtility.SetEnable(this, btnSearch, false);
			ThreadHelperUtility.SetEnable(this, btnGenerate_CascadeDelete, false);
			ThreadHelperUtility.SetEnable(this, btnGenerateSchemaScript, false);
			ThreadHelperUtility.SetEnable(this, btnSubmitSchema, false);

			Stopwatch sw = new Stopwatch();
			string elapsedTimeOld = String.Empty;
			sw.Start();
			while (bTimerThreadActive)
			{
				TimeSpan ts = sw.Elapsed;
				string elapsedTimeNew = String.Format("{0:00}:{1:00}:{2:00}",
					ts.Hours, ts.Minutes, ts.Seconds);
				if (elapsedTimeOld != elapsedTimeNew)
				{
					ThreadHelperUtility.SetText(this, statusStrip1, TSlblTime, elapsedTimeNew);
					elapsedTimeOld = elapsedTimeNew;
				}
			}
			ThreadHelperUtility.SetEnable(this, btnGenerate_Insert, true);
			ThreadHelperUtility.SetEnable(this, btnSearch, true);
			ThreadHelperUtility.SetEnable(this, btnGenerate_CascadeDelete, true);
			ThreadHelperUtility.SetEnable(this, btnGenerateSchemaScript, true);
			ThreadHelperUtility.SetEnable(this, btnSubmitSchema, true);

			if (bTimerErrorIndicator)
			{
				ThreadHelperUtility.SetText(this, statusStrip1, TSlblStatus, "Status: Error");
			}
			else
			{
				ThreadHelperUtility.SetText(this, statusStrip1, TSlblStatus, "Status: Completed Succesfully");
			}
		}
		private void SearchStringThreadProcSafe(string Version, string SearchString, string FilePattern)
		{
			ProcessStartInfo cmdStartInfo = new ProcessStartInfo();
			cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
			cmdStartInfo.RedirectStandardOutput = true;
			cmdStartInfo.RedirectStandardError = true;
			cmdStartInfo.RedirectStandardInput = true;
			cmdStartInfo.UseShellExecute = false;
			cmdStartInfo.CreateNoWindow = true;

			Process cmdProcess = new Process();
			cmdProcess.StartInfo = cmdStartInfo;
			cmdProcess.ErrorDataReceived += cmd_Error;
			cmdProcess.OutputDataReceived += cmd_DataReceived;
			cmdProcess.EnableRaisingEvents = true;
			cmdProcess.Start();
			cmdProcess.BeginOutputReadLine();
			cmdProcess.BeginErrorReadLine();

			try
			{
				if (Version == "5.0")
				{
					File.Delete("C:\\Devl\\Products\\Upstream\\5.0.00\\Dependencies\\Source\\QFC\\SearchResults.txt");
					cmdProcess.StandardInput.WriteLine("cd C:\\Devl\\Products\\Upstream\\5.0.00\\Dependencies\\Source\\QFC");
				}
				else if (Version == "8.0")
				{
					File.Delete("C:\\Devl\\Products\\Upstream\\8.0.00\\Dependencies\\Source\\QFC\\SearchResults.txt");
					cmdProcess.StandardInput.WriteLine("cd C:\\Devl\\Products\\Upstream\\8.0.00\\Dependencies\\Source\\QFC");
				}
				else if (Version == "16.0")
				{
					File.Delete("C:\\Devl\\Products\\Upstream\\Main\\Dependencies\\Source\\QFC\\SearchResults.txt");
					cmdProcess.StandardInput.WriteLine("cd C:\\Devl\\Products\\Upstream\\Main\\Dependencies\\Source\\QFC");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error");
				timerThreadErrorIndicator(true);
				timerThreadActive(false);		
				return;
			}

			try
			{
				string SearchCommand = "findstr /s /n /i /p /c:";
				string OutputFile = ">> SearchResults.txt";
				string sFull = string.Format("{0}\"{1}\" {2} {3}", SearchCommand, SearchString, FilePattern, OutputFile);
				cmdProcess.StandardInput.WriteLine(sFull);
				cmdProcess.StandardInput.WriteLine("start SearchResults.txt");
				cmdProcess.StandardInput.WriteLine("exit");                  //Execute exit.

				cmdProcess.WaitForExit();

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error");
				timerThreadErrorIndicator(true);
			}
			finally
			{
				timerThreadActive(false);
			}
		}
		private void GenInsertThreadProcSafe(string InputString, string IDInsert, string TableName)
		{
			try
			{
				StringBuilder sb = new StringBuilder();

				if (!InputString.Contains("\n"))
				{
					sb.Append("Improper input");
				}
				else
				{
					sb.Append(SqlGenEngine.InsertEngine(InputString, IDInsert, TableName));
				}

				CreateGeneratedScriptFile(sb);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error");
				timerThreadErrorIndicator(true);
			}
			finally
			{
				timerThreadActive(false);
			}
			
		}
		private void GenCascadeDeleteThreadProcSafe(string sFKName, bool bUseIncrements, string sInput, string sCascadeOption, string sSchema)
		{
			try
			{
				StreamReader streamReader = new StreamReader("C:\\Users\\robert_salinas\\Desktop\\Environment Definitions\\Generated Schemas\\" + GetSchemaFileName(sSchema));
				string line;
				DataTable dtDatabaseSchema = new DataTable("Database Schema");
				bool FirstIteration = true;
				StringBuilder sb = new StringBuilder();

				while ((line = streamReader.ReadLine()) != null)
				{
					if (FirstIteration) dtDatabaseSchema = DataTableHelperUtility.AddColumns(dtDatabaseSchema, line);
					if (!FirstIteration) dtDatabaseSchema = DataTableHelperUtility.AddRows(dtDatabaseSchema, line);
					FirstIteration = false;
				}
				if (sCascadeOption == "Single")
				{
					sb = SqlGenEngine.CascadeDeleteSingleEngine(dtDatabaseSchema, sFKName, bUseIncrements, sInput);
					sb.Append(SqlGenEngine.AppendOriginalDelStatement(sInput));
					sb.Insert(0, "--Foriegn Key Cascade: " + sFKName + "\n");
					sb = StringUtility.CleanSB(sb);
				}
				else if (sCascadeOption == "Full")
				{
					sb = SqlGenEngine.CascadeDeleteFullEngine(dtDatabaseSchema,sFKName, bUseIncrements, sInput);
					sb.Append(SqlGenEngine.AppendOriginalDelStatement(sInput));
					sb = StringUtility.CleanSB(sb);
				}
				else if (sCascadeOption == "All")
				{
					sb = SqlGenEngine.CascadeDeleteAllEngine(dtDatabaseSchema,sFKName, bUseIncrements, sInput);
					sb.Append(SqlGenEngine.AppendOriginalDelStatement(sInput));
					sb = StringUtility.CleanSB(sb);				
				}

				CreateGeneratedScriptFile(sb);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error");
				timerThreadErrorIndicator(true);
			}
			finally
			{
				timerThreadActive(false);
			}
		}
		#endregion
	
		#region Constructor
		public frmQuickStart()
		{
			InitializeComponent();

			cmbCascadeOption.SelectedIndex = 0;
			cmbVersion.SelectedIndex = 0;
			cmbFilePattern.SelectedIndex = 0;
			cmbLoadDB.SelectedIndex = 0;
			ClientDictionary.Add("", "");
			ClientDictionary.Add("CORE 5.0", "QBS");
			ClientDictionary.Add("CORE 8.0", "QBS");
			ClientDictionary.Add("CORE 16.0", "QBS");
			LoadSchemas();

			try
			{
				string[] files = Directory.GetDirectories("R:\\InternalClientInstallers\\Projects",
					"*.*", SearchOption.TopDirectoryOnly);

				// Display all the files.
				foreach (string file in files)
				{
					sGlobalFile = file;
					bDirBreak = true;
					int RecursionLevelMax= 2; //the max number of times DirSearch will recurse unto itself		
					DirSearch(file, RecursionLevelMax);
				}

				var dataSource = new List<Client>();
				var dataSource2 = new List<Client>();
				var dataSource3 = new List<Client>();

				foreach (var pair in ClientDictionary)
				{
					dataSource.Add(new Client() { CmbName = pair.Key + " - " + pair.Value, Name = pair.Key, Code = pair.Value });
					dataSource2.Add(new Client() { CmbName = pair.Key + " - " + pair.Value, Name = pair.Key, Code = pair.Value });
					if (pair.Key != "")
					{
						dataSource3.Add(new Client() { CmbName = pair.Key + " - " + pair.Value, Name = pair.Key, Code = pair.Value });
					}	
				}

				//Setup data binding
				this.cmbProcess.DataSource = dataSource;
				this.cmbProcess.DisplayMember = "CmbName";
				this.cmbProcess.ValueMember = null;//"Value";

				this.cmbGUI.DataSource = dataSource2;
				this.cmbGUI.DisplayMember = "CmbName";
				this.cmbGUI.ValueMember = null;//"Value";

				this.cmbLoadEnv.DataSource = dataSource3;
				this.cmbLoadEnv.DisplayMember = "CmbName";
				this.cmbLoadEnv.ValueMember = null;//"Value";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error");
				btnSetup.Enabled = false;
			}
		}
		#endregion

		private void LoadSchemas()
		{
			var dataSource4 = new List<Client>();
			Dictionary<string, string> ClientSchemaDictionary = new Dictionary<string, string>();

			string[] files = Directory.GetFiles("C:\\Users\\robert_salinas\\Desktop\\Environment Definitions\\Generated Schemas",
				"*.*", SearchOption.TopDirectoryOnly);
			
			foreach (string f in files)
			{
				//string FileDateCreated = File.GetCreationTime(f).ToString("MM/dd/yyyy");
				string FileDateCreated = File.GetLastWriteTime(f).ToString("MM/dd/yyyy");
				int index = f.LastIndexOf('\\') + 1;
				string SchemaFileName = f.Substring(index, f.Length - index);

				index = SchemaFileName.LastIndexOf('.');
				SchemaFileName = SchemaFileName.Substring(0, index) + " " + FileDateCreated;

				dataSource4.Add(new Client() { CmbName = SchemaFileName });
			}

			this.cmbSchema.DataSource = dataSource4;
			this.cmbSchema.DisplayMember = "CmbName";
			this.cmbSchema.ValueMember = null;//"Value";
		}
		private string GetSchemaFileName(string Schema)
		{
			string[] files = Directory.GetFiles("C:\\Users\\robert_salinas\\Desktop\\Environment Definitions\\Generated Schemas",
				"*.*", SearchOption.TopDirectoryOnly);

			foreach (string f in files)
			{
				int index = f.LastIndexOf('\\') + 1;
				string SchemaFileNameWithExt = f.Substring(index, f.Length - index);

				index = SchemaFileNameWithExt.LastIndexOf('.');
				string SchemaFileNameNoExt = SchemaFileNameWithExt.Substring(0, index);

				index = Schema.LastIndexOf(' ');
				string cmbSchemaFileName = Schema.Substring(0, index);

				if (SchemaFileNameNoExt == cmbSchemaFileName)
				{
					return SchemaFileNameWithExt;
				}
			}
			return String.Empty;
		}
		private void DirSearch(string sDir, int RecursionLevelMax, int RecursionLevelCurrent = 0)
		{
			string[] files = Directory.GetDirectories(sDir, "*.*", SearchOption.TopDirectoryOnly);

			foreach (string f in files)
			{
				if (!bDirBreak) return;
				int index = f.LastIndexOf('\\') + 1;

				if (
						(f.Contains("DEV") || f.Contains("REL") || f.Contains("TST") || f.Contains("CNV")) &&
						(!f.Substring(index, f.Length - index).Contains("2015")) &&
						(!f.Substring(index, f.Length - index).Contains("2014"))
					)
				{
					int lastindex = f.LastIndexOf('_');
					int finalindex = 3;

					if (lastindex != -1)
					{
						finalindex = lastindex - index;
					}

					string code = f.Substring(index, finalindex);
					//One final check if code is over 3 characters
					if (code.Length > 3 && code.Contains('_'))
					{
						code = code.Substring(0, 3);
					}
					
					ClientDictionary.Add(sGlobalFile.Substring(37), code);
					bDirBreak = false;
				}

				if (RecursionLevelCurrent >= RecursionLevelMax) 
				{
					bDirBreak = false;
				}
				DirSearch(f,RecursionLevelMax,RecursionLevelCurrent++);
			}	
		}
		private void btnSetup_Click(object sender, EventArgs e)
		{
			Log.Clear();
			Log.Add("Process Summary");
			QPECHandler((Client)cmbProcess.SelectedValue);
			QPECConfigHandler((Client)cmbProcess.SelectedValue);
			Log.Add(" ");
			Log.Add("GUI Summary");
			UpstreamMainAppHandler((Client)cmbGUI.SelectedValue);
			QEnvironmentDefinitionsHandler((Client)cmbGUI.SelectedValue);
			MessageBox.Show(String.Join("\n", Log), "Log");
		}
		private bool UpstreamMainAppHandler(Client Client)
		{
			if (!Client.IsNull())
			{
				try
				{
					if (Client.Name == "CORE 5.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core5UpstreamMainApp);
						File.WriteAllLines(BaseDirectories.Full5UpstreamMainApp, lines);
					}
					else if (Client.Name == "CORE 8.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core8UpstreamMainApp);
						File.WriteAllLines(BaseDirectories.Full8UpstreamMainApp, lines);
					}
					else if (Client.Name == "CORE 16.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core16UpstreamMainApp);
						File.WriteAllLines(BaseDirectories.Full16UpstreamMainApp, lines);
					}
					else
					{
						List<string> ClientDirectories = GetClientDirectories("UpstreamMainApp", Client);

						if (ClientDirectories.Count() == 0)
						{
							//MessageBox.Show("Could not find any templates in UpstreamMainApp directory");
							Log.Add("UpstreamMainApp: No change - Could not find any templates in directory");
							return false;
						}
						else if (ClientDirectories.Count() > 1)
						{
							//MessageBox.Show("Found more than 1 template for selected environment in UpstreamMainApp directory");
							Log.Add("UpstreamMainApp: No change - Found more than 1 template for selected environment in directory");
							return false;
						}
						else if (ClientDirectories.Count() == 1)
						{
							string[] lines = File.ReadAllLines(ClientDirectories[0]);
							File.WriteAllLines(GetBaseDirectoryWithVersion(ClientDirectories[0], "UpstreamMainApp"), lines);
						}
					}
					//MessageBox.Show("Successfully changed UpstreamMainApp.exe.config environment definition.");
					Log.Add("UpstreamMainApp: Changed succesfully");
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Error");
					return false;
				}
			}
			else return false;
		}
		private bool QPECHandler(Client Client)
		{
			if (!Client.IsNull())
			{
				try
				{
					if (Client.Name == "CORE 5.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core5QPEC);
						File.WriteAllLines(BaseDirectories.Full5QPEC, lines);
					}
					else if (Client.Name == "CORE 8.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core8QPEC);
						File.WriteAllLines(BaseDirectories.Full8QPEC, lines);
					}
					else if (Client.Name == "CORE 16.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core16QPEC);
						File.WriteAllLines(BaseDirectories.Full16QPEC, lines);
					}
					else
					{
						List<string> ClientDirectories = GetClientDirectories("QPEC", Client);

						if (ClientDirectories.Count() == 0)
						{
							//MessageBox.Show("Could not find any templates in QPEC directory");
							Log.Add("QPEC: No change - Could not find any templates in directory");
							return false;
						}
						else if (ClientDirectories.Count() > 1)
						{
							//MessageBox.Show("Found more than 1 template for selected environment in QPEC directory");
							Log.Add("QPEC: No change - Found more than 1 template for selected environment in directory");
							return false;
						}
						else if (ClientDirectories.Count() == 1)
						{
							string[] lines = File.ReadAllLines(ClientDirectories[0]);
							File.WriteAllLines(GetBaseDirectoryWithVersion(ClientDirectories[0], "QPEC"), lines);
						}

					}
					//MessageBox.Show("Successfully changed QPEC environment definition.");
					Log.Add("QPEC: Changed succesfully");
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Error");
					return false;
				}
			}
			else return false;
		}
		private bool QPECConfigHandler(Client Client)
		{
			if (!Client.IsNull())
			{
				try
				{
					if (Client.Name == "CORE 5.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core5QPECConfig);
						File.WriteAllLines(BaseDirectories.Full5QPECConfig, lines);
					}
					else if (Client.Name == "CORE 8.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core8QPECConfig);
						File.WriteAllLines(BaseDirectories.Full8QPECConfig, lines);
					}
					else if (Client.Name == "CORE 16.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core16QPECConfig);
						File.WriteAllLines(BaseDirectories.Full16QPECConfig, lines);
					}
					else
					{
						List<string> ClientDirectories = GetClientDirectories("QPEC", Client);

						if (ClientDirectories.Count() == 0)
						{
							//MessageBox.Show("Could not find any templates in QPEC Config directory");
							Log.Add("QPEC Config: No change - Could not find any templates in directory");
							return false;
						}
						else if (ClientDirectories.Count() > 1)
						{
							//MessageBox.Show("Found more than 1 template for selected environment in QPEC Config directory");
							Log.Add("QPEC Config: No change - Found more than 1 template for selected environment in directory");
							return false;
						}
						else if (ClientDirectories.Count() == 1)
						{
							string[] lines = File.ReadAllLines(ClientDirectories[0]);
							File.WriteAllLines(GetBaseDirectoryWithVersion(ClientDirectories[0], "QPEC Config"), lines);
						}

					}
					//MessageBox.Show("Successfully changed QPEC config environment definition.");
					Log.Add("QPEC Config: Changed succesfully");
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Error");
					return false;
				}
			}
			else return false;
		}
		private bool QEnvironmentDefinitionsHandler(Client Client)
		{
			if (!Client.IsNull())
			{
				try
				{
					if (Client.Name == "CORE 5.0")
					{
						string[] lines = File.ReadAllLines(EnvDefDirectories.Core5QEnvironmentDefinitions);
						File.WriteAllLines(BaseDirectories.Full5QEnvironmentDefinitions, lines);
					}
					else if (Client.Name == "CORE 8.0")
					{
						//Do nothing
						Log.Add("QEnvironmentDefinitions: No change - Could not find any templates in directory");
						return false;
					}
					else if (Client.Name == "CORE 16.0")
					{
						//Do nothing
						Log.Add("QEnvironmentDefinitions: No change - Could not find any templates in directory");
						return false;
					}
					else
					{
						List<string> ClientDirectories = GetClientDirectories("QEnvironmentDefinitions", Client);

						if (ClientDirectories.Count() == 0)
						{
							//MessageBox.Show("Could not find any templates in QEnvironmentDefinitions directory");
							Log.Add("QEnvironmentDefinitions: No change - Could not find any templates in directory");
							return false;
						}
						else if (ClientDirectories.Count() > 1)
						{
							//MessageBox.Show("Found more than 1 template for selected environment in QEnvironmentDefinitions directory");
							Log.Add("QEnvironmentDefinitions: No change - Found more than 1 template for selected environment in directory");
							return false;
						}
						else if (ClientDirectories.Count() == 1)
						{
							string[] lines = File.ReadAllLines(ClientDirectories[0]);
							File.WriteAllLines(GetBaseDirectoryWithVersion(ClientDirectories[0], "QEnvironmentDefinitions"), lines);
						}

					}
					//MessageBox.Show("Successfully changed QEnvironmentDefinitions environment definition.");
					Log.Add("QEnvironmentDefinitions: Changed succesfully");
					return true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString(), "Error");
					return false;
				}
			}
			else return false;
		}
		private string GetBaseDirectoryWithVersion(string sDir, string sFile)
		{
			int index = EnvDefDirectories.EnvDefBase.LastIndexOf('\\') + 1;
			string sBaseFolder = EnvDefDirectories.EnvDefBase.Substring(index, EnvDefDirectories.EnvDefBase.Length - index);

			index = sDir.IndexOf(sBaseFolder);
			string Directory = sDir.Substring(index, sDir.Length - index);

			index = (Directory.LastIndexOf('\\') + 1);
			Directory = Directory.Substring(0, index);

			if (Directory.Contains("5.0"))
			{
				if (sFile == "QPEC") return BaseDirectories.Full5QPEC;
				if (sFile == "UpstreamMainApp") return BaseDirectories.Full5UpstreamMainApp;
				if (sFile == "QPEC Config") return BaseDirectories.Full5QPECConfig;
				if (sFile == "QEnvironmentDefinitions") return BaseDirectories.Full5QEnvironmentDefinitions;

			}
			else if (Directory.Contains("8.0"))
			{
				if (sFile == "QPEC") return BaseDirectories.Full8QPEC;
				if (sFile == "UpstreamMainApp") return BaseDirectories.Full8UpstreamMainApp;
				if (sFile == "QPEC Config") return BaseDirectories.Full8QPECConfig;
			}
			else if (Directory.Contains("16.0"))
			{
				if (sFile == "QPEC") return BaseDirectories.Full16QPEC;
				if (sFile == "UpstreamMainApp") return BaseDirectories.Full16UpstreamMainApp;
				if (sFile == "QPECConfig") return BaseDirectories.Full16QPECConfig;
			}
			return null;
		}
		private List<string> GetClientDirectories(string Search, Client Client)
		{
			List<string> ClientDirectory = new List<string>();
			string[] files = Directory.GetDirectories(EnvDefDirectories.EnvDefBase,
				"*.*", SearchOption.AllDirectories);

			// Display all the files.
			foreach (string f in files)
			{
				int index = f.LastIndexOf('\\') + 1;
				string sBaseFolder = f.Substring(index, f.Length - index);

				if (sBaseFolder == Search)
				{
					string[] Subfiles = Directory.GetFiles(f, "*.*", SearchOption.TopDirectoryOnly);
					foreach (string s in Subfiles)
					{
						if (s.Contains(Client.Code))
						{
							ClientDirectory.Add(s);
						}
					}
				}
			}
			return ClientDirectory;
		}
		private void btnSearch_Click(object sender, EventArgs e)
		{
			if (txtSearchString.Text == string.Empty)
			{
				errorProvider2.SetError(txtSearchString, "Please Enter Search String");
				return;
			}
			else
			{
				errorProvider2.SetError(txtSearchString, "");
			}

			TimerThread = new Thread(new ThreadStart(this.TimerThreadProcSafe));
			timerThreadActive(true);
			TimerThread.Start();

			string Version = cmbVersion.Text;
			string SearchString = txtSearchString.Text;
			string FilePattern = cmbFilePattern.Text;
			SearchStringThread = new Thread(() => SearchStringThreadProcSafe(Version, SearchString, FilePattern));
			SearchStringThread.Start();



			pbWait.Visible = false;
			timerThreadActive(false); ;
		}
		private void cmd_DataReceived(object sender, DataReceivedEventArgs e)
		{
			Console.WriteLine("Output from other process");
			Console.WriteLine(e.Data);
			if(!String.IsNullOrWhiteSpace(e.Data))
			{
				RTTLog.Add(e.Data);
			}		
		}
		static void cmd_Error(object sender, DataReceivedEventArgs e)
		{
			Console.WriteLine("Error from other process");
			Console.WriteLine(e.Data);
			
		}
		private void btnGenerate_Insert_Click(object sender, EventArgs e)
		{
			string InputString = rttInput.Text;
			string IDInsert = cmbIDInsert.Text;
			string TableName = txtTableName_Insert.Text;

			if (string.IsNullOrWhiteSpace(TableName))
			{
				errorProvider1.SetError(txtTableName_Insert, "Please Enter Table Name");
				return;
			}
			else
			{
				errorProvider1.SetError(txtTableName_Insert, "");
			}

			long stringByteCount = System.Text.ASCIIEncoding.Unicode.GetByteCount(InputString);
			long stringByteThreshold = 28000000;

			if (stringByteCount > stringByteThreshold)
			{
				decimal overCapacity = stringByteCount - stringByteThreshold;
				overCapacity /= stringByteThreshold;
				overCapacity *= 100;

				if (Math.Round(overCapacity, 0) == 0)
				{
					overCapacity = 1;
				}
				else
				{
					overCapacity = Math.Round(overCapacity, 0);
				}

				if (MessageBox.Show("Dataset is large and may cause memory issues.\n" + overCapacity.ToString() + "% over recommended capacity.\nContinue?", 
					"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
			}

			TimerThread = new Thread(new ThreadStart(this.TimerThreadProcSafe));
			timerThreadActive(true);
			TimerThread.Start();

			GenInsertThread = new Thread(() => GenInsertThreadProcSafe(InputString, IDInsert, TableName));
			GenInsertThread.Start();
		}	
		private void txtTableName_TextChanged(object sender, EventArgs e)
		{
			errorProvider1.Clear();
		}
		private void txtSearchString_TextChanged(object sender, EventArgs e)
		{
			errorProvider2.Clear();
		}
		private void btnGenerateSchemaScript_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Sql.sLoadSchema);
			CreateGeneratedScriptFile(sb);
		}
		private void btnSubmitSchema_Click(object sender, EventArgs e)
		{
			try
			{
				Client SelectedClient = (Client)cmbLoadEnv.SelectedValue;
				string sSchemaFileName = SelectedClient.Name + "_" + SelectedClient.Code + "_" + cmbLoadDB.Text + ".txt";
				using (FileStream stream = File.Open("C:\\Users\\robert_salinas\\Desktop\\Environment Definitions\\Generated Schemas\\" + sSchemaFileName, FileMode.Create))
				{
				}
				using (var writer = new StreamWriter("C:\\Users\\robert_salinas\\Desktop\\Environment Definitions\\Generated Schemas\\" + sSchemaFileName))
				{
					writer.Write(rttInput.Text);
				}
				LoadSchemas();
				MessageBox.Show("Successfully uploaded schema","Success");
			}
			catch (Exception ex) 
			{
				MessageBox.Show(ex.ToString(),"Error");
			}
		}
		private void btnGenerate_CascadeDelete_Click(object sender, EventArgs e)
		{
			string sCascadeOption = cmbCascadeOption.Text;
			string sFKName = txtFKName.Text;
			bool bUseIncrements = chkUseIncrements.Checked;
			string sInput = rttInput.Text;
			string sSchema = cmbSchema.Text;

			if (sCascadeOption == "Single" || sCascadeOption == "Full" || sCascadeOption == "All")
			{
				//Error Handling
				bool bSuccess = true;

				if (string.IsNullOrWhiteSpace(sFKName))
				{
					errorProvider3.SetError(txtFKName, "Please Enter FK Name");
					bSuccess = false;
				}
				else
				{
					errorProvider3.SetError(txtFKName, "");
				}
				if (!bSuccess) return;
			}

			TimerThread = new Thread(new ThreadStart(this.TimerThreadProcSafe));
			timerThreadActive(true);
			TimerThread.Start();

			GenCascadeDeleteThread = new Thread(() => GenCascadeDeleteThreadProcSafe(sFKName, bUseIncrements, sInput, sCascadeOption, sSchema));
			GenCascadeDeleteThread.Start();	
		}
		private void CreateGeneratedScriptFile(StringBuilder sb)
		{
			ProcessStartInfo cmdStartInfo = new ProcessStartInfo();
			cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
			cmdStartInfo.RedirectStandardOutput = true;
			cmdStartInfo.RedirectStandardError = true;
			cmdStartInfo.RedirectStandardInput = true;
			cmdStartInfo.UseShellExecute = false;
			cmdStartInfo.CreateNoWindow = true;

			Process cmdProcess = new Process();
			cmdProcess.StartInfo = cmdStartInfo;
			cmdProcess.ErrorDataReceived += cmd_Error;
			cmdProcess.OutputDataReceived += cmd_DataReceived;
			cmdProcess.EnableRaisingEvents = true;
			cmdProcess.Start();
			cmdProcess.BeginOutputReadLine();
			cmdProcess.WaitForExit(50);
			cmdProcess.Kill();
		
			using (FileStream stream = File.Open("C:\\Users\\robert_salinas\\Desktop\\Environment Definitions\\GeneratedScript.txt", FileMode.Create))
			{
			}
			using (var writer = new StreamWriter("C:\\Users\\robert_salinas\\Desktop\\Environment Definitions\\GeneratedScript.txt"))
			{
				writer.Write(sb.ToString());
			}

			cmdProcess.Start();
			cmdProcess.StandardInput.WriteLine("cd C:\\Users\\robert_salinas\\Desktop\\Environment Definitions");
			cmdProcess.StandardInput.WriteLine("start GeneratedScript.txt");
			cmdProcess.StandardInput.WriteLine("exit");
			cmdProcess.WaitForExit();
		}
		private void frmQuickStart_Load(object sender, EventArgs e)
		{
			Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
		}
		private void OnApplicationExit(object sender, EventArgs e)
		{
			Environment.Exit(Environment.ExitCode);
		}

		private void cmbSchema_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void cmbCascadeOption_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
	}
}
