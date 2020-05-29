using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace kill_process_when_idle
{
    public partial class kill_process_when_idle : Form
    {
        //參數設定區
        const String PROGRAM_NAME = "ERP閒置中斷程式";
        const String KILL_PROCESS_NAME = "erp2017m";
        const String KILL_PROCESS_KEYWORD = "ERP2017";
        int MAX_REMAIN_TIME = 480;
        int REMAIN_TIME = 480;


        //抓取運行程式標題的DLL引入
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        private string GetCaptionOfActiveWindow()
        {
            var strTitle = string.Empty;
            var handle = GetForegroundWindow();
            // Obtain the length of the text   
            var intLength = GetWindowTextLength(handle) + 1;
            var stringBuilder = new StringBuilder(intLength);
            if (GetWindowText(handle, stringBuilder, intLength) > 0)
            {
                strTitle = stringBuilder.ToString();
            }
            return strTitle;
        }


        //覆寫windows關機事件
        protected override void WndProc(ref Message aMessage)
        {
            const int WM_QUERYENDSESSION = 0x0011;
            const int WM_ENDSESSION = 0x0016;

            //如果程式收到關機訊號 則自己關閉
            if (aMessage.Msg == WM_QUERYENDSESSION || aMessage.Msg == WM_ENDSESSION)
                Process.GetCurrentProcess().Kill();
            base.WndProc(ref aMessage);
        }

        public void StartCount() {
            if (timer1.Enabled == false) {
                REMAIN_TIME = MAX_REMAIN_TIME;
                timer1.Start();
            }
        }
        public void StopCount()
        {
            if (timer1.Enabled == true)
            {
                timer1.Stop();
                lbl_remain_time.Text = MAX_REMAIN_TIME.ToString();
            }
        }

        public void KillProgram() {
            foreach (var process in Process.GetProcessesByName(KILL_PROCESS_NAME))
            {
                process.Kill();
            }
        }

        public void InitUI()
        {
            this.notifyIcon1.Visible = false;

            this.Text = PROGRAM_NAME;
            lbl_program_name.Text = PROGRAM_NAME;
        }

        public kill_process_when_idle()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitUI();

            //避免多開
            if (Process.GetProcessesByName("kill_process_when_idle").Count() > 1)
            {
                MessageBox.Show("此程式不能多開!");
                Process.GetCurrentProcess().Kill();
            }
        }
        private void kill_process_when_idle_Shown(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = true;
            this.Hide();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (REMAIN_TIME <= 0) {
                KillProgram();
                timer1.Stop();
            }
            lbl_remain_time.Text = REMAIN_TIME.ToString();
            REMAIN_TIME--;
        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            lbl_current_program_name.Text = GetCaptionOfActiveWindow();
            if (Process.GetProcessesByName(KILL_PROCESS_NAME).Count() > 0)
            {
                if (lbl_current_program_name.Text.Contains(KILL_PROCESS_KEYWORD))
                {
                    StopCount();
                }
                else
                {
                    StartCount();
                }
            }
            else {
                StopCount();
            }
        }
    }
}
