using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using System.Timers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Switch to Buttons", Group = "Button", Description = "Map from one swicth to two momentary buttons")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Momentary, "ButtonOn")]
    [PluginOutput(DeviceBindingCategory.Momentary, "ButtonOff")]
    public class SwitchToButtons : Plugin
    {

        [PluginGui("Button Press Durration (ms)")]
        public int durration { get; set; }

        private System.Timers.Timer onTimer, offTimer;

        public SwitchToButtons() {
            durration = 50;
            onTimer = new Timer {
                Interval = durration,
                AutoReset = false,
                Enabled = false
            };
            offTimer = new Timer {
                Interval = durration,
                AutoReset = false,
                Enabled = false
            };
            onTimer.Elapsed += this.OnOnTimerElapsed;
            offTimer.Elapsed += this.OnOffTimerElapsed;
        }

        private void OnOnTimerElapsed(System.Object source, ElapsedEventArgs e) {
            WriteOutput(1, 0);
        }

        private void OnOffTimerElapsed(System.Object source, ElapsedEventArgs e) {
            WriteOutput(0, 0);
        }

        public override void Update(params short[] values)
        {
            if (values[0] == 0)
            {
                WriteOutput(0, 1);
                //Thread.Sleep(durration);
                offTimer.Interval = durration;
                offTimer.Start();
                //WriteOutput(0, 0);
            }
            else
            {
                WriteOutput(1, 1);
                //Thread.Sleep(durration);
                onTimer.Interval = durration;
                onTimer.Start();
                //WriteOutput(1, 0);
            }
        }
    }
}
