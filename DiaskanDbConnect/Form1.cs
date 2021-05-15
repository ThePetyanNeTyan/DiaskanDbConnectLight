using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace DiaskanDbConnect
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;

        private bool newRowAdding = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            try 
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT *, 'Delete' AS [Command] FROM Cars", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlDataAdapter);
                sqlBuilder.GetInsertCommand();
                sqlBuilder.GetUpdateCommand();
                sqlBuilder.GetDeleteCommand();

                dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet, "Cars");

                dataGridView1.DataSource = dataSet.Tables["Cars"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[8, i] = linkCell;
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReloadData()
        {
            try
            {
                dataSet.Tables["Cars"].Clear();

                sqlDataAdapter.Fill(dataSet, "Cars");

                dataGridView1.DataSource = dataSet.Tables["Cars"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[8, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\krita\source\repos\DiaskanDbConnect\DiaskanDbConnect\Database1.mdf;Integrated Security=True");
            sqlConnection.Open();

            LoadData();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try 
            {
                if (e.ColumnIndex == 8)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();

                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;

                            dataGridView1.Rows.RemoveAt(rowIndex);

                            dataSet.Tables["Cars"].Rows[rowIndex].Delete();

                            sqlDataAdapter.Update(dataSet, "Cars");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;

                        DataRow row = dataSet.Tables["Cars"].NewRow();

                        row["CarName"] = dataGridView1.Rows[rowIndex].Cells["CarName"].Value;
                        row["CarYear"] = dataGridView1.Rows[rowIndex].Cells["CarYear"].Value;
                        row["CarType"] = dataGridView1.Rows[rowIndex].Cells["CarType"].Value;
                        row["CarKpp"] = dataGridView1.Rows[rowIndex].Cells["CarKpp"].Value;
                        row["CarEngage"] = dataGridView1.Rows[rowIndex].Cells["CarEngage"].Value;
                        row["CarGruz"] = dataGridView1.Rows[rowIndex].Cells["CarGruz"].Value;
                        row["CarMesto"] = dataGridView1.Rows[rowIndex].Cells["CarMesto"].Value;

                        dataSet.Tables["Cars"].Rows.Add(row);
                        dataSet.Tables["Cars"].Rows.RemoveAt(dataSet.Tables["Cars"].Rows.Count - 1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                        dataGridView1.Rows[e.RowIndex].Cells[8].Value = "Delete";

                        sqlDataAdapter.Update(dataSet, "Cars");
                        newRowAdding = false;
                    }
                    else if (task == "Update")
                    {
                        int r = e.RowIndex;

                        dataSet.Tables["Cars"].Rows[r]["CarName"] = dataGridView1.Rows[r].Cells["CarName"].Value;
                        dataSet.Tables["Cars"].Rows[r]["CarYear"] = dataGridView1.Rows[r].Cells["CarYear"].Value;
                        dataSet.Tables["Cars"].Rows[r]["CarType"] = dataGridView1.Rows[r].Cells["CarType"].Value;
                        dataSet.Tables["Cars"].Rows[r]["CarKpp"] = dataGridView1.Rows[r].Cells["CarKpp"].Value;
                        dataSet.Tables["Cars"].Rows[r]["CarEngage"] = dataGridView1.Rows[r].Cells["CarEngage"].Value;
                        dataSet.Tables["Cars"].Rows[r]["CarGruz"] = dataGridView1.Rows[r].Cells["CarGruz"].Value;
                        dataSet.Tables["Cars"].Rows[r]["CarMesto"] = dataGridView1.Rows[r].Cells["CarMesto"].Value;

                        sqlDataAdapter.Update(dataSet, "Cars");

                        dataGridView1.Rows[e.RowIndex].Cells[8].Value = "Delete";
                    }

                    ReloadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try 
            {
                if (newRowAdding == false)
                {
                    newRowAdding = true;

                    int lastRow = dataGridView1.Rows.Count - 2;

                    DataGridViewRow row = dataGridView1.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[8, lastRow] = linkCell;

                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (newRowAdding == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[8, rowIndex] = linkCell;

                    editingRow.Cells["Command"].Value = "Update";
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);

            if (dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                TextBox textBox = e.Control as TextBox;

                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }

            if (dataGridView1.CurrentCell.ColumnIndex == 6)
            {
                TextBox textBox = e.Control as TextBox;

                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }

            if (dataGridView1.CurrentCell.ColumnIndex == 7)
            {
                TextBox textBox = e.Control as TextBox;

                if (textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }

        }

        private void Column_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
