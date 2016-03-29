using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickStart
{
	public static class ThreadHelperUtility
	{
		delegate void SetEnableCallback(Form Form, Control Control, bool bEnable);
		delegate void SetTextCallback(Form Form, Control Control, string sText);
		delegate void SetTextCallbackStatusStrip(Form Form, StatusStrip StatusStrip, ToolStripStatusLabel ToolStripStatusLabel, string sText);

		public static void SetEnable(Form Form, Control Control, bool bEnable)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (Control.InvokeRequired)
			{
				SetEnableCallback d = new SetEnableCallback(SetEnable);
				Form.Invoke(d, new object[] { Form, Control, bEnable });
			}
			else
			{
				Control.Enabled = bEnable;
			}
		}
		public static void SetText(Form Form, Control Control, string sText)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (Control.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(SetText);
				Form.Invoke(d, new object[] { Form, Control, sText });
			}
			else
			{
				Control.Text = sText;
			}
		}
		public static void SetText(Form Form, StatusStrip StatusStrip, ToolStripStatusLabel ToolStripStatusLabel, string sText)
		{
			// InvokeRequired required compares the thread ID of the
			// calling thread to the thread ID of the creating thread.
			// If these threads are different, it returns true.
			if (StatusStrip.InvokeRequired)
			{
				SetTextCallbackStatusStrip d = new SetTextCallbackStatusStrip(SetText);
				Form.Invoke(d, new object[] { Form, StatusStrip, ToolStripStatusLabel, sText });
			}
			else
			{
				for (int i = 0; i <= StatusStrip.Items.Count; i++)
				{
					if (StatusStrip.Items[i].Name == ToolStripStatusLabel.Name)
					{
						StatusStrip.Items[i].Text = sText;
						break;
					}
				}
			}
		}
	}
}
