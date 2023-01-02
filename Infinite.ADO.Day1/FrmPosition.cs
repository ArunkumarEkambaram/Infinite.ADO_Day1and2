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

namespace Infinite.ADO.Day1
{
    public partial class FrmPosition : Form
    {
        private DataTable dt = null;
        public FrmPosition()
        {
            InitializeComponent();
        }
        //DataGRidView - GridProducts
        //Form - FrmPositon
        //Label - LblMessage
        private void FrmPosition_Load(object sender, EventArgs e)
        {
            SqlConnection conObj = new SqlConnection("data source=EGAIK-PC; initial catalog=Northwind; integrated security=true;");
            SqlCommand cmdObj = new SqlCommand("Select * from Products", conObj);
            if (conObj.State == ConnectionState.Closed)
            {
                conObj.Open();
            }
            SqlDataReader reader = cmdObj.ExecuteReader();
            if (reader.HasRows)
            {
                dt = new DataTable();
                dt.Load(reader);
                GridProducts.DataSource = dt;
            }
            else
            {
                // MessageBox.Show("No Record found");
                LblMessage.ForeColor = Color.Red;
                LblMessage.Text = "No Record Found";
            }
            reader.Close();
            cmdObj.Dispose();
            conObj.Close();
        }
    }
}
