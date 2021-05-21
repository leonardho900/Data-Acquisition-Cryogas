using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Data_Acquisition

{
    public partial class Form1 : Form
    {


        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter DtA = new MySqlDataAdapter();
        MySqlDataReader sqlRd;

        DataSet DS = new DataSet();

        String server = "localhost";
        String username = "root";
        String password = "12345";
        String database = "dataentry";

        public object DataGridView1 { get; internal set; }

        public Form1()
        {
            InitializeComponent();
        }

        public void upLoadData()
        {
            sqlConn.ConnectionString = "server=" + server + ";" + "user id=" + username + ";" +
               "password=" + password + ";" + "database=" + database;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "SELECT * FROM dataentry.dataentry";

            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.Show();

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            upLoadData();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(this);
            form3.txtID.Text = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
            form3.processComboBox.Text = this.dataGridView1.CurrentRow.Cells[1].Value.ToString();
            form3.txtProcessName.Text = this.dataGridView1.CurrentRow.Cells[2].Value.ToString();
            form3.txtA.Text = this.dataGridView1.CurrentRow.Cells[3].Value.ToString();
            form3.txtB.Text = this.dataGridView1.CurrentRow.Cells[4].Value.ToString();
            form3.txtC.Text = this.dataGridView1.CurrentRow.Cells[5].Value.ToString();
            form3.txtManPower.Text = this.dataGridView1.CurrentRow.Cells[6].Value.ToString();
            form3.txtDuration.Text = this.dataGridView1.CurrentRow.Cells[7].Value.ToString();
            form3.Show();

        }

        public void searchData(string valueToFind)
        {


        }

  

        public void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                sqlConn.Open();
                sqlQuery = "select * from dataentry.dataentry WHERE CONCAT(`ID`,`Process Name`,`Process Type`) LIKE '%" + txtSearch.Text + "%'";

                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();

                sqlConn.Close();
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlQuery, sqlConn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
