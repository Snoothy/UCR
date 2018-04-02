using System;

namespace HidWizards.UCR.Core.Models
{
    public class State
    {
        public string Title { get; set; }
        public Guid Guid { get; set; }

        public State()
        {
        }

        public State(string title)
        {
            Title = title;
            Guid = Guid.NewGuid();
        }
    }
}
