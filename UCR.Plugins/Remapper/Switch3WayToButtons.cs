using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using System.Timers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Switch 3 Way to Buttons", Group = "Button", Description = "Map from one On-Off-On toggle swicth to three momentary buttons")]
    [PluginInput(DeviceBindingCategory.Momentary, "Switch Up")]
    [PluginInput(DeviceBindingCategory.Momentary, "Switch Down")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button Up")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button Center")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button Down")]
    public class Switch3WayToButtons : Plugin
    {

        [PluginGui("Button Press Durration (ms)")]
        public int durration { get; set; }

        private System.Timers.Timer topTimer, centerTimer, bottomTimer;

        public Switch3WayToButtons() {
            durration = 50;
            topTimer = new Timer {
                Interval = durration,
                AutoReset = false,
                Enabled = false
            };
            centerTimer = new Timer {
                Interval = durration,
                AutoReset = false,
                Enabled = false
            };
            bottomTimer = new Timer {
                Interval = durration,
                AutoReset = false,
                Enabled = false
            };
            topTimer.Elapsed += this.OnTopTimeElapsed;
            centerTimer.Elapsed += this.OnCenterTimerElapsed;
            bottomTimer.Elapsed += this.OnBottomTimerElapsed;
        }

        private void OnTopTimeElapsed(System.Object source, ElapsedEventArgs e) {
            WriteOutput(0, 0);
        }

        private void OnCenterTimerElapsed(System.Object source, ElapsedEventArgs e) {
            WriteOutput(1, 0);
        }

        private void OnBottomTimerElapsed(System.Object source, ElapsedEventArgs e) {
            WriteOutput(2, 0);
        }

        public override void Update(params short[] values)
        {
            if (values[0] == 0 && values[1] == 0) //center
            {
                WriteOutput(1, 1);
                centerTimer.Interval = durration;
                centerTimer.Start();
            } 
            else if (values[0] == 0 && values[1] == 1) //top
            {
                WriteOutput(0, 1);
                topTimer.Interval = durration;
                topTimer.Start();
            } 
            else //bottom
            {
                WriteOutput(2, 1);
                bottomTimer.Interval = durration;
                bottomTimer.Start();
            }
        }
    }
}
