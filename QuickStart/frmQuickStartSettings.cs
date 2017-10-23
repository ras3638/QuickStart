using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

            gridSettingsInsGen.Tag = new SettingTag(INSERT_GEN, ArrowDirection.Right);
            gridSettingsCascadeDel.Tag = new SettingTag(CASCADE_DELETE_GEN, ArrowDirection.Right);
            gridSettingsSearchString.Tag = new SettingTag(SEARCH_STRING, ArrowDirection.Right);
            gridSettingsUpdGen.Tag = new SettingTag(UPDATE_GEN, ArrowDirection.Right);
            gridSettingsLoadSchema.Tag = new SettingTag(LOAD_SCHEMA, ArrowDirection.Right);

            DataGridViewRow row1 = (DataGridViewRow)gridSettingsInsGen.RowTemplate.Clone();
            SettingManager.AddSetting(INSERT_GEN, "Max Byte Threshold", (long)12000000);
            //SettingManager.OverwriteDefaultType(sTabInsertGen, "Max Byte Threshold", typeof(long));
            row1.CreateCells(gridSettingsInsGen, "Max Byte Threshold", 12000000);
            gridSettingsInsGen.Rows.Add(row1);

            DataGridViewRow row7 = (DataGridViewRow)gridSettingsInsGen.RowTemplate.Clone();
            SettingManager.AddSetting(INSERT_GEN, "Enable Oracle Functionality", false);
            row7.CreateCells(gridSettingsInsGen, "Enable Oracle Functionality", false);
            gridSettingsInsGen.Rows.Add(row7);

            DataGridViewRow row8 = (DataGridViewRow)gridSettingsInsGen.RowTemplate.Clone();
            SettingManager.AddSetting(INSERT_GEN, "Enable Excel Functionality", false);
            row8.CreateCells(gridSettingsInsGen, "Enable Excel Functionality", false);
            gridSettingsInsGen.Rows.Add(row8);

            DataGridViewRow row2 = (DataGridViewRow)gridSettingsCascadeDel.RowTemplate.Clone();
            SettingManager.AddSetting(CASCADE_DELETE_GEN, "Multiline support for None cascade option", false);
            row2.CreateCells(gridSettingsCascadeDel, "Multiline support for None cascade option", false);
            gridSettingsCascadeDel.Rows.Add(row2);

            DataGridViewRow row3 = (DataGridViewRow)gridSettingsSearchString.RowTemplate.Clone();
            SettingManager.AddSetting(SEARCH_STRING, "Number of top lines to remove from SearchResults.txt", (int)6);
            row3.CreateCells(gridSettingsSearchString, "Number of top lines to remove from SearchResults.txt", 6);
            gridSettingsSearchString.Rows.Add(row3);

            DataGridViewRow row4 = (DataGridViewRow)gridSettingsSearchString.RowTemplate.Clone();
            SettingManager.AddSetting(SEARCH_STRING, "Number of bottom lines to remove from SearchResults.txt", (int)2);
            row4.CreateCells(gridSettingsSearchString, "Number of bottom lines to remove from SearchResults.txt", 2);
            gridSettingsSearchString.Rows.Add(row4);

            DataGridViewRow row6 = (DataGridViewRow)gridSettingsSearchString.RowTemplate.Clone();
            SettingManager.AddSetting(SEARCH_STRING, "Enable Zip Functionality", true);
            row6.CreateCells(gridSettingsSearchString, "Enable Zip Functionality", true);
            gridSettingsSearchString.Rows.Add(row6);

            DataGridViewRow row5 = (DataGridViewRow)gridSettingsUpdGen.RowTemplate.Clone();
            SettingManager.AddSetting(UPDATE_GEN, "Not in Use", "Not in Use");
            row5.CreateCells(gridSettingsUpdGen, "Not in Use", "Not in Use");
            gridSettingsUpdGen.Rows.Add(row5);

            DataGridViewRow row9 = (DataGridViewRow)gridSettingsLoadSchema.RowTemplate.Clone();
            SettingManager.AddSetting(LOAD_SCHEMA, "Allow Custom Env", false);
            row9.CreateCells(gridSettingsLoadSchema, "Allow Custom Env", false);
            gridSettingsLoadSchema.Rows.Add(row9);

            //DataGridViewRow row10 = (DataGridViewRow)gridSettingsUpdGen.RowTemplate.Clone();
            //SettingManager.AddSetting(gridSettingsUpdGen.Tag.ToString(), "QFC 16.0 Zip Path2", "C:\\Devl\\Products\\Upstream\\Main\\Dependencies\\Zip Files");
            //row10.CreateCells(gridSettingsUpdGen, "QFC 16.0 Zip Path2", "C:\\Devl\\Products\\Upstream\\Main\\Dependencies\\Zip Files");
            //gridSettingsUpdGen.Rows.Add(row10);


            //Fills the row headers
            gridSettingsInsGen.DefaultCellStyle.SelectionBackColor = gridSettingsInsGen.DefaultCellStyle.BackColor;
            gridSettingsInsGen.DefaultCellStyle.SelectionForeColor = gridSettingsInsGen.DefaultCellStyle.ForeColor;

            gridSettingsCascadeDel.DefaultCellStyle.SelectionBackColor = gridSettingsCascadeDel.DefaultCellStyle.BackColor;
            gridSettingsCascadeDel.DefaultCellStyle.SelectionForeColor = gridSettingsCascadeDel.DefaultCellStyle.ForeColor;

            gridSettingsSearchString.DefaultCellStyle.SelectionBackColor = gridSettingsSearchString.DefaultCellStyle.BackColor;
            gridSettingsSearchString.DefaultCellStyle.SelectionForeColor = gridSettingsSearchString.DefaultCellStyle.ForeColor;

            gridSettingsUpdGen.DefaultCellStyle.SelectionBackColor = gridSettingsUpdGen.DefaultCellStyle.BackColor;
            gridSettingsUpdGen.DefaultCellStyle.SelectionForeColor = gridSettingsUpdGen.DefaultCellStyle.ForeColor;

            gridSettingsLoadSchema.DefaultCellStyle.SelectionBackColor = gridSettingsLoadSchema.DefaultCellStyle.BackColor;
            gridSettingsLoadSchema.DefaultCellStyle.SelectionForeColor = gridSettingsLoadSchema.DefaultCellStyle.ForeColor;

            //Removes the grid selection arrows
            gridSettingsInsGen.RowHeadersDefaultCellStyle.Padding = new Padding(this.gridSettingsInsGen.RowHeadersWidth);
            gridSettingsCascadeDel.RowHeadersDefaultCellStyle.Padding = new Padding(this.gridSettingsCascadeDel.RowHeadersWidth);
            gridSettingsSearchString.RowHeadersDefaultCellStyle.Padding = new Padding(this.gridSettingsSearchString.RowHeadersWidth);
            gridSettingsUpdGen.RowHeadersDefaultCellStyle.Padding = new Padding(this.gridSettingsUpdGen.RowHeadersWidth);
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
                if (iVisibleRows > 0) iGridHeight += gridSettingsInsGen.Rows[iVisibleRows - 1].Height;
            }
            gridSettingsCascadeDel.Location = new Point(gridSettingsInsGen.Location.X, gridSettingsInsGen.Location.Y + iGridHeight);

            iVisibleRows = 0;
            iGridHeight = gridSettingsCascadeDel.ColumnHeadersHeight;
            foreach (DataGridViewRow row in gridSettingsCascadeDel.Rows)
            {
                if (row.Visible) iVisibleRows++;
                if (iVisibleRows > 0) iGridHeight += gridSettingsCascadeDel.Rows[iVisibleRows - 1].Height; 
            }
            gridSettingsSearchString.Location = new Point(gridSettingsCascadeDel.Location.X, gridSettingsCascadeDel.Location.Y + iGridHeight);

            iVisibleRows = 0;
            iGridHeight = gridSettingsSearchString.ColumnHeadersHeight;
            foreach (DataGridViewRow row in gridSettingsSearchString.Rows)
            {
                if (row.Visible) iVisibleRows++;
                if (iVisibleRows > 0)iGridHeight += gridSettingsSearchString.Rows[iVisibleRows - 1].Height;
            }
            gridSettingsUpdGen.Location = new Point(gridSettingsSearchString.Location.X, gridSettingsSearchString.Location.Y + iGridHeight);

            iVisibleRows = 0;
            iGridHeight = gridSettingsUpdGen.ColumnHeadersHeight;
            foreach (DataGridViewRow row in gridSettingsUpdGen.Rows)
            {
                if (row.Visible) iVisibleRows++;
                if (iVisibleRows > 0) iGridHeight += gridSettingsUpdGen.Rows[iVisibleRows - 1].Height;
            }
            gridSettingsLoadSchema.Location = new Point(gridSettingsUpdGen.Location.X, gridSettingsUpdGen.Location.Y + iGridHeight);

            //Add extra height to grid to avoid vertical scrollbar from appearing
            AddExtraDataGridHeight(gridSettingsInsGen);
            AddExtraDataGridHeight(gridSettingsSearchString);
            AddExtraDataGridHeight(gridSettingsCascadeDel);
            AddExtraDataGridHeight(gridSettingsUpdGen);
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
        private void CollapseRows(string sFromTab, string sSource, DataGridView dgv)
        {
            SettingTag A = (SettingTag)dgv.Tag;
            if (sFromTab != sSource)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    dgv.Rows[i].Visible = false;
                }
                A.ArrowDirection = ArrowDirection.Right;
                dgv.Tag = A;
                dgv.Refresh();
            }
        }
        private void PopulateRows(string sFromTab, string sSource, DataGridView dgv)
        {
            SettingTag A = (SettingTag)dgv.Tag;
            if (sFromTab == sSource)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    dgv.Rows[i].Visible = true;
                }
                A.ArrowDirection = ArrowDirection.Down;
                dgv.Tag = A;
                dgv.Refresh();
            }
        }

        private void SaveSettings()
        {
            this.Validate();
            gridSettingsInsGen.EndEdit();
            gridSettingsCascadeDel.EndEdit();
            gridSettingsSearchString.EndEdit();
            gridSettingsUpdGen.EndEdit();
            gridSettingsLoadSchema.EndEdit();

            foreach (DataGridViewRow row in gridSettingsInsGen.Rows)
            {
                SettingManager.UpdateSetting(INSERT_GEN, row.Cells[0].Value.ToString(), row.Cells[1].Value);
            }
            foreach (DataGridViewRow row in gridSettingsCascadeDel.Rows)
            {
                SettingManager.UpdateSetting(CASCADE_DELETE_GEN, row.Cells[0].Value.ToString(), row.Cells[1].Value);
            }
            foreach (DataGridViewRow row in gridSettingsSearchString.Rows)
            {
                SettingManager.UpdateSetting(SEARCH_STRING, row.Cells[0].Value.ToString(), row.Cells[1].Value);
            }
            foreach (DataGridViewRow row in gridSettingsUpdGen.Rows)
            {
                SettingManager.UpdateSetting(UPDATE_GEN, row.Cells[0].Value.ToString(), row.Cells[1].Value);
            }
            foreach (DataGridViewRow row in gridSettingsLoadSchema.Rows)
            {
                SettingManager.UpdateSetting(LOAD_SCHEMA, row.Cells[0].Value.ToString(), row.Cells[1].Value);
            }
        }

        private void gridSettings_Click(object sender, EventArgs e)
        {
            GridSettingsClickHandler(sender, e, gridSettingsInsGen);
        }
        private void gridSettingsCascadeDel_Click(object sender, EventArgs e)
        {
            GridSettingsClickHandler(sender, e, gridSettingsCascadeDel);
        }
        private void gridSettingsUpdGen_Click(object sender, EventArgs e)
        {
            GridSettingsClickHandler(sender, e, gridSettingsUpdGen);
        }
        private void gridSettingsSearchString_Click(object sender, EventArgs e)
        {
            GridSettingsClickHandler(sender, e, gridSettingsSearchString);
        }
        private void gridSettingsLoadSchema_Click(object sender, EventArgs e)
        {
            GridSettingsClickHandler(sender, e, gridSettingsLoadSchema);
        }
        private void GridSettingsClickHandler(object sender, EventArgs e, DataGridView dgv)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView dgvSender = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgvSender.HitTest(args.X, args.Y);
            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    dgv.Rows[i].Visible = (dgv.Rows[i].Visible == true) ? false : true;
                }

                SettingTag A = (SettingTag)dgv.Tag;
                A.ArrowDirection = dgv.Rows[0].Visible == true ? ArrowDirection.Down : ArrowDirection.Right;
                dgv.Tag = A;
                dgv.Refresh();
                GridAutoSize();
            }
        }
        private void tsSettingsReset_Click(object sender, EventArgs e)
        {
            SaveSettings();
            foreach (DataGridViewRow row in gridSettingsCascadeDel.Rows)
            {
                string sValue = SettingManager.GetSettingDefaultValue(CASCADE_DELETE_GEN, row.Cells[0].Value.ToString()).ToString();
                row.Cells[1].Value = sValue;
            }
            foreach (DataGridViewRow row in gridSettingsInsGen.Rows)
            {
                string sValue = SettingManager.GetSettingDefaultValue(INSERT_GEN, row.Cells[0].Value.ToString()).ToString();
                row.Cells[1].Value = sValue;
            }
            foreach (DataGridViewRow row in gridSettingsSearchString.Rows)
            {
                string sValue = SettingManager.GetSettingDefaultValue(SEARCH_STRING, row.Cells[0].Value.ToString()).ToString();
                row.Cells[1].Value = sValue;
            }
            foreach (DataGridViewRow row in gridSettingsLoadSchema.Rows)
            {
                string sValue = SettingManager.GetSettingDefaultValue(LOAD_SCHEMA, row.Cells[0].Value.ToString()).ToString();
                row.Cells[1].Value = sValue;
            }
            foreach (DataGridViewRow row in gridSettingsUpdGen.Rows)
            {
                SettingTag A = (SettingTag)gridSettingsUpdGen.Tag;
                string sValue = SettingManager.GetSettingDefaultValue(UPDATE_GEN, row.Cells[0].Value.ToString()).ToString();
                row.Cells[1].Value = sValue;
            }
        }


        private void tsSettingsQFC_Click(object sender, EventArgs e)
        {
            ShowSettings(tsSettingsSearchString.Tag.ToString());
        }
        private void tsSettingsGenerate_Insert_Click(object sender, EventArgs e)
        {
            ShowSettings(tsSettingsGenerate_Insert.Tag.ToString());
        }
        private void tsSettingsGenerate_Update_Click(object sender, EventArgs e)
        {
            ShowSettings(tsSettingsGenerate_Update.Tag.ToString());
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
        private void gridSettingsUpdGen_SelectionChanged(object sender, EventArgs e)
        {
            gridSettingsUpdGen.ClearSelection();
        }
        private void gridSettingsCascadeDel_SelectionChanged(object sender, EventArgs e)
        {
            gridSettingsCascadeDel.ClearSelection();
        }
        private void gridSettingsLoadSchema_SelectionChanged(object sender, EventArgs e)
        {
            gridSettingsLoadSchema.ClearSelection();
        }
        private void gridSettingsSearchString_SelectionChanged(object sender, EventArgs e)
        {
            gridSettingsSearchString.ClearSelection();
        }
        private void tsSettingsCascade_Delete_Click(object sender, EventArgs e)
        {
            ShowSettings(tsSettingsCascade_Delete.Tag.ToString());
        }
        private void tsSettingsSearchString_Click(object sender, EventArgs e)
        {
            ShowSettings(tsSettingsSearchString.Tag.ToString());
        }
        private void ShowSettings(string sFromTab)
        {
            panelMainTab.Hide();

            CollapseRows(sFromTab, INSERT_GEN, gridSettingsInsGen);
            CollapseRows(sFromTab, CASCADE_DELETE_GEN, gridSettingsCascadeDel);
            CollapseRows(sFromTab, SEARCH_STRING, gridSettingsSearchString);
            CollapseRows(sFromTab, UPDATE_GEN, gridSettingsUpdGen);
            CollapseRows(sFromTab, LOAD_SCHEMA, gridSettingsLoadSchema);

            PopulateRows(sFromTab, INSERT_GEN, gridSettingsInsGen);
            PopulateRows(sFromTab, CASCADE_DELETE_GEN, gridSettingsCascadeDel);
            PopulateRows(sFromTab, SEARCH_STRING, gridSettingsSearchString);
            PopulateRows(sFromTab, UPDATE_GEN, gridSettingsUpdGen);
            PopulateRows(sFromTab, LOAD_SCHEMA, gridSettingsLoadSchema);

            GridAutoSize();
            panelSettings.Show();
        }

        private void DisplayTriangleArrow(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == -1 && e.RowIndex == -1)
            {
                Control Cntrl = (Control)sender;
                SettingTag A = (SettingTag)Cntrl.Tag;
                List<Point> TrianglePoints = new List<Point>();
                bool bHandled = false;

                if (A.ArrowDirection == ArrowDirection.Right)
                {
                    TrianglePoints.Add(new Point(4, 4));
                    TrianglePoints.Add(new Point(10, 8));
                    TrianglePoints.Add(new Point(4, 12));
                    bHandled = true;
                }
                else if (A.ArrowDirection == ArrowDirection.Down)
                {
                    TrianglePoints.Add(new Point(4, 6));
                    TrianglePoints.Add(new Point(12, 6));
                    TrianglePoints.Add(new Point(8, 12));
                    bHandled = true;
                }
                if(bHandled)
                {
                    //Draw polygon to screen.
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    e.PaintBackground(e.ClipBounds, true);
                    e.Graphics.FillPolygon(Brushes.Black, TrianglePoints.ToArray());
                    e.Handled = true;
                }
            }
            else
            {
                e.Graphics.SmoothingMode = SmoothingMode.None;
            }
        }
        private void gridSettingsInsGen_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DisplayTriangleArrow(sender, e);
        }
        private void gridSettingsCascadeDel_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DisplayTriangleArrow(sender, e);
        }
        private void gridSettingsUpdGen_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DisplayTriangleArrow(sender, e);
        }
        private void gridSettingsLoadSchema_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DisplayTriangleArrow(sender, e);
        }
        private void gridSettingsSearchString_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DisplayTriangleArrow(sender, e);
        }
    }
}

