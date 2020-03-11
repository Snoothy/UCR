using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Views;
using System;
using System.Windows.Forms;

namespace HidWizards.UCR.Utilities
{
    class UCRTrayIcon
    {
        internal bool Visible { get { return TrayIcon.Visible; } set { TrayIcon.Visible = value; } }
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

            TrayIcon.Text = "Universal Control Remapper";
            TrayIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            TrayIcon.DoubleClick += TrayIcon_OnDoubleClick;
            TrayIcon.ContextMenuStrip = new ContextMenuStrip();
            TrayIcon.ContextMenuStrip.ShowCheckMargin = false;
            TrayIcon.ContextMenuStrip.ShowImageMargin = false;
            TrayIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { ShowStrip, new ToolStripSeparator(), StartLastProfileStrip, StopProfileStrip, new ToolStripSeparator(), ExitStrip });
            TrayIcon.Visible = true;
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

        private void ExitStrip_Click(object sender, EventArgs e)
        {
            _parent.Close();
        }

        private void StartLastProfileStrip_Click(object sender, EventArgs e)
        {
            _parent.Context.SubscriptionsManager.ActivateLastProfile();
        }
        private void ShowStrip_Click(object sender, EventArgs e)
        {
            _parent.Show();
            _parent.WindowState = System.Windows.WindowState.Normal;
            _parent.Activate();
        }

        private void TrayIcon_OnDoubleClick(object sender, EventArgs e)
        {
            _parent.Show();
            _parent.WindowState = System.Windows.WindowState.Normal;
            _parent.Activate();
        }
    }
}
