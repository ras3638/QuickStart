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
using System.IO.Compression;
namespace QuickStart
{
	public partial class frmQuickStart : Form
	{
		#region Class Variables

		List<string> Log = new List<string>();
		//List<string> RTTLog = new List<string>();

		//Constants
		public const string QFC = "QFC";
		public const string DEVL = "Devl";
		public const string LOAD_SCHEMA = "Load Schema";
		public const string SCRIPT_GEN = "Script Gen";
		public const string INSERT_GEN = "Insert Gen";
		public const string CASCADE_DELETE_GEN = "Cascade Delete Gen";

		//static bool UseActualNulls = true;
		volatile List<Thread> ManagedThreadList = new List<Thread>();
		volatile string sCurrentTab = DEVL;

		Thread TimerThread;
		Thread SearchStringThread;
		Thread GenInsertThread;
		Thread GenCascadeDeleteThread;
		
		#endregion

		#region Worker Thread Methods

		private void BlockParallelThreads(Thread CurrentThread)
		{
			//Blocks the calling thread from running if other parallel threads are ahead of queue.
			while (true)
			{
				//Slow down?
				Thread.Sleep(500);

				List<Thread> temp = new List<Thread>();

				foreach(Thread t in ManagedThreadList.ToList())
				{
					if (t.Name == CurrentThread.Name) temp.Add(t);
				}

				if(temp.Any())
				{
					if (temp[0].ManagedThreadId == CurrentThread.ManagedThreadId)
					{
						//First in Queue. Time to work
						break;
					}
				}
				else
				{
					//Normally this method will be called when ManagedThreadList contains at least 1 thread. 
					//If that is the case, this should never hit
					break;
				}
			}
		}
		private void TimerThreadProcSafe(TimerObjectManager A)
		{			
			A.SetTimerStatusText("Status: Executing...");
			ThreadHelperUtility.SetText(this, statusStrip1, TSlblStatus, "Status: Executing...");

			A.SetTimerSuccessIconVisible(false);
			A.SetTimerErrorIconVisible(false);
			tsSuccessIcon.Visible = false;
			tsErrorIcon.Visible = false;

			Stopwatch sw = new Stopwatch();	
			sw.Start();

			//Disable Execute buttons
			A.SetTimerExecuteIconEnable(false);	
			//Enable Stop Buttons
			A.SetTimerStopIconEnable(true);
			//Commit
			UpdateExecuteAndStopIcons(A);

			bool bTrack = true;
			string elapsedTime = String.Empty;
			string sLaunchingTabName = A.GetName();

			while (A.GetTimerThreadActive())
			{
				//Timer is active
				while (A.GetPauseTimerIndicator())
				{
					//Timer is on pause. Wait
					Thread.Sleep(50);
				}
				TimeSpan tsCurrent = sw.Elapsed;
				elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", tsCurrent.Hours, tsCurrent.Minutes, tsCurrent.Seconds);
				A.SetTimerText(elapsedTime);
				if (sCurrentTab == sLaunchingTabName)
				{
					ThreadHelperUtility.SetText(this, statusStrip1, TSlblTime, elapsedTime);
					bTrack = true;
				}
				else
				{
					if (bTrack)
					{
						//Limit the number of objects created by using bTrack
						TimerObjectManager B = TimerObjectState.Retrieve(sCurrentTab);
						ThreadHelperUtility.SetText(this, statusStrip1, TSlblTime, B.GetTimerText());
						bTrack = false;
					}			
				}
			}
			//Enable Execute buttons
			A.SetTimerExecuteIconEnable(true);
			//Disable Cancel Buttons
			A.SetTimerStopIconEnable(false);
			//Commit
			UpdateExecuteAndStopIcons(A);

			if (A.GetTimerCancelIndicator())
			{
				A.SetTimerStatusText("Status: Canceled");
				A.SetTimerErrorIconVisible(true);
				if (sCurrentTab == sLaunchingTabName)
				{
					ThreadHelperUtility.SetText(this, statusStrip1, TSlblStatus, "Status: Canceled");
					tsErrorIcon.Visible = true;
				}		
			}
			else
			{
				if (A.GetTimerErrorIndicator())
				{
					A.SetTimerStatusText("Status: Error");
					A.SetTimerErrorIconVisible(true);
					if (sCurrentTab == sLaunchingTabName)
					{
						ThreadHelperUtility.SetText(this, statusStrip1, TSlblStatus, "Status: Error");
						tsErrorIcon.Visible = true;
					}			
				}
				else
				{
					A.SetTimerStatusText("Status: Completed Succesfully");
					A.SetTimerSuccessIconVisible(true);
					if (sCurrentTab == sLaunchingTabName)
					{
						ThreadHelperUtility.SetText(this, statusStrip1, TSlblStatus, "Status: Completed Succesfully");
						tsSuccessIcon.Visible = true;
					}		
				}
			}
		}
		private void UpdateExecuteAndStopIcons(TimerObjectManager A)
		{
			if (A.GetName() == tsInsertGen.Tag.ToString())
			{
				ThreadHelperUtility.SetEnable(this, tsInsertGen, tsGenerate_Insert, A.GetTimerExecuteIconEnable());
				ThreadHelperUtility.SetEnable(this, tsInsertGen, tsStopGenerate_Insert, A.GetTimerStopIconEnable());
			}
			else if (A.GetName() == tsCascadeDelete.Tag.ToString()) 
			{
				ThreadHelperUtility.SetEnable(this, tsCascadeDelete, tsGenerate_CascadeDelete, A.GetTimerExecuteIconEnable());
				ThreadHelperUtility.SetEnable(this, tsCascadeDelete, tsStopGenerate_CascadeDelete, A.GetTimerStopIconEnable());
			}
			else if (A.GetName() == tsQFC.Tag.ToString()) 
			{
				ThreadHelperUtility.SetEnable(this, tsQFC, tsSearch, A.GetTimerExecuteIconEnable());
				ThreadHelperUtility.SetEnable(this, tsQFC, tsStopSearch, A.GetTimerStopIconEnable());
			}
			else if (A.GetName() == tsLoadSchema.Tag.ToString())
			{
				ThreadHelperUtility.SetEnable(this, tsLoadSchema, tsGenerate_SchemaScript, A.GetTimerExecuteIconEnable());
				ThreadHelperUtility.SetEnable(this, tsLoadSchema, tsSubmit_Schema, A.GetTimerExecuteIconEnable());
			}
		}
		private void SearchStringThreadProcSafe(string sVersion, string sSearchString, string sFilePattern, TimerObjectManager A)
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
				string sPath = String.Empty;
				if (sVersion == "5.0")
				{
					sPath = (string)SettingManager.GetSettingValue(QFC, "QFC 5.0 Zip Path");

				}
				else if (sVersion == "8.0")
				{
					sPath = (string)SettingManager.GetSettingValue(QFC, "QFC 8.0 Zip Path");
				}
				else if (sVersion == "16.0")
				{
					sPath = (string)SettingManager.GetSettingValue(QFC, "QFC 16.0 Zip Path");
				}
				File.Delete(sPath + "\\SearchResults.txt");
				cmdProcess.StandardInput.WriteLine("cd " + sPath);

				List<string> ZipFilesListFull = Directory.GetFiles(sPath, "*.zip*", SearchOption.TopDirectoryOnly).ToList();
				List<string> ZipFilesListNoExt = new List<string>();
				List<string> DirectoriesList = Directory.GetDirectories(sPath, "*.*", SearchOption.TopDirectoryOnly).ToList();

				//Remove the zip extension
				foreach (string s in ZipFilesListFull)
				{
					ZipFilesListNoExt.Add(s.Substring(0, s.Length - 4));
				}

				List<string> DiffsList = ZipFilesListNoExt.Except(DirectoriesList).ToList();

				if (DiffsList.Count > 0 && (bool)SettingManager.GetSettingValue(QFC, "Enable Zip Functionality"))
				{
					A.SetPauseTimerInidicator(true);
					if (MessageBox.Show("Search functionality requires all QFC zip files to be unzipped but found " + DiffsList.Count() + " unzipped files. Application will start unzip operation.  \nContinue?",
						"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						A.SetTimerThreadActive(false);
						return;
					}
					A.SetPauseTimerInidicator(false);
					foreach (string sZipToExtract in DiffsList)
					{
						int iStartIndex = sZipToExtract.LastIndexOf("\\") + 1;
						string sZipFile = sZipToExtract.Substring(iStartIndex, sZipToExtract.Length - iStartIndex);
						string UnzipCommand = "unzip -o -d " + sZipFile + " " + sZipFile + ".zip";
						cmdProcess.StandardInput.WriteLine(UnzipCommand);
					}
				}
			}
			catch (ThreadAbortException ex)
			{
				cmdProcess.StandardInput.WriteLine("exit");
				if (!A.GetTimerCancelIndicator())
				{
					//We got an unrequested Abort
					A.SetTimerErrorIndicator(true);
					MessageBox.Show(ex.ToString(), "Error");
				}
			}
			catch (Exception ex)
			{
				cmdProcess.StandardInput.WriteLine("exit");
				MessageBox.Show(ex.ToString(), "Error");

				A.SetTimerErrorIndicator(true);
				A.SetTimerThreadActive(false);
				return;
			}

			try
			{
				string SearchCommand = "findstr /s /n /i /p /c:";
				string OutputFile = ">> SearchResults.txt";
				string sFull = string.Format("{0}\"{1}\" {2} {3}", SearchCommand, sSearchString, sFilePattern, OutputFile);
				cmdProcess.StandardInput.WriteLine(sFull);
				cmdProcess.StandardInput.WriteLine("start SearchResults.txt");
				cmdProcess.StandardInput.WriteLine("exit");                  //Execute exit.

				cmdProcess.WaitForExit();

			}
			catch (ThreadAbortException ex)
			{
				if (!A.GetTimerCancelIndicator())
				{
					//We got an unrequested Abort
					A.SetTimerErrorIndicator(true);
					MessageBox.Show(ex.ToString(), "Error");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error");
				A.SetTimerErrorIndicator(true);
			}
			finally
			{
				cmdProcess.StandardInput.WriteLine("exit");
				A.SetTimerThreadActive(false);
			}
		}
		private void GenInsertThreadProcSafe(string sInputString, string sIDInsert, string sTableName, string sGenFile, TimerObjectManager A)
		{
			//Only want one GenInsertThread thread running at any time
			BlockParallelThreads(Thread.CurrentThread);

			try
			{
				StringBuilder sb = new StringBuilder();

				if (!sInputString.Contains("\n"))
				{
					sb.Append("@@Error:Improper input");
				}
				else
				{
					sb.Append(SqlGenEngine.InsertEngine(sInputString, sIDInsert, sTableName));
				}
				string x = sb.ToString();
				CreateGeneratedScriptFile(sb, sGenFile);
			}
			catch (ThreadAbortException ex)
			{
				if (!A.GetTimerCancelIndicator())
				{
					//We got an unrequested Abort
					A.SetTimerErrorIndicator(true);
					MessageBox.Show(ex.ToString(), "Error");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error");
				A.SetTimerErrorIndicator(true);
			}
			finally
			{
				if (ManagedThreadList.Contains(Thread.CurrentThread))
				{
					ManagedThreadList.Remove(Thread.CurrentThread);
				}

				if (ManagedThreadList.Find(item => item.Name == INSERT_GEN) == null)
				{
					//There are no more active GenInsertThread. Stop the timer
					A.SetTimerThreadActive(false);
				}
			}		
		}
		private void GenCascadeDeleteThreadProcSafe(string sFKName, bool bUseIncrements, string sInput, string sCascadeOption, string sSchema, TimerObjectManager A)
		{
			try
			{
				StreamReader streamReader = new StreamReader(BaseDirectories.GeneratedSchemas + "\\" + GetSchemaFileName(sSchema));
				string sReadLine;
				DataTable dtDatabaseSchema = new DataTable("Database Schema");
				bool bFirstIteration = true;
				StringBuilder sb = new StringBuilder();
				
				while ((sReadLine = streamReader.ReadLine()) != null)
				{
					if (bFirstIteration) dtDatabaseSchema = DataTableHelperUtility.AddColumns(dtDatabaseSchema, sReadLine);
					if (!bFirstIteration) dtDatabaseSchema = DataTableHelperUtility.AddRows(dtDatabaseSchema, sReadLine);
					bFirstIteration = false;
				}

				if (sCascadeOption == "Single")
				{
					sb = SqlGenEngine.CascadeDeleteSingleEngine(dtDatabaseSchema, sFKName, bUseIncrements, sInput);				
				}
				else if (sCascadeOption == "Full")
				{
					sb = SqlGenEngine.CascadeDeleteFullEngine(dtDatabaseSchema,sFKName, bUseIncrements, sInput);
				}
				else if (sCascadeOption == "All")
				{
					sb = SqlGenEngine.CascadeDeleteAllEngine(dtDatabaseSchema,sFKName, bUseIncrements, sInput);
				}
				else if (sCascadeOption == "None")
				{
					sb = SqlGenEngine.CascadeDeleteNoneEngine(dtDatabaseSchema, sFKName, bUseIncrements, sInput);
				}

				if (sb.ToString().Contains("@@Error:"))
				{
					A.SetTimerErrorIndicator(true);
				}
				else
				{
					if(sCascadeOption == "Single") sb.Insert(0, "--Foriegn Key Cascade: " + sFKName + "\n");
					//sb.Append(SqlGenEngine.AppendOriginalDelStatement(sInput));							
				}

				sb = StringUtility.CleanSB(sb);

				CreateGeneratedScriptFile(sb, "GeneratedScript.txt");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(),"Error");
				A.SetTimerErrorIndicator(true);
			}
			finally
			{
				A.SetTimerThreadActive(false);
			}
		}
		#endregion
	
		#region Constructor
		public frmQuickStart()
		{
			InitializeComponent();
			tsInsertGen.Renderer = new ToolStripOverride();
			tsCascadeDelete.Renderer = new ToolStripOverride();
			tsLoadSchema.Renderer = new ToolStripOverride();
			tsDevl.Renderer = new ToolStripOverride();
			tsQFC.Renderer = new ToolStripOverride();
			cmbCascadeOption.SelectedIndex = 0;
			cmbVersion.SelectedIndex = 0;
			cmbFilePattern.SelectedIndex = 0;
			cmbLoadDB.SelectedIndex = 0;
			LoadBaseDirectories();
			LoadSchemas();
			LoadSettings();

			new TimerObjectManager(CASCADE_DELETE_GEN);
			new TimerObjectManager(INSERT_GEN);
			new TimerObjectManager(LOAD_SCHEMA);
			new TimerObjectManager(DEVL);
			new TimerObjectManager(QFC);

			var dataSource = new List<Client>();
			var dataSource2 = new List<Client>();
			var dataSource3 = new List<Client>();

			IDictionary<string, string> ClientDictionary = new Dictionary<string, string>();
			ClientDictionary.Add("", "");

			try
			{
				//throw new  NotImplementedException();
				if (!UseCachedClient())
				{
					ClientDictionary.Add("CORE 5.0", "QBS");
					ClientDictionary.Add("CORE 8.0", "QBS");
					ClientDictionary.Add("CORE 16.0", "QBS");

					string[] files = Directory.GetDirectories("R:\\InternalClientInstallers\\Projects",
						"*.*", SearchOption.TopDirectoryOnly);

					// Display all the files.
					foreach (string f in files)
					{
						int iRecursionLevelMax = 2; //the max number of times DirSearch will recurse unto itself		
						KeyValuePair<string, string> kvp = FindClient(f, iRecursionLevelMax, new KeyValuePair<string, string>());
						if (kvp.Key != null) ClientDictionary.Add(kvp);
					}
				}
				else
				{
					StreamReader streamReader = new StreamReader(BaseDirectories.CacheClientBase);
					string sReadLine;
					while ((sReadLine = streamReader.ReadLine()) != null)
					{
						List<string> Split = StringUtility.HighMemSplit(sReadLine, " - ");
						if (Split.Count == 2 && !String.IsNullOrWhiteSpace(Split[0]) && !String.IsNullOrWhiteSpace(Split[1]))
						{
							ClientDictionary.Add(Split[0], Split[1]);
						}
					}
					streamReader.Dispose();
				}
				using (var writer = new StreamWriter(BaseDirectories.CacheClientBase))
				{
					foreach (var pair in ClientDictionary)
					{
						
						dataSource.Add(new Client() { CmbName = pair.Key + " - " + pair.Value, Name = pair.Key, Code = pair.Value });
						dataSource2.Add(new Client() { CmbName = pair.Key + " - " + pair.Value, Name = pair.Key, Code = pair.Value });
						if (pair.Key != "")
						{
							writer.WriteLine(pair.Key + " - " + pair.Value);
							dataSource3.Add(new Client() { CmbName = pair.Key + " - " + pair.Value, Name = pair.Key, Code = pair.Value });
						}
					}			
				}
				//Setup data binding
				this.cmbProcess.DataSource = dataSource;
				this.cmbGUI.DataSource = dataSource2;
				this.cmbLoadEnv.DataSource = dataSource3;
			}
			catch (IOException)
			{
				tsSetup.Enabled = false;
				tsSubmit_Schema.Enabled = false;
				dataSource.Add(new Client() { CmbName = "Could not connect to R drive", Name = "", Code = "" });
				this.cmbProcess.DataSource = dataSource;
				this.cmbGUI.DataSource = dataSource;
				this.cmbLoadEnv.DataSource = dataSource;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				this.cmbProcess.DisplayMember = "CmbName";
				this.cmbGUI.DisplayMember = "CmbName";
				this.cmbLoadEnv.DisplayMember = "CmbName";
				this.cmbProcess.ValueMember = null;
				this.cmbGUI.ValueMember = null;
				this.cmbLoadEnv.ValueMember = null;
			}
		}
		#endregion
		private bool UseCachedClient()
		{
			if (!File.Exists(BaseDirectories.CacheClientBase))
			{
				File.Create(BaseDirectories.CacheClientBase);
				return false;
			}
			else
			{
				DateTime FileDateCreated = File.GetCreationTime(BaseDirectories.CacheClientBase);
				DateTime CurrentDate = DateTime.Now;

				if (FileDateCreated.AddDays(30) < CurrentDate)
				{
					//Overwrite and clear the file
					File.Create(BaseDirectories.CacheClientBase);
					return false;
				}
			}
			return true;
		}
		private void LoadBaseDirectories()
		{
			if (!Directory.Exists(BaseDirectories.QuickStartBase)) Directory.CreateDirectory(BaseDirectories.QuickStartBase);
			if (!Directory.Exists(BaseDirectories.Core5QPECBase)) Directory.CreateDirectory(BaseDirectories.Core5QPECBase);
			if (!Directory.Exists(BaseDirectories.Core8QPECBase)) Directory.CreateDirectory(BaseDirectories.Core8QPECBase);
			if (!Directory.Exists(BaseDirectories.Core16QPECBase)) Directory.CreateDirectory(BaseDirectories.Core16QPECBase);
			if (!Directory.Exists(BaseDirectories.Core5QPECConfigBase)) Directory.CreateDirectory(BaseDirectories.Core5QPECConfigBase);
			if (!Directory.Exists(BaseDirectories.Core8QPECConfigBase)) Directory.CreateDirectory(BaseDirectories.Core8QPECConfigBase);
			if (!Directory.Exists(BaseDirectories.Core16QPECConfigBase)) Directory.CreateDirectory(BaseDirectories.Core16QPECConfigBase);
			if (!Directory.Exists(BaseDirectories.Core5QEnvironmentDefinitionsBase)) Directory.CreateDirectory(BaseDirectories.Core5QEnvironmentDefinitionsBase);
			if (!Directory.Exists(BaseDirectories.Core5UpstreamMainAppBase)) Directory.CreateDirectory(BaseDirectories.Core5UpstreamMainAppBase);
			if (!Directory.Exists(BaseDirectories.Core8UpstreamMainAppBase)) Directory.CreateDirectory(BaseDirectories.Core8UpstreamMainAppBase);
			if (!Directory.Exists(BaseDirectories.Core16UpstreamMainAppBase)) Directory.CreateDirectory(BaseDirectories.Core16UpstreamMainAppBase);
			if (!Directory.Exists(BaseDirectories.GeneratedSchemas)) Directory.CreateDirectory(BaseDirectories.GeneratedSchemas);		
		}
		private void LoadSchemas()
		{
			var dataSource4 = new List<Client>();
			Dictionary<string, string> ClientSchemaDictionary = new Dictionary<string, string>();

			string[] files = Directory.GetFiles(BaseDirectories.GeneratedSchemas,
				"*.*", SearchOption.TopDirectoryOnly);
			
			foreach (string f in files)
			{
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
			string[] files = Directory.GetFiles(BaseDirectories.GeneratedSchemas,
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
		private KeyValuePair<string, string> FindClient(string sDir, int iRecursionLevelMax, KeyValuePair<string, string> RunningKVP, int iRecursionLevelCurrent = 0)
		{
			string[] files = Directory.GetDirectories(sDir, "*.*", SearchOption.TopDirectoryOnly);

			//Loop through all the files and try to find a code
			foreach (string f in files)
			{			
				if (RunningKVP.Key != null) return RunningKVP;
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

					string sClientCode = f.Substring(index, finalindex);
					//One final check if code is over 3 characters
					if (sClientCode.Length > 3 && sClientCode.Contains('_'))
					{
						sClientCode = sClientCode.Substring(0, 3);
					}
		
					int iIndexOfClientName = f.Substring(37).IndexOf("\\");
					string sClientName = f.Substring(37).Substring(0, iIndexOfClientName);
					return new KeyValuePair<string, string>(sClientName, sClientCode);
				}
			}

			//Exit out if we are too deep
			if (iRecursionLevelCurrent >= iRecursionLevelMax) return RunningKVP;

			//Did not find a code. Go down a layer
			foreach (string f in files)
			{		
				iRecursionLevelCurrent++;
				RunningKVP = FindClient(f, iRecursionLevelMax, RunningKVP, iRecursionLevelCurrent);
			}

			return RunningKVP;
		}
		private bool UpstreamMainAppHandler(Client Client)
		{
			if (!Client.IsNull())
			{
				try
				{
					if (Client.Name == "CORE 5.0")
					{
						string[] lines = File.ReadAllLines(BaseDirectories.Core5UpstreamMainApp);
						File.WriteAllLines(BaseDirectories.Full5UpstreamMainApp, lines);
					}
					else if (Client.Name == "CORE 8.0")
					{
						string[] lines = File.ReadAllLines(BaseDirectories.Core8UpstreamMainApp);
						File.WriteAllLines(BaseDirectories.Full8UpstreamMainApp, lines);
					}
					else if (Client.Name == "CORE 16.0")
					{
						string[] lines = File.ReadAllLines(BaseDirectories.Core16UpstreamMainApp);
						File.WriteAllLines(BaseDirectories.Full16UpstreamMainApp, lines);
					}
					else
					{
						List<string> ClientDirectories = GetClientDirectories("UpstreamMainApp", Client);

						if (ClientDirectories.Count() == 0)
						{
							Log.Add("UpstreamMainApp: No change - Could not find any templates in directory");
							return false;
						}
						else if (ClientDirectories.Count() > 1)
						{
							Log.Add("UpstreamMainApp: No change - Found more than 1 template for selected environment in directory");
							return false;
						}
						else if (ClientDirectories.Count() == 1)
						{
							string[] lines = File.ReadAllLines(ClientDirectories[0]);
							File.WriteAllLines(GetBaseDirectoryWithVersion(ClientDirectories[0], "UpstreamMainApp"), lines);
						}
					}
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
						string[] lines = File.ReadAllLines(BaseDirectories.Core5QPEC);
						File.WriteAllLines(BaseDirectories.Full5QPEC, lines);
					}
					else if (Client.Name == "CORE 8.0")
					{
						string[] lines = File.ReadAllLines(BaseDirectories.Core8QPEC);
						File.WriteAllLines(BaseDirectories.Full8QPEC, lines);
					}
					else if (Client.Name == "CORE 16.0")
					{
						string[] lines = File.ReadAllLines(BaseDirectories.Core16QPEC);
						File.WriteAllLines(BaseDirectories.Full16QPEC, lines);
					}
					else
					{
						List<string> ClientDirectories = GetClientDirectories("QPEC", Client);

						if (ClientDirectories.Count() == 0)
						{
							Log.Add("QPEC: No change - Could not find any templates in directory");
							return false;
						}
						else if (ClientDirectories.Count() > 1)
						{
							Log.Add("QPEC: No change - Found more than 1 template for selected environment in directory");
							return false;
						}
						else if (ClientDirectories.Count() == 1)
						{
							string[] lines = File.ReadAllLines(ClientDirectories[0]);
							File.WriteAllLines(GetBaseDirectoryWithVersion(ClientDirectories[0], "QPEC"), lines);
						}

					}
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
						string[] lines = File.ReadAllLines(BaseDirectories.Core5QPECConfig);
						File.WriteAllLines(BaseDirectories.Full5QPECConfig, lines);
					}
					else if (Client.Name == "CORE 8.0")
					{
						string[] lines = File.ReadAllLines(BaseDirectories.Core8QPECConfig);
						File.WriteAllLines(BaseDirectories.Full8QPECConfig, lines);
					}
					else if (Client.Name == "CORE 16.0")
					{
						string[] lines = File.ReadAllLines(BaseDirectories.Core16QPECConfig);
						File.WriteAllLines(BaseDirectories.Full16QPECConfig, lines);
					}
					else
					{
						List<string> ClientDirectories = GetClientDirectories("QPEC Config", Client);

						if (ClientDirectories.Count() == 0)
						{
							Log.Add("QPEC Config: No change - Could not find any templates in directory");
							return false;
						}
						else if (ClientDirectories.Count() > 1)
						{
							Log.Add("QPEC Config: No change - Found more than 1 template for selected environment in directory");
							return false;
						}
						else if (ClientDirectories.Count() == 1)
						{
							string[] lines = File.ReadAllLines(ClientDirectories[0]);
							File.WriteAllLines(GetBaseDirectoryWithVersion(ClientDirectories[0], "QPEC Config"), lines);
						}

					}
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
						string[] lines = File.ReadAllLines(BaseDirectories.Core5QEnvironmentDefinitions);
						File.WriteAllLines(BaseDirectories.Full5QEnvironmentDefinitions, lines);
					}
					else if (Client.Name == "CORE 8.0")
					{
						//Version 8.0 does not use QEnvironmentDefinitions
						Log.Add("QEnvironmentDefinitions: No change - Could not find any templates in directory");
						return false;
					}
					else if (Client.Name == "CORE 16.0")
					{
						//Version 16.0 does not use QEnvironmentDefinitions
						Log.Add("QEnvironmentDefinitions: No change - Could not find any templates in directory");
						return false;
					}
					else
					{
						List<string> ClientDirectories = GetClientDirectories("QEnvironmentDefinitions", Client);

						if (ClientDirectories.Count() == 0)
						{
							Log.Add("QEnvironmentDefinitions: No change - Could not find any templates in directory");
							return false;
						}
						else if (ClientDirectories.Count() > 1)
						{
							Log.Add("QEnvironmentDefinitions: No change - Found more than 1 template for selected environment in directory");
							return false;
						}
						else if (ClientDirectories.Count() == 1)
						{
							string[] lines = File.ReadAllLines(ClientDirectories[0]);
							File.WriteAllLines(GetBaseDirectoryWithVersion(ClientDirectories[0], "QEnvironmentDefinitions"), lines);
						}

					}
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
			int index = BaseDirectories.QuickStartBase.LastIndexOf('\\') + 1;
			string sBaseFolder = BaseDirectories.QuickStartBase.Substring(index, BaseDirectories.QuickStartBase.Length - index);

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
			string[] files = Directory.GetDirectories(BaseDirectories.QuickStartBase,
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
		private void cmd_DataReceived(object sender, DataReceivedEventArgs e)
		{
			Console.WriteLine("Output from other process");
			Console.WriteLine(e.Data);
			//if(!String.IsNullOrWhiteSpace(e.Data))
			//{
			//	RTTLog.Add(e.Data);
			//}		
		}
		static void cmd_Error(object sender, DataReceivedEventArgs e)
		{
			Console.WriteLine("Error from other process");
			Console.WriteLine(e.Data);
			
		}
		private void txtTableName_TextChanged(object sender, EventArgs e)
		{
			errorProvider1.Clear();
		}
		private void txtSearchString_TextChanged(object sender, EventArgs e)
		{
			errorProvider2.Clear();
		}
		private void CreateGeneratedScriptFile(StringBuilder sb, string GenFile)
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

			using (FileStream stream = File.Open(BaseDirectories.QuickStartBase + "\\" + GenFile, FileMode.Create))
			{}
			using (var writer = new StreamWriter(BaseDirectories.QuickStartBase + "\\" + GenFile))
			{
				writer.Write(sb.ToString());
			}

			cmdProcess.Start();
			cmdProcess.StandardInput.WriteLine("cd " + BaseDirectories.QuickStartBase);
			cmdProcess.StandardInput.WriteLine("start " + GenFile);
			cmdProcess.StandardInput.WriteLine("exit");
			cmdProcess.WaitForExit();
		}
		private void frmQuickStart_Load(object sender, EventArgs e)
		{
			Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
			panelSettings.Hide();
		}
		
		private void OnApplicationExit(object sender, EventArgs e)
		{
			Environment.Exit(Environment.ExitCode);
		}
		private void cmbSchema_SelectedIndexChanged(object sender, EventArgs e)
		{
			errorProvider3.Clear();
		}
		private void txtFKName_TextChanged(object sender, EventArgs e)
		{
			errorProvider2.Clear();
		}
		private void tsRightClickRemove_Click(object sender, EventArgs e)
		{
			string sSchema = cmbSchema.Text;
			File.Delete(BaseDirectories.GeneratedSchemas + "\\" + GetSchemaFileName(sSchema));
			LoadSchemas();
			cmbSchema.SelectedIndex = -1;
		}
		private void cmbSchema_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && cmbSchema.DroppedDown)
			{
				string sSchema = cmbSchema.Text;
				File.Delete(BaseDirectories.GeneratedSchemas + "\\" + GetSchemaFileName(sSchema));
				LoadSchemas();
				cmbSchema.SelectedIndex = -1;

				//Make sure no other processing happens
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Down && !cmbSchema.DroppedDown)
			{
				cmbSchema.DroppedDown = true;
			}	
		}

		private void tsGenerate_Insert_Click(object sender, EventArgs e)
		{
			//string sInputString = rttInput.Text.Trim();
			string sInputString = rttInput.Text; //Trim causes an issue if last columns are empty strings. Really we should be trimming new lines only.

			string sIDInsert = cmbIDInsert.Text;
			string sTableName = txtTableName_Insert.Text;
			bool bProcessed = false;
			string sTabName = tabControl2.SelectedTab.Tag.ToString();
			TimerObjectManager A = new TimerObjectManager(sTabName);

			if (string.IsNullOrWhiteSpace(sTableName))
			{
				errorProvider1.SetError(txtTableName_Insert, "Please Enter Table Name");
				return;
			}
			else
			{
				errorProvider1.SetError(txtTableName_Insert, "");
			}

			
			if (chkSplit.Checked)
			{
				if (StringUtility.IsStringOverMemory(sInputString))
				{
					List<string> ListToProcess = StringUtility.SplitLargeString(sInputString);
					int iFileNumber = 1;

					foreach (string sInput in ListToProcess)
					{
						string sGenFile = "GeneratedScript" + iFileNumber.ToString() + ".txt";	
						GenInsertThread = new Thread(() => GenInsertThreadProcSafe(sInput, sIDInsert, sTableName, sGenFile, A));
						GenInsertThread.Start();
						GenInsertThread.Name = INSERT_GEN;
						ManagedThreadList.Add(GenInsertThread);
						
						bProcessed = true;
						iFileNumber++;
					}
				}
			}
			else
			{
				if (StringUtility.IsStringOverMemory(sInputString))
				{
					if (MessageBox.Show("Dataset is large and may cause memory issues.\n" + StringUtility.StringOverCapacity(sInputString).ToString() + "% over recommended capacity.\nContinue?",
							"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						return;
					}
				}
			}

			if (!bProcessed)
			{
				
				GenInsertThread = new Thread(() => GenInsertThreadProcSafe(sInputString, sIDInsert, sTableName, "GeneratedScript1.txt", A));
				GenInsertThread.Start();
				GenInsertThread.Name = INSERT_GEN;
				ManagedThreadList.Add(GenInsertThread);				
			}

			//TimerThread = new Thread(new ThreadStart(this.TimerThreadProcSafe));
			A.SetTimerThreadActive(true);

			TimerThread = new Thread(() => TimerThreadProcSafe(A));	
			TimerThread.Start();
		}
		

		private void tsGenerateCascadeDelete_Click(object sender, EventArgs e)
		{
			string sCascadeOption = cmbCascadeOption.Text;
			string sFKName = txtFKName.Text;
			bool bUseIncrements = chkUseIncrements.Checked;
			string sInput = rttInput.Text;
			string sSchema = cmbSchema.Text;
			string sTabName = tabControl2.SelectedTab.Tag.ToString();

			if (sCascadeOption == "Single" || sCascadeOption == "Full" || sCascadeOption == "All")
			{
				//Error Handling
				bool bSuccess = true;

				if (string.IsNullOrWhiteSpace(sFKName) && sCascadeOption != "All")
				{
					errorProvider3.SetError(txtFKName, "Please Enter FK Name");
					bSuccess = false;
				}
				else
				{
					errorProvider3.SetError(txtFKName, "");
				}
				if (cmbSchema.SelectedIndex == -1)
				{
					errorProvider2.SetError(cmbSchema, "Please Enter Schema");
					bSuccess = false;
				}
				else
				{
					errorProvider2.SetError(cmbSchema, "");
				}
				if (!bSuccess) return;
			}

			//TimerThread = new Thread(new ThreadStart(this.TimerThreadProcSafe));
			TimerObjectManager A = TimerObjectState.Retrieve(sTabName);
			A.SetTimerThreadActive(true);
			TimerThread = new Thread(() => TimerThreadProcSafe(A));	
			TimerThread.Start();

			GenCascadeDeleteThread = new Thread(() => GenCascadeDeleteThreadProcSafe(sFKName, bUseIncrements, sInput, sCascadeOption, sSchema, A));
			GenCascadeDeleteThread.Start();
			ManagedThreadList.Add(GenCascadeDeleteThread);
		}

		private void tsGenerate_SchemaScript_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			string GenFile = "GeneratedScript.txt";
			sb.Append(Sql.sLoadSchema);
			CreateGeneratedScriptFile(sb,GenFile);
		}

		private void tsSubmit_Schema_Click(object sender, EventArgs e)
		{
			string sTabName = tabControl2.SelectedTab.Tag.ToString();
			TimerObjectManager A = TimerObjectState.Retrieve(sTabName);

			try
			{
				A.SetTimerThreadActive(true);
				TimerThread = new Thread(() => TimerThreadProcSafe(A));
				
				TimerThread.Start();

				Client SelectedClient = (Client)cmbLoadEnv.SelectedValue;
				string sSchemaFileName = SelectedClient.Name + "_" + SelectedClient.Code + "_" + cmbLoadDB.Text + ".txt";
				using (FileStream stream = File.Open(BaseDirectories.GeneratedSchemas + "\\" + sSchemaFileName, FileMode.Create))
				{
				}
				using (var writer = new StreamWriter(BaseDirectories.GeneratedSchemas + "\\" + sSchemaFileName))
				{
					writer.Write(rttInput.Text);
				}
				LoadSchemas();
				MessageBox.Show("Successfully uploaded schema", "Success");
				A.SetTimerErrorIndicator(false);
			}
			catch (Exception ex)
			{	
				MessageBox.Show(ex.ToString(), "Error");
				A.SetTimerErrorIndicator(true);
			}
			finally
			{
				A.SetTimerThreadActive(false);
			}
		}

		private void tsSetup_Click(object sender, EventArgs e)
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

		private void tsSearch_Click(object sender, EventArgs e)
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

			string sTabName = tabControl1.SelectedTab.Tag.ToString();

			TimerObjectManager A = TimerObjectState.Retrieve(sTabName);
			A.SetTimerThreadActive(true);

			TimerThread = new Thread(() => TimerThreadProcSafe(A));	
			TimerThread.Start();

			string Version = cmbVersion.Text;
			string SearchString = txtSearchString.Text;
			string FilePattern = cmbFilePattern.Text;
			SearchStringThread = new Thread(() => SearchStringThreadProcSafe(Version, SearchString, FilePattern, A));
			SearchStringThread.Start();
			SearchStringThread.Name = QFC;
			ManagedThreadList.Add(SearchStringThread);
		}
		private void StopCurrentThread(string sLaunchingTab)
		{
			TimerObjectManager A = TimerObjectState.Retrieve(sLaunchingTab);
			A.SetTimerCancelIndicator(true);

			lock (ManagedThreadList)
			{
				foreach (Thread t in ManagedThreadList.ToList())
				{
					if(t.Name == sLaunchingTab)
					{
						t.Abort();
						t.Join();
						ManagedThreadList.Remove(t);
					}
				}
			}
			
			A.SetTimerThreadActive(false);
		}

		private void tsStopGenerate_Insert_Click(object sender, EventArgs e)
		{
			string sTabName = tabControl2.SelectedTab.Tag.ToString();
			StopCurrentThread(sTabName);
		}

		private void tsStopGenerate_CascadeDelete_Click(object sender, EventArgs e)
		{
			string sTabName = tabControl2.SelectedTab.Tag.ToString();
			StopCurrentThread(sTabName);
		}

		private void tsStopSearch_Click(object sender, EventArgs e)
		{
			string sTabName = tabControl1.SelectedTab.Tag.ToString();
			StopCurrentThread(sTabName);
		}

		private void tsStopSetup_Click(object sender, EventArgs e)
		{
			string sTabName = tabControl2.SelectedTab.Tag.ToString();
			StopCurrentThread(sTabName);
		}

		private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
		{
			string sNewTabName = tabControl2.SelectedTab.Tag.ToString();
			
			TimerObjectManager A = TimerObjectState.Retrieve(sNewTabName);

			TSlblStatus.Text = A.GetTimerStatusText();
			tsSuccessIcon.Visible = A.GetTimerSuccessIconVisible();
			tsErrorIcon.Visible = A.GetTimerErrorIconVisible();
			TSlblTime.Text = A.GetTimerText();

			sCurrentTab = sNewTabName;	
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string sNewTabName1 = tabControl1.SelectedTab.Tag.ToString();
			string sNewTabName2 = tabControl2.SelectedTab.Tag.ToString();

			if (sNewTabName1 == SCRIPT_GEN)
			{
				TimerObjectManager B = TimerObjectState.Retrieve(sNewTabName2);
				TSlblStatus.Text = B.GetTimerStatusText();
				tsSuccessIcon.Visible = B.GetTimerSuccessIconVisible();
				tsErrorIcon.Visible = B.GetTimerErrorIconVisible();
				TSlblTime.Text = B.GetTimerText();
				sCurrentTab = sNewTabName2;
			}
			else
			{
				TimerObjectManager A = TimerObjectState.Retrieve(sNewTabName1);
				TSlblStatus.Text = A.GetTimerStatusText();
				tsSuccessIcon.Visible = A.GetTimerSuccessIconVisible();
				tsErrorIcon.Visible = A.GetTimerErrorIconVisible();
				TSlblTime.Text = A.GetTimerText();
				sCurrentTab = sNewTabName1;
			}
		}
	}	 
}
