using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HidWizards.UCR.Utilities
{
    class UCRTrayIcon
    {
        internal bool Visible { get { return TrayIcon.Visible; } set { TrayIcon.Visible = value; } }
        private MainWindow _parent;
        private NotifyIcon TrayIcon = new NotifyIcon();
        private ToolStripItem ShowStrip = new ToolStripMenuItem();
        private ToolStripItem StartLastProfileStrip = new ToolStripMenuItem();
        private ToolStripItem StopProfileStrip = new ToolStripMenuItem();
        private ToolStripItem ExitStrip = new ToolStripMenuItem();

        public UCRTrayIcon(MainWindow parent)
        {
            ShowStrip.Text = "Show";
            ShowStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            ShowStrip.Click += ShowStrip_Click;

            StartLastProfileStrip.Text = "Activate Last Profile";
            StartLastProfileStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            StartLastProfileStrip.Click += StartLastProfileStrip_Click;
            StartLastProfileStrip.Enabled = false;

            StopProfileStrip.Text = "Stop Active Profile";
            StopProfileStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            StopProfileStrip.Click += StopProfileStrip_Click; ;
            StopProfileStrip.Enabled = false;

            ExitStrip.Text = "Exit";
            ExitStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            ExitStrip.Click += ExitStrip_Click;

            _parent = parent;
            _parent.Context.ActiveProfileChangedEvent += Context_ActiveProfileChangedEvent;

            TrayIcon.Text = "Universal Control Remapper";
            TrayIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            TrayIcon.DoubleClick += TrayIcon_OnDoubleClick;
            TrayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            TrayIcon.ContextMenuStrip.ShowCheckMargin = false;
            TrayIcon.ContextMenuStrip.ShowImageMargin = false;
            TrayIcon.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { ShowStrip, new System.Windows.Forms.ToolStripSeparator(), StartLastProfileStrip, StopProfileStrip, new System.Windows.Forms.ToolStripSeparator(), ExitStrip });
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
            StopProfileStrip.Enabled = true;
        }
        private void ShowStrip_Click(object sender, EventArgs e)
        {
            _parent.Show();
            _parent.Activate();
        }

        private void TrayIcon_OnDoubleClick(object sender, EventArgs e)
        {
            _parent.Show();
        }
    }
}
