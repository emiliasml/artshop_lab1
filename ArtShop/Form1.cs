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

namespace ArtShop
{
    public partial class Form1 : Form
    {
        string connectionString = "Server=DESKTOP-EMSML\\SQLEXPRESS; Database=ArtShop;Integrated Security=true;";
        DataSet ds = new DataSet();
        SqlDataAdapter parentAdapter = new SqlDataAdapter();
        SqlDataAdapter childAdapter = new SqlDataAdapter();
        BindingSource bsParent = new BindingSource();
        BindingSource bsChild = new BindingSource();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Art Shop for broke artists";
            GridFill();
        }

        void GridFill()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    parentAdapter.SelectCommand = new SqlCommand("SELECT * FROM CLIENTS", connection);
                    childAdapter.SelectCommand = new SqlCommand("SELECT * FROM POSSESSIONS", connection);
                    parentAdapter.Fill(ds, "Clients");
                    childAdapter.Fill(ds, "Possessions");
                    bsParent.DataSource = ds.Tables["Clients"];
                    DataColumn pk = ds.Tables["Clients"].Columns["idClient"];
                    DataColumn fk = ds.Tables["Possessions"].Columns["idClient"];
                    DataRelation relation = new DataRelation("FK_Clients_Possessions", pk, fk);
                    ds.Relations.Add(relation);
                    bsChild.DataSource = bsParent;
                    bsChild.DataMember = "FK_Clients_Possessions";
                    dataGridView1.DataSource = bsParent;
                    dataGridView2.DataSource = bsChild;
                    dataGridView2.Columns[4].Visible = false;
                    SaveButton.Visible = updateButton.Visible = deleteButton.Visible = updateButtonChild.Visible = false;
                    ClearTextBoxesandLabels();
                    title.Text = "";

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    childAdapter.SelectCommand.Connection = connection;
                    parentAdapter.SelectCommand.Connection = connection;
                    ds.Tables["Possessions"].Clear();
                    ds.Tables["Clients"].Clear();
                    childAdapter.Fill(ds,"Possessions");
                    parentAdapter.Fill(ds, "Clients");
                    SaveButton.Visible = updateButton.Visible = deleteButton.Visible = false;
                    ClearTextBoxesandLabels();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string idPoss = dataGridView2.CurrentRow.Cells["IdPossession"].Value.ToString();
                    childAdapter.DeleteCommand = new SqlCommand("DELETE FROM Possessions WHERE " +
                        "IdPossession = @IdPoss", connection);
                    childAdapter.DeleteCommand.Parameters.Add("@IdPoss", SqlDbType.Int).Value = idPoss;
                    childAdapter.DeleteCommand.Connection = connection;
                    childAdapter.DeleteCommand.ExecuteNonQuery();
                    deleteButton.Visible = false;
                    MessageBox.Show("Deleted!");
                }
            }

            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    childAdapter.InsertCommand = new SqlCommand("INSERT INTO Possessions" +
                        "(PossessionName, Price, City, IdClient) VALUES (@PossessionName, " +
                        "@Price, @City, @IdClient)", connection);
                    string parentID = dataGridView1.CurrentRow.Cells["IdClient"].Value.ToString();
                    childAdapter.InsertCommand.Parameters.Add("@IdClient", SqlDbType.Int).Value = parentID;
                    childAdapter.InsertCommand.Parameters.Add("@Price", SqlDbType.Int).Value = txtbox2.Text;
                    childAdapter.InsertCommand.Parameters.Add("@City", SqlDbType.VarChar).Value = txtbox3.Text;
                    childAdapter.InsertCommand.Parameters.Add("@PossessionName", SqlDbType.VarChar).Value = txtbox1.Text;
                    childAdapter.InsertCommand.Connection = connection;
                    childAdapter.InsertCommand.ExecuteNonQuery();
                    SaveButton.Visible = false;
                    ClearTextBoxesandLabels();
                    MessageBox.Show("Added successsfully!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

 

        private void ClearTextBoxesandLabels()
        {
            txtbox1.Clear();
            txtbox2.Clear();
            txtbox3.Clear();
            txtbox4.Clear();
            txtbox5.Clear();
            title.Text = label1.Text = label2.Text = label3.Text = label4.Text = label5.Text = "";
            txtbox1.Visible = txtbox2.Visible = txtbox3.Visible = txtbox4.Visible = txtbox5.Visible = false;
        }

        private void prepareForAdd()
        {
            ClearTextBoxesandLabels();
            title.Text = "INSERT a possession for selected client";
            label1.Text = "Possesion name:";
            label2.Text = "Price:";
            label3.Text = "City:";
            txtbox1.Visible = txtbox2.Visible = txtbox3.Visible = true;
            SaveButton.Visible = true;
        }


        private void prepareForUpdateChild()
        {
            ClearTextBoxesandLabels();
            title.Text = "UPDATE a possession for selected client";
            label1.Text = "Possesion name:";
            label2.Text = "Price:";
            label3.Text = "City:";
            label4.Text = "IdClient:";
            txtbox1.Visible = txtbox2.Visible = txtbox3.Visible = txtbox4.Visible = true;
            updateButtonChild.Visible = true;

        }


        private void prepareForUpdate()
        {
            ClearTextBoxesandLabels();
            title.Text = "UPDATE selected client";
            label1.Text = "First name:";
            label2.Text = "Last name:";
            label3.Text = "E-mail:";
            label4.Text = "Adress:";
            label5.Text = "Id card:";
            txtbox1.Visible = txtbox2.Visible = txtbox3.Visible = txtbox4.Visible = txtbox5.Visible = true;
            updateButton.Visible = true;
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            deleteButton.Visible = false;
            updateButton.Visible = false;
            prepareForAdd();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            prepareForUpdate();
            txtbox1.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            txtbox2.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            txtbox3.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            txtbox4.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            txtbox5.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            SaveButton.Visible = false;

        }

        private void dataGridView2_Click(object sender, EventArgs e)
        {
            ClearTextBoxesandLabels();
            deleteButton.Visible = true;
            SaveButton.Visible = false;
            updateButton.Visible = false;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    parentAdapter.UpdateCommand = new SqlCommand("UPDATE Clients" +
                        " SET FirstName=@fn, LastName=@ln, Email=@email, Adress=@adr, IdCard=@idc " +
                        " WHERE IdClient = @idClient", connection);
                    parentAdapter.UpdateCommand.Parameters.Add("@idClient", SqlDbType.Int).Value = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    parentAdapter.UpdateCommand.Parameters.Add("@fn", SqlDbType.VarChar).Value = txtbox1.Text;
                    parentAdapter.UpdateCommand.Parameters.Add("@ln", SqlDbType.VarChar).Value = txtbox2.Text;
                    parentAdapter.UpdateCommand.Parameters.Add("@email", SqlDbType.VarChar).Value = txtbox3.Text;
                    parentAdapter.UpdateCommand.Parameters.Add("@adr", SqlDbType.VarChar).Value = txtbox4.Text;
                    parentAdapter.UpdateCommand.Parameters.Add("@idc", SqlDbType.VarChar).Value = txtbox5.Text;
                    parentAdapter.UpdateCommand.Connection = connection;
                    parentAdapter.UpdateCommand.ExecuteNonQuery();
                    MessageBox.Show("Updated!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            prepareForUpdateChild();
            txtbox1.Text = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            txtbox2.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString();
            txtbox3.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString();
            txtbox4.Text = dataGridView2.CurrentRow.Cells[4].Value.ToString();


        }

        private void updateButtonChild_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    childAdapter.UpdateCommand = new SqlCommand("UPDATE Possessions" +
                        " SET PossessionName=@posName, Price =@price, City=@city, IdClient=@idclient"+
                        " WHERE IdPossession = @IdP", connection);
                    childAdapter.UpdateCommand.Parameters.Add("@IdP", SqlDbType.Int).Value = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                    childAdapter.UpdateCommand.Parameters.Add("@posName", SqlDbType.VarChar).Value = txtbox1.Text;
                    childAdapter.UpdateCommand.Parameters.Add("@price", SqlDbType.VarChar).Value = txtbox2.Text;
                    childAdapter.UpdateCommand.Parameters.Add("@city", SqlDbType.VarChar).Value = txtbox3.Text;
                    childAdapter.UpdateCommand.Parameters.Add("@idclient", SqlDbType.VarChar).Value = txtbox4.Text;
                    childAdapter.UpdateCommand.Connection = connection;
                    childAdapter.UpdateCommand.ExecuteNonQuery();
                    MessageBox.Show("Updated!");
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
