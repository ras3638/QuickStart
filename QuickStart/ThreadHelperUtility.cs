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
		delegate void SetEnableCallbackToolStrip(Form Form, ToolStrip ToolStrip, ToolStripButton ToolStripStatusButton, bool bEnable);
		delegate void SetTextCallback(Form Form, Control Control, string sText);
		delegate void SetEnableCallback(Form Form, Control Control, bool bEnable);
		delegate void SetVisibleCallback(Form Form, StatusStrip StatusStrip, ToolStripStatusLabel ToolStripStatusLabel, bool bVisible);
		delegate void SetTextCallbackStatusStrip(Form Form, StatusStrip StatusStrip, ToolStripStatusLabel ToolStripStatusLabel, string sText);

		public static void SetEnable(Form Form, Control Control, bool bEnable)
		{
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

		public static void SetVisible(Form Form, StatusStrip StatusStrip, ToolStripStatusLabel ToolStripStatusLabel, bool bVisible)
		{
			if (StatusStrip.InvokeRequired)
			{
				SetVisibleCallback d = new SetVisibleCallback(SetVisible);
				Form.Invoke(d, new object[] { Form, StatusStrip, ToolStripStatusLabel, bVisible });
			}
			else
			{
				ToolStripStatusLabel.Visible = bVisible;
			}
		}
		public static void SetText(Form Form, Control Control, string sText)
		{
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
		public static void SetEnable(Form Form, ToolStrip ToolStrip, ToolStripButton ToolStripStatusButton, bool bEnable)
		{
			if (ToolStrip.InvokeRequired)
			{
				SetEnableCallbackToolStrip d = new SetEnableCallbackToolStrip(SetEnable);
				Form.Invoke(d, new object[] { Form, ToolStrip, ToolStripStatusButton, bEnable });
			}
			else
			{
				ToolStripStatusButton.Enabled = bEnable;
			}
		}
		public static void SetText(Form Form, StatusStrip StatusStrip, ToolStripStatusLabel ToolStripStatusLabel, string sText)
		{
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
