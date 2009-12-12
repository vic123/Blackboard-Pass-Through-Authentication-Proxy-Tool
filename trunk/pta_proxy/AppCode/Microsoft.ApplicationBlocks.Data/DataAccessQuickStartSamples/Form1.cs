using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Xml;

namespace QuickStartSamples_CS
{
	/// <summary>
	/// Data Access Application Block Quick Start Samples.
	/// Please run CreateStoredProcedures.sql in SQL Query Analyzer
	/// to create database objects used by these examples.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.TextBox txtConnectionString;
		internal System.Windows.Forms.Button cmdSample5;
		internal System.Windows.Forms.Button cmdSample4;
		internal System.Windows.Forms.Button cmdSample3;
		internal System.Windows.Forms.Button cmdSample2;
		internal System.Windows.Forms.Button cmdClearResults;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.RichTextBox txtResults;
		internal System.Windows.Forms.Button cmdSample1;
		private System.Windows.Forms.ToolTip toolTip;
		internal System.Windows.Forms.Button cmdSample6;
        internal System.Windows.Forms.Button cmdSample7;
        internal System.Windows.Forms.Button cmdSample8;
        internal System.Windows.Forms.Button cmdSample9;
        internal System.Windows.Forms.Button cmdSample10;
		private System.ComponentModel.IContainer components;

		public Form1()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Label2 = new System.Windows.Forms.Label();
			this.txtConnectionString = new System.Windows.Forms.TextBox();
			this.cmdSample5 = new System.Windows.Forms.Button();
			this.cmdSample4 = new System.Windows.Forms.Button();
			this.cmdSample3 = new System.Windows.Forms.Button();
			this.cmdSample2 = new System.Windows.Forms.Button();
			this.cmdClearResults = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.txtResults = new System.Windows.Forms.RichTextBox();
			this.cmdSample1 = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.cmdSample6 = new System.Windows.Forms.Button();
			this.cmdSample7 = new System.Windows.Forms.Button();
			this.cmdSample8 = new System.Windows.Forms.Button();
			this.cmdSample9 = new System.Windows.Forms.Button();
			this.cmdSample10 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Label2
			// 
			this.Label2.Location = new System.Drawing.Point(264, 16);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(136, 16);
			this.Label2.TabIndex = 20;
			this.Label2.Text = "Connection String";
			// 
			// txtConnectionString
			// 
			this.txtConnectionString.Location = new System.Drawing.Point(264, 32);
			this.txtConnectionString.Name = "txtConnectionString";
			this.txtConnectionString.Size = new System.Drawing.Size(480, 20);
			this.txtConnectionString.TabIndex = 19;
			this.txtConnectionString.Text = "Server=(local);Database=Northwind;Integrated Security=True;";
			// 
			// cmdSample5
			// 
			this.cmdSample5.Location = new System.Drawing.Point(8, 216);
			this.cmdSample5.Name = "cmdSample5";
			this.cmdSample5.Size = new System.Drawing.Size(224, 32);
			this.cmdSample5.TabIndex = 18;
			this.cmdSample5.Text = "Perform Transactional Update";
			this.cmdSample5.Click += new System.EventHandler(this.cmdSample5_Click);
			// 
			// cmdSample4
			// 
			this.cmdSample4.Location = new System.Drawing.Point(8, 168);
			this.cmdSample4.Name = "cmdSample4";
			this.cmdSample4.Size = new System.Drawing.Size(224, 32);
			this.cmdSample4.TabIndex = 17;
			this.cmdSample4.Text = "Look Up Single Item";
			this.cmdSample4.Click += new System.EventHandler(this.cmdSample4_Click);
			// 
			// cmdSample3
			// 
			this.cmdSample3.Location = new System.Drawing.Point(8, 120);
			this.cmdSample3.Name = "cmdSample3";
			this.cmdSample3.Size = new System.Drawing.Size(224, 32);
			this.cmdSample3.TabIndex = 16;
			this.cmdSample3.Text = "Retrieve Single Row";
			this.cmdSample3.Click += new System.EventHandler(this.cmdSample3_Click);
			// 
			// cmdSample2
			// 
			this.cmdSample2.Location = new System.Drawing.Point(8, 72);
			this.cmdSample2.Name = "cmdSample2";
			this.cmdSample2.Size = new System.Drawing.Size(224, 32);
			this.cmdSample2.TabIndex = 15;
			this.cmdSample2.Text = "Retrieve Multiple Rows using DataSet";
			this.cmdSample2.Click += new System.EventHandler(this.cmdSample2_Click);
			// 
			// cmdClearResults
			// 
			this.cmdClearResults.Location = new System.Drawing.Point(8, 504);
			this.cmdClearResults.Name = "cmdClearResults";
			this.cmdClearResults.Size = new System.Drawing.Size(224, 32);
			this.cmdClearResults.TabIndex = 14;
			this.cmdClearResults.Text = "Clear Results";
			this.cmdClearResults.Click += new System.EventHandler(this.cmdClearResults_Click);
			// 
			// Label1
			// 
			this.Label1.Location = new System.Drawing.Point(264, 72);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(136, 16);
			this.Label1.TabIndex = 13;
			this.Label1.Text = "Results";
			// 
			// txtResults
			// 
			this.txtResults.Location = new System.Drawing.Point(264, 88);
			this.txtResults.Name = "txtResults";
			this.txtResults.Size = new System.Drawing.Size(480, 440);
			this.txtResults.TabIndex = 12;
			this.txtResults.Text = "";
			// 
			// cmdSample1
			// 
			this.cmdSample1.Location = new System.Drawing.Point(8, 24);
			this.cmdSample1.Name = "cmdSample1";
			this.cmdSample1.Size = new System.Drawing.Size(224, 32);
			this.cmdSample1.TabIndex = 11;
			this.cmdSample1.Text = "Retrieve Multiple Rows using DataReader";
			this.cmdSample1.Click += new System.EventHandler(this.cmdSample1_Click);
			// 
			// cmdSample6
			// 
			this.cmdSample6.Location = new System.Drawing.Point(8, 264);
			this.cmdSample6.Name = "cmdSample6";
			this.cmdSample6.Size = new System.Drawing.Size(224, 32);
			this.cmdSample6.TabIndex = 21;
			this.cmdSample6.Text = "Retrieve XML Data";
			this.cmdSample6.Click += new System.EventHandler(this.cmdSample6_Click);
			// 
			// cmdSample7
			// 
			this.cmdSample7.Location = new System.Drawing.Point(8, 312);
			this.cmdSample7.Name = "cmdSample7";
			this.cmdSample7.Size = new System.Drawing.Size(224, 32);
			this.cmdSample7.TabIndex = 22;
			this.cmdSample7.Text = "Fill DataSet";
			this.cmdSample7.Click += new System.EventHandler(this.cmdSample7_Click);
			// 
			// cmdSample8
			// 
			this.cmdSample8.Location = new System.Drawing.Point(8, 360);
			this.cmdSample8.Name = "cmdSample8";
			this.cmdSample8.Size = new System.Drawing.Size(224, 32);
			this.cmdSample8.TabIndex = 23;
			this.cmdSample8.Text = "Update data source";
			this.cmdSample8.Click += new System.EventHandler(this.cmdSample8_Click);
			// 
			// cmdSample9
			// 
			this.cmdSample9.Location = new System.Drawing.Point(8, 408);
			this.cmdSample9.Name = "cmdSample9";
			this.cmdSample9.Size = new System.Drawing.Size(224, 32);
			this.cmdSample9.TabIndex = 25;
			this.cmdSample9.Text = "Perform update with parameter binding";
			this.cmdSample9.Click += new System.EventHandler(this.cmdSample9_Click);
			// 
			// cmdSample10
			// 
			this.cmdSample10.Location = new System.Drawing.Point(8, 456);
			this.cmdSample10.Name = "cmdSample10";
			this.cmdSample10.Size = new System.Drawing.Size(224, 32);
			this.cmdSample10.TabIndex = 26;
			this.cmdSample10.Text = "Fill Strong Typed Dataset";
			this.cmdSample10.Click += new System.EventHandler(this.cmdSample10_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(768, 550);
			this.Controls.Add(this.cmdSample10);
			this.Controls.Add(this.cmdSample9);
			this.Controls.Add(this.cmdSample8);
			this.Controls.Add(this.cmdSample7);
			this.Controls.Add(this.cmdSample6);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.txtConnectionString);
			this.Controls.Add(this.cmdSample5);
			this.Controls.Add(this.cmdSample4);
			this.Controls.Add(this.cmdSample3);
			this.Controls.Add(this.cmdSample2);
			this.Controls.Add(this.cmdClearResults);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.txtResults);
			this.Controls.Add(this.cmdSample1);
			this.Name = "Form1";
			this.Text = "Data Access Application Block for .NET QuickStart Samples (CS)";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private SqlConnection GetConnection(string connectionString)
		{
			SqlConnection connection = new SqlConnection(txtConnectionString.Text);
			connection.Open();

			return connection;
		}

		private void cmdSample1_Click(object sender, System.EventArgs e)
		{
			// SqlDataReader that will hold the returned results		
			SqlDataReader dr = null;
			
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
					
				// Call ExecuteReader static method of SqlHelper class that returns a SqlDataReader
				// We pass in database connection string, stored procedure name and a "1" as value of categoryID parameter
				dr = SqlHelper.ExecuteReader(connection, "getProductsByCategory",  1);
    						
				// display results in textbox on the form.
				txtResults.Clear();
    			
				// iterate through SqlDataReader
				while (dr.Read())
				{
					// get the value of second column in the datareader (product description)
					txtResults.Text = txtResults.Text + dr.GetValue(1) + Environment.NewLine;
				}
			}
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(dr != null)
					((IDisposable)dr).Dispose();

				if(connection != null)
					connection.Dispose();
			}
									
		}	

		private void cmdSample2_Click(object sender, System.EventArgs e)
		{
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// DataSet that will hold the returned results
				DataSet ds;
			
				// Call ExecuteDataset static method of SqlHelper class that returns a Dataset
				// We pass in database connection string, command type, stored procedure name and categoryID SqlParameter
				// that has a value of "1"
				ds = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "getProductsByCategory", new SqlParameter("@CategoryID", 1) );
						
				// Get XML representation of the dataset and display results in text box
				txtResults.Text = ds.GetXml();
			}
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}
			
		}

		private void cmdSample3_Click(object sender, System.EventArgs e)
		{			
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Set up parameters (1 input and 3 output) 
				SqlParameter [] arParms = new SqlParameter[4];
			
				// @ProductID Input Parameter 
				// assign value = 1
				arParms[0] = new SqlParameter("@ProductID", SqlDbType.Int ); 
				arParms[0].Value = 1;

				// @ProductName Output Parameter
				arParms[1] = new SqlParameter("@ProductName", SqlDbType.NVarChar, 40);
				arParms[1].Direction = ParameterDirection.Output;

				// @UnitPrice Output Parameter
				arParms[2] = new SqlParameter("@UnitPrice", SqlDbType.Money);
				arParms[2].Direction = ParameterDirection.Output;

				// @QtyPerUnit Output Parameter
				arParms[3] = new SqlParameter("@QtyPerUnit", SqlDbType.NVarChar, 20);
				arParms[3].Direction = ParameterDirection.Output;

				// Call ExecuteNonQuery static method of SqlHelper class
				// We pass in database connection string, command type, stored procedure name and an array of SqlParameter objects
				SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "getProductDetails", arParms);
				
				// Display results in text box using the values of output parameters	
				txtResults.Text = arParms[1].Value  + ", " + arParms[2].Value  + ", " + arParms[3].Value;
			}
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}
		}

		private void cmdSample4_Click(object sender, System.EventArgs e)
		{
			// String variable that will hold the returned result
			string productName;
						
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{				
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Call ExecuteScalar static method of SqlHelper class that returns an Object. Then cast the return value to string.
				// We pass in database connection string, command type, stored procedure name, and 1 as a value for ProductID SqlParameter
				productName = (string)SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, "getProductName", new SqlParameter("@ProductID", 1));
				txtResults.Text = productName;				
			}
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}	
		}

		private void cmdSample5_Click(object sender, System.EventArgs e)
		{
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				using (SqlTransaction trans = connection.BeginTransaction())
				{
					// Establish command parameters
					// @AccountNo (From Account)
					SqlParameter paramFromAcc = new SqlParameter("@AccountNo", SqlDbType.Char, 20);
					paramFromAcc.Value = "12345";

					// @AccountNo (To Account)
					SqlParameter paramToAcc = new SqlParameter("@AccountNo", SqlDbType.Char, 20);
					paramToAcc.Value = "67890";

					// @Money (Credit amount)
					SqlParameter paramCreditAmount = new SqlParameter("@Amount", SqlDbType.Money );
					paramCreditAmount.Value = 500;

					// @Money (Debit amount)
					SqlParameter paramDebitAmount = new SqlParameter("@Amount", SqlDbType.Money );
					paramDebitAmount.Value = 500;

					try
					{	
						// Call ExecuteNonQuery static method of SqlHelper class for debit and credit operations.
						// We pass in SqlTransaction object, command type, stored procedure name, and a comma delimited list of SqlParameters
					
						// Perform the debit operation
						SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "Debit", paramFromAcc, paramDebitAmount );

						// Perform the credit operation
						SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "Credit", paramToAcc, paramCreditAmount );
						
						trans.Commit();
						txtResults.Text = "Transfer Completed";
						
					}
					catch (Exception ex)
					{
						// throw exception						
						trans.Rollback();
						txtResults.Text = "Transfer Error";
						throw ex;						
					}
				}
			}
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}	
		}
		
		private void cmdSample6_Click(object sender, System.EventArgs e)
		{  
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Call ExecuteXmlReader static method of SqlHelper class that returns an XmlReader
                // We pass in an open database connection object, command type, and command text
                XmlReader xreader = SqlHelper.ExecuteXmlReader(connection, CommandType.Text, "SELECT * FROM Products FOR XML AUTO " );
					
                // read the contents of xml reader and populate the results text box:
                txtResults.Clear();
                while (!xreader.EOF)
                {
                    if(xreader.IsStartElement()) 
                        txtResults.Text += xreader.ReadOuterXml() + Environment.NewLine;
                }   
			
                // close XmlReader
                xreader.Close();
            }
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			// Add tooltips to command buttons
			toolTip.SetToolTip(cmdSample1, "Retrieving Multiple Rows Using a SqlDataReader");
			toolTip.SetToolTip(cmdSample2, "Retrieving Multiple Rows Using a DataSet");
			toolTip.SetToolTip(cmdSample3, "Retrieving a Single Row");
			toolTip.SetToolTip(cmdSample4, "Looking Up a Single Item");
			toolTip.SetToolTip(cmdSample5, "Performing Transactional Updates");
			toolTip.SetToolTip(cmdSample6, "Retrieving XML Data");		
            toolTip.SetToolTip(cmdSample7, "Retrieving Multiple Rows Using a existing DataSet");
            toolTip.SetToolTip(cmdSample8, "Updating the data source with DataSet changes");
            toolTip.SetToolTip(cmdSample9, "Performing update with parameter binding");
            toolTip.SetToolTip(cmdSample10, "Retrieving Multiple Rows Using a existing strong typed DataSet");
		}

		private void cmdClearResults_Click(object sender, System.EventArgs e)
		{
			txtResults.Clear();
		}

        private void cmdSample7_Click(object sender, System.EventArgs e)
        {
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{
							
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// DataSet that will hold the returned results
				DataSet ds = new DataSet();
            			
				// Call FillDataset static method of SqlHelper class that fills a existing Dataset
				// We pass in database connection string, command type, stored procedure name and categoryID SqlParameter
				// that has a value of "1"
				SqlHelper.FillDataset(connection, CommandType.StoredProcedure, "getProductsByCategory", ds, new string[] {"Products"}, new SqlParameter("@CategoryID", 1) );
						
				// Get XML representation of the dataset and display results in text box
				txtResults.Text = ds.GetXml();
			}
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}
        }

        private void cmdSample8_Click(object sender, System.EventArgs e)
        {
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// DataSet that will hold the returned results
				DataSet ds = new DataSet();
			
                // Call FillDataset static method of SqlHelper class that fills a Dataset
                // We pass in database connection string, command type, stored procedure name and categoryID SqlParameter
                // that has a value of "1"
                SqlHelper.FillDataset(connection, CommandType.StoredProcedure, "getProductsByCategory", ds, new string[] {"Products"}, new SqlParameter("@CategoryID", 1) );

                // DataTable that hold the returned results
                DataTable table = ds.Tables["Products"];

                // Add a new product to existing DataSet
                DataRow addedRow = table.Rows.Add(new object[] {DBNull.Value, "New product", DBNull.Value, 10});
            
                // Modify a existing product
                table.Rows[0]["ProductName"] = "Modified product";

                // Create the command that will be used for insert operations
                SqlCommand insertCommand = SqlHelper.CreateCommand( connection, "addProduct", "ProductName", "UnitPrice" );
                
                // Create the command that will be used for update operations
				// The stored procedure also performs a SELECT to allow updating the DataSet with other changes (Identity columns, changes performed by triggers, etc)
                SqlCommand updateCommand = SqlHelper.CreateCommand( connection, "updateProduct", "ProductID", "ProductName", "LastUpdate" );
            
                // Create the command that will be used for delete operations
                SqlCommand deleteCommand = SqlHelper.CreateCommand( connection, "deleteProduct", "ProductID" );

				try
				{
					// Update the data source with the DataSet changes
                    SqlHelper.UpdateDataset(insertCommand, deleteCommand, updateCommand, ds, "Products");

                    //Get the new product id. This id was generated in the data source
                    txtResults.Text = "ProductID: " + addedRow["ProductID"].ToString();
				}
				catch(DBConcurrencyException)
				{
					MessageBox.Show("A concurrency error has ocurred while trying to update the data source", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtResults.Text = "The following rows wasn´t updated: ";
                    foreach( DataRow currentRow in table.Rows )
                    {
						if ( currentRow.RowState != DataRowState.Unchanged )
                        {
							txtResults.Text += Environment.NewLine + "Product ID: " + currentRow["ProductID"].ToString() + 
								" Product Name: " + currentRow["ProductName"].ToString();
                        }
                    }
				}
			}
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}
        }

        private void cmdSample9_Click(object sender, System.EventArgs e)
        {
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			// DataSet that will hold the returned results
            ProductDS productDS = new ProductDS();
			
            try
			{
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Call FillDataset static method of SqlHelper class that fills a Dataset
                // We pass in database connection string, command type, stored procedure name and categoryID SqlParameter
                // that has a value of "1"
                SqlHelper.FillDataset(connection, CommandType.StoredProcedure, "getProductsByCategory", productDS, new string[] {productDS.Products.TableName}, new SqlParameter("@CategoryID", 1) );

                //Modify a existing product
                productDS.Products[0].ProductName = "Modified product";

                //Apply changes in the data source
                SqlHelper.ExecuteDatasetTypedParams(connection, "updateProduct", productDS.Products[0]);
                    
                txtResults.Text = "The product " + productDS.Products[0].ProductID + " has been modified. Now, its name is " + productDS.Products[0].ProductName; 
            }
            catch(DBConcurrencyException)
            {
				MessageBox.Show("A concurrency error has ocurred while trying to update the data source", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}
        }

        private void cmdSample10_Click(object sender, System.EventArgs e)
        {
			// SqlConnection that will be used to execute the sql commands
			SqlConnection connection = null;

			try
			{
				try
				{
					connection = GetConnection(txtConnectionString.Text);
				}
				catch
				{
					MessageBox.Show("The connection with the database can´t be established", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Strong Typed DataSet that will hold the returned results
				ProductDS productDS = new ProductDS();
	            			
				// Call FillDataset static method of SqlHelper class that fills a existing Dataset
				// We pass in database connection string, command type, stored procedure name and categoryID SqlParameter
				// that has a value of "1"
				SqlHelper.FillDataset(connection, CommandType.StoredProcedure, "getProductsByCategory", productDS, new string[] {"Products"}, new SqlParameter("@CategoryID", 1) );
							
				// Get XML representation of the dataset and display results in text box
				txtResults.Text = productDS.GetXml();
			}
			catch(Exception ex)
			{
				string errMessage = "";
				for( Exception tempException = ex; tempException != null ; tempException = tempException.InnerException )
				{
					errMessage += tempException.Message + Environment.NewLine + Environment.NewLine;
				}

				MessageBox.Show( string.Format( "There are some problems while trying to use the Data Access Application block, please check the following error messages: {0}"
					+ Environment.NewLine + "This test requires some modifications to the Northwind database. Please make sure the database has been initialized using the SetUpDataBase.bat database script, or from the  Install Quickstart option on the Start menu.", errMessage ), 
					"Application error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			finally
			{
				if(connection != null)
					connection.Dispose();
			}
        }		
	}
}


