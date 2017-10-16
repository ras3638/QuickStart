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
        //This utility class helps update form objects across threads

		delegate void SetEnableCallbackToolStrip(Form Form, ToolStrip ToolStrip, ToolStripButton ToolStripStatusButton, bool bEnable);
		delegate void SetTextCallback(Form Form, Control Control, string sText);
		delegate void SetEnableCallback(Form Form, Control Control, bool bEnable);
        delegate void SetVisibleCallback(Form Form, StatusStrip StatusStrip, ToolStripStatusLabel ToolStripStatusLabel, bool bVisible);
        delegate void SetVisibleCallbackStatusStripDropDownItem(Form Form, StatusStrip StatusStrip, ToolStripDropDownButton ToolStripStatusDropDownButton, ToolStripItem ToolStripStatusItem, bool bVisible);
        delegate void SetTextCallbackStatusStripLabel(Form Form, StatusStrip StatusStrip, ToolStripStatusLabel ToolStripStatusLabel, string sText);
        delegate void SetTextCallbackStatusStripDropDown(Form Form, StatusStrip StatusStrip, ToolStripDropDownButton ToolStripStatusDropDownButton, string sText);
        delegate void SetTextCallbackStatusStripDropDownItem(Form Form, StatusStrip StatusStrip, ToolStripDropDownButton ToolStripStatusDropDownButton, ToolStripItem ToolStripStatusItem, string sText);

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
        public static void SetVisible(Form Form, StatusStrip StatusStrip, ToolStripDropDownButton ToolStripStatusDropDownButton, ToolStripItem ToolStripStatusItem, bool bVisible)
        {
            if (StatusStrip.InvokeRequired)
            {
                SetVisibleCallbackStatusStripDropDownItem d = new SetVisibleCallbackStatusStripDropDownItem(SetVisible);
                Form.Invoke(d, new object[] { Form, StatusStrip, ToolStripStatusDropDownButton, ToolStripStatusItem, bVisible });
            }
            else
            {
                ToolStripStatusItem.Visible = bVisible;
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
            //Container Flow: ToolStrip -> ToolStripStatusButton
            //Updates text of a toolstrip button across threads. 
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
            //Container Flow: StatusStrip -> ToolStripStatusLabel
            //Updates text of a toolstrip label across threads. 
            if (StatusStrip.InvokeRequired)
			{
				SetTextCallbackStatusStripLabel d = new SetTextCallbackStatusStripLabel(SetText);
				Form.Invoke(d, new object[] { Form, StatusStrip, ToolStripStatusLabel, sText });
			}
			else
			{
				for (int i = 0; i < StatusStrip.Items.Count; i++)
				{
					if (StatusStrip.Items[i].Name == ToolStripStatusLabel.Name)
					{
						StatusStrip.Items[i].Text = sText;
						break;
					}
				}
			}
		}
        public static void SetText(Form Form, StatusStrip StatusStrip, ToolStripDropDownButton ToolStripStatusDropDown, string sText)
        {
            //Container Flow: StatusStrip -> ToolStripStatusLabel
            //Updates text of a toolstrip label across threads. 
            if (StatusStrip.InvokeRequired)
            {
                SetTextCallbackStatusStripDropDown d = new SetTextCallbackStatusStripDropDown(SetText);
                Form.Invoke(d, new object[] { Form, StatusStrip, ToolStripStatusDropDown, sText });
            }
            else
            {
                for (int i = 0; i < StatusStrip.Items.Count; i++)
                {
                    if (StatusStrip.Items[i].Name == ToolStripStatusDropDown.Name)
                    {
                        StatusStrip.Items[i].Text = sText;
                        break;
                    }
                }
            }
        }
        public static void SetText(Form Form, StatusStrip StatusStrip, ToolStripDropDownButton ToolStripStatusDropDownButton, ToolStripItem ToolStripStatusItem, string sText)
        {
            //Container Flow: StatusStrip -> ToolStripDropDownButton -> ToolStripItem
            //Update text of a toolstrip item (ex: textbox) contained in a toolstrip dropdown across threads
            if (StatusStrip.InvokeRequired)
            {
                SetTextCallbackStatusStripDropDownItem d = new SetTextCallbackStatusStripDropDownItem(SetText);
                Form.Invoke(d, new object[] { Form, StatusStrip, ToolStripStatusDropDownButton, ToolStripStatusItem, sText });
            }
            else
            {
                for (int i = 0; i < StatusStrip.Items.Count; i++)
                {
                    if (StatusStrip.Items[i].Name == ToolStripStatusDropDownButton.Name)
                    {
                        for(int j = 0; j < ToolStripStatusDropDownButton.DropDownItems.Count; j++)
                        {
                            if (ToolStripStatusDropDownButton.DropDownItems[j].Name == ToolStripStatusItem.Name)
                            {
                                ToolStripStatusDropDownButton.DropDownItems[j].Text = sText;
                                break;
                            }
                         }
                    }
                }
            }
        }
    }
}
