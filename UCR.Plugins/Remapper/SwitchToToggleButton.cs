using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using System.Timers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Switch to Toggle Buttons", Group = "Button", Description = "Map from one switch to a single button that is pressed each time the switch moves.")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Momentary, "Button")]
    public class SwitchToToggleButton : Plugin
    {

        [PluginGui("Button Press Durration (ms)")]
        public int durration { get; set; }

        //This exists to allow a fast flip back and forth to register as only one button press so that switch state and game state can be put into sync.
        [PluginGui("Min time between presses (ms)")]
        public int minFlip { get; set; }

        private System.Timers.Timer flipTimer, offTimer;
        private bool doPress = true;

        public SwitchToToggleButton() {
            minFlip = 250;
            durration = 50;
            flipTimer = new Timer {
                Interval = durration,
                AutoReset = false,
                Enabled = false
            };
            flipTimer.Elapsed += this.OnFlipTimerElapsed;
            offTimer = new Timer {
                Interval = minFlip,
                AutoReset = false,
                Enabled = false
            };
            offTimer.Elapsed += this.OnOffTimerElapsed;
        }

        private void OnFlipTimerElapsed(System.Object source, ElapsedEventArgs e) {
            doPress = true;
        }

        private void OnOffTimerElapsed(System.Object source, ElapsedEventArgs e) {
            WriteOutput(0, 0);
        }

        public override void Update(params short[] values)
        {
            if(doPress)
            {
                WriteOutput(0, 1);
                offTimer.Interval = durration;
                offTimer.Start();
                doPress = false;
                flipTimer.Interval = minFlip;
                flipTimer.Start();
            }
        }
    }
}
