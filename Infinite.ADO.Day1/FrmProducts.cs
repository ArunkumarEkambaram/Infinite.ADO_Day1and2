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
    public partial class FrmProducts : Form
    {
        private SqlConnection conObj = null;
        private SqlCommand cmdObj = null;

        public FrmProducts()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString);
            cmdObj = new SqlCommand("usp_GetProductsById", conObj);
            cmdObj.CommandType = CommandType.StoredProcedure;
            cmdObj.Parameters.AddWithValue("@ProductId", TxtProductId.Text);

            if (conObj.State == ConnectionState.Closed)
            {
                conObj.Open();
            }
            SqlDataReader reader = cmdObj.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();//Read the first row
                TxtProductName.Text = reader["ProductName"].ToString();
                TxtSupplierId.Text = reader["SupplierId"].ToString();
                TxtCategoryId.Text = reader["CategoryId"].ToString();
                TxtQuantityPerUnit.Text = reader["QuantityPerUnit"].ToString();
                TxtUnitPrice.Text = reader["UnitPrice"].ToString();
                TxtUnitsInStock.Text = reader["UnitsInStock"].ToString();
            }
            else
            {
                LblMessage.Text = "No Records";
                ResetControls();
            }
            reader.Close();
            cmdObj.Dispose();
            conObj.Close();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetControls();
        }

        //Reset Controls
        public void ResetControls()
        {
            TxtProductId.Text = "";
            TxtProductName.Text = "";
            TxtSupplierId.Text = "";
            TxtCategoryId.Text = "";
            TxtQuantityPerUnit.Text = "";
            TxtUnitPrice.Text = "";
            TxtQuantityPerUnit.Text = "";
            TxtUnitsInStock.Text = "";
        }

        //add namespace - using System.Configuration;
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString);
                cmdObj = new SqlCommand("usp_InsertProducts", conObj)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmdObj.Parameters.AddWithValue("@ProductName", TxtProductName.Text);
                cmdObj.Parameters.AddWithValue("@SupplierId", TxtSupplierId.Text);
                cmdObj.Parameters.AddWithValue("@CategoryId", TxtCategoryId.Text);
                cmdObj.Parameters.AddWithValue("@QuantityPerUnit", TxtQuantityPerUnit.Text);
                cmdObj.Parameters.AddWithValue("@UnitPrice", TxtUnitPrice.Text);
                cmdObj.Parameters.AddWithValue("@UnitsInStock", TxtUnitsInStock.Text);
                //Get Output Parameter
                SqlParameter IdParameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmdObj.Parameters.Add(IdParameter);

                if (conObj.State == ConnectionState.Closed)
                {
                    conObj.Open();
                }
                //To Perform DML Operations
                int res = cmdObj.ExecuteNonQuery();
                if (res > 0)
                {
                    LblMessage.Text = $"Product created successfully\nNew Product Id : {IdParameter.Value}";
                }
            }
            catch (SqlException ex)
            {
                LblMessage.Text = $"SQL Error :{ex.Message}";
            }
            catch (Exception ex)
            {
                LblMessage.Text = $"Error :{ex.Message}";
            }
            finally
            {
                if (cmdObj != null)
                    cmdObj.Dispose();
                if (conObj != null)
                    conObj.Close();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString);
                cmdObj = new SqlCommand("usp_UpdateProduct", conObj)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmdObj.Parameters.AddWithValue("@ProductId", TxtProductId.Text);
                cmdObj.Parameters.AddWithValue("@UnitPrice", TxtUnitPrice.Text);
                cmdObj.Parameters.AddWithValue("@UnitsInStock", TxtUnitsInStock.Text);
                if (conObj.State == ConnectionState.Closed)
                {
                    conObj.Open();
                }
                //To perform DML operation
                int res = cmdObj.ExecuteNonQuery();
                if (res > 0)
                {
                    LblMessage.Text = "Product Updated Successfully";
                }
            }
            catch (SqlException ex)
            {
                LblMessage.Text = ex.Message;
            }
            catch (Exception ex)
            {
                LblMessage.Text = ex.Message;
            }
            finally
            {
                if (cmdObj != null)
                    cmdObj.Dispose();
                if (conObj != null)
                    conObj.Close();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindCon"].ConnectionString))
                {
                    using (cmdObj = new SqlCommand("usp_DeleteProduct", conObj))
                    {
                        cmdObj.CommandType = CommandType.StoredProcedure;
                        cmdObj.Parameters.AddWithValue("@ProductId", TxtProductId.Text);

                        if (conObj.State == ConnectionState.Closed)
                        {
                            conObj.Open();
                        }
                        if (MessageBox.Show("Do you want to delete this product?", "Delete Product", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            int res = cmdObj.ExecuteNonQuery();

                            if (res > 0)
                            {
                                LblMessage.Text = $"{TxtProductName.Text} is Product Deleted";
                                ResetControls();
                            }
                        }
                    }//cmdObj will be disposed here
                }//conObj will be disposed here
            }
            catch(SqlException ex)
            {
                LblMessage.Text = ex.Message;
            }
            catch(Exception ex)
            {
                LblMessage.Text = ex.Message;
            }
        }
    }
}
