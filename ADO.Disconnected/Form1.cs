using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace ADO.Disconnected
{
    public partial class Form1 : Form
    {
        private SqlConnection conObj = null;
        private SqlDataAdapter adapterObj = null;
        private DataSet dsObj = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Providing connection information
            using (conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString))
            {
                //creating data adapter with command and connection object
                using (adapterObj = new SqlDataAdapter("Select * from Products", conObj))
                {
                    dsObj = new DataSet(); //Initialize DataSet
                    //Filling data from Database to Dataset, when fill() called adapater will open the connection and as soon data stored in dataset, connection will automatically closed
                    adapterObj.Fill(dsObj, "Prod");
                    //Bound the Gridview
                    GridProduct.DataSource = dsObj.Tables["Prod"];
                }
            }
        }
    }
}
