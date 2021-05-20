
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.IO;
using MySql.Data.MySqlClient;

namespace Data_Acquisition
{
    public partial class Form3 : Form
    {

        Bitmap bitmap;

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

        int x = 0;
        System.Timers.Timer t;

        int h, m, s;
        private Form1 parentForm;

        public Form3(Form1 parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;

        }
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        private void upLoadData()
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
        }


        private void Form2_Load(object sender, EventArgs e)
        {
            t = new System.Timers.Timer();
            t.Interval = 1000;//1s
            t.Elapsed += OnTimeEvent;

        }

        private void OnTimeEvent(Object sender, ElapsedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                s += 1;
                if (s == 60)
                {
                    s = 0;
                    m += 1;
                }
                if (m == 60)
                {
                    m = 0;
                    h += 1;
                }
                txtDuration.Text = string.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), m.ToString().PadLeft(2, '0'), s.ToString().PadLeft(2, '0'));
            }));

        }


        private void btnLapTime_Click(object sender, EventArgs e)
        {
            x += 1;
            listBox1.Items.Add(x + ". " + txtDuration.Text);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            listBox1.SelectedIndex = -1;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (t.Enabled)
            {
                t.Stop();
                stopwatch.Stop();
                btnStart.Text = "Start";
            }
            else
            {
                t.Start();
                stopwatch.Start();
                btnStart.Text = "Stop";
            }
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            processComboBox.Text = "Cleaning"; txtProcessName.Text = ""; txtA.Text = ""; txtB.Text = ""; txtC.Text = ""; txtManPower.Text = ""; txtDuration.Text = "";
            stopwatch.Reset();
            txtDuration.Text = "00:00:00";
            x = 0;
            listBox1.Items.Clear();
            s = 0;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            sqlConn.ConnectionString = "server=" + server + ";" + "user id=" + username + ";" +
               "password=" + password + ";" + "database=" + database;
            sqlConn.Open();


            try
            {
                MySqlCommand sqlCmd = new MySqlCommand();
                sqlCmd.Connection = sqlConn;

                sqlCmd.CommandText = "UPDATE `dataentry`.`dataentry` SET `ID` = @ID, `Process Type` = @ProcessType, `Process Name` = @ProcessName, `Material A` = @MaterialA, `Material B` = @MaterialB, `Material C` = @MaterialC, `Man Power` = @ManPower, `Duration` = @Duration WHERE (`ID` = @ID)";
                ;

                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@ID", txtID.Text);
                sqlCmd.Parameters.AddWithValue("@ProcessType", processComboBox.Text);
                sqlCmd.Parameters.AddWithValue("@ProcessName", txtProcessName.Text);
                sqlCmd.Parameters.AddWithValue("@MaterialA", txtA.Text);
                sqlCmd.Parameters.AddWithValue("@MaterialB", txtB.Text);
                sqlCmd.Parameters.AddWithValue("@MaterialC", txtC.Text);
                sqlCmd.Parameters.AddWithValue("@ManPower", txtManPower.Text);
                sqlCmd.Parameters.AddWithValue("@Duration", txtDuration.Text);

                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("Value Altered.");
                this.Close();
                sqlConn.Close();
                parentForm.upLoadData();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConn.Close();
            }
            upLoadData();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan Lap = stopwatch.Elapsed;
            listBox1.Text = "Timer" + string.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), m.ToString().PadLeft(2, '0'), s.ToString().PadLeft(2, '0'));
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Stop();
            Application.DoEvents();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            sqlConn.ConnectionString = "server=" + server + ";" + "user id=" + username + ";" +
                  "password=" + password + ";" + "database=" + database;
            sqlConn.Open();

            string MessageBoxTitle = "Delete Line";
            string MessageBoxContent = "Are you sure you want to delete the line?";

            DialogResult dialogResult = MessageBox.Show(MessageBoxContent, MessageBoxTitle, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    foreach (DataGridViewRow item in parentForm.dataGridView1.SelectedRows)
                    {
                        parentForm.dataGridView1.Rows.RemoveAt(item.Index);
                    }

                    MySqlCommand sqlCmd = new MySqlCommand();
                    sqlCmd.Connection = sqlConn;

                    sqlCmd.CommandText = "delete from dataentry.dataentry where ID=@ID";

                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.AddWithValue("@ID", txtID.Text);

                    sqlCmd.ExecuteNonQuery();
                    MessageBox.Show("Record Deleted Successfully!");
                    this.Close();
                    sqlConn.Close();
                    parentForm.upLoadData();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConn.Close();
                }
                upLoadData();
            }

            else if (dialogResult == DialogResult.No)
            {
            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            stopwatch.Reset();
            txtDuration.Text = "00:00:00";
            x = 0;
            listBox1.Items.Clear();
            s = 0;
        }


    }
}
