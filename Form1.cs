using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace EditingDatabaseRecords
{
    public partial class frmPhoneDB : Form
    {
        public frmPhoneDB()
        {
            InitializeComponent();
        }

        SqlConnection phoneConnecction;
        SqlCommand phoneCommand;
        SqlDataAdapter phoneAdapter;
        DataTable phoneTable;
        CurrencyManager phoneManager;

        private void frmPhoneDB_Load(object sne3der, EventArgs e)
        {
            // get the file path for SQLPhoneDB.mdf
            string fullPath = Path.GetFullPath("SQLPhoneDB.mdf");
            // connect to Phone database
            phoneConnecction = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename="+fullPath+";Integrated Security=True; Connect Timeout=30; User Instance=True");
            phoneConnecction.Open();

            // establish command object
            phoneCommand = new SqlCommand("SELECT * FROM phonetable ORDER BY ContactName", phoneConnecction);

            // establish data adapter/data table
            phoneAdapter = new SqlDataAdapter();
            phoneAdapter.SelectCommand = phoneCommand;
            phoneTable = new DataTable();
            phoneAdapter.Fill(phoneTable);

            // bind controls to data table
            txtID.DataBindings.Add("Text", phoneTable, "ContactID");
            txtName.DataBindings.Add("Text", phoneTable, "ContactName");
            txtNumber.DataBindings.Add("Text", phoneTable, "ContactNumber");

            // establish currency manager
            phoneManager = (CurrencyManager)this.BindingContext[phoneTable];

            SetState("View");
        }

        private void frmPhoneDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // save the updated phone table
                SqlCommandBuilder phoneAdapterCommands = new SqlCommandBuilder(phoneAdapter);
                phoneAdapter.Update(phoneTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving database to file:\r\n" + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // close the connection
            phoneConnecction.Close();
            //dispose of the objects
            phoneConnecction.Dispose();
            phoneCommand.Dispose();
            phoneAdapter.Dispose();
            phoneTable.Dispose();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            phoneManager.Position = 0;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            phoneManager.Position--;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            phoneManager.Position++;
        }
        
        private void btnLast_Click(object sender, EventArgs e)
        {
            phoneManager.Position = phoneManager.Count - 1;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SetState("Edit");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            phoneManager.EndCurrentEdit();
            SetState("View");
            phoneTable.DefaultView.Sort = "ContactName";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            phoneManager.CancelCurrentEdit();
            SetState("View");
        }

        private void SetState(string appState)
        {
            switch (appState) 
            {
                case "View":
                    btnFirst.Enabled = true;
                    btnPrevious.Enabled = true;
                    btnNext.Enabled = true;
                    btnLast.Enabled = true;
                    btnEdit.Enabled = true;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    txtID.BackColor = Color.White;
                    txtID.ForeColor = Color.Black;
                    txtName.ReadOnly = true;
                    txtNumber.ReadOnly = true;
                    break;
                default: // "Edit" mode
                    btnFirst.Enabled = false;
                    btnPrevious.Enabled = false;
                    btnNext.Enabled = false;
                    btnLast.Enabled = false;
                    btnEdit.Enabled = false;
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                    txtID.BackColor = Color.Red;
                    txtID.ForeColor = Color.White;
                    txtName.ReadOnly = false;
                    txtNumber.ReadOnly = false;
                    break;
            }
            txtName.Focus();
        }
    }
}
