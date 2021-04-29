using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrafficService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            sim = new Sim(this.Handle);
            service = new Service(sim, version);
        }

        private readonly Sim sim;
        private readonly Service service;
        private Http http = null;
        private const string version = "0.1.3";

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == Sim.WM_USER_SIMCONNECT)
            {
                if (sim.SimConnect != null)
                {
                    try
                    {
                        sim.SimConnect.ReceiveMessage();
                    }
                    catch (Exception e)
                    {
                        Logger.Log("SimConnect receive failed: " + e.Message);
                    }
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            sim.CloseConnection();
            StopHttp();
        }

        private int GetHttpPort()
        {
            int port = (int)Properties.Settings.Default["HttpPort"];
            if (int.TryParse(portTextBox.Text, out int inputPort))
            {
                port = inputPort;
            }
            return port;
        }

        private void StartHttp()
        {
            if (http != null)
            {
                return;
            }
            int port = GetHttpPort();
            string url = "http://localhost:" + port + "/";
            http = new Http(url, service.ProcessRequest);
            try
            {
                http.Start();
                startStopHttpButton.Text = "Stop";
                httpStatusLabel.Text = "Service started.";
                portTextBox.Text = port.ToString();
                Properties.Settings.Default["HttpPort"] = port;
            } catch (Exception e)
            {
                Logger.Log("HTTP failed: " + e.Message + "[" + url + "]");
                http.Stop();
                http = null;
                httpStatusLabel.Text = "HTTP failed: " + e.Message;
            }
        }

        private void StopHttp()
        {
            if (http == null)
            {
                return;
            }
            http.Stop();
            http = null;
            startStopHttpButton.Text = "Start";
            httpStatusLabel.Text = "Service stopped.";
        }

        private void StartStopHttpButton_Click(object sender, EventArgs e)
        {
            if (http == null)
            {
                StartHttp();
            }
            else
            {
                StopHttp();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon.Icon = new Icon(this.Icon, 40, 40);
            portTextBox.Text = ((int)Properties.Settings.Default["HttpPort"]).ToString();
            bool autoStart = (bool)Properties.Settings.Default["AutoStart"];
            autoStartCheckBox.Checked = autoStart;
            startMinimizedCheckBox.Checked = (bool)Properties.Settings.Default["StartMinimized"];
            minimizeToTrayCheckBox.Checked = (bool)Properties.Settings.Default["MinimizeToTray"];
            if (autoStart)
            {
                StartHttp();
            }
        }

        private void AutoStartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["AutoStart"] = autoStartCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void StartMinimizedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["StartMinimized"] = startMinimizedCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void MinimizeToTrayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["MinimizeToTray"] = minimizeToTrayCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && (bool)Properties.Settings.Default["MinimizeToTray"])
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (startMinimizedCheckBox.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
