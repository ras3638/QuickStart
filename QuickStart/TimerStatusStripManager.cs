using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickStart
{
	public static class TimerObjectState
	{
		private static Dictionary<string,TimerObjectManager> ThreadObjectManagerDictionary = new  Dictionary<string,TimerObjectManager>();
	
		public static TimerObjectManager Retrieve(string sSourceName)
		{
			return ThreadObjectManagerDictionary[sSourceName];
		}
		public static void SaveStatus(TimerObjectManager x)
		{
			if (ThreadObjectManagerDictionary.ContainsKey(x.GetName()))
			{
				ThreadObjectManagerDictionary[x.GetName()] = x;
			}
			else
			{
				ThreadObjectManagerDictionary.Add(x.GetName(), x);
			}
		}
	}
	public class TimerObjectManager
	{
		bool bTimerCancelIndicator { get; set; }
		bool bTimerErrorIndicator { get; set; }
		bool bTimerThreadActive { get; set; }
		bool bPauseTimerIndicator { get; set; }

		bool bTimerErrorIconVisible { get; set; }
		bool bTimerSuccessIconVisible { get; set; }
		bool bTimerExecuteIconEnable { get; set; }
		bool bTimerStopIconEnable { get; set; }
		string sTimerStatusText { get; set; }
		string sTimerText { get; set; } 
		string sSourceName {get; set; }

        Exception exErrorException { get; set; }
        string sCustomMessage { get; set; }

        string sMessages { get; set; }
        public TimerObjectManager(string sSourceName)
		{
			this.sSourceName = sSourceName;
			bTimerCancelIndicator = false;
			bTimerErrorIndicator = false;
			bTimerThreadActive = false;

			bTimerErrorIconVisible = false;
			bTimerSuccessIconVisible = false;
			bTimerExecuteIconEnable = true;
			bTimerStopIconEnable = false;
			bPauseTimerIndicator = false;

            sTimerStatusText = "Status: N/A";
			sTimerText = "00:00:00";

            sCustomMessage = null;
            exErrorException = null;
            SaveStatus();
		}

		//// Getters ////

		public string GetName()
		{
			return sSourceName;
		}
		public bool GetTimerCancelIndicator()
		{
			UpdateStatus();
			return bTimerCancelIndicator;
		}
		public bool GetTimerErrorIndicator()
		{
			UpdateStatus();
			return bTimerErrorIndicator;
		}
		public bool GetPauseTimerIndicator()
		{
			UpdateStatus();
			return bPauseTimerIndicator;
		}
		public bool GetTimerThreadActive()
		{
			UpdateStatus();
			return bTimerThreadActive;
		}
		public bool GetTimerErrorIconVisible()
		{
			UpdateStatus();
			return bTimerErrorIconVisible;
		}
		public bool GetTimerSuccessIconVisible()
		{
			UpdateStatus();
			return bTimerSuccessIconVisible;
		}
		public bool GetTimerExecuteIconEnable()
		{
			UpdateStatus();
			return bTimerExecuteIconEnable;
		}
		public bool GetTimerStopIconEnable()
		{
			UpdateStatus();
			return bTimerStopIconEnable;
		}
		public string GetTimerStatusText()
		{
			UpdateStatus();
			return sTimerStatusText;
		}
		public string GetTimerText()
		{
			UpdateStatus();
			return sTimerText;
		}
        public Exception GetErrorException()
        {
            UpdateStatus();
            return exErrorException;
        }
        public string GetCustomMessage()
        {
            UpdateStatus();
            return sCustomMessage;
        }
        //// Setters ////
        public void SetCustomMessage(string sCustomMessage)
        {
            this.sCustomMessage = sCustomMessage;
            SaveStatus();
        }
        public void SetErrorException(Exception ex)
        {
            this.exErrorException = ex;
            SaveStatus();
        }
        public void SetTimerCancelIndicator(bool bTimerCancelIndicator)
		{
			this.bTimerCancelIndicator = bTimerCancelIndicator;
			SaveStatus();
		}
		public void SetTimerErrorIndicator(bool bTimerErrorIndicator, Exception ex = null, string sCustomMessage = null)
		{
			this.bTimerErrorIndicator = bTimerErrorIndicator;
            this.exErrorException = ex;
            this.sCustomMessage = sCustomMessage;
			SaveStatus();
		}
        public void SetMessages(string sMessages)
        {
            this.sMessages = sMessages;
            SaveStatus();
        }
		public void SetTimerThreadActive(bool bKeepActive)
		{
			this.bTimerThreadActive = bKeepActive;
			//reset status indicator when restarting the timer
			if (bKeepActive)
			{
				this.bTimerErrorIndicator = false;
				this.bTimerCancelIndicator = false;
				this.bPauseTimerIndicator = false;
                this.exErrorException = null;
                this.sCustomMessage = null;
			}
			SaveStatus();
		}
		public void SetPauseTimerInidicator(bool bPauseTimerIndicator)
		{
			this.bPauseTimerIndicator = bPauseTimerIndicator;
			SaveStatus();
		}
		public void SetTimerErrorIconVisible(bool bTimerErrorIconVisible)
		{
			this.bTimerErrorIconVisible = bTimerErrorIconVisible;
			SaveStatus();
		}
		public void SetTimerSuccessIconVisible(bool bTimerSuccessIconVisible)
		{
			this.bTimerSuccessIconVisible = bTimerSuccessIconVisible;
			SaveStatus();
		}
		public void SetTimerExecuteIconEnable(bool bTimerExecuteIconEnable)
		{
			this.bTimerExecuteIconEnable = bTimerExecuteIconEnable;
			SaveStatus();
		}
		public void SetTimerStopIconEnable(bool bTimerStopIconEnable)
		{
			this.bTimerStopIconEnable = bTimerStopIconEnable;
			SaveStatus();
		}
		public void SetTimerStatusText(string sTimerStatusText)
		{
			this.sTimerStatusText = sTimerStatusText;
			SaveStatus();
		}
		public void SetTimerText(string sTimerText)
		{
			this.sTimerText = sTimerText;
			SaveStatus();
		}
		private void SaveStatus()
		{
			TimerObjectState.SaveStatus(this);
		}
		private void UpdateStatus()
		{
			TimerObjectManager A = TimerObjectState.Retrieve(this.GetName());

			this.bTimerCancelIndicator = A.bTimerCancelIndicator;
			this.bTimerErrorIndicator = A.bTimerErrorIndicator;
			this.bTimerThreadActive = A.bTimerThreadActive;
			this.bPauseTimerIndicator = A.bPauseTimerIndicator;

			this.bTimerSuccessIconVisible = A.bTimerSuccessIconVisible;
			this.bTimerErrorIconVisible = A.bTimerErrorIconVisible;
			this.bTimerExecuteIconEnable = A.bTimerExecuteIconEnable;
			this.bTimerStopIconEnable = A.bTimerStopIconEnable;
			this.sTimerStatusText = A.sTimerStatusText;
			this.sTimerText = A.sTimerText;

            this.exErrorException = A.exErrorException;
            this.sCustomMessage = A.sCustomMessage;

			SaveStatus();
		}
	}
	
}

