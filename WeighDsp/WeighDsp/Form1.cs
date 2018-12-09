using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace WeighDsp
{
    public partial class Form1 : Form
    {
        Thread thr;
        string weigh = ".000000";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cbCom.Text;
            }
            catch { }
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
                serialPort1.DataReceived += SerialPort1_DataReceived;
              }
            else
            {
                try
                {
                    serialPort1.Close();
                }
                catch { }
            }
            btnConnect.Text = serialPort1.IsOpen ? "Ngắt kết nối" : "Kết nối";
            cbCom.Enabled= serialPort1.IsOpen ? false : true;
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            txtRcv.Invoke(new Action(() =>
            {
                txtRcv.Text +="\nNhận: "+ serialPort1.ReadLine()+"\n";
            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbCom.DataSource = SerialPort.GetPortNames();
            serialPort1.BaudRate = 1200;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.DataBits = 8;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            txtRcv.Text += "\nGửi: " + txtSend.Text+"\n";
            weigh = txtSend.Text;
            txtSend.Clear();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                    if ((txtSend.Text.Length == 0))
                    {
                        serialPort1.Write(weigh + "0=");
                    }
                    else
                    {
                        serialPort1.Write(".0000000=");
                    }
            }
        }
    }
}
