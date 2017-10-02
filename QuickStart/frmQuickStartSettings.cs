using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickStart
{
	public partial class frmQuickStart : Form
	{
		private void LoadSettings()
		{
			//All new settings must be defined here

			//to_char(to_date(updt_date,'dd/Month/yy'),'dd-Month-yyyy')

			DataGridViewRow row1 = (DataGridViewRow)gridSettingsInsGen.RowTemplate.Clone();
			SettingManager.AddSetting(gridSettingsInsGen.Tag.ToString(), "Max Byte Threshold", (long)12000000);
			//SettingManager.OverwriteDefaultType(sTabInsertGen, "Max Byte Threshold", typeof(long));
			row1.CreateCells(gridSettingsInsGen, "Max Byte Threshold", 12000000);
			gridSettingsInsGen.Rows.Add(row1);

			DataGridViewRow row7 = (DataGridViewRow)gridSettingsInsGen.RowTemplate.Clone();
			SettingManager.AddSetting(gridSettingsInsGen.Tag.ToString(), "Enable Oracle Functionality", false);
			row7.CreateCells(gridSettingsInsGen, "Enable Oracle Functionality", false);
			gridSettingsInsGen.Rows.Add(row7);

			DataGridViewRow row8 = (DataGridViewRow)gridSettingsInsGen.RowTemplate.Clone();
			SettingManager.AddSetting(gridSettingsInsGen.Tag.ToString(), "Enable Excel Functionality", false);
			row8.CreateCells(gridSettingsInsGen, "Enable Excel Functionality", false);
			gridSettingsInsGen.Rows.Add(row8);

			DataGridViewRow row2 = (DataGridViewRow)gridSettingsCascadeDel.RowTemplate.Clone();
			SettingManager.AddSetting(gridSettingsCascadeDel.Tag.ToString(), "Multiline support for None cascade option", false);
			row2.CreateCells(gridSettingsCascadeDel, "Multiline support for None cascade option", false);
			gridSettingsCascadeDel.Rows.Add(row2);

            DataGridViewRow row3 = (DataGridViewRow)gridSettingsSearchString.RowTemplate.Clone();
            SettingManager.AddSetting(gridSettingsSearchString.Tag.ToString(), "Number of top lines to remove from SearchResults.txt", (int)6);
            row3.CreateCells(gridSettingsSearchString, "Number of top lines to remove from SearchResults.txt", 6);
            gridSettingsSearchString.Rows.Add(row3);

            DataGridViewRow row4 = (DataGridViewRow)gridSettingsSearchString.RowTemplate.Clone();
            SettingManager.AddSetting(gridSettingsSearchString.Tag.ToString(), "Number of bottom lines to remove from SearchResults.txt", (int)2);
            row4.CreateCells(gridSettingsSearchString, "Number of bottom lines to remove from SearchResults.txt", 2);
            gridSettingsSearchString.Rows.Add(row4);

            //DataGridViewRow row5 = (DataGridViewRow)gridSettingsQFC.RowTemplate.Clone();
            //SettingManager.AddSetting(gridSettingsQFC.Tag.ToString(), "QFC 16.0 Zip Path", "C:\\Devl\\Products\\Upstream\\Main\\Dependencies\\Zip Files");
            //row5.CreateCells(gridSettingsQFC, "QFC 16.0 Zip Path", "C:\\Devl\\Products\\Upstream\\Main\\Dependencies\\Zip Files");
            //gridSettingsQFC.Rows.Add(row5);

            DataGridViewRow row6 = (DataGridViewRow)gridSettingsSearchString.RowTemplate.Clone();
			SettingManager.AddSetting(gridSettingsSearchString.Tag.ToString(), "Enable Zip Functionality", true);
			row6.CreateCells(gridSettingsSearchString, "Enable Zip Functionality", true);
			gridSettingsSearchString.Rows.Add(row6);

            DataGridViewRow row9 = (DataGridViewRow)gridSettingsLoadSchema.RowTemplate.Clone();
            SettingManager.AddSetting(gridSettingsLoadSchema.Tag.ToString(), "Allow Custom Env", false);
            row9.CreateCells(gridSettingsLoadSchema, "Allow Custom Env", false);
            gridSettingsLoadSchema.Rows.Add(row9);

			gridSettingsInsGen.TopLeftHeaderCell.Value = "v";
			gridSettingsCascadeDel.TopLeftHeaderCell.Value = "v";
			gridSettingsSearchString.TopLeftHeaderCell.Value = "v";
            gridSettingsLoadSchema.TopLeftHeaderCell.Value = "v";

			//Fills the row headers
			gridSettingsInsGen.DefaultCellStyle.SelectionBackColor = gridSettingsInsGen.DefaultCellStyle.BackColor;
			gridSettingsInsGen.DefaultCellStyle.SelectionForeColor = gridSettingsInsGen.DefaultCellStyle.ForeColor;

			gridSettingsCascadeDel.DefaultCellStyle.SelectionBackColor = gridSettingsCascadeDel.DefaultCellStyle.BackColor;
			gridSettingsCascadeDel.DefaultCellStyle.SelectionForeColor = gridSettingsCascadeDel.DefaultCellStyle.ForeColor;

			gridSettingsSearchString.DefaultCellStyle.SelectionBackColor = gridSettingsSearchString.DefaultCellStyle.BackColor;
			gridSettingsSearchString.DefaultCellStyle.SelectionForeColor = gridSettingsSearchString.DefaultCellStyle.ForeColor;

            gridSettingsLoadSchema.DefaultCellStyle.SelectionBackColor = gridSettingsLoadSchema.DefaultCellStyle.BackColor;
            gridSettingsLoadSchema.DefaultCellStyle.SelectionForeColor = gridSettingsLoadSchema.DefaultCellStyle.ForeColor;

			//Removes the grid selection arrows
			gridSettingsInsGen.RowHeadersDefaultCellStyle.Padding = new Padding(this.gridSettingsInsGen.RowHeadersWidth);
			gridSettingsCascadeDel.RowHeadersDefaultCellStyle.Padding = new Padding(this.gridSettingsCascadeDel.RowHeadersWidth);
			gridSettingsSearchString.RowHeadersDefaultCellStyle.Padding = new Padding(this.gridSettingsSearchString.RowHeadersWidth);
            gridSettingsLoadSchema.RowHeadersDefaultCellStyle.Padding = new Padding(this.gridSettingsLoadSchema.RowHeadersWidth);
			//gridSettingsCascadeDel.EnableHeadersVisualStyles = false;

			//GridAutoSize();
		}
		private void GridAutoSize()
		{
			int iVisibleRows = 0;
			int iGridHeight = gridSettingsInsGen.ColumnHeadersHeight;
			foreach (DataGridViewRow r in gridSettingsInsGen.Rows)
			{
				if (r.Visible) iVisibleRows++;

				if (iVisibleRows > 0)
				{
					iGridHeight += gridSettingsInsGen.Rows[iVisibleRows - 1].Height;
				}
				gridSettingsCascadeDel.Location = new Point(gridSettingsInsGen.Location.X, gridSettingsInsGen.Location.Y + iGridHeight);
			}

			iVisibleRows = 0;
			iGridHeight = gridSettingsCascadeDel.ColumnHeadersHeight;
			foreach (DataGridViewRow row in gridSettingsCascadeDel.Rows)
			{
				if (row.Visible) iVisibleRows++;

				if (iVisibleRows > 0)
				{
					iGridHeight += gridSettingsCascadeDel.Rows[iVisibleRows - 1].Height;
				}
				gridSettingsSearchString.Location = new Point(gridSettingsCascadeDel.Location.X, gridSettingsCascadeDel.Location.Y + iGridHeight);
			}

            iVisibleRows = 0;
            iGridHeight = gridSettingsSearchString.ColumnHeadersHeight;
            foreach (DataGridViewRow row in gridSettingsSearchString.Rows)
            {
                if (row.Visible) iVisibleRows++;

                if (iVisibleRows > 0)
                {
                    iGridHeight += gridSettingsSearchString.Rows[iVisibleRows - 1].Height;
                }
                gridSettingsLoadSchema.Location = new Point(gridSettingsSearchString.Location.X, gridSettingsSearchString.Location.Y + iGridHeight);
            }

            //Add extra height to grid to avoid vertical scrollbar from appearing
            AddExtraDataGridHeight(gridSettingsInsGen);
            AddExtraDataGridHeight(gridSettingsSearchString);
            AddExtraDataGridHeight(gridSettingsCascadeDel);
            AddExtraDataGridHeight(gridSettingsLoadSchema);

		}
        private void AddExtraDataGridHeight(DataGridView grid)
        {
            int iTotalRowHeight = grid.ColumnHeadersHeight;

            foreach (DataGridViewRow row in grid.Rows)
                iTotalRowHeight += row.Height;

            grid.Height = iTotalRowHeight;
            grid.Height += 10;
        }
		private void GridAutoSize(string sFromTab)
		{
			//Collapse Insert Gen section
			if (sFromTab != gridSettingsInsGen.Tag.ToString())
			{
				for (int i = 0; i < gridSettingsInsGen.Rows.Count; i++)
				{
					gridSettingsInsGen.Rows[i].Visible = false;
				}
				gridSettingsInsGen.TopLeftHeaderCell.Value = ">";
			}
			if (sFromTab != gridSettingsCascadeDel.Tag.ToString())
			{
				for (int i = 0; i < gridSettingsCascadeDel.Rows.Count; i++)
				{
					gridSettingsCascadeDel.Rows[i].Visible = false;
				}
				gridSettingsCascadeDel.TopLeftHeaderCell.Value = ">";
			}
			if (sFromTab != gridSettingsSearchString.Tag.ToString())
			{
				for (int i = 0; i < gridSettingsSearchString.Rows.Count; i++)
				{
					gridSettingsSearchString.Rows[i].Visible = false;
				}
				gridSettingsSearchString.TopLeftHeaderCell.Value = ">";
			}
            if (sFromTab != gridSettingsLoadSchema.Tag.ToString())
            {
                for (int i = 0; i < gridSettingsLoadSchema.Rows.Count; i++)
                {
                    gridSettingsLoadSchema.Rows[i].Visible = false;
                }
                gridSettingsLoadSchema.TopLeftHeaderCell.Value = ">";
            }
			////Populate Cascade Delete Section
			if (sFromTab == gridSettingsCascadeDel.Tag.ToString())
			{
				for (int i = 0; i < gridSettingsCascadeDel.Rows.Count; i++)
				{
					gridSettingsCascadeDel.Rows[i].Visible = true;
				}
				gridSettingsCascadeDel.TopLeftHeaderCell.Value = "v";
			}
			//Populate Insert Gen Section
			if (sFromTab == gridSettingsInsGen.Tag.ToString())
			{
				for (int i = 0; i < gridSettingsInsGen.Rows.Count; i++)
				{
					gridSettingsInsGen.Rows[i].Visible = true;
				}
				gridSettingsInsGen.TopLeftHeaderCell.Value = "v";
			}
			//Populate QFC Section
			if (sFromTab == gridSettingsSearchString.Tag.ToString())
			{
				for (int i = 0; i < gridSettingsSearchString.Rows.Count; i++)
				{
					gridSettingsSearchString.Rows[i].Visible = true;
				}
				gridSettingsSearchString.TopLeftHeaderCell.Value = "v";
			}
            //Populate Load Schema Section
            if (sFromTab == gridSettingsLoadSchema.Tag.ToString())
            {
                for (int i = 0; i < gridSettingsLoadSchema.Rows.Count; i++)
                {
                    gridSettingsLoadSchema.Rows[i].Visible = true;
                }
                gridSettingsLoadSchema.TopLeftHeaderCell.Value = "v";
            }

			GridAutoSize();
		}
		private void SaveSettings()
		{
			this.Validate();
			gridSettingsInsGen.EndEdit();
			gridSettingsCascadeDel.EndEdit();
			gridSettingsSearchString.EndEdit();
            gridSettingsLoadSchema.EndEdit();

			foreach (DataGridViewRow row in gridSettingsInsGen.Rows)
			{
				SettingManager.UpdateSetting(gridSettingsInsGen.Tag.ToString(), row.Cells[0].Value.ToString(), row.Cells[1].Value);
			}
			foreach (DataGridViewRow row in gridSettingsCascadeDel.Rows)
			{
				SettingManager.UpdateSetting(gridSettingsCascadeDel.Tag.ToString(), row.Cells[0].Value.ToString(), row.Cells[1].Value);
			}
			foreach (DataGridViewRow row in gridSettingsSearchString.Rows)
			{
				SettingManager.UpdateSetting(gridSettingsSearchString.Tag.ToString(), row.Cells[0].Value.ToString(), row.Cells[1].Value);
			}
            foreach (DataGridViewRow row in gridSettingsLoadSchema.Rows)
            {
                SettingManager.UpdateSetting(gridSettingsLoadSchema.Tag.ToString(), row.Cells[0].Value.ToString(), row.Cells[1].Value);
            }
		}

		private void gridSettings_Click(object sender, EventArgs e)
		{
			MouseEventArgs args = (MouseEventArgs)e;
			DataGridView dgv = (DataGridView)sender;
			DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);
			if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
			{
				for (int i = 0; i < gridSettingsInsGen.Rows.Count; i++)
				{
					gridSettingsInsGen.Rows[i].Visible = (gridSettingsInsGen.Rows[i].Visible == true) ? false : true;
				}

				gridSettingsInsGen.TopLeftHeaderCell.Value = (gridSettingsInsGen.Rows[0].Visible == true) ? "v" : ">";
				GridAutoSize();
			}
		}
		private void gridSettingsCascadeDel_Click(object sender, EventArgs e)
		{
			MouseEventArgs args = (MouseEventArgs)e;
			DataGridView dgv = (DataGridView)sender;
			DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);
			if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
			{
				for (int i = 0; i < gridSettingsCascadeDel.Rows.Count; i++)
				{
					gridSettingsCascadeDel.Rows[i].Visible = (gridSettingsCascadeDel.Rows[i].Visible == true) ? false : true;
				}

				gridSettingsCascadeDel.TopLeftHeaderCell.Value = (gridSettingsCascadeDel.Rows[0].Visible == true) ? "v" : ">";
				GridAutoSize();
			}
		}
		private void tsSettingsReset_Click(object sender, EventArgs e)
		{
			SaveSettings();
			foreach (DataGridViewRow row in gridSettingsCascadeDel.Rows)
			{
				string sValue = SettingManager.GetSettingDefaultValue(gridSettingsCascadeDel.Tag.ToString(), row.Cells[0].Value.ToString()).ToString();
				row.Cells[1].Value = sValue;
			}
			foreach (DataGridViewRow row in gridSettingsInsGen.Rows)
			{
				string sValue = SettingManager.GetSettingDefaultValue(gridSettingsInsGen.Tag.ToString(), row.Cells[0].Value.ToString()).ToString();
				row.Cells[1].Value = sValue;
			}
			foreach (DataGridViewRow row in gridSettingsSearchString.Rows)
			{
				string sValue = SettingManager.GetSettingDefaultValue(gridSettingsSearchString.Tag.ToString(), row.Cells[0].Value.ToString()).ToString();
				row.Cells[1].Value = sValue;
			}
		}
		private void tsSettingsQFC_Click(object sender, EventArgs e)
		{
			ShowSettings(tsSettingsSearchString.Tag.ToString());
		}

		private void gridSettingsSearchString_SelectionChanged(object sender, EventArgs e)
		{
			gridSettingsSearchString.ClearSelection();
		}
		private void tsSettingsGenerate_Insert_Click(object sender, EventArgs e)
		{
			ShowSettings(tsSettingsGenerate_Insert.Tag.ToString());
		}
        private void tsSettingsLoad_Schema_Click(object sender, EventArgs e)
        {
            ShowSettings(tsSettingsLoad_Schema.Tag.ToString());
        }
		private void tsSettingsReturn_Click(object sender, EventArgs e)
		{
			panelMainTab.Show();
			panelSettings.Hide();
			SaveSettings();

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

		private void gridSettingsInsGen_SelectionChanged(object sender, EventArgs e)
		{
			gridSettingsInsGen.ClearSelection();
		}

		private void gridSettingsCascadeDel_SelectionChanged(object sender, EventArgs e)
		{
			gridSettingsCascadeDel.ClearSelection();
		}
        private void gridSettingsLoadSchema_SelectionChanged(object sender, EventArgs e)
        {
            gridSettingsLoadSchema.ClearSelection();
        }

		private void tsSettingsCascade_Delete_Click(object sender, EventArgs e)
		{
			ShowSettings(tsSettingsCascade_Delete.Tag.ToString());
		}

		private void ShowSettings(string sFromTab)
		{
			panelMainTab.Hide();
			GridAutoSize(sFromTab);
			panelSettings.Show();
		}

		private void gridSettingsSearchString_Click(object sender, EventArgs e)
		{
			MouseEventArgs args = (MouseEventArgs)e;
			DataGridView dgv = (DataGridView)sender;
			DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);
			if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
			{
				for (int i = 0; i < gridSettingsSearchString.Rows.Count; i++)
				{
					gridSettingsSearchString.Rows[i].Visible = (gridSettingsSearchString.Rows[i].Visible == true) ? false : true;
				}

				gridSettingsSearchString.TopLeftHeaderCell.Value = (gridSettingsSearchString.Rows[0].Visible == true) ? "v" : ">";
				GridAutoSize();
			}
		}
        private void gridSettingsLoadSchema_Click(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);
            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                for (int i = 0; i < gridSettingsLoadSchema.Rows.Count; i++)
                {
                    gridSettingsLoadSchema.Rows[i].Visible = (gridSettingsLoadSchema.Rows[i].Visible == true) ? false : true;
                }

                gridSettingsLoadSchema.TopLeftHeaderCell.Value = (gridSettingsLoadSchema.Rows[0].Visible == true) ? "v" : ">";
                GridAutoSize();
            }
        }
        private void tsSettingsSearchString_Click(object sender, EventArgs e)
        {
            ShowSettings(tsSettingsSearchString.Tag.ToString());
        }
    }
}
