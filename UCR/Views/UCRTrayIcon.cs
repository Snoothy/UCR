using System;
using System.Windows.Forms;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Views
{
    class UCRTrayIcon
    {
        internal bool Visible { get { return TrayIcon.Visible; } set { TrayIcon.Visible = value; } }
        public bool Minimized { get; private set; }
        private readonly MainWindow _parent;
        private readonly NotifyIcon TrayIcon = new NotifyIcon();
        private readonly ToolStripItem ShowStrip = new ToolStripMenuItem();
        private readonly ToolStripItem StartLastProfileStrip = new ToolStripMenuItem();
        private readonly ToolStripItem StopProfileStrip = new ToolStripMenuItem();
        private readonly ToolStripItem ExitStrip = new ToolStripMenuItem();

        public UCRTrayIcon(MainWindow parent)
        {
            ShowStrip.Text = "Show";
            ShowStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ShowStrip.Click += ShowStrip_Click;

            StartLastProfileStrip.Text = "Activate Last Profile";
            StartLastProfileStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            StartLastProfileStrip.Click += StartLastProfileStrip_Click;
            StartLastProfileStrip.Enabled = false;

            StopProfileStrip.Text = "Stop Active Profile";
            StopProfileStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            StopProfileStrip.Click += StopProfileStrip_Click; ;
            StopProfileStrip.Enabled = false;

            ExitStrip.Text = "Exit";
            ExitStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ExitStrip.Click += ExitStrip_Click;

            _parent = parent;
            _parent.Context.ActiveProfileChangedEvent += Context_ActiveProfileChangedEvent;
            _parent.Context.MinimizedToTrayEvent += Context_MinimizedToTrayEvent;

            TrayIcon.Text = "Universal Control Remapper";
            TrayIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            TrayIcon.DoubleClick += TrayIcon_OnDoubleClick;
            TrayIcon.BalloonTipTitle = "UCR is still running";
            TrayIcon.BalloonTipText = "UCR has been minimized to the system tray. Double-click the icon to restore, or right click for more options.";
            TrayIcon.ContextMenuStrip = new ContextMenuStrip();
            TrayIcon.ContextMenuStrip.ShowCheckMargin = false;
            TrayIcon.ContextMenuStrip.ShowImageMargin = false;
            TrayIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { ShowStrip, new ToolStripSeparator(), StartLastProfileStrip, StopProfileStrip, new ToolStripSeparator(), ExitStrip });
            TrayIcon.Visible = Minimized;
        }

        private void StopProfileStrip_Click(object sender, EventArgs e)
        {
            _parent.DeactivateProfile(sender, null);
        }

        private void Context_ActiveProfileChangedEvent(Profile profile)
        {
            if (profile == null)
            {
                StopProfileStrip.Enabled = false;
            }
            else
            {
                StopProfileStrip.Enabled = true;
                StartLastProfileStrip.Text = $"Activate Last Profile: {profile.Title}";
                StartLastProfileStrip.Enabled = true;
            }
        }

        private void Context_MinimizedToTrayEvent()
        {
            Minimized = true;
            TrayIcon.Visible = true;
            TrayIcon.ShowBalloonTip(5000);
        }

        private void ExitStrip_Click(object sender, EventArgs e)
        {
            _parent.CloseWindow();
            if (_parent.Context.IsNotSaved)
            {
                Minimized = false;
                TrayIcon.Visible = false;
            }
        }

        private void StartLastProfileStrip_Click(object sender, EventArgs e)
        {
            _parent.Context.SubscriptionsManager.ActivateLastProfile();
        }
        private void ShowStrip_Click(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private void TrayIcon_OnDoubleClick(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        public void ProfileRenamed(string oldTitle, string newTitle)
        {
            if (StartLastProfileStrip.Text == $"Activate Last Profile: {oldTitle}")
                StartLastProfileStrip.Text = $"Activate Last Profile: {newTitle}";
        }

        public void ShowMainWindow()
        {
            _parent.ShowWindow();
            Minimized = false;
            TrayIcon.Visible = false;
        }

    }
}
