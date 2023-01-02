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
using System.Configuration;

namespace Infinite.ADO.Day1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //1 - Create a connection
            SqlConnection con = new SqlConnection();
            //Pass Connection Parameters
            con.ConnectionString = ConfigurationManager.ConnectionStrings["HRCon"].ConnectionString;

            //2 - Creata a Command
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Select * from Position";//Command Text or Query
            cmd.Connection = con; //Providing connection information to command

            //3 - Open a connection
            con.Open();

            //4 - Execute the Query
            SqlDataReader reader = cmd.ExecuteReader();

            //Convert to Table Format
            DataTable dt = new DataTable();
            dt.Load(reader);

            //Bound DataTable to DataGridView
            GvPosition.DataSource = dt;

            //5 - CLose the connection
            con.Close();
        }
    }
}
