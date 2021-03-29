using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;

namespace RelaperCallouts.Util
{
    public class MusicEvent
    {
        public MusicEvent(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void Prepare()
        {
            NativeFunction.Natives.PREPARE_MUSIC_EVENT(Name);
        }

        public void Trigger()
        {
            NativeFunction.Natives.TRIGGER_MUSIC_EVENT(Name);
        }

        public void Dismiss()
        {
            NativeFunction.Natives.CANCEL_MUSIC_EVENT(Name);
        }
    }
}
