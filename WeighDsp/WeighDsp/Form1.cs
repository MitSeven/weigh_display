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
        string weigh = "000.000";
        string text="";
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
            byte[] by = new byte[64];
            int len = serialPort1.Read(by, 0, 64);
            byte[] rcv = new byte[len];
            Array.Copy(by, rcv, len);
            string data = System.Text.Encoding.ASCII.GetString(rcv);
            Thread.Sleep(10);
            txtRcv.Invoke(new Action(() =>
            {
                txtRcv.Text += data;
            }));
            if (txtRcv.Text.Length >12)
            {
                text = txtRcv.Text;
                txtRcv.Invoke(new Action(() => { txtRcv.Clear(); }));
            }
        }
        public string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
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
                if (text.Length ==13)
                {
                    string rev = text.Substring(3, 6);
                    string wei = "";
                    for (int i = 5; i >= 0; i--)
                    {
                        wei += rev[i];
                    }
                    wei = wei.Trim();
                    if (wei.Length == 5)
                    {
                        weigh = wei + "00";
                    }
                    else if (wei.Length == 6)
                    {
                        weigh = wei + "0";
                    }
                    txtRcv.Clear();
                }
                if (weigh.Length == 7)
                {
                    serialPort1.Write(weigh + "0=");
                }
            }
            
        }
    }
}
