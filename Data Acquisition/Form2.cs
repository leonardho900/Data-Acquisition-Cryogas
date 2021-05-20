
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
    public partial class Form2 : Form
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

        public Form2(Form1 parentForm)
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

            try
            {
                sqlConn.Open();
                sqlQuery = "INSERT INTO `dataentry`.`dataentry` (`ID`, `Process Type`, `Process Name`, `Material A`, `Material B`, `Material C`, `Man Power`, `Duration`)" +
                    "values('" + txtID.Text + "','" + processComboBox.Text + "','" + txtProcessName.Text
                    + "','" + txtA.Text + "','" + txtB.Text + "','" + txtC.Text + "','" + txtManPower.Text + "','" + txtDuration.Text + "')";

                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();

                sqlConn.Close();
                MessageBox.Show("Value Entered.");
                this.Close();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            stopwatch.Reset();
            txtDuration.Text = "00:00:00";
            x = 0;
            listBox1.Items.Clear();
            s = 0;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
