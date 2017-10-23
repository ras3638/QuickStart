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
using System.Drawing.Drawing2D;

namespace QuickStart
{
	public partial class frmQuickStart : Form
	{
		#region Class Variables

		List<string> Log = new List<string>();

		//Constants
		public const string SEARCH_STRING = "Search String";
		public const string DEVL = "Devl";
		public const string LOAD_SCHEMA = "Load Schema";
		public const string SCRIPT_GEN = "Script Gen";
		public const string INSERT_GEN = "Insert Gen";
		public const string CASCADE_DELETE_GEN = "Cascade Delete Gen";
        public const string UPDATE_GEN = "Update Gen";

        List<Client> dataSource = new List<Client>();
        List<Client> dataSource2 = new List<Client>();
        List<Client> dataSource3 = new List<Client>();

		//static bool UseActualNulls = true;
		volatile List<Thread> ManagedThreadList = new List<Thread>();
		volatile string sCurrentTab = DEVL;

		Thread TimerThread;
		Thread SearchStringThread;
		Thread GenInsertThread;
		Thread GenCascadeDeleteThread;
        Thread GenUpdateThread;

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
		private void TimerThreadProcSafe(TimerObjectState A)
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

            //Reset error/custom messages
            A.SetErrorException(null);
            A.SetCustomMessage(null);
            //Commit
            UpdateErrorMessages(A);

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
						TimerObjectState B = TimerObjectManager.Retrieve(sCurrentTab);
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
                    ThreadHelperUtility.SetVisible(this, statusStrip1, tsErrorIcon, true);
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
						//tsErrorIcon.Visible = true;
                        ThreadHelperUtility.SetVisible(this, statusStrip1, tsErrorIcon, true);
                        
                    }			
				}
				else
				{
					A.SetTimerStatusText("Status: Completed Succesfully");
					A.SetTimerSuccessIconVisible(true);
					if (sCurrentTab == sLaunchingTabName)
					{
						ThreadHelperUtility.SetText(this, statusStrip1, TSlblStatus, "Status: Completed Succesfully");
                        //tsSuccessIcon.Visible = true;
                        ThreadHelperUtility.SetVisible(this, statusStrip1, tsSuccessIcon, true);
					}		
				}
			}
            //Update error messages (if any)
            UpdateErrorMessages(A);
        }
        private void UpdateErrorMessages(TimerObjectState A)
        {
            if (A.GetErrorException() == null && A.GetCustomMessage() == null)
            {
                //No error messages. Clear it
                ThreadHelperUtility.SetText(this, statusStrip1, tsMessages, "Messages(0)");
                for (int i = 0; i < tsMessages.DropDownItems.Count; i++)
                {
                    ThreadHelperUtility.SetText(this, statusStrip1, tsMessages, tsMessages.DropDownItems[i], "");
                    if (i != 0) ThreadHelperUtility.SetVisible(this, statusStrip1, tsMessages, tsMessages.DropDownItems[i], false);
                    
                }
                return;
            }

            //Only 1 message (either error or custom) is currently supported. Prioritize error over custom
            string sMessage = String.Empty;

            if(A.GetCustomMessage() != null)
            {
                sMessage = A.GetCustomMessage();
            }
                
            if(A.GetErrorException() != null)
            {
                sMessage = A.GetErrorException().ToString();
            }

            List<string> Splitter = StringUtility.HighMemSplit(sMessage, "\r\n");

            for (int i = 0; i < Splitter.Count && i < tsMessages.DropDownItems.Count; i++)
            {
                //if (i > tsMessages.DropDownItems.Count) break;
                ThreadHelperUtility.SetText(this, statusStrip1, tsMessages, tsMessages.DropDownItems[i], Splitter[i]);
                ThreadHelperUtility.SetText(this, statusStrip1, tsMessages, "Messages(1)");
                ThreadHelperUtility.SetVisible(this, statusStrip1, tsMessages, tsMessages.DropDownItems[i], true);
            }

        }
		private void UpdateExecuteAndStopIcons(TimerObjectState A)
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
			else if (A.GetName() == tsSearchString.Tag.ToString()) 
			{
				ThreadHelperUtility.SetEnable(this, tsSearchString, tsSearch, A.GetTimerExecuteIconEnable());
				ThreadHelperUtility.SetEnable(this, tsSearchString, tsStopSearch, A.GetTimerStopIconEnable());
			}
			else if (A.GetName() == tsLoadSchema.Tag.ToString())
			{
				ThreadHelperUtility.SetEnable(this, tsLoadSchema, tsGenerate_SchemaScript, A.GetTimerExecuteIconEnable());
				ThreadHelperUtility.SetEnable(this, tsLoadSchema, tsSubmit_Schema, A.GetTimerExecuteIconEnable());
			}
            else if (A.GetName() == tsUpdateGen.Tag.ToString())
            {
                ThreadHelperUtility.SetEnable(this, tsUpdateGen, tsGenerate_Update, A.GetTimerExecuteIconEnable());
                ThreadHelperUtility.SetEnable(this, tsUpdateGen, tsStopGenerate_Update, A.GetTimerStopIconEnable());
            }
        }
        private void ThrottleThreadProcSafe(int processId, double limit)
        {
            ProcessManager.ThrottleProcess(processId, limit);
        }
        private void SearchStringThreadProcSafe(string sSearchPath, string sSearchString, string sFilePattern, TimerObjectState A)
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
            cmdProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
            //cmdProcess.BeginOutputReadLine();
            //cmdProcess.BeginErrorReadLine();
            cmdProcess.StartInfo.RedirectStandardOutput = true;

            //test
            //ThrottleThread = new Thread(() => ThrottleThreadProcSafe(cmdProcess.Id, 10));
            //ThrottleThread.Start();

            
            int iTopRowsToRemove = (int)SettingManager.GetSettingValue(SEARCH_STRING, "Number of top lines to remove from SearchResults.txt");
            int iBottomRowsToRemove = (int)SettingManager.GetSettingValue(SEARCH_STRING, "Number of bottom lines to remove from SearchResults.txt");

            try
			{
                string sDriveLetter = Path.GetPathRoot(Environment.CurrentDirectory);
                string sSearchDriveLetter = Path.GetPathRoot(sSearchPath);

                if (sDriveLetter != sSearchDriveLetter)
                {
                    //Two additional lines
                    cmdProcess.StandardInput.WriteLine(sSearchDriveLetter.Substring(0,2));
                    iTopRowsToRemove = iTopRowsToRemove + 2;
                }

                cmdProcess.StandardInput.WriteLine("cd " + sSearchPath);

				List<string> ZipFilesListFull = Directory.GetFiles(sSearchPath, "*.zip*", SearchOption.TopDirectoryOnly).ToList();
				List<string> ZipFilesListNoExt = new List<string>();
				List<string> DirectoriesList = Directory.GetDirectories(sSearchPath, "*.*", SearchOption.TopDirectoryOnly).ToList();

				//Remove the zip extension
				foreach (string s in ZipFilesListFull)
				{
					ZipFilesListNoExt.Add(s.Substring(0, s.Length - 4));
				}

				List<string> DiffsList = ZipFilesListNoExt.Except(DirectoriesList).ToList();

				if (DiffsList.Count > 0 && (bool)SettingManager.GetSettingValue(SEARCH_STRING, "Enable Zip Functionality"))
				{
					A.SetPauseTimerInidicator(true);
					if (MessageBoxEx.Show("Search functionality requires all zip files to be unzipped but found " + DiffsList.Count() + " unzipped files. Application will start unzip operation.  \nContinue?",
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
					A.SetTimerErrorIndicator(true,ex);
                    //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
			}
			catch (Exception ex)
			{
				cmdProcess.StandardInput.WriteLine("exit");
				//MessageBox.Show(ex.Message, "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                A.SetTimerErrorIndicator(true, ex);
                A.SetTimerThreadActive(false);
				return;
			}

			try
			{
				string SearchCommand = "findstr /s /n /i /p /c:";
                //string OutputFile = ">> \\SearchResults.txt";
                //string sFull = string.Format("{0}\"{1}\" {2} {3}", SearchCommand, sSearchString, sFilePattern, OutputFile);
                string sFull = string.Format("{0}\"{1}\" {2}", SearchCommand, sSearchString, sFilePattern);

                cmdProcess.StandardInput.WriteLine(sFull);
                //cmdProcess.WaitForExit();
                cmdProcess.StandardInput.WriteLine("exit");

                StringBuilder sb = new StringBuilder(cmdProcess.StandardOutput.ReadToEnd());
                CreateGeneratedScriptFile(sb, "SearchResults.txt", iTopRowsToRemove, iBottomRowsToRemove);

            }
            catch (ThreadAbortException ex)
			{
				if (!A.GetTimerCancelIndicator())
				{
					//We got an unrequested Abort
					A.SetTimerErrorIndicator(true, ex);
                    //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
			}
			catch (Exception ex)
			{
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                A.SetTimerErrorIndicator(true, ex);
			}
			finally
			{
				cmdProcess.StandardInput.WriteLine("exit");
				A.SetTimerThreadActive(false);
			}
            
        }
		private void GenInsertThreadProcSafe(string sInputString, string sIDInsert, string sTableName, string sGenFile, TimerObjectState A)
		{
			//Only want one GenInsertThread thread running at any time
			BlockParallelThreads(Thread.CurrentThread);

			try
			{
				StringBuilder sb = new StringBuilder();

				if (!sInputString.Contains("\n"))
				{
					sb.Append("@@Error:Improper input - No new lines found");
				}
				else
				{
					sb.Append(SqlGenEngine.InsertEngine(sInputString, sIDInsert, sTableName));
				}
				string x = sb.ToString();
				CreateGeneratedScriptFile(sb, sGenFile);

                if (sb.ToString().Contains("@@Error:"))
                {
                    A.SetTimerErrorIndicator(true, null, "See generated file for error details");
                }
            }
			catch (ThreadAbortException ex)
			{
				if (!A.GetTimerCancelIndicator())
				{
					//We got an unrequested Abort
					A.SetTimerErrorIndicator(true, ex);
                }
			}
			catch (Exception ex)
			{
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                A.SetTimerErrorIndicator(true, ex);
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
		private void GenCascadeDeleteThreadProcSafe(string sFKName, bool bUseIncrements, string sInput, string sCascadeOption, string sSchema, TimerObjectState A)
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
					A.SetTimerErrorIndicator(true, null, "See generated file for error details");
				}
				else
				{
					if(sCascadeOption == "Single") sb.Insert(0, "--Foriegn Key Cascade: " + sFKName + "\n");
					//sb.Append(SqlGenEngine.AppendOriginalDelStatement(sInput));							
				}

				sb = StringUtility.CleanSB(sb);

				CreateGeneratedScriptFile(sb, "GeneratedScript.txt");
			}
            catch(ThreadAbortException ex)
            {
                if (!A.GetTimerCancelIndicator())
                {
                    //We got an unrequested Abort
                    A.SetTimerErrorIndicator(true, ex);
                }
            }
			catch (Exception ex)
			{
                A.SetTimerErrorIndicator(true,ex);
			}
			finally
			{
				A.SetTimerThreadActive(false);
			}
		}
        private void GenUpdateThreadProcSafe(string sInput, string sTableName, string sSchema, string sGenFile, TimerObjectState A)
        {
            //Only want one GenUpdateThread thread running at any time
            BlockParallelThreads(Thread.CurrentThread);

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

                sb = SqlGenEngine.UpdateGenEngine(dtDatabaseSchema, sTableName, sInput);
                CreateGeneratedScriptFile(sb, sGenFile);

                if (sb.ToString().Contains("@@Warning:"))
                {
                    //Don't log a warning as error
                    A.SetTimerErrorIndicator(false, null, "See generated file for warning details");
                }
            }
            catch(ThreadAbortException ex)
            {
                if (!A.GetTimerCancelIndicator())
                {
                    //We got an unrequested Abort
                    A.SetTimerErrorIndicator(true, ex);
                }
            }
            catch(Exception ex)
            {
                A.SetTimerErrorIndicator(true, ex);
            }
            finally
            {
                if (ManagedThreadList.Contains(Thread.CurrentThread))
                {
                    ManagedThreadList.Remove(Thread.CurrentThread);
                }

                if (ManagedThreadList.Find(item => item.Name == UPDATE_GEN) == null)
                {
                    //There are no more active GenUpdateThread. Stop the timer
                    A.SetTimerThreadActive(false);
                }
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
			tsSearchString.Renderer = new ToolStripOverride();
            tsUpdateGen.Renderer = new ToolStripOverride();

            statusStrip1.Renderer = new ToolStripDropDownOverride();
			cmbCascadeOption.SelectedIndex = 0;
			cmbFilePattern.SelectedIndex = 0;
			cmbLoadDB.SelectedIndex = 0;
			LoadBaseDirectories();
			LoadSchemas();
			LoadSettings();

            new TimerObjectState(CASCADE_DELETE_GEN);
			new TimerObjectState(INSERT_GEN);
			new TimerObjectState(LOAD_SCHEMA);
			new TimerObjectState(DEVL);
			new TimerObjectState(SEARCH_STRING);
            new TimerObjectState(UPDATE_GEN);

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
                //CachedClient is empty
                if (ClientDictionary.Count == 1) throw new IOException();
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

                bool bUseCustomEnv = (bool)SettingManager.GetSettingValue(LOAD_SCHEMA, "Allow Custom Env");

                if (bUseCustomEnv)
                {
                    //Allow user to enter an environment
                    this.cmbLoadDB.SelectedIndex = -1;
                    this.cmbLoadEnv.DropDownStyle = ComboBoxStyle.Simple;
                    this.cmbLoadDB.DropDownStyle = ComboBoxStyle.Simple;
                    this.cmbLoadEnv.Text = "";
                    
                }
                else
                {
                    this.cmbLoadDB.SelectedIndex = 0;
                    this.cmbLoadEnv.DropDownStyle = ComboBoxStyle.DropDownList;
                    this.cmbLoadDB.DropDownStyle = ComboBoxStyle.DropDownList;
                    this.cmbLoadEnv.DataSource = dataSource3;
                }
			}
			catch (IOException)
			{
				tsSetup.Enabled = false;
				dataSource.Add(new Client() { CmbName = "-", Name = "", Code = "" });
                dataSource3.Add(new Client() { CmbName = "-", Name = "", Code = "" });
                this.cmbProcess.DataSource = dataSource;
				this.cmbGUI.DataSource = dataSource;
				this.cmbLoadEnv.DataSource = dataSource;
			}
			catch (Exception ex)
			{
                MessageBoxEx.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                if (!File.Exists(BaseDirectories.CacheClientBase))
                {
                    File.Create(BaseDirectories.CacheClientBase);
                    return false;
                }
                else
                {
                    DateTime FileDateModified = File.GetLastWriteTime(BaseDirectories.CacheClientBase);
                    DateTime CurrentDate = DateTime.Now;

                    if (FileDateModified.AddDays(30) < CurrentDate)
                    {
                        //Overwrite and clear the file
                        File.Create(BaseDirectories.CacheClientBase);
                        return false;
                    }
                }
            }
            catch
            {
                //Some security exception occurred when trying to create cached text file. Skip this functionality
                return false;
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

			this.cmbSchema_CascadeDel.DataSource = dataSource4;
			this.cmbSchema_CascadeDel.DisplayMember = "CmbName";
			this.cmbSchema_CascadeDel.ValueMember = null;//"Value";

            this.cmbSchema_Update.DataSource = dataSource4;
            this.cmbSchema_Update.DisplayMember = "CmbName";
            this.cmbSchema_Update.ValueMember = null;//"Value";
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
                    MessageBoxEx.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBoxEx.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBoxEx.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBoxEx.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
		private void CreateGeneratedScriptFile(StringBuilder sb, string GenFile, int iTopRowsToRemove = 0, int iBottomRowsToRemove = 0)
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

			using (FileStream stream = File.Open(BaseDirectories.QuickStartBase + "\\" + GenFile, FileMode.Create)){}
			using (var writer = new StreamWriter(BaseDirectories.QuickStartBase + "\\" + GenFile))
            {
                string s = sb.ToString();
                try
                {
                    //Remove top rows
                    for (int i = 1; i <= iTopRowsToRemove; i++)
                    {
                        s = s.Substring(s.IndexOf(Environment.NewLine) + 1);
                    }

                    //Remove bottom rows
                    for (int i = 1; i <= iBottomRowsToRemove; i++)
                    {
                        s = s.Remove(s.LastIndexOf(Environment.NewLine));
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        s = sb.ToString();

                        //Top and Bottom Row user defined settings no good. Use default
                        iTopRowsToRemove = (int)SettingManager.GetSettingDefaultValue(gridSettingsSearchString.Tag.ToString(), "Number of top lines to remove from SearchResults.txt");
                        iBottomRowsToRemove = (int)SettingManager.GetSettingDefaultValue(gridSettingsSearchString.Tag.ToString(), "Number of bottom lines to remove from SearchResults.txt");

                        //Remove top rows
                        for (int i = 1; i <= iTopRowsToRemove; i++)
                        {
                            s = s.Substring(s.IndexOf(Environment.NewLine) + 1);
                        }
                        //Remove bottom rows
                        for (int i = 1; i <= iBottomRowsToRemove; i++)
                        {
                            s = s.Remove(s.LastIndexOf(Environment.NewLine));
                        }
                    }
                    catch(Exception)
                    {
                        //Default failed. Skip entirely
                        s = sb.ToString();
                    }
                }
                writer.Write(s);
            }

			cmdProcess.Start();
			cmdProcess.StandardInput.WriteLine("cd " + BaseDirectories.QuickStartBase);
			cmdProcess.StandardInput.WriteLine("start " + GenFile);
			cmdProcess.StandardInput.WriteLine("exit");
			cmdProcess.WaitForExit();
		}
		private void StopCurrentThread(string sLaunchingTab)
		{
			TimerObjectState A = TimerObjectManager.Retrieve(sLaunchingTab);
			A.SetTimerCancelIndicator(true);

            //the problem is that this forcefully kills all findstr procs regardless of whether launched from QuickStart or elsewhere
            //a better design would be to setup a session id/proc id dictionary and stop all pqids for a given a session id
            if (sLaunchingTab == SEARCH_STRING)
            {
                foreach (Process proc in Process.GetProcessesByName("findstr"))
                {
                    proc.Kill();
                }
            }

            lock (ManagedThreadList)
            {
                foreach (Thread t in ManagedThreadList.ToList())
                 {
                    if (t.Name == sLaunchingTab)
                    {
                        t.Abort();
                        t.Join();
                        ManagedThreadList.Remove(t);
                    }
                }
            }
			A.SetTimerThreadActive(false);
		}
        #region EventHandlers
        private void frmQuickStart_Load(object sender, EventArgs e)
        {
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            panelSettings.Hide();
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
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
            errorProvider4.Clear();
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
            TimerObjectState A = TimerObjectManager.Retrieve(sNewTabName);

            TSlblStatus.Text = A.GetTimerStatusText();
            tsSuccessIcon.Visible = A.GetTimerSuccessIconVisible();
            tsErrorIcon.Visible = A.GetTimerErrorIconVisible();
            TSlblTime.Text = A.GetTimerText();
            UpdateErrorMessages(A);
            sCurrentTab = sNewTabName;
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sNewTabName1 = tabControl1.SelectedTab.Tag.ToString();
            string sNewTabName2 = tabControl2.SelectedTab.Tag.ToString();

            if (sNewTabName1 == SCRIPT_GEN)
            {
                TimerObjectState B = TimerObjectManager.Retrieve(sNewTabName2);
                TSlblStatus.Text = B.GetTimerStatusText();
                tsSuccessIcon.Visible = B.GetTimerSuccessIconVisible();
                tsErrorIcon.Visible = B.GetTimerErrorIconVisible();
                TSlblTime.Text = B.GetTimerText();
                UpdateErrorMessages(B);
                sCurrentTab = sNewTabName2;
            }
            else
            {
                TimerObjectState A = TimerObjectManager.Retrieve(sNewTabName1);
                TSlblStatus.Text = A.GetTimerStatusText();
                tsSuccessIcon.Visible = A.GetTimerSuccessIconVisible();
                tsErrorIcon.Visible = A.GetTimerErrorIconVisible();
                TSlblTime.Text = A.GetTimerText();
                UpdateErrorMessages(A);
                sCurrentTab = sNewTabName1;
            }
        }
        private void btnOpenFileDialog_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSearchPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void txtSearchPath_TextChanged(object sender, EventArgs e)
        {
            errorProvider5.Clear();
        }
        private void cmbCascadeOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = cmbCascadeOption.SelectedIndex;
            if (cmbCascadeOption.Text == "None")
            {
                txtFKName.ReadOnly = true;
                txtFKName.Text = String.Empty;
            }
            else
            {
                txtFKName.ReadOnly = false;
            }
        }
        private void cmbLoadEnv_TextUpdate(object sender, EventArgs e)
        {
            errorProvider6.Clear();
        }
        private void cmbLoadDB_TextUpdate(object sender, EventArgs e)
        {
            errorProvider7.Clear();
        }
        private void txtTableName_Update_TextChanged(object sender, EventArgs e)
        {
            errorProvider8.Clear();
        }
        private void tsGenerate_UpdateGen_Click(object sender, EventArgs e)
        {
            bool bSuccess = true;
            string sInputString = rttInput.Text;
            string sSchema = cmbSchema_Update.Text;
            string sTableName = txtTableName_Update.Text;
            string sTabName = tabControl2.SelectedTab.Tag.ToString();
            bool bProcessed = false;
            TimerObjectState A = TimerObjectManager.Retrieve(sTabName);

            //Error handling
            if (string.IsNullOrWhiteSpace(sTableName))
            {
                errorProvider8.SetError(txtTableName_Update, "Please Enter Table Name");
                bSuccess = false;
            }
            else
            {
                errorProvider8.SetError(txtTableName_Update, "");
            }
            if (cmbSchema_Update.SelectedIndex == -1)
            {
                errorProvider9.SetError(cmbSchema_Update, "Please Enter Schema. If no options are available, then submit a schema in Load Schema tab");
                bSuccess = false;
            }
            else
            {
                errorProvider9.SetError(cmbSchema_Update, "");
            }

            if (!bSuccess) return;

            if (chkSplit_UpdateGen.Checked)
            {
                if (StringUtility.IsStringOverMemory(sInputString))
                {
                    List<string> ListToProcess = StringUtility.SplitLargeString(sInputString);
                    int iFileNumber = 1;

                    foreach (string sInput in ListToProcess)
                    {
                        string sGenFile = "GeneratedUpdateScript" + iFileNumber.ToString() + ".txt";
                        GenUpdateThread = new Thread(() => GenUpdateThreadProcSafe(sInput, sTableName, sSchema, sGenFile, A));
                        GenUpdateThread.Start();
                        GenUpdateThread.Name = UPDATE_GEN;
                        ManagedThreadList.Add(GenUpdateThread);

                        bProcessed = true;
                        iFileNumber++;
                    }
                }
            }
            else
            {
                if (StringUtility.IsStringOverMemory(sInputString))
                {
                    if (MessageBoxEx.Show("Dataset is large and may cause memory issues.\n Recommendation is to check: Split To multiple files\n" + StringUtility.StringOverCapacity(sInputString).ToString() + "% over recommended capacity.\nContinue?",
                            "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            if (!bProcessed)
            {
                GenUpdateThread = new Thread(() => GenUpdateThreadProcSafe(sInputString, sTableName, sSchema, "GeneratedUpdateScript1.txt", A));
                GenUpdateThread.Start();
                GenUpdateThread.Name = UPDATE_GEN;
                ManagedThreadList.Add(GenUpdateThread);
            }

            A.SetTimerThreadActive(true);
            TimerThread = new Thread(() => TimerThreadProcSafe(A));
            TimerThread.Start();
        }
        private void tsStopGenerate_UpdateGen_Click(object sender, EventArgs e)
        {
            string sTabName = tabControl2.SelectedTab.Tag.ToString();
            StopCurrentThread(sTabName);
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
            try
            {
                string sSchema = cmbSchema_CascadeDel.Text;
                File.Delete(BaseDirectories.GeneratedSchemas + "\\" + GetSchemaFileName(sSchema));
                LoadSchemas();
                cmbSchema_CascadeDel.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cmbSchema_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete && cmbSchema_CascadeDel.DroppedDown)
                {
                    string sSchema = cmbSchema_CascadeDel.Text;
                    File.Delete(BaseDirectories.GeneratedSchemas + "\\" + GetSchemaFileName(sSchema));
                    LoadSchemas();
                    cmbSchema_CascadeDel.SelectedIndex = -1;

                    //Make sure no other processing happens
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Down && !cmbSchema_CascadeDel.DroppedDown)
                {
                    cmbSchema_CascadeDel.DroppedDown = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            TimerObjectState A = new TimerObjectState(sTabName);

            if (string.IsNullOrWhiteSpace(sTableName))
            {
                errorProvider1.SetError(txtTableName_Insert, "Please Enter Table Name");
                return;
            }
            else
            {
                errorProvider1.SetError(txtTableName_Insert, "");
            }


            if (chkSplit_InsertGen.Checked)
            {
                if (StringUtility.IsStringOverMemory(sInputString))
                {
                    List<string> ListToProcess = StringUtility.SplitLargeString(sInputString);
                    int iFileNumber = 1;

                    foreach (string sInput in ListToProcess)
                    {
                        string sGenFile = "GeneratedInsertScript" + iFileNumber.ToString() + ".txt";
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
                    if (MessageBoxEx.Show("Dataset is large and may cause memory issues.\n Recommendation is to check: Split To multiple files\n" + StringUtility.StringOverCapacity(sInputString).ToString() + "% over recommended capacity.\nContinue?",
                            "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            if (!bProcessed)
            {

                GenInsertThread = new Thread(() => GenInsertThreadProcSafe(sInputString, sIDInsert, sTableName, "GeneratedInsertScript1.txt", A));
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
            string sSchema = cmbSchema_CascadeDel.Text;
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
                if (cmbSchema_CascadeDel.SelectedIndex == -1)
                {
                    errorProvider2.SetError(cmbSchema_CascadeDel, "Please Enter Schema. If no options are available, then submit a schema in Load Schema tab");
                    bSuccess = false;
                }
                else
                {
                    errorProvider2.SetError(cmbSchema_CascadeDel, "");
                }
                if (!bSuccess) return;
            }

            //TimerThread = new Thread(new ThreadStart(this.TimerThreadProcSafe));
            TimerObjectState A = TimerObjectManager.Retrieve(sTabName);
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
            CreateGeneratedScriptFile(sb, GenFile);
        }
        private void tsSubmit_Schema_Click(object sender, EventArgs e)
        {
            //Error handling
            bool bSuccess = true;
            if (cmbLoadEnv.Text == "-" || string.IsNullOrWhiteSpace(cmbLoadEnv.Text))
            {
                errorProvider6.SetError(cmbLoadEnv, "Please Enter Environment. If no options are available, then allow custom environment in Settings");
                bSuccess = false;
            }
            if (string.IsNullOrWhiteSpace(cmbLoadDB.Text))
            {
                errorProvider7.SetError(cmbLoadDB, "Please Enter Database");
                bSuccess = false;
            }
            if (!bSuccess) return;
            string sTabName = tabControl2.SelectedTab.Tag.ToString();
            TimerObjectState A = TimerObjectManager.Retrieve(sTabName);

            try
            {
                A.SetTimerThreadActive(true);
                TimerThread = new Thread(() => TimerThreadProcSafe(A));

                TimerThread.Start();

                if (string.IsNullOrWhiteSpace(rttInput.Text))
                {
                    A.SetTimerErrorIndicator(true, null, "Please submit schema information by following these steps: \r\n 1) Click Generate \r\n 2) Copy/paste sql into SQL Server (Oracle not supported) \r\n 3) Copy results with headers into QuickStart \r\n 4) Click Submit");
                    A.SetTimerThreadActive(false);
                    return;
                }
                bool bUseCustomEnv = (bool)SettingManager.GetSettingValue(LOAD_SCHEMA, "Allow Custom Env");
                string sSchemaFileName = String.Empty;

                if (bUseCustomEnv)
                {
                    sSchemaFileName = cmbLoadEnv.Text + "_" + cmbLoadDB.Text + ".txt";
                }
                else
                {
                    Client SelectedClient = (Client)cmbLoadEnv.SelectedValue;
                    sSchemaFileName = SelectedClient.Name + "_" + SelectedClient.Code + "_" + cmbLoadDB.Text + ".txt";

                }
                using (FileStream stream = File.Open(BaseDirectories.GeneratedSchemas + "\\" + sSchemaFileName, FileMode.Create))
                {
                }
                using (var writer = new StreamWriter(BaseDirectories.GeneratedSchemas + "\\" + sSchemaFileName))
                {
                    writer.Write(rttInput.Text);
                }
                LoadSchemas();
                //MessageBox.Show("Successfully uploaded schema", "Success",MessageBoxButtons.OK, MessageBoxIcon.Information);


                A.SetTimerErrorIndicator(false, null, "Successfully uploaded schema");
                errorProvider2.Clear();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                A.SetTimerErrorIndicator(true, ex);
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
            MessageBoxEx.Show(String.Join("\n", Log), "Log");
        }
        private void tsSearch_Click(object sender, EventArgs e)
        {
            //Error handling
            bool bSuccess = true;

            if (string.IsNullOrWhiteSpace(txtSearchString.Text))
            {
                errorProvider4.SetError(txtSearchString, "Please Enter Search String");
                bSuccess = false;
            }
            else
            {
                errorProvider4.SetError(txtSearchString, "");
            }

            try
            {
                if (Path.GetPathRoot(txtSearchPath.Text) == string.Empty)
                {
                    errorProvider5.SetError(btnOpenFileDialog, "Search Path must be a valid path");
                    bSuccess = false;
                }
                else
                {
                    errorProvider5.SetError(btnOpenFileDialog, "");
                }
            }
            catch (Exception)
            {
                errorProvider5.SetError(btnOpenFileDialog, "Search Path must be a valid path");
                bSuccess = false;
            }

            if (!bSuccess) return;
            string sTabName = tabControl1.SelectedTab.Tag.ToString();

            TimerObjectState A = TimerObjectManager.Retrieve(sTabName);
            A.SetTimerThreadActive(true);

            TimerThread = new Thread(() => TimerThreadProcSafe(A));
            TimerThread.Start();

            string SearchPath = txtSearchPath.Text;
            string SearchString = txtSearchString.Text;
            string FilePattern = cmbFilePattern.Text;
            SearchStringThread = new Thread(() => SearchStringThreadProcSafe(SearchPath, SearchString, FilePattern, A));
            SearchStringThread.Start();
            SearchStringThread.Name = SEARCH_STRING;
            ManagedThreadList.Add(SearchStringThread);
        }
        #endregion EventHandlers


    }
}
