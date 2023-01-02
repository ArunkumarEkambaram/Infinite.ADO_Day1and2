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

namespace ADO.Disconnected
{
    public partial class FrmProducts : Form
    {
        private SqlConnection conObj = null;
        private SqlDataAdapter adapterObj = null;
        private DataSet ds = null;

        public FrmProducts()
        {
            InitializeComponent();
        }

        public DataColumn[] GeneratePrimaryKey(DataSet ds)
        {
            DataColumn[] dc = new DataColumn[1];
            dc[0] = ds.Tables["Products"].Columns["ProductId"];
            return dc;
        }

        //public DataColumn[] GeneratePrimaryKey(DataSet ds, string tableName, string columnName)
        //{
        //    DataColumn[] dc = new DataColumn[1];
        //    dc[0] = ds.Tables[tableName].Columns[columnName];
        //    return dc;
        //}

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            using (conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString))
            {
                using (adapterObj = new SqlDataAdapter("Select * from Products", conObj))
                {
                    ds = new DataSet();
                    adapterObj.Fill(ds, "Products");
                    ds.WriteXml(@"G:\Infinite\ProductsDataSet1.xml", XmlWriteMode.WriteSchema);
                    //Creating a Primary Key for Dataset

                    ds.Tables["Products"].PrimaryKey = this.GeneratePrimaryKey(ds);

                    DataRow dr = ds.Tables["Products"].Rows.Find(TxtProductId.Text);
                    if (dr != null)
                    {
                        TxtProductName.Text = dr["ProductName"].ToString();
                        TxtSupplierId.Text = dr["SupplierId"].ToString();
                        CbCategory.Text = dr["CategoryId"].ToString();
                        TxtUnitPrice.Text = dr["UnitPrice"].ToString();
                        TxtUnitsInStock.Text = dr["UnitsInStock"].ToString();
                        TxtQuantityPerUnit.Text = dr["QuantityPerUnit"].ToString();
                    }
                    else
                    {
                        LblMessage.Text = "No Record found";
                    }
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString))
            {
                using (adapterObj = new SqlDataAdapter("Select * from Products", conObj))
                {
                    //Generate command
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapterObj);//Generates Insert, Update, Delete command
                    ds = new DataSet();
                    adapterObj.Fill(ds, "Products");
                    //Create a new row
                    DataRow dr = ds.Tables["Products"].NewRow();
                    //Provide necessary value for each column
                    dr["ProductName"] = TxtProductName.Text;
                    dr["SupplierId"] = TxtSupplierId.Text;
                    dr["CategoryId"] = CbCategory.SelectedValue;//Get value from combobox
                    dr["UnitPrice"] = TxtUnitPrice.Text;
                    dr["UnitsInStock"] = TxtUnitsInStock.Text;
                    dr["QuantityPerUnit"] = TxtQuantityPerUnit.Text;
                    //Attach the row to Dataset
                    ds.Tables["Products"].Rows.Add(dr);

                    //Update the dataset to database
                    int res = adapterObj.Update(ds, "Products");
                    ds.Clear();
                    adapterObj.Fill(ds, "Products");
                    if (res > 0)
                    {
                        int row = ds.Tables["Products"].Rows.Count - 1;
                        LblMessage.Text = $"Product Created\nProduct Id :{ds.Tables["Products"].Rows[row]["ProductId"]}";
                    }
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            using (conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString))
            {
                using (adapterObj = new SqlDataAdapter("Select * from Products", conObj))
                {
                    ds = new DataSet();
                    adapterObj.Fill(ds, "Products");
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapterObj);

                    //Create a Primary Key                   
                    ds.Tables["Products"].PrimaryKey = GeneratePrimaryKey(ds);

                    //Find the row
                    DataRow dr = ds.Tables["Products"].Rows.Find(TxtProductId.Text);
                    if (dr != null)
                    {
                        //Perform Update
                        dr.BeginEdit();
                        dr["UnitPrice"] = TxtUnitPrice.Text;
                        dr["QuantityPerUnit"] = TxtQuantityPerUnit.Text;
                        dr["UnitsInStock"] = TxtUnitsInStock.Text;
                        dr.EndEdit();

                        //Update the changes to database
                        int res = adapterObj.Update(ds, "Products");
                        if (res > 0)
                        {
                            LblMessage.Text = "Product Updated";
                        }
                    }
                    else
                    {
                        LblMessage.Text = "No records found";
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            using (conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString))
            {
                using (adapterObj = new SqlDataAdapter("Select * from Products", conObj))
                {
                    ds = new DataSet();
                    adapterObj.Fill(ds, "Products");
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapterObj);

                    //Create Primary Key                   
                    ds.Tables["Products"].PrimaryKey = this.GeneratePrimaryKey(ds);

                    DataRow dr = ds.Tables["Products"].Rows.Find(TxtProductId.Text);
                    if (dr != null)
                    {
                        dr.Delete();//Delete the row                      
                        // ds.Tables["p1"].Rows.Find(TxtProductId.Text)?.Delete();
                        int res = adapterObj.Update(ds, "Products");
                        if (res > 0)
                        {
                            LblMessage.Text = "Product Deleted";
                        }
                    }
                    else
                    {
                        LblMessage.Text = "Product not exists";
                    }
                }
            }
        }

        private void FrmProducts_Load(object sender, EventArgs e)
        {
            using (conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString))
            {
                using (adapterObj = new SqlDataAdapter("Select CategoryId, CategoryName from Categories", conObj))
                {
                    ds = new DataSet();
                    adapterObj.Fill(ds, "Categories");

                    //Bound to Combobox                   
                    CbCategory.DataSource = ds.Tables["Categories"];
                    CbCategory.DisplayMember = "CategoryName";
                    CbCategory.ValueMember = "CategoryId";
                    CbCategory.Text = "--Select--";//Set default value for combo box
                }
                GridProducts.DataSource = Reload();
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            GridProducts.DataSource = Reload();
        }

        public DataTable Reload()
        {
            using (conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString))
            {
                using (adapterObj = new SqlDataAdapter("Select * from Products", conObj))
                {
                    DataTable dt = new DataTable();
                    adapterObj.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
