using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SelectAll
{
    public partial class frmSelectAll : Form
    {
        int TotalCheckBoxes = 0;
        int TotalCheckedCheckBoxes = 0;
        CheckBox HeaderCheckBox = null;
        bool IsHeaderCheckBoxClicked = false;

        public frmSelectAll()
        {
            InitializeComponent();
        }

        private void frmSelectAll_Load(object sender, EventArgs e)
        {
            AddHeaderCheckBox();

            HeaderCheckBox.KeyUp += new KeyEventHandler(HeaderCheckBox_KeyUp);
            HeaderCheckBox.MouseClick += new MouseEventHandler(HeaderCheckBox_MouseClick);
            dgvSelectAll.CellValueChanged += new DataGridViewCellEventHandler(dgvSelectAll_CellValueChanged);
            dgvSelectAll.CurrentCellDirtyStateChanged += new EventHandler(dgvSelectAll_CurrentCellDirtyStateChanged);
            dgvSelectAll.CellPainting += new DataGridViewCellPaintingEventHandler(dgvSelectAll_CellPainting);

            BindGridView();
        }

        private void BindGridView()
        {
            dgvSelectAll.DataSource = GetDataSource();

            TotalCheckBoxes = dgvSelectAll.RowCount;
            TotalCheckedCheckBoxes = 0;
        }

        private DataTable GetDataSource()
        {
            DataTable dTable = new DataTable();

            DataRow dRow = null;
            DateTime dTime;
            Random rnd = new Random();

            dTable.Columns.Add("IsChecked", System.Type.GetType("System.Boolean"));
            dTable.Columns.Add("RandomNo");
            dTable.Columns.Add("Date");
            dTable.Columns.Add("Time");

            for (int n = 0; n < 10; ++n)
            {
                dRow = dTable.NewRow();
                dTime = DateTime.Now;

                dRow["IsChecked"] = "false";
                dRow["RandomNo"] = rnd.NextDouble();
                dRow["Date"] = dTime.ToString("MM/dd/yyyy");
                dRow["Time"] = dTime.ToString("hh:mm:ss tt");

                dTable.Rows.Add(dRow);
                dTable.AcceptChanges();
            }

            return dTable;
        }

        private void dgvSelectAll_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsHeaderCheckBoxClicked)
                RowCheckBoxClick((DataGridViewCheckBoxCell)dgvSelectAll[e.ColumnIndex, e.RowIndex]);
        }

        private void dgvSelectAll_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvSelectAll.CurrentCell is DataGridViewCheckBoxCell)
                dgvSelectAll.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void HeaderCheckBox_MouseClick(object sender, MouseEventArgs e)
        {
            HeaderCheckBoxClick((CheckBox)sender);
        }

        private void HeaderCheckBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
                HeaderCheckBoxClick((CheckBox)sender);
        }

        private void dgvSelectAll_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == 0)
                    ResetHeaderCheckBoxLocation(e.ColumnIndex, e.RowIndex);
        }

        private void AddHeaderCheckBox()
        {
            HeaderCheckBox = new CheckBox();

            HeaderCheckBox.Size = new Size(15, 15);

            //Add the CheckBox into the DataGridView
            this.dgvSelectAll.Controls.Add(HeaderCheckBox);
        }

        private void ResetHeaderCheckBoxLocation(int ColumnIndex, int RowIndex)
        {
            //Get the column header cell bounds
            Rectangle oRectangle = this.dgvSelectAll.GetCellDisplayRectangle(ColumnIndex, RowIndex, true);

            Point oPoint = new Point();

            oPoint.X = oRectangle.Location.X + (oRectangle.Width - HeaderCheckBox.Width) / 2 + 1;
            oPoint.Y = oRectangle.Location.Y + (oRectangle.Height - HeaderCheckBox.Height) / 2 + 1;

            //Change the location of the CheckBox to make it stay on the header
            HeaderCheckBox.Location = oPoint;
        }

        private void HeaderCheckBoxClick(CheckBox HCheckBox)
        {
            IsHeaderCheckBoxClicked = true;

            foreach (DataGridViewRow Row in dgvSelectAll.Rows)
                ((DataGridViewCheckBoxCell)Row.Cells["chkBxSelect"]).Value = HCheckBox.Checked;

            dgvSelectAll.RefreshEdit();

            TotalCheckedCheckBoxes = HCheckBox.Checked ? TotalCheckBoxes : 0;

            IsHeaderCheckBoxClicked = false;
        }

        private void RowCheckBoxClick(DataGridViewCheckBoxCell RCheckBox)
        {
            if (RCheckBox != null)
            {
                //Modifiy Counter;            
                if ((bool)RCheckBox.Value && TotalCheckedCheckBoxes < TotalCheckBoxes)
                    TotalCheckedCheckBoxes++;
                else if (TotalCheckedCheckBoxes > 0)
                    TotalCheckedCheckBoxes--;

                //Change state of the header CheckBox.
                if (TotalCheckedCheckBoxes < TotalCheckBoxes)
                    HeaderCheckBox.Checked = false;
                else if (TotalCheckedCheckBoxes == TotalCheckBoxes)
                    HeaderCheckBox.Checked = true;
            }
        }
    }
}
